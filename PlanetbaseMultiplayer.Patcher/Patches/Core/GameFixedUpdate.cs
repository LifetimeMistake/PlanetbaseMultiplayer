using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Core
{
    [HarmonyPatch(typeof(GameManager), "fixedUpdate", new[] { typeof(float) })]
    class GameFixedUpdate
    {
        static bool Prefix(GameManager __instance)
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Multiplayer.Client.OnFixedUpdate();
            return true;
        }
    }
}
