using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Module), "updateMining", new[] { typeof(float) })]
    class Hook_UpdateMining
    {
        static bool Prefix(Module __instance, float timeStep)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            if(__instance.hasFlag(2) && (__instance as Selectable).hasInteraction<InteractionWork>() && (__instance as Buildable).isBuilt())
            {
                __instance.mProductionProgressIndicator.setIcon(ResourceTypeList.OreInstance.getIcon());
                if (!__instance.mProductionProgressIndicator.isValidValue())
                    __instance.mProductionProgressIndicator.setValue(0f);
                Character character = __instance.mInteractions[0].getCharacter();
                float num = 0.5f + (float)(__instance as Selectable).getInteractionCount() * 0.5f;
                if(__instance.mProductionProgressIndicator.increase(timeStep * character.getWorkSpeed() * num / 150f))
                {
                    __instance.mProductionProgressIndicator.setValue(0f);
                    Vector3 position;
                    if(__instance.findValidProductionPosition(out position))
                    {
                        ResourceConstructionData data = new ResourceConstructionData(ResourceTypeList.OreInstance.GetType().Name, ResourceSubtype.None,
                            (Vector3_Serializable)position, (Quaternion_Serializable)__instance.mObject.transform.rotation, Location.Exterior, false);
                        Globals.LocalClient.OnProductionCompleted(__instance, ProducerType.Module, new ResourceConstructionData[] { data }, new ResourceDestructionData[] { });
                    }
                }
            }
            return false;
        }
    }
}
