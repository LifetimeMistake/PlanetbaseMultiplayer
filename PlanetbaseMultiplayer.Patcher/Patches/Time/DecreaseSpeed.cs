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
    class DecreaseSpeed
    {
        static bool Prefix()
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer mode

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return true; // Player isn't the simulation owner

            PlanetbaseMultiplayer.Client.Time.TimeManager timeManager = Multiplayer.Client.TimeManager;

            float timeScale = timeManager.GetCurrentSpeed();
            timeScale /= 2f;

            if (timeScale < 1f)
                timeScale = 1f;

            timeManager.SetSpeed(timeScale);
            MessageToast.Show($"Set game speed to x{timeScale}", 3f);
            return false;
        }
    }
}
