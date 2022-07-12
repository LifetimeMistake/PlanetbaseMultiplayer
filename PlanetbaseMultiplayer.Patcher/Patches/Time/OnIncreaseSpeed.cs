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
        static bool Prefix(GameStateGame __instance)
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer) // Player isn't the simulation owner
            {
                MessageToast.Show($"Only the simulation owner can control time!", 3f);
                return false;
            }

            Client.Time.TimeManager timeManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Time.TimeManager>();

            float timeScale = timeManager.GetCurrentSpeed();
            timeScale *= 2f;

            if (timeScale > 8f)
                timeScale = 8f;

            MessageToast.Show(StringList.get("speed_set") + " x" + timeScale, 3f);
            TimeManager.getInstance().increaseSpeed();
            return false;
        }
    }

    [HarmonyPatch(typeof(TimeManager), "increaseSpeed")]
    class OnIncreaseSpeed
    {
        static bool Prefix()
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer) 
                return false; // Player isn't the simulation owner

            Client.Time.TimeManager timeManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Time.TimeManager>();

            float timeScale = timeManager.GetCurrentSpeed();
            timeScale *= 2f;
            if (timeScale > 8f)
                timeScale = 8f;

            timeManager.SetSpeed(timeScale);
            return false;
        }
    }
}
