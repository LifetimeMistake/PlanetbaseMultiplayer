using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Server.Players;
using PlanetbaseMultiplayer.Server.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class DisconnectRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(DisconnectRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Server server = context.ServiceLocator.LocateService<Server>();
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();
            TimeManager timeManager = context.ServiceLocator.LocateService<TimeManager>();

            Console.WriteLine($"Player {sourcePlayerId} requested a graceful disconnect");
            playerManager.DestroyPlayer(sourcePlayerId, DisconnectReason.DisconnectRequest);

            // We reply with another disconnect request to let the client know that we're ready for a graceful disconnect
            DisconnectRequestPacket disconnectReply = new DisconnectRequestPacket(DisconnectReason.DisconnectRequestResponse);
            server.SendPacketToPlayer(disconnectReply, sourcePlayerId);

            server.playerConnections.Remove(sourcePlayerId);
            if (playerManager.GetPlayers().Count(p => p.State == PlayerState.ConnectedLoadingData) == 0)
                timeManager.UnfreezeTime();
        }
    }
}
