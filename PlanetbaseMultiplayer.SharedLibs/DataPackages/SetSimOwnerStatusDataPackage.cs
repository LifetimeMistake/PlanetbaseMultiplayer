using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class SetSimOwnerStatusDataPackage : IDataPackage
    {
        public bool IsSimulationOwner;

        public SetSimOwnerStatusDataPackage(bool isSimulationOwner)
        {
            IsSimulationOwner = isSimulationOwner;
        }
    }
}
