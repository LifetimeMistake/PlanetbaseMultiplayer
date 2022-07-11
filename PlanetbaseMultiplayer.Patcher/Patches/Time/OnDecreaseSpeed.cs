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
    [HarmonyPatch(typeof(GameStateGame), "decreaseSpeed")]
    class OnDecreaseSpeedUI
    {
        static bool Prefix()
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
            timeScale /= 2f;

            if (timeScale < 1f)
                timeScale = 1f;

            MessageToast.Show(StringList.get("speed_set") + " x" + timeScale, 3f);
            TimeManager.getInstance().decreaseSpeed();
            return false;
        }
    }

    [HarmonyPatch(typeof(TimeManager), "decreaseSpeed")]
    class OnDecreaseSpeed
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
            timeScale /= 2f;

            if (timeScale < 1f)
                timeScale = 1f;

            timeManager.SetSpeed(timeScale);
            return false;
        }
    }
}
