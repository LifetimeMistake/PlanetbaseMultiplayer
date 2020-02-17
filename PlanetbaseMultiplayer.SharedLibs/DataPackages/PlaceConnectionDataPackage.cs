using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class PlaceConnectionDataPackage : IDataPackage
    {
        public int Module1_Id;
        public int Module2_Id;

        public PlaceConnectionDataPackage(int module1_Id, int module2_Id)
        {
            Module1_Id = module1_Id;
            Module2_Id = module2_Id;
        }
    }
}
