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

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            PlanetbaseMultiplayer.Client.Environment.DisasterManager disasterManager = Multiplayer.Client.DisasterManager;
            Type instance = __instance.GetType();

            FieldInfo mSandstormTimeInfo = Reflection.GetPrivateFieldOrThrow(instance, "mSandstormTime", true);
            FieldInfo mTimeInfo = Reflection.GetPrivateFieldOrThrow(instance, "mTime", true);

            float disasterLength = (float)Reflection.GetInstanceFieldValue(__instance, mSandstormTimeInfo);
            float currentTime = (float)Reflection.GetInstanceFieldValue(__instance, mTimeInfo);

            DisasterType disasterType = 0;

            disasterManager.CreateDisaster(disasterType, disasterLength, currentTime);

            return false;
        }
    }
}
