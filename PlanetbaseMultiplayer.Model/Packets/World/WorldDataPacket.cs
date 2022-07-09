using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.World
{
    [Serializable]
    public class WorldDataPacket : Packet
    {
        public WorldData World;

        public WorldDataPacket(WorldData world)
        {
            World = world;
        }
    }
}
