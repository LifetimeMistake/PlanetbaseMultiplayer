using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.Sandstorm
{
    [HarmonyPatch(typeof(Planetbase.Sandstorm), "trigger")]
    public class TriggerSandstorm
    {
        static bool Prefix(Planetbase.Sandstorm __instance)
        {
            if (Multiplayer.Client == null)
                return true;

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            Client.Environment.DisasterManager disasterManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Environment.DisasterManager>();

            float disasterLength = UnityEngine.Random.Range(90f, 180f);
            float currentTime = 0f;

            disasterManager.CreateDisaster(DisasterType.Sandstorm, disasterLength, currentTime);

            return false;
        }
    }
}
