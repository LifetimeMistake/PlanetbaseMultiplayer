using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
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

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            PlayerDataUpdatedPacket playerDataUpdatedPacket = (PlayerDataUpdatedPacket)packet;
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();

            playerManager.OnPlayerUpdated(playerDataUpdatedPacket.PlayerId, playerDataUpdatedPacket.Player);
            if (playerDataUpdatedPacket.Player == client.LocalPlayer)
            {
                // Update the local client data
                client.LocalPlayer = playerDataUpdatedPacket.Player;
            }
        }
    }
}
