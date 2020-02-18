using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateGame), "isSaveAllowed")]
    public class Override_IsSaveAllowed_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return !Globals.IsInMultiplayerMode;
        }
    }
}
