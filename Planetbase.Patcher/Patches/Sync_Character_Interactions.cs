using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Interaction), "updateAll", new[] { typeof(float) })]
    class Override_UpdateAll_Interactions
    {
        static bool Prefix(float timeStep)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Globals.InteractionManager.UpdateAll(timeStep);
            return false;
        }
    }
    [HarmonyPatch(typeof(Interaction), "postInit")]
    class Sync_Add_Interaction
    {
        static bool Prefix(Interaction __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (Globals.LocalPlayer.ClientState != SharedLibs.ClientState.ConnectedReady) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return true;
            Globals.InteractionManager.AddInteraction(__instance);
            return true;
        }
    }

}
