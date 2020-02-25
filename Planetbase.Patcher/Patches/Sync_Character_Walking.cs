using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Character), "startWalking", new[] {  typeof(Target), typeof(Selectable[]) })]
    class Sync_Character_Walking
    {
        static bool Prefix(Character __instance, Target target, Selectable[] indirectTargets)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.LocalClient.OnCharacterStartWalking(__instance, target, indirectTargets);
            return false;
        }
    }
}
