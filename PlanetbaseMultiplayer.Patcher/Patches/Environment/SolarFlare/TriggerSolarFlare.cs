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

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.SolarFlare
{
    [HarmonyPatch(typeof(Planetbase.SolarFlare), "trigger")]
    public class TriggerSolarFlare
    {
        static bool Prefix(Planetbase.SolarFlare __instance)
        {
            if (Multiplayer.Client == null)
                return true;

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            PlanetbaseMultiplayer.Client.Environment.DisasterManager disasterManager = Multiplayer.Client.DisasterManager;

            float disasterLength = UnityEngine.Random.Range(90f, 180f);
            float currentTime = 0f;

            disasterManager.CreateDisaster(DisasterType.SolarFlare, disasterLength, currentTime);

            return false;
        }
    }
}
