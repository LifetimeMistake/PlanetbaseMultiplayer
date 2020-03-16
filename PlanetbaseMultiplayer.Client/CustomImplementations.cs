using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client
{
    public static class MultiplayerMethods
    {
        public static void RecycleSelectable(RecycleSelectableDataPackage pkg)
        {
            Selectable selectable = MultiplayerUtil.FindSelectableFromId(pkg.SelectableId);
            if (selectable == null) { UnityEngine.Debug.LogError($"Could not find selectable object with Id: {pkg.SelectableId}"); return; }
            if (selectable != null)
                selectable.destroy();
        }

        public static void CharacterStartWalking(CharacterStartWalkingDataPackage pkg)
        {
            Character character = Character.find(pkg.CharacterId);
            if(character == null) { UnityEngine.Debug.LogError("CharacterStartWalking: character was null"); return; }
            Target target = new Target((Vector3)pkg.TargetPosition, pkg.TargetLocation);
            Selectable selectable = MultiplayerUtil.FindSelectableFromId(pkg.TargetSelectableId);
            if (selectable != null) target.mSelectable = selectable;
            target.mRadius = pkg.TargetRadius;
            if(!pkg.TargetRotation.IsEmpty)
                target.mRotation = (Quaternion)pkg.TargetRotation;
            target.mFlags = pkg.TargetFlags;

            List<Selectable> indirectTargets_list = new List<Selectable>();
            foreach(int indirectTarget_id in pkg.IndirectTargetIds)
            {
                Selectable s = MultiplayerUtil.FindSelectableFromId(indirectTarget_id);
                if (s != null) indirectTargets_list.Add(s);
                else UnityEngine.Debug.LogWarning("CharacterStartWalking: s was null");
            }

            character.setPosition((Vector3)pkg.StartingPosition);
            character.setRotation((Quaternion)pkg.StartingRotation);
            character.MP_startWalking(target, indirectTargets_list.Count == 0 ? null : indirectTargets_list.ToArray());
        }

        public static void BuildableBuilt(BuildableBuiltDataPackage pkg)
        {
            Selectable buildableBase = MultiplayerUtil.FindSelectableFromId(pkg.BuildableId);
            if (buildableBase == null) { UnityEngine.Debug.LogError("BuildableBuilt: buildableBase was null"); return; }
            if (!(buildableBase is Buildable)) { UnityEngine.Debug.LogError("BuildableBuilt: cannot cast Selectable to Buildable"); return; }
            Buildable buildable = (Buildable)buildableBase;
            if (buildable.mState != BuildableState.Built)
            {
                buildable.mBuildProgress.setValue(1f);
                buildable.updateBuild(0f);
                buildable.onBuilt();
            }
        }

        public static void DecideNextSandstorm(DecideNextSandstormDataPackage pkg)
        {
            DisasterManager.getInstance().getSandstorm().mTimeToNextSandstorm = pkg.mTimeToNextSandstorm;
        }

        public static void TriggerSandstorm(TriggerSandstormDataPackage pkg)
        {
            Sandstorm sandstorm = DisasterManager.getInstance().getSandstorm();
            sandstorm.mTime = 0f;
            sandstorm.mSandstormTime = pkg.mSandstormTime;
            sandstorm.mSandstormInProgress = true;
            sandstorm.onStart();
        }

        public static void EndSandstorm()
        {
            Sandstorm sandstorm = DisasterManager.getInstance().getSandstorm();
            sandstorm.mSandstormInProgress = false;
            Singleton<EnvironmentManager>.getInstance().refreshAmbientSound();
            sandstorm.destroyParticles();
        }
    }
}
