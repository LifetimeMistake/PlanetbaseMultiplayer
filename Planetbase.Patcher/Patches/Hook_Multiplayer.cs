using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Planetbase;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateTitle), MethodType.Constructor, new[] { typeof(GameState) })]
    public class Hook_Multiplayer
    {
        public static void Postfix(GameStateTitle __instance)
        {
            Client.Client c = new Client.Client();
            c.Start();
            UnityEngine.Debug.Log("Multiplayer mode started!");
        }
    }
}
