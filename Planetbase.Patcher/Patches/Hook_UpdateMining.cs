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
            return true;
        }
    }
}
