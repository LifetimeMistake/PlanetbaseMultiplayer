using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class PlayerJoinedPacket : Packet
    {
        public Player Player;

        public PlayerJoinedPacket(Player player)
        {
            Player = player;
        }
    }
}
