﻿using HarmonyLib;
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

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.Blizzard
{
    [HarmonyPatch(typeof(Planetbase.Blizzard), "onEnd")]
    public class EndBlizzard
    {
        static bool Prefix(Planetbase.Blizzard __instance)
        {
            if (Multiplayer.Client == null)
                return true;

            Client.Simulation.SimulationManager simulationManager = Multiplayer.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            Client.Environment.DisasterManager disasterManager = Multiplayer.ServiceLocator.LocateService<Client.Environment.DisasterManager>();

            disasterManager.EndDisaster();

            return false;
        }
    }
}
