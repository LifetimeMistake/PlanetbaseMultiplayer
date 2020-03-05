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
#if DEBUG
    [HarmonyPatch(typeof(GameManager), "onGui")]
    public class Debug_UI
    {
        static void Postfix()
        {
            if (!Globals.IsInMultiplayerMode) return;
#if DEBUG
            RenderIdIndicator();
            RenderEventLog();
            RenderPacketCounter();
#endif
        }

        public static void RenderIdIndicator()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), $"NextId: {IdGenerator.getInstance().mNextId}", style);
        }

        public static void RenderEventLog()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string evt in Globals.LocalClient.debug_eventList.Reverse<string>())
            {
                sb.AppendLine(evt);
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(50, Screen.height - 350, 700, 300), sb.ToString(), style);
        }

        public static void RenderPacketCounter()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(50, Screen.height - 370, 700, 15), $"Incoming packet count: {Globals.LocalClient.lastTick_PacketCount}/t", style);
        }
    }
    [HarmonyPatch(typeof(Buildable), "onUserPlaced")]
    class buildable_onUserPlaced
    {
        static void Postfix(Buildable __instance)
        {
            if (!Globals.IsInMultiplayerMode) return;
#if DEBUG
            Globals.LocalClient.debug_eventList.Add($"{__instance.GetType().Name} placed with id {__instance.getId()}");
#endif
        }
    }
    [HarmonyPatch(typeof(GameStateGame), "onGui")]
    class render_DebugManager
    {
        static void Postfix()
        {
            if (!Globals.IsInMultiplayerMode) return;
            DebugManager.getInstance().onGui();
        }
    }
#endif
}
