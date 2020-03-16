using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    // Applies fixes for crashes.
    [HarmonyPatch(typeof(InteractionBuild), "update", new[] { typeof(float) })]
    class Fix_InteractionBuild_Crash
    {
        static bool Prefix(InteractionBuild __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Buildable buildable = (Buildable)__instance.mSelectable;
            if (buildable.mState == BuildableState.Built) return false;
            return true;
        }
    }
    [HarmonyPatch(typeof(Serialization), "deserializeId", new[] { typeof(XmlNode) })]
    class Fix_DeserializeId_Crash
    {
        static bool Prefix(XmlNode node, ref int __result)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            int num = Serialization.deserializeInt(node);
            UnityEngine.Debug.Log($"deserialize id: {num}");
            if (Serialization.mIds != null) // Skip check if the hashset is null
            {
                if (Serialization.mIds.Contains(num))
                {
                    UnityEngine.Debug.LogError(string.Concat(new object[]
                    {
                    "Duplicate ID in file: ",
                    num,
                    ", savegame has probably been tampered with, or a trainer has been used: ",
                    Path.GetFileName(Serialization.mPath)
                    }));
                }
                Serialization.mIds.Add(num);
            }
            __result = num;
            return false;
        }
    }
}
