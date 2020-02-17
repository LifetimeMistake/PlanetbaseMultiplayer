using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public class Packet
    {
        public PacketType Type;
        public IDataPackage Data;

        public Packet(PacketType type, IDataPackage data)
        {
            Type = type;
            Data = data;
        }

        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static Packet Deserialize(byte[] source)
        {
            using (var ms = new MemoryStream(source))
            {
                var formatter = new BinaryFormatter();
                return (Packet)formatter.Deserialize(ms);
            }
        }
    }
}