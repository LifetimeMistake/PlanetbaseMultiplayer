using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Buildable), "onBuilt")]
    class Sync_BuildableOnBuild
    {
        static bool Prefix(Buildable __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return true;
            if (Globals.LocalPlayer.ClientState != ClientState.ConnectedReady) return true;
            Globals.LocalClient.OnBuildableBuilt(__instance.getId());
            return true;
        }
    }
}
