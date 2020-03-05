using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class TriggerSandstormDataPackage : IDataPackage
    {
        public float mSandstormTime;

        public TriggerSandstormDataPackage(float mSandstormTime)
        {
            this.mSandstormTime = mSandstormTime;
        }
    }
}
