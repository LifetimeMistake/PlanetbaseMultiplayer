using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
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

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            PlayerJoinedPacket playerJoinedPacket = (PlayerJoinedPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;

            processorContext.Client.PlayerManager.OnPlayerAdded(playerJoinedPacket.Player);

            MessageLogFlags flags;
            if (playerJoinedPacket.Player.Name == "freddy")
                flags = MessageLogFlags.MessageSoundPowerDown;
            else
                flags = MessageLogFlags.MessageSoundNormal;

            MessageLog.Show($"Player is joining game: {playerJoinedPacket.Player.Name}", null, flags);
        }
    }
}
