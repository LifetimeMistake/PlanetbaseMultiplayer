using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterStartWalkingDataPackage : IDataPackage
    {
        public int CharacterId;
        public int TargetFlags;
        public float TargetRadius;
        public Location TargetLocation;
        public Vector3_Serializable TargetPosition;
        public Quaternion_Serializable TargetRotation;
        public int TargetSelectableId;
        public int[] IndirectTargetIds;

        public CharacterStartWalkingDataPackage(int characterId, int targetFlags, float targetRadius, Location targetLocation, Vector3_Serializable targetPosition, Quaternion_Serializable targetRotation, int targetSelectableId, int[] indirectTargetIds)
        {
            CharacterId = characterId;
            TargetFlags = targetFlags;
            TargetRadius = targetRadius;
            TargetLocation = targetLocation;
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;
            TargetSelectableId = targetSelectableId;
            IndirectTargetIds = indirectTargetIds;
        }
    }
}
