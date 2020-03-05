using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GuiMenuSystem), "onButtonEnable", new[] { typeof(object) })]
    class Sync_Buildable_SetEnabled_True
    {
        static bool Prefix(GuiMenuSystem __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Buildable selectedBuildable = Selection.getSelectedBuildable();
            if (selectedBuildable != null)
                Globals.LocalClient.OnBuildableEnable(selectedBuildable);
            __instance.setActionMenu();
            return false;
        }
    }
    [HarmonyPatch(typeof(GuiMenuSystem), "onButtonDisable", new[] { typeof(object) })]
    class Sync_Buildable_SetEnabled_False
    {
        static bool Prefix(GuiMenuSystem __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Buildable selectedBuildable = Selection.getSelectedBuildable();
            if (selectedBuildable != null)
                Globals.LocalClient.OnBuildableDisable(selectedBuildable);
            __instance.setActionMenu();
            return false;
        }
    }
}
