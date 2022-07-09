using PlanetbaseMultiplayer.Client.Environment.Disasters;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.World;
using PlanetbaseMultiplayer.Model.World.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Environment
{
    public class DisasterManager : IDisasterManager
    {
        private Client client;
        private IDisasterProxy disasterProxy;
        private Disaster? disaster;
        public bool IsInitialized { get; private set; }


        public DisasterManager(Client client)
        {
            this.client = client;
        }

        public bool Initialize()
        {
            IsInitialized = true;
            return true;
        }

        public bool AnyDisasterInProgress()
        {
            return disaster != null;
        }

        public Disaster? GetDisasterInProgress()
        {
            return disaster;
        }

        public IDisasterProxy GetCurrentDisasterProxy()
        {
            return disasterProxy;
        }

        public float GetCurrentTime()
        {
            if (disaster == null)
                return 0f;
            return disaster.Value.CurrentTime;
        }

        public void OnCreateDisaster(Disaster disaster)
        {
            this.disaster = disaster;
            Planetbase.DisasterManager disasterManager = Planetbase.DisasterManager.getInstance();
            
            switch (disaster.Type)
            {
                case DisasterType.Sandstorm:
                    disasterProxy = new SandstormProxy(disaster.CurrentTime, disaster.DisasterLength, disasterManager.getSandstorm());
                    break;
                case DisasterType.Blizzard:
                    disasterProxy = new BlizzardProxy(disaster.CurrentTime, disaster.DisasterLength, disasterManager.getBlizzard());
                    break;
                case DisasterType.SolarFlare:
                    disasterProxy = new SolarFlareProxy(disaster.CurrentTime, disaster.DisasterLength, disasterManager.getSolarFlare());
                    break;
                default:
                    throw new ArgumentException("Unknown disaster type");
            }
            disasterProxy.StartDisaster();
        }

        public void CreateDisaster(Disaster disaster)
        {
            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            CreateDisasterPacket createDisasterPacket = new CreateDisasterPacket(disaster);
            client.SendPacket(createDisasterPacket);
        }

        public void CreateDisaster(DisasterType disasterType, float disasterLength, float currentTime)
        {
            if (AnyDisasterInProgress())
                return;

            Disaster disaster = new Disaster(disasterType, disasterLength, currentTime);
            CreateDisaster(disaster);
        }

        public void OnUpdateDisaster(float currentTime)
        {
            if (this.disaster == null || this.disasterProxy == null)
                return;

            Disaster disaster = this.disaster.Value;
            disaster.CurrentTime = currentTime;
            this.disaster = disaster; // Nullable structs are a pain

            disasterProxy.UpdateDisaster(currentTime);
        }

        public void UpdateDisaster(float currentTime)
        {
            if (!AnyDisasterInProgress())
                return;

            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            UpdateDisasterPacket updateDisasterPacket = new UpdateDisasterPacket(currentTime);
            client.SendPacket(updateDisasterPacket);
        }

        public void OnEndDisaster()
        {
            disasterProxy.EndDisaster();
            disaster = null;
            disasterProxy = null;
        }

        public void EndDisaster()
        {
            if (!AnyDisasterInProgress())
                return;

            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            EndDisasterPacket endDisasterPacket = new EndDisasterPacket();
            client.SendPacket(endDisasterPacket);
        }

        public bool Serialize(XmlDocument document)
        {
            return true;
        }

        public bool Deserialize(XmlDocument document)
        {
            return true;
        }

        public bool Save(WorldData world)
        {
            DisasterData disasterData = new DisasterData(disaster);
            world.Disasters = disasterData;
            return true;
        }

        public bool Load(WorldData world)
        {
            if (world.Disasters.CurrentDisaster.HasValue)
            {
                OnCreateDisaster(world.Disasters.CurrentDisaster.Value);
            }
            else
            {
                disaster = world.Disasters.CurrentDisaster;
            }

            return true;
        }
    }
}