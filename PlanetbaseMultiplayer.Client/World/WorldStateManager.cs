using Planetbase;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.World
{
    public class WorldStateManager : IWorldStateManager
    {
        private Client client;
        private WorldStateData worldStateData;
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

        public void UpdateWorldData(WorldStateData worldStateData)
        {
            this.worldStateData = worldStateData;
            // Planetbase only supports loading save data from a file
            // instead of rewriting a lot of game logic, we compromise
            string tmpPath = Path.GetTempFileName();
            File.WriteAllText(tmpPath, worldStateData.XmlData);
            SaveData save = new SaveData(tmpPath, DateTime.Now);
            GameManager.getInstance().setNewState(new GameStateGame(save.getPath(), save.getPlanetIndex(), null));
        }

        public WorldStateData GetWorldData()
        {
            return worldStateData;
        }
    }
}
