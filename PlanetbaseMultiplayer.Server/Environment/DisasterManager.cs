using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Environment
{
    public class DisasterManager : IDisasterManager
    {
        private Server server;
        private Disaster? disaster;
        public bool IsInitialized { get; private set; }

        public DisasterManager(Server server)
        {
            this.server = server;
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public bool AnyDisasterInProgress()
        {
            return disaster != null;
        }

        public Disaster? GetDisasterInProgress()
        {
            return disaster;
        }

        public float GetCurrentTime()
        {
            if (disaster == null)
                return 0f;
            return disaster.Value.CurrentTime;
        }

        public void CreateDisaster(Disaster disaster)
        {
            this.disaster = disaster;
            CreateDisasterPacket createDisasterPacket = new CreateDisasterPacket(disaster);
            server.SendPacketToAll(createDisasterPacket);
        }

        public void CreateDisaster(DisasterType disasterType, float disasterLength, float currentTime)
        {
            if (AnyDisasterInProgress())
                return;

            Disaster disaster = new Disaster(disasterType, disasterLength, currentTime);
            CreateDisaster(disaster);
        }

        public void UpdateDisaster(float currentTime)
        {
            if (!AnyDisasterInProgress())
                return;

            Disaster disaster = this.disaster.Value;
            disaster.CurrentTime = currentTime;

            UpdateDisasterPacket updateDisasterPacket = new UpdateDisasterPacket(currentTime);
            server.SendPacketToAll(updateDisasterPacket);
        }

        public void EndDisaster()
        {
            if (!AnyDisasterInProgress())
                return;

            disaster = null;
            EndDisasterPacket endDisasterPacket = new EndDisasterPacket();
            server.SendPacketToAll(endDisasterPacket);
        }
    }
}
