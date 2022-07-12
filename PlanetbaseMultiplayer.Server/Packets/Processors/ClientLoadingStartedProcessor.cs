using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class ClientLoadingStartedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(ClientLoadingStartedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            ServerProcessorContext processorContext = (ServerProcessorContext)context;
            PlayerManager playerManager = processorContext.ServiceLocator.LocateService<PlayerManager>();

            if (!playerManager.PlayerExists(sourcePlayerId))
                return; // what

            Player player = playerManager.GetPlayer(sourcePlayerId);
            player.State = PlayerState.ConnectedLoadingData;
            playerManager.UpdatePlayer(player);
        }
    }
}
