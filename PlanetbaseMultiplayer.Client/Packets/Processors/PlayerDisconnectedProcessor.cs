using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
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

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            PlayerDisconnectedPacket playerDisconnectedPacket = (PlayerDisconnectedPacket)packet;
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();

            if (client.LocalPlayer.HasValue)
            {
                if (client.LocalPlayer.Value.Id != playerDisconnectedPacket.PlayerId)
                {
                    Player player = playerManager.GetPlayer(playerDisconnectedPacket.PlayerId);
                    string reason = DisconnectReasonUtils.ReasonToString(playerDisconnectedPacket.Reason);
                    MessageLog.Show($"Player {player.Name} left the game: {reason}", null, MessageLogFlags.MessageSoundNormal);
                }

                if (client.LocalPlayer.Value.Id == playerDisconnectedPacket.PlayerId)
                {
                    // The server has freed up resources associated with our player
                    // This may mean that we requested a disconnect
                    // or we're in for a big surprise
                    client.LocalPlayer = null;
                }
            }

            playerManager.OnPlayerRemoved(playerDisconnectedPacket.PlayerId);
        }
    }
}
