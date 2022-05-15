using Planetbase;
using PlanetbaseMultiplayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.UI
{
    public static class MessageToast
    {
        public static bool Show(string message, float time)
        {
            if (!(GameManager.getInstance().getGameState() is GameStateGame))
                return false;

            GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
            MethodInfo addToastInfo;
            if (!Reflection.TryGetPrivateMethod(gameState.GetType(), "addToast", true, out addToastInfo))
            {
                Debug.LogError("Failed to find method \"addToast\"");
                return false;
            }

            Reflection.InvokeInstanceMethod(gameState, addToastInfo, new object[] { message, time });
            return true;
        }
    }
}
