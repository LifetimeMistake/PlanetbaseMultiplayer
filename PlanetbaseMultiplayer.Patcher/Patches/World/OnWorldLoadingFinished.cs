using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches.World
{
    [HarmonyPatch(typeof(GameManager), "fixedUpdate", new[] { typeof(float) })]
    class OnWorldLoadingFinished
    {
        private static GameManager.State previousState = GameManager.State.Loading;
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (Multiplayer.Client == null) return;
            if (!(GameManager.getInstance().getGameState() is GameStateGame)) return;

            GameManager gameManager = GameManager.getInstance();
            FieldInfo mStateInfo = Reflection.GetPrivateFieldOrThrow(gameManager.GetType(), "mState", true);

            GameManager.State currentState = (GameManager.State)Reflection.GetInstanceFieldValue(gameManager, mStateInfo);
            if (previousState == currentState) return;
            previousState = currentState;
            if (currentState != GameManager.State.Updating) return;

            Debug.Log("World loading finished!");
            ClientReadyPacket clientReadyPacket = new ClientReadyPacket();
            Multiplayer.Client.SendPacket(clientReadyPacket);
        }
    }
}
