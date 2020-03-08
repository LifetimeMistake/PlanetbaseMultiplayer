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
    [HarmonyPatch(typeof(ConstructionComponent), "completeProduction")]
    class Override_completeProduction_Patch
    {
        static bool Prefix(ConstructionComponent __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            return true;
        }
    }
}
