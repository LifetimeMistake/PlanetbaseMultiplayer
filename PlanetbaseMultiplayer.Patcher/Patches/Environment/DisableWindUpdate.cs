using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment
{
    [HarmonyPatch(typeof(EnvironmentManager), "updateWind", new[] { typeof(float) })]
    public class DisableWindUpdate
    {
        public static bool Prefix(EnvironmentManager __instance)
        {
            if (Multiplayer.Client == null)
                return true; // Not in multiplayer

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? localPlayer = Multiplayer.Client.LocalPlayer;
            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || localPlayer == null || localPlayer.Value != simulationOwner.Value)
                return false; // Only the simulation owner should run the update method

            return true;
        }
    }
}