using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches.Time
{
    [HarmonyPatch(typeof(TimeManager), "getTimeScale")]
    class TimeScaleUpdate
    {
        static bool Prefix(ref float __result)
        {
            if (Multiplayer.Client == null)
                return true;

            Client.Time.TimeManager timeManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Time.TimeManager>();

            if (timeManager.IsPaused())
            {
                __result = 0f;
                return false;
            }

            __result = timeManager.GetCurrentSpeed();
            return false;
        }

    }
}
