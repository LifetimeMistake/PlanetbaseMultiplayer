using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateGame), "onRecycle")]
    class Override_Recycle_Patch
    {
        static bool Prefix(GameStateGame __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Selectable selected = Selection.getSelected();
            if(selected != null)
            {
                Console.WriteLine($"Recycled: {selected.GetType()}");
                Globals.LocalClient.SendPacket(new Packet(PacketType.RecycleSelectable, new RecycleSelectableDataPackage(selected.getId())));
            }
            return true;
		}
    }
}
