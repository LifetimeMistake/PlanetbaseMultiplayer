using Lidgren.Network;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Model.Utils;
using PlanetbaseMultiplayer.Model.World;
using PlanetbaseMultiplayer.Server.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using PlanetbaseMultiplayer.Server.World;
using PlanetbaseMultiplayer.Server.Time;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PlanetbaseMultiplayer.Server.Environment;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Server.Autofac;
using PlanetbaseMultiplayer.Model;

namespace PlanetbaseMultiplayer.Server
{
    public class Server
    {
        private bool isInitialized;
        private NetServer server;
        private ServerSettings settings;
        private PacketRouter router;
        private ServiceLocator serviceLocator;
        public Dictionary<Guid, long> playerConnections;

        public ServerSettings Settings { get { return settings; } }
        public ServiceLocator ServiceLocator { get { return serviceLocator; } }
        //public Dictionary<Guid, long> PlayerConnections { get { return playerConnections; } }

        public Server(ServerSettings settings, PacketRouter router, ServiceLocator serviceLocator, SynchronizationContext synchronizationContext)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.router = router ?? throw new ArgumentNullException(nameof(router));
            this.serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

            WorldData worldData;
            using (FileStream stream = File.OpenRead(settings.SavePath))
            {
                worldData = WorldData.Deserialize(stream);
            }
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            playerConnections = new Dictionary<Guid, long>();
        }

        public void Initialize()
        {
            if (isInitialized)
                throw new InvalidOperationException("The server had already been initialized.");

            foreach (IManager manager in serviceLocator.LocateServicesOfType<IManager>())
            {
                try
                {
                    manager.Initialize();
                }
                catch(Exception ex)
                {
                    throw new Exception($"Could not initialize manager \"{manager.GetType().Name}\": {ex}");
                }
            }

            isInitialized = true;
        }

