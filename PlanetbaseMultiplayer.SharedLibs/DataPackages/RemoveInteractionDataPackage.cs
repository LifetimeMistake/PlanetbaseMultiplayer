using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class RemoveInteractionDataPackage : IDataPackage
    {
        public Guid InteractionId;

        public RemoveInteractionDataPackage(Guid interactionId)
        {
            InteractionId = interactionId;
        }
    }
}
