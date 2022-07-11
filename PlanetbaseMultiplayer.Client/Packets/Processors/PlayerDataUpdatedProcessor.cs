using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class PlayerDataUpdatedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(PlayerDataUpdatedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            PlayerDataUpdatedPacket playerDataUpdatedPacket = (PlayerDataUpdatedPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;
            PlayerManager playerManager = processorContext.ServiceLocator.LocateService<PlayerManager>();

            playerManager.OnPlayerUpdated(playerDataUpdatedPacket.PlayerId, playerDataUpdatedPacket.Player);
            if (playerDataUpdatedPacket.Player == processorContext.Client.LocalPlayer)
            {
                // Update the local client data
                processorContext.Client.LocalPlayer = playerDataUpdatedPacket.Player;
            }
        }
    }
}
