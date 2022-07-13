using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Players;
using PlanetbaseMultiplayer.Server.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class ClientReadyProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(ClientReadyPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();
            TimeManager timeManager = context.ServiceLocator.LocateService<TimeManager>();

            if (!playerManager.PlayerExists(sourcePlayerId))
                return; // what

            Player player = playerManager.GetPlayer(sourcePlayerId);
            player.State = PlayerState.ConnectedReady;
            playerManager.UpdatePlayer(player);

            if (playerManager.GetPlayers().Count(p => p.State == PlayerState.ConnectedLoadingData) == 0)
                timeManager.UnfreezeTime();
        }
    }
}
