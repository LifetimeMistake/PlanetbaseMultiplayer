using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Interaction), "serialize", new[] { typeof(XmlNode), typeof(string) })]
    class Hook_Serialization_Patch_Interaction
    {
        static void Postfix(Interaction __instance, XmlNode parent)
        {
            if (!Globals.IsInMultiplayerMode) return;
            Globals.InteractionManager.AppendMultiplayerId(__instance, parent.LastChild);
        }
    }
    [HarmonyPatch(typeof(Resource), "serialize", new[] { typeof(XmlNode), typeof(string) })]
    class Hook_Serialization_Patch_Resource
    {
        static void Postfix(Resource __instance, XmlNode parent)
        {
            if (!Globals.IsInMultiplayerMode) return;
            Globals.ResourceManager.AppendMultiplayerId(__instance, parent.LastChild);
        }
    }
}
