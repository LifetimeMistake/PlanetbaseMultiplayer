using Planetbase;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Autofac;
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
    public class WorldDataManager : IWorldDataManager
    {
        private ServiceLocator serviceLocator;
        private Client client;
        public bool IsInitialized { get; private set; }

        public WorldDataManager(ServiceLocator serviceLocator, Client client)
        {
            this.serviceLocator = serviceLocator;
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

        public void LoadWorldData(WorldData worldData)
        {
            // Planetbase only supports loading save data from a file
            // instead of rewriting a lot of game logic, we compromise
            string tmpPath = Path.GetTempFileName();
            File.WriteAllText(tmpPath, worldData.XmlData);
            SaveData save = new SaveData(tmpPath, DateTime.Now);
            GameManager.getInstance().setNewState(new GameStateGame(save.getPath(), save.getPlanetIndex(), null));

            // Deserialize services

            foreach (IPersistent persistent in serviceLocator.LocateServicesOfType<IPersistent>())
            {
                try
                {
                    persistent.Load(worldData);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load world data into persistent manager \"{persistent.GetType().Name}\": {ex}", ex);
                }
            }
        }

        public WorldData SaveWorldData()
        {
            GameStateGame gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            string xmlData = WorldSerializer.Serialize(gameStateGame);
            WorldData worldData = new WorldData();
            worldData.XmlData = xmlData;

            // Serialize services

            foreach (IPersistent persistent in serviceLocator.LocateServicesOfType<IPersistent>())
            {
                try
                {
                    persistent.Save(worldData);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to save world data from persistent manager \"{persistent.GetType().Name}\": {ex}", ex);
                }
            }
            return worldData;
        }
    }
}
