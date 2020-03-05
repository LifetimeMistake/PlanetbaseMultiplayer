using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    // Applies fixes for crashes caused by network delay.
    [HarmonyPatch(typeof(InteractionBuild), "update", new[] { typeof(float) })]
    class Fix_Interaction_Crashes
    {
        static bool Prefix(InteractionBuild __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Buildable buildable = (Buildable)__instance.mSelectable;
            if (buildable.mState == BuildableState.Built) return false;
            return true;
        }
    }
    [HarmonyPatch(typeof(Resource), "destroy")]
    class Fix_Resource_Crash
    {
        static bool Prefix(Resource __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            try
            {
                Resource.mResourceDictionary.Remove(__instance.mObject);
            }
            catch(Exception) { }
            try
            {
                Resource.mResources.Remove(__instance);
            }
            catch (Exception) { }
            try
            {
                Resource.mTypeResources[__instance.mResourceType].Remove(__instance);
            }
            catch (Exception) { }
            try
            {
                __instance.end();
            }
            catch (Exception) { }
            return false;
        }
    }
}
