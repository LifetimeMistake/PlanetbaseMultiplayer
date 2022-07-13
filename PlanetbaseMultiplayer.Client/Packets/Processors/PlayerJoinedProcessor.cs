using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.UI;
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
    public class PlayerJoinedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(PlayerJoinedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            PlayerJoinedPacket playerJoinedPacket = (PlayerJoinedPacket)packet;
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();

            playerManager.OnPlayerAdded(playerJoinedPacket.Player);

            MessageLogFlags flags;
            if (playerJoinedPacket.Player.Name.ToLower() == "freddy")
                flags = MessageLogFlags.MessageSoundPowerDown;
            else
                flags = MessageLogFlags.MessageSoundNormal;

            MessageLog.Show($"Player is joining game: {playerJoinedPacket.Player.Name}", null, flags);
        }
    }
}