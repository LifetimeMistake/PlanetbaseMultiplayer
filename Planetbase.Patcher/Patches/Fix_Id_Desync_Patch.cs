using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateGame), "tryPlaceComponent")]
    class Fix_PlaceComponent_Desync
    {
        static void Prefix(GameStateGame __instance)
        {
            if (!Globals.IsInMultiplayerMode) return;
            if(__instance.mPlacedComponent == null)
            {
                // The Id is about to be incremented. Sync it with other players.
                Globals.LocalClient.Send_IncrementId_Packet();
            }
        }
    }
    [HarmonyPatch(typeof(GameStateGame), "tryPlaceModule")]
    class Fix_PlaceModule_Desync
    {
        static void Prefix(GameStateGame __instance)
        {
            if (!Globals.IsInMultiplayerMode) return;
            if (__instance.mActiveModule == null)
            {
                // The Id is about to be incremented. Sync it with other players.
                Globals.LocalClient.Send_IncrementId_Packet();
            }
        }
    }
}
