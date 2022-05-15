using Lidgren.Network;
using Planetbase;
using PlanetbaseMultiplayer.Client.GameStates;
using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Collections;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Model.Utils;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client
{
    public class Client
    {
        private GameStateMultiplayer gameStateMultiplayer;
        private ConnectionOptions connectionOptions;
        private NetClient client;
        private ConcurrentQueue<Packet> packetQueue;
        private Player localPlayer;
        private PacketRouter router;
        private ClientProcessorContext processorContext;

        private PlayerManager playerManager;
        private SimulationManager simulationManager;
        private Time.TimeManager timeManager;

        public Player LocalPlayer { get { return localPlayer; } }
        public PlayerManager PlayerManager { get { return playerManager; } }
        public SimulationManager SimulationManager { get { return SimulationManager; } }
        public Time.TimeManager TimeManager { get { return timeManager; } }

        public Client(GameStateMultiplayer gameStateMultiplayer)
        {
            this.gameStateMultiplayer = gameStateMultiplayer;
            packetQueue = new ConcurrentQueue<Packet>();
            processorContext = new ClientProcessorContext(this);

            router = new PacketRouter(processorContext);
            foreach (PacketProcessor packetProcessor in PacketProcessor.GetProcessors())
                router.RegisterPacketProcessor(packetProcessor);

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            NetPeerConfiguration config = new NetPeerConfiguration("PlanetbaseMultiplayer");
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            client.RegisterReceivedCallback(new SendOrPostCallback(MessageReceived));

            playerManager = new PlayerManager(this);
            simulationManager = new SimulationManager(this);
            timeManager = new Time.TimeManager(this);
            Initialize();
        }

        private void Initialize()
        {
            playerManager.Initialize();
            simulationManager.Initialize();
            timeManager.Initialize();
        }

        public bool Connect(ConnectionOptions connectionOptions)
        {
            this.connectionOptions = connectionOptions;
            if (client.Status != NetPeerStatus.Running)
                client.Start();

            client.Connect(connectionOptions.Host, connectionOptions.Port);
            return true;
        }

        public void RequestDisconnect(DisconnectReason reason = DisconnectReason.DisconnectRequest)
        {
            DisconnectRequestPacket disconnectRequestPacket = new DisconnectRequestPacket(reason);
            SendPacket(disconnectRequestPacket);
        }

        public void Disconnect()
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
                client.Disconnect("Disconnected");

            if (client.Status == NetPeerStatus.Running)
                client.Shutdown("Disconnected");
        }

        // Handle incoming messages
        // Data packets are enqueued for later processing
        public void MessageReceived(object obj)
        {
            NetPeer peer = obj as NetPeer;
            NetIncomingMessage msg = peer.ReadMessage();
            switch (msg.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    OnIncomingStatusChange(peer, msg);
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    OnIncomingDiscoveryResponse(peer, msg);
                    break;
                case NetIncomingMessageType.Data:
                    OnIncomingData(peer, msg);
                    break;
                case NetIncomingMessageType.DebugMessage:
                    OnIncomingDebugMessage(peer, msg);
                    break;
                default:
                    Debug.Log($"Unhandled packet received: {msg.MessageType}, Data: {Encoding.UTF8.GetString(msg.ReadBytes(msg.LengthBytes))}");
                    break;
            }
        }

        private void OnIncomingDiscoveryResponse(NetPeer peer, NetIncomingMessage msg)
        {
            string serverType = msg.ReadString();
            if (serverType != "PlanetbaseMultiplayer.Server")
            {
                Debug.Log("Received response was not from a valid server type");
                return;
            }
            Debug.Log($"Found server of type {serverType} at {msg.SenderEndPoint}! Connecting!");
            client.Connect(msg.SenderEndPoint);
        }

        private void OnIncomingStatusChange(NetPeer peer, NetIncomingMessage msg)
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                Debug.Log("Client connected!");
                SessionDataRequestPacket sessionDataRequestPacket = new SessionDataRequestPacket();
                SendPacket(sessionDataRequestPacket);
                Debug.Log("Sending session data request");
            }
            if (client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                Debug.Log("Lost connection with the game server!");
                if (GameManager.getInstance().getGameState() is GameStateGame)
                {
                    GameStateGame state = GameManager.getInstance().getGameState() as GameStateGame;
                    MethodInfo onExitGameForRealInfo;
                    if (!Reflection.TryGetPrivateMethod(state.GetType(), "onExitGameForReal", true, out onExitGameForRealInfo))
                    {
                        Debug.LogError("Failed to find \"onExitGameForReal\"");
                        return;
                    }

                    void wrapper(object parameter)
                    {
                        onExitGameForRealInfo.Invoke(state, new object[] { parameter });
                    }

                    ShowMessageBox(new GuiDefinitions.Callback(wrapper), "Connection terminated", "Lost connection with the game server.");
                }
            }
        }

        private void OnIncomingData(NetPeer peer, NetIncomingMessage msg)
        {
            try
            {
                Packet packet = Packet.Deserialize(msg.ReadBytes(msg.LengthBytes));
                packetQueue.Enqueue(packet);
            }
            catch (Exception e)
            {
                Debug.Log($"Error while receiving packet: {e.Message}");
            }
        }

        private void OnIncomingDebugMessage(NetPeer peer, NetIncomingMessage msg)
        {
            Debug.Log($"DEBUG: {msg.ReadString()}");
        }

        public void OnSessionDataReceived(SessionData session)
        {
            Debug.Log("Received session data: " + session.ServerName + ", PasswordProtected: "
                + session.PasswordProtected + ", Player count: " + session.PlayerCount);

            AuthenticateRequestPacket authenticateRequestPacket;
            if (session.PasswordProtected)
            {
                authenticateRequestPacket = new AuthenticateRequestPacket(connectionOptions.Username, connectionOptions.Password);
            }
            else
            {
                authenticateRequestPacket = new AuthenticateRequestPacket(connectionOptions.Username, null);
            }

            Debug.Log("Sending authenticate request");
            SendPacket(authenticateRequestPacket);
        }

        public void OnAuthenticationSuccessful(Player localPlayer, Player[] players)
        {
            this.localPlayer = localPlayer;
            foreach (Player player in players)
                playerManager.OnPlayerAdded(player); // Sync players

            Debug.Log("Sending world data request");
            WorldDataRequestPacket worldDataRequestPacket = new WorldDataRequestPacket();
            SendPacket(worldDataRequestPacket);
        }

        public void OnWorldDataReceived(WorldStateData world)
        {

        }

        // Called by a patch in FixedUpdate, processes all packets currently in the queue
        // This has to be done to avoid race condition crashes
        public void ProcessPackets()
        {
            while(packetQueue.Count > 0)
            {
                Packet packet = packetQueue.Dequeue();
                if(!router.ProcessPacket(Guid.Empty, packet))
                {
                    Debug.Log("Unhandled packet received: " + packet.GetType().FullName);
                }
            }
        }

        public void SendPacket(Packet packet, ChannelType channelType = ChannelType.ReliableOrdered)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write(packet.Serialize());
#if DEBUG
            Debug.Log($"Send packet: {packet.GetType().FullName}");
#endif
            NetDeliveryMethod deliveryMethod = ChannelTypeUtils.ChannelTypeToLidgren(channelType);
            client.SendMessage(msg, deliveryMethod);
        }

        public void ShowMessageBox(GuiDefinitions.Callback callback, string title, string text)
        {
            gameStateMultiplayer.ShowMessageBox(callback, title, text);
        }
    }
}
