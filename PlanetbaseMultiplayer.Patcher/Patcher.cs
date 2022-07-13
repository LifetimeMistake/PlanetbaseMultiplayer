using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace PlanetbaseMultiplayer.Patcher
{
    public static class Patcher
    {
        public static Harmony HarmonyInstance;
        public static void Execute()
        {
            Debug.Log("Creating harmony instance");
#if DEBUG
            Harmony.DEBUG = true;
#endif
            HarmonyInstance = new Harmony("com.planetbase.multiplayermod.harmony");
            Debug.Log("Patching game!");
            HarmonyInstance.PatchAll();
            Debug.Log($"Installed {HarmonyInstance.GetPatchedMethods().Count()} patches!");
        }
    }
}
