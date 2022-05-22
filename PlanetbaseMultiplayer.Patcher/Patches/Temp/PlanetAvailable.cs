using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Temp
{
    [HarmonyPatch(typeof(Planet), "isAvailable")]
    public class PlanetAvailable
    {
        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Planet), "getMilestonesToUnlock")]
    public class Milestones
    {
        static bool Prefix(ref int __result)
        {
            __result = 0;
            return false;
        }
    }
}
