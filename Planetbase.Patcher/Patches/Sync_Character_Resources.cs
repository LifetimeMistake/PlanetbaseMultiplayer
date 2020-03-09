using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Resource), "postInit")]
    class Sync_Resource_Create
    {
        static bool Prefix(Resource __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (Globals.LocalPlayer.ClientState != SharedLibs.ClientState.ConnectedReady) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return true;
            Globals.ResourceManager.AddResource(__instance);
            return true;
        }
    }
    [HarmonyPatch(typeof(Resource), "destroy")]
    class Sync_Resource_Destroy
    {
        static bool Prefix(Resource __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.RemoveResource(__instance);
            return false;
        }
    }
    [HarmonyPatch(typeof(Character), "loadResource")]
    class Sync_Character_Resources
    {
        static bool Prefix(Character __instance, Resource resource)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.LoadResource(resource, __instance);
            return false;
        }
    }
    [HarmonyPatch(typeof(Character), "unloadResource")]
    class Sync_Character_UnloadResource
    {
        static bool Prefix(Character __instance, Resource.State newState)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.UnloadResource(__instance, newState);
            return false;
        }
    }
    [HarmonyPatch(typeof(Buildable), "addConstructionMaterial", new[] { typeof(Resource) })]
    class Sync_Character_AddMaterial
    {
        static bool Prefix(Buildable __instance, Resource resource)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.AddConstructionMaterial(__instance, resource);
            return false;
        }
    }
    [HarmonyPatch(typeof(Character), "storeResource", new[] { typeof(Module) })]
    class Sync_Character_StoreResource
    {
        static bool Prefix(Character __instance, Module module)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.StoreResource(__instance, module);
            return false;
        }
    }
    [HarmonyPatch(typeof(Character), "embedResource", new[] { typeof(ConstructionComponent), typeof(Resource.State) })]
    class Sync_Character_EmbedResource
    {
        static bool Prefix(Character __instance, ConstructionComponent component, Resource.State resourceState)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.EmbedResource(__instance, component, resourceState);
            return false;
        }
    }
    [HarmonyPatch(typeof(Resource), "onExtract")]
    class Sync_ExtractResource
    {
        static bool Prefix(Resource __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.ResourceManager.ExtractResource(__instance);
            return false;
        }
    }
}
