using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Character), "loadResource")]
    class Sync_Character_Resources
    {
        static bool Prefix(Character __instance, Resource resource)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.LocalClient.OnCharacterLoadResource(__instance, resource);
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
            Globals.LocalClient.OnCharacterUnloadResource(__instance, newState);
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
            Globals.LocalClient.OnAddConstructionMaterial(__instance.getId(), resource.getId());
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
            Globals.LocalClient.OnCharacterStoreResource(__instance.getId(), module.getId());
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
            Globals.LocalClient.OnCharacterEmbedResource(__instance.getId(), component.getId(), resourceState);
            return false;
        }
    }
    [HarmonyPatch(typeof(Character), "destroyResource")]
    class Sync_Character_DestroyResource
    {
        static bool Prefix(Character __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.LocalClient.OnCharacterDestroyResource(__instance.getId());
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
            Globals.LocalClient.OnExtractResource(__instance.getId());
            return false;
        }
    }
}
