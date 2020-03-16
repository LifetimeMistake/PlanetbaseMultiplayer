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
    [HarmonyPatch(typeof(Interaction), "deserializeAll", new[] { typeof(XmlNode) })]
    class Hook_Deserialization_Patch_Interaction
    {
        static bool Prefix(XmlNode node)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Globals.InteractionManager.DeserializeAll(node);
            return false;
        }
    }
    [HarmonyPatch(typeof(Resource), "deserializeAll", new[] { typeof(XmlNode) })]
    class Hook_Deserialization_Patch_Resource
    {
        static bool Prefix(XmlNode node)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Globals.ResourceManager.DeserializeAll(node);
            return false;
        }
    }
    [HarmonyPatch(typeof(Construction), "deserializeModules")]
    class Hook_Deserialization_Patch_Modules
    {
        static bool Prefix(ref bool __result)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            __result = true;
            Globals.ConstructionManager.DeserializeModules();
            return false;
        }
    }
    [HarmonyPatch(typeof(Construction), "deserializeConnections")]
    class Hook_Deserialization_Patch_Connections
    {
        static bool Prefix(ref bool __result)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            __result = true;
            Globals.ConstructionManager.DeserializeConnections();
            return false;
        }
    }
}
