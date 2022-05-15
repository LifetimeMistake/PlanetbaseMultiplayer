using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets
{
    [Serializable]
    public abstract class Packet
    {
        private static BinaryFormatter serializer;

        static Packet()
        {
            serializer = new BinaryFormatter();
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static Packet Deserialize(byte[] data)
        {
            using (Stream stream = new MemoryStream(data))
            {
                return (Packet)serializer.Deserialize(stream);
            }
        }
    }
}
