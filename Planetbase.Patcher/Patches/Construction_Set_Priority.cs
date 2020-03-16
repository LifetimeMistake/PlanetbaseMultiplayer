using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GuiMenuSystem), "onButtonPriorityUp", new[] { typeof(object) })]
    class Construction_Set_Priority_Up
    {
        static bool Prefix(GuiMenuSystem __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Construction selectedConstruction = Selection.getSelectedConstruction();
            if (selectedConstruction != null)
            {
                Globals.ConstructionManager.SetPriority(selectedConstruction, true);
            }
            __instance.setActionMenu();
            return false;
        }
    }
    [HarmonyPatch(typeof(GuiMenuSystem), "onButtonPriorityDown", new[] { typeof(object) })]
    class Construction_Set_Priority_Down
    {
        static bool Prefix(GuiMenuSystem __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Construction selectedConstruction = Selection.getSelectedConstruction();
            if (selectedConstruction != null)
            {
                Globals.ConstructionManager.SetPriority(selectedConstruction, false);
            }
            __instance.setActionMenu();
            return false;
        }
    }
}
