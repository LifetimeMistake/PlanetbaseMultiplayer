using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches.Core
{
#if DEBUG
    [HarmonyPatch(typeof(GameManager), "onGui")]
    public class DebugLogger
    {

        static void Postfix()
        {
            if (Multiplayer.Client == null) return;
            ShowDebugLog();
        }


        static void ShowDebugLog()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Client.Debugging.DebugManager debugManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Debugging.DebugManager>();

            try
            {
                foreach (string line in debugManager.GetLog())
                    stringBuilder.AppendLine(line);
            }
            catch(Exception)
            {
                Debug.Log("WARN: Debug list access collision");
                return;
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(10, 80, 400, 700), stringBuilder.ToString(), style);
        }
    }

    [HarmonyPatch(typeof(Debug), "Log", new[] { typeof(object) })]
    public class UnityDebugLogHook
    { 
        static void Postfix(object message)
        {
            if (Multiplayer.Client == null) return;
            Client.Debugging.DebugManager debugManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Debugging.DebugManager>();
            debugManager.AddMessage((message == null) ? "Null" : message.ToString());
        }
    }

    [HarmonyPatch(typeof(Console), "WriteLine", new[] { typeof(string) })]
    public class SystemWriteLineHook
    {
        static void Postfix(string value)
        {
            if (Multiplayer.Client == null) return;
            Client.Debugging.DebugManager debugManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Debugging.DebugManager>();
            debugManager.AddMessage(value);
        }
    }
#endif
}
