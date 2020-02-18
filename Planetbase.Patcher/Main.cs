using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;


namespace PlanetbaseMultiplayer.Patcher
{
    public static class Patcher
    {
        public static HarmonyInstance Harmony;
        public static void Execute()
        {
            UnityEngine.Debug.Log("Creating harmony instance");
            HarmonyInstance.DEBUG = true;
            Harmony = HarmonyInstance.Create("com.planetbase.multiplayermod.harmony");
            UnityEngine.Debug.Log("Patching game!");
            Harmony.PatchAll();
            UnityEngine.Debug.Log($"Installed {Harmony.GetPatchedMethods().Count()} patches!");
        }
    }
}
