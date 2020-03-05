using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(Sandstorm), "trigger")]
    class Sync_Disaster_Sandstorm_Trigger
    {
        static bool Prefix(Sandstorm __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.LocalClient.OnSandstormTrigger(__instance);
            return true;
        }
    }
    [HarmonyPatch(typeof(Sandstorm), "onEnd")]
    class Sync_Disaster_Sandstorm_End
    {
        static bool Prefix()
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            Globals.LocalClient.OnSandstormEnd();
            return true;
        }
    }
    [HarmonyPatch(typeof(Sandstorm), "decideNextSandstorm")]
    class Sync_Disaster_Sandstorm_DecideNext
    {
        static bool Prefix(Sandstorm __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false;
            __instance.mTimeToNextSandstorm = UnityEngine.Random.Range(2400f, 4800f);
            if (PlanetManager.getCurrentPlanet().getSandstormRisk() == Planet.Quantity.Low)
            {
                __instance.mTimeToNextSandstorm *= 2f;
            }
            GameplayModifier gameplayModifier = Singleton<ChallengeManager>.getInstance().getGameplayModifier(GameplayModifierType.DisasterFrequency);
            if (gameplayModifier != null)
            {
                __instance.mTimeToNextSandstorm *= 1f / Mathf.Clamp(gameplayModifier.getFloat(), 0.1f, 100f);
            }
            Globals.LocalClient.OnSandstormDecideNext(__instance);
            return false;
        }
    }
}
