using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Time
{
    [HarmonyPatch(typeof(TimeManager), "setNormalSpeed")]
    class OnSetNormalSpeed
    {
        static bool Prefix()
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            PlanetbaseMultiplayer.Client.Time.TimeManager timeManager = Multiplayer.Client.TimeManager;
            timeManager.SetNormalSpeed();
            return false;
        }
    }
}
