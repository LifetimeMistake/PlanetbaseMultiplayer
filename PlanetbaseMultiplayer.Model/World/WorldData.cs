using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.World.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    [Serializable]
    public class WorldData
    {
        private static BinaryFormatter serializer;

        public string XmlData; // Will be removed once we can handle loading the world on our own
        public DisasterData Disasters; // Incomplete, only handles enough to sync the current disaster
        public EnvironmentData Environment;

        static WorldData()
        {
            serializer = new BinaryFormatter();
        }

        public bool IsValid()
        {
            return XmlData != null && 
                Disasters != null && 
                Environment != null;
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static WorldData Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return (WorldData)serializer.Deserialize(stream);
            }
        }

        public static WorldData Deserialize(Stream stream, bool leaveStreamOpen = true)
        {
            WorldData data = (WorldData)serializer.Deserialize(stream);
            if (!leaveStreamOpen)
                stream.Close();

            return data;
        }
    }
}
