using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class ClientStatePackage : IDataPackage
    {
        public ClientState State;

        public ClientStatePackage(ClientState state)
        {
            State = state;
        }
    }
}
