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
}
