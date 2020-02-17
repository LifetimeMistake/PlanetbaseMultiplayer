using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public class Player
    {
        public long UniqueId;
        public ClientState ClientState;
        public bool IsSimulationOwner;

        public Player(long uniqueId, ClientState clientState, bool isSimulationOwner)
        {
            UniqueId = uniqueId;
            ClientState = clientState;
            IsSimulationOwner = isSimulationOwner;
        }
    }

    public enum ClientState
    {
        ConnectedUnauthorized = 0,
        ConnectedWaitingForAuth = 1,
        WaitingForSaveData = 2,
        LoadingSaveData = 3,
        ConnectedReady = 4
    }
}
