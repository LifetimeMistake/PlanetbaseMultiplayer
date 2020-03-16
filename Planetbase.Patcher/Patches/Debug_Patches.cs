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
            RenderMultiplayerInteractions();
            RenderMultiplayerResources();
            RenderMultiplayerConstructions();
#endif
        }
        
        public static void RenderMultiplayerInteractions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MultiplayerId | ResourceType | IsMultiplayer");
            foreach (Resource resource in Resource.mResources)
            {
                string id = Globals.ResourceManager.Contains(resource) ? Globals.ResourceManager.Find(resource).ToString() : "Empty";
                sb.AppendLine($"{id} | {resource.mResourceType.GetType().Name} | {Globals.ResourceManager.Contains(resource)}");
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(500, Screen.height - 950, 400, 300), sb.ToString(), style);
        }

        public static void RenderMultiplayerResources()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MultiplayerId | InteractionType | IsMultiplayer");
            foreach (Interaction interaction in Interaction.mInteractions)
            {
                string id = Globals.InteractionManager.Contains(interaction) ? Globals.InteractionManager.Find(interaction).ToString() : "Empty";
                sb.AppendLine($"{id} | {interaction.GetType().Name} | {Globals.InteractionManager.Contains(interaction)}");
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(50, Screen.height - 950, 400, 300), sb.ToString(), style);
        }

        public static void RenderMultiplayerConstructions()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MultiplayerId | ConstructionType | ConstructionSubtype | IsMultiplayer");
            foreach (Construction construction in Construction.mConstructions)
            {
                string id = Globals.ConstructionManager.Contains(construction) ? Globals.ConstructionManager.Find(construction).ToString() : "Empty";
                sb.AppendLine($"{id} | {construction.GetType().Name} | {construction.getName()} | {Globals.ConstructionManager.Contains(construction)}");
            }
            foreach (ConstructionComponent component in ConstructionComponent.mComponents)
            {
                string id = Globals.ConstructionManager.Contains(component) ? Globals.ConstructionManager.Find(component).ToString() : "Empty";
                sb.AppendLine($"{id} | {component.GetType().Name} | {component.getComponentType().GetType().Name} | {Globals.ConstructionManager.Contains(component)}");
            }
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.white;
            GUI.Label(new Rect(1050, Screen.height - 950, 400, 300), sb.ToString(), style);
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
            GUI.Label(new Rect(50, Screen.height - 350, 400, 300), sb.ToString(), style);
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
