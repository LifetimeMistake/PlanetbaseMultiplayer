using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class PlayerDataUpdatedPacket : Packet
    {
        public Guid PlayerId;
        public Player Player;

        public PlayerDataUpdatedPacket(Guid playerId, Player player)
        {
            PlayerId = playerId;
            Player = player;
        }
    }
}
