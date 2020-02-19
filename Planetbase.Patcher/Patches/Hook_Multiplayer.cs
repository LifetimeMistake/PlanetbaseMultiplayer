using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateTitle), MethodType.Constructor, new[] { typeof(GameState) })]
    class Hook_Multiplayer
    {
        public static void Postfix(GameStateTitle __instance)
        {
            Client.Client c = new Client.Client();
            c.Start();
            UnityEngine.Debug.Log("Multiplayer mode ready!");
        }
    }
    [HarmonyPatch(typeof(GameManager), "fixedUpdate", new[] { typeof(float) })]
    class Hook_fixedUpdate
    {
        static void Postfix(GameManager __instance)
        {
            if (!Globals.IsInMultiplayerMode) return;
            if (Globals.LocalClient.packetQueue.Count == 0) return;
            Packet packet = Globals.LocalClient.packetQueue.Dequeue();
            Globals.LocalClient.ProcessPacket(packet);
        }
    }

    [HarmonyPatch(typeof(Util), "captureScreenshot", new[] { typeof(int) })]
    class override_captureScreenshot
    {
        static bool Prefix(ref byte[] __result)
        {
            if (Globals.IsInMultiplayerMode)
            {
                __result = new byte[0];
                return false;
            }
            else
                return true;
        }
    }
}
