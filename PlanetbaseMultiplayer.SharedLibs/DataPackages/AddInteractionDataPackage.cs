using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class AddInteractionDataPackage : IDataPackage
    {
        public Guid InteractionId;
        public string InteractionType;
        public string XmlData;
        public Vector3_Serializable CharacterPosition;
        public Quaternion_Serializable CharacterRotation;

        public AddInteractionDataPackage(Guid interactionId, string interactionType, string xmlData, Vector3_Serializable characterPosition, Quaternion_Serializable characterRotation)
        {
            InteractionId = interactionId;
            InteractionType = interactionType;
            XmlData = xmlData;
            CharacterPosition = characterPosition;
            CharacterRotation = characterRotation;
        }
    }
}