        public void Start()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("PlanetbaseMultiplayer");
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = settings.Port;
            server = new NetServer(config);
            server.RegisterReceivedCallback(new SendOrPostCallback(ProcessMessage));
            server.Start();
            Console.WriteLine($"Server running on port {server.Port}");
        }

        public void Shutdown(bool gracefulShutdown = true)
        {
            WorldDataManager worldDataManager = serviceLocator.LocateService<WorldDataManager>();
            PlayerManager playerManager = serviceLocator.LocateService<PlayerManager>();

            if (gracefulShutdown)
            {
                // Add graceful shutdown logic
                if (worldDataManager.RequestWorldData())
                {
                    // Add logic
                }

                // Graceful disconnect
                foreach (Player player in playerManager.GetPlayers())
                    playerManager.DestroyPlayer(player, DisconnectReason.ServerClosing);
            }

            server.Shutdown("The server is shutting down");
        }

        public void ProcessMessage(object peerObj)
        {
            NetPeer peer = peerObj as NetPeer;
            NetIncomingMessage msg = peer.ReadMessage();
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    OnIncomingStatusChange(peer, msg);
                    break;
                case NetIncomingMessageType.DiscoveryRequest:
                    OnIncomingDiscoveryRequest(peer, msg);
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    OnIncomingConnectionApproval(peer, msg);
                    break;
                case NetIncomingMessageType.Data:
                    OnIncomingData(peer, msg);
                    break;
                case NetIncomingMessageType.WarningMessage:
                    OnIncomingWarningMessage(peer, msg);
                    break;
                case NetIncomingMessageType.DebugMessage:
                    OnIncomingDebugMessage(peer, msg);
                    break;
            }

            server.Recycle(msg);
        }

        public void OnIncomingStatusChange(NetPeer peer, NetIncomingMessage msg)
        {
            TimeManager timeManager = serviceLocator.LocateService<TimeManager>();

            PlayerManager playerManager = serviceLocator.LocateService<PlayerManager>();
            if (msg.SenderConnection.Status == NetConnectionStatus.Disconnecting || msg.SenderConnection.Status == NetConnectionStatus.Disconnected)
            {
                long id = msg.SenderConnection.RemoteUniqueIdentifier;
                if (!playerConnections.Values.Any(connectionId => connectionId == id))
                {
                    Console.WriteLine("Player connection closed gracefully.");
                    return;
                }

                KeyValuePair<Guid, long> kvp = playerConnections.First(p => p.Value == id);

                Guid playerId = kvp.Key;
                // Player has not sent a DisconnectRequestPacket before closing the connection
                // The reason of their disconnect is unknown, presumably lost connection
                if (playerManager.PlayerExists(playerId))
                {
                    playerManager.DestroyPlayer(playerId, DisconnectReason.ConnectionLost);
                    if (playerManager.GetPlayers().Count(p => p.State == PlayerState.ConnectedLoadingData) == 0)
                        timeManager.UnfreezeTime();
                }

                playerConnections.Remove(playerId);
            }
        }

        public void OnIncomingDiscoveryRequest(NetPeer peer, NetIncomingMessage msg)
        {
            Console.WriteLine($"Received server discovery request from {msg.SenderConnection.RemoteEndPoint.Address}:{msg.SenderConnection.RemoteEndPoint.Port}");
            NetOutgoingMessage response = server.CreateMessage();
            response.Write("PlanetbaseMultiplayer.Server");
            server.SendDiscoveryResponse(response, msg.SenderEndPoint);
        }

        public void OnIncomingConnectionApproval(NetPeer peer, NetIncomingMessage msg)
        {
            Console.WriteLine($"Received connection approval request from {msg.SenderConnection.RemoteEndPoint.Address}:{msg.SenderConnection.RemoteEndPoint.Port}");
            long id = msg.SenderConnection.RemoteUniqueIdentifier;
            if (!AuthorizeConnection(peer, msg))
            {
                Console.WriteLine($"Client's {id} connection has been denied");
                msg.SenderConnection.Deny();
                return;
            }

            Guid guid = Guid.NewGuid();
            playerConnections.Add(guid, id);
            Console.WriteLine($"Client's {id} connection has been approved");
            msg.SenderConnection.Approve();
        }

        public void OnIncomingData(NetPeer peer, NetIncomingMessage msg)
        {
            try
            {
                Packet packet = Packet.Deserialize(msg.ReadBytes(msg.LengthBytes));
                Guid sourcePlayerId = playerConnections.First(kvp => kvp.Value == msg.SenderConnection.RemoteUniqueIdentifier).Key;
                ProcessPacket(sourcePlayerId, packet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while routing packet: {e.Message}");
            }
        }

        public void OnIncomingWarningMessage(NetPeer peer, NetIncomingMessage msg)
        {
            Console.WriteLine($"WARNING: {msg.ReadString()}");
        }

        public void OnIncomingDebugMessage(NetPeer peer, NetIncomingMessage msg)
        {
            Console.WriteLine($"DEBUG: {msg.ReadString()}");
        }

        public void KickPlayer(Guid playerId)
        {
            PlayerManager playerManager = serviceLocator.LocateService<PlayerManager>();

            playerManager.DestroyPlayer(playerId, DisconnectReason.KickedOut);
            DisconnectRequestPacket disconnectRequest = new DisconnectRequestPacket(DisconnectReason.KickedOut);
            SendPacketToPlayer(disconnectRequest, playerId);
        }

        // Can be used to specify custom authoriation logic
        // This makes it possible to, for example, IP ban players before they start using server resources
        // By default we accept all player connections
        public bool AuthorizeConnection(NetPeer peer, NetIncomingMessage msg)
        {
            return true;
        }

        public void ProcessPacket(Guid sourcePlayerId, Packet packet)
        {
            if (!router.ProcessPacket(sourcePlayerId, packet))
            {
#if DEBUG
                Console.WriteLine("Unknown packet dropped: " + packet.GetType().FullName);
#else
                Console.WriteLine("Unknown packet dropped: " + packet.GetType().Name);
#endif
            }
        }

        public bool SendPacketToAll(Packet packet, ChannelType channelType = ChannelType.ReliableOrdered)
        {
#if DEBUG
            Console.WriteLine($"Send - Type: {packet.GetType().Name}");
#endif

            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write(packet.Serialize());
            NetDeliveryMethod deliveryMethod = ChannelTypeUtils.ChannelTypeToLidgren(channelType);
            server.SendToAll(msg, deliveryMethod, 1);
            return true;
        }

        public bool SendPacketToPlayer(Packet packet, Guid playerId, ChannelType channelType = ChannelType.ReliableOrdered)
        {
#if DEBUG
            Console.WriteLine($"Send - Type: {packet.GetType().Name}");
#endif
            if (!playerConnections.ContainsKey(playerId))
            { 
                Console.WriteLine("Failed to send packet to player: Recipient connection ID not found.");
                return false;
            }

            long recipientId = playerConnections[playerId];

            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write(packet.Serialize());
            NetDeliveryMethod deliveryMethod = ChannelTypeUtils.ChannelTypeToLidgren(channelType);
            server.SendMessage(msg, server.Connections.First(p => p.RemoteUniqueIdentifier == recipientId), deliveryMethod, 1);
            return true;
        }

        public bool SendPacketToAllExcept(Packet packet, Guid playerId, ChannelType channelType = ChannelType.ReliableOrdered)
        {
#if DEBUG
            Console.WriteLine($"Send - Type: {packet.GetType().Name}");
#endif
            if (!playerConnections.ContainsKey(playerId)) 
            { 
                Console.WriteLine("Failed to send packet to players: Excluded player not found."); 
                return false;
            }

            long excludedId = playerConnections[playerId];

            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write(packet.Serialize());
            NetDeliveryMethod deliveryMethod = ChannelTypeUtils.ChannelTypeToLidgren(channelType);
            server.SendToAll(msg, server.Connections.First(p => p.RemoteUniqueIdentifier == excludedId), deliveryMethod, 1);
            return true;
        }
    }
}
