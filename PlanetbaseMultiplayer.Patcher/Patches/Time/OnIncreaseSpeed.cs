using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Time
{
    [HarmonyPatch(typeof(GameStateGame), "increaseSpeed")]
    class OnIncreaseSpeedUI
    {
        static bool Prefix()
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer) // Player isn't the simulation owner
            {
                MessageToast.Show($"Only the simulation owner can control time!", 3f);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(TimeManager), "increaseSpeed")]
    class OnIncreaseSpeed
    {
        static bool Prefix()
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer) 
                return false; // Player isn't the simulation owner

            PlanetbaseMultiplayer.Client.Time.TimeManager timeManager = Multiplayer.Client.TimeManager;

            float timeScale = timeManager.GetCurrentSpeed();
            timeScale *= 2f;

            if (timeScale > 8f)
                timeScale = 8f;

            timeManager.SetSpeed(timeScale);
            return false;
        }
    }
}
