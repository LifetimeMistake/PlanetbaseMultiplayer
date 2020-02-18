using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameManager), "fixedUpdate", new[] { typeof(float) })]
    class Hook_WorldLoadingFinished
    {
        private static GameManager.State previousState = GameManager.State.Loading;
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!Globals.IsInMultiplayerMode) return;
            if (!(GameManager.getInstance().getGameState() is GameStateGame)) return;
            GameManager.State currentState = GameManager.getInstance().mState;
            if (previousState == currentState) return;
            previousState = currentState;
            if (currentState != GameManager.State.Updating) return;
            UnityEngine.Debug.Log("world loaded!");
            Globals.LocalClient.OnWorldLoadingFinished();
        }
    }
}
