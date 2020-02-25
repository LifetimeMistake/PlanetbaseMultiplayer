using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Character), "postInit")]
    class Disable_Character_AI
    {
        static void Postfix(Character __instance, ref BaseAi ___mAi)
        {
            if (!Globals.IsInMultiplayerMode) return;
            if (Globals.LocalPlayer.IsSimulationOwner) return;
            ___mAi = new EmptyAi();
        }
    }
}
