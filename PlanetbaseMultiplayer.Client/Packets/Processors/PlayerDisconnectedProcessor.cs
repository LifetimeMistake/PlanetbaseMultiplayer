using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class PlayerDisconnectedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(PlayerDisconnectedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            PlayerDisconnectedPacket playerDisconnectedPacket = (PlayerDisconnectedPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;
            PlayerManager playerManager = processorContext.ServiceLocator.LocateService<PlayerManager>();

            if (processorContext.Client.LocalPlayer.HasValue)
            {
                if (processorContext.Client.LocalPlayer.Value.Id != playerDisconnectedPacket.PlayerId)
                {
                    Player player = playerManager.GetPlayer(playerDisconnectedPacket.PlayerId);
                    string reason = DisconnectReasonUtils.ReasonToString(playerDisconnectedPacket.Reason);
                    MessageLog.Show($"Player {player.Name} left the game: {reason}", null, MessageLogFlags.MessageSoundNormal);
                }

                if (processorContext.Client.LocalPlayer.Value.Id == playerDisconnectedPacket.PlayerId)
                {
                    // The server has freed up resources associated with our player
                    // This may mean that we requested a disconnect
                    // or we're in for a big surprise
                    processorContext.Client.LocalPlayer = null;
                }
            }

            playerManager.OnPlayerRemoved(playerDisconnectedPacket.PlayerId);
        }
    }
}
