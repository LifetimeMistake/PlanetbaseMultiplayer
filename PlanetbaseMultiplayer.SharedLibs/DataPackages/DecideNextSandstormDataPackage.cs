using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class DecideNextSandstormDataPackage : IDataPackage
    {
        public float mTimeToNextSandstorm;

        public DecideNextSandstormDataPackage(float mTimeToNextSandstorm)
        {
            this.mTimeToNextSandstorm = mTimeToNextSandstorm;
        }
    }
}
