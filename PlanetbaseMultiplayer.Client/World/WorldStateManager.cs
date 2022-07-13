using Planetbase;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Client.World
{
    public class WorldStateManager : IWorldStateManager
    {
        private Client client;
        private WorldData worldStateData;
        public bool IsInitialized { get; private set; }

        public WorldStateManager(Client client)
        {
            this.client = client;
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public bool RequestWorldData()
        {
            WorldDataRequestPacket worldDataRequestPacket = new WorldDataRequestPacket();
            client.SendPacket(worldDataRequestPacket);
            return true;
        }

        public void UpdateWorldData(WorldData worldStateData)
        {
            this.worldStateData = worldStateData;
            XmlDocument document = new XmlDocument();
            document.LoadXml(worldStateData.XmlData);
            client.DisasterManager.Deserialize(document);
            // Planetbase only supports loading save data from a file
            // instead of rewriting a lot of game logic, we compromise
            string tmpPath = Path.GetTempFileName();
            File.WriteAllText(tmpPath, worldStateData.XmlData);
            SaveData save = new SaveData(tmpPath, DateTime.Now);
            GameManager.getInstance().setNewState(new GameStateGame(save.getPath(), save.getPlanetIndex(), null));
        }

        public WorldData GetWorldData()
        {
            return worldStateData;
        }
    }
}
