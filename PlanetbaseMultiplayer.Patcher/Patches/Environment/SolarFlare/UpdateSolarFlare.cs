using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.SolarFlare
{
    [HarmonyPatch(typeof(Planetbase.SolarFlare), "update")]
    public class UpdateSolarFlare
    {
        static bool Prefix(Planetbase.SolarFlare __instance, ref float timeStep)
        {
            if (Multiplayer.Client == null)
                return true;

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            Type solarFlareType = __instance.GetType();
            FieldInfo mSolarFlareInProgress = Reflection.GetPrivateFieldOrThrow(solarFlareType, "mSolarFlareInProgress", true);
            bool solarFlareInProgress = (bool)Reflection.GetInstanceFieldValue(__instance, mSolarFlareInProgress);
            if (solarFlareInProgress)
            {
                FieldInfo mTimeInfo = Reflection.GetPrivateFieldOrThrow(solarFlareType, "mTime", true);
                FieldInfo mSolarFlareTimeInfo = Reflection.GetPrivateFieldOrThrow(solarFlareType, "mSolarFlareTime", true);
                MethodInfo onEndInfo = Reflection.GetPrivateMethodOrThrow(solarFlareType, "onEnd", true);

                float mTime = (float)Reflection.GetInstanceFieldValue(__instance, mTimeInfo) + timeStep;
                Reflection.SetInstanceFieldValue(__instance, mTimeInfo, mTime);
                float mSolarFlareTime = (float)Reflection.GetInstanceFieldValue(__instance, mSolarFlareTimeInfo);

                if (mTime > mSolarFlareTime)
                {
                    // End solar flare
                    Reflection.InvokeInstanceMethod(__instance, onEndInfo, new object[] { });
                }
            }
            else
            {
                // Create a new solar flare
                MethodInfo updateDetectionInfo = Reflection.GetPrivateMethodOrThrow(solarFlareType, "updateDetection", true);
                MethodInfo decideNextSolarFlareInfo = Reflection.GetPrivateMethodOrThrow(solarFlareType, "decideNextSolarFlare", true);
                FieldInfo mTimeToNextSolarFlareInfo = Reflection.GetPrivateFieldOrThrow(solarFlareType, "mTimeToNextSolarFlare", true);
                float mTimeToNextSolarFlare = (float)Reflection.GetInstanceFieldValue(__instance, mTimeToNextSolarFlareInfo);

                Reflection.InvokeInstanceMethod(__instance, updateDetectionInfo, new object[] { mTimeToNextSolarFlare, timeStep });
                mTimeToNextSolarFlare -= timeStep;
                Reflection.SetInstanceFieldValue(__instance, mTimeToNextSolarFlareInfo, mTimeToNextSolarFlare);
                if (mTimeToNextSolarFlare < 0f && Singleton<EnvironmentManager>.getInstance().getTimeOfDay() < 0.25f && !Singleton<DisasterManager>.getInstance().anyInProgress())
                {
                    // Trigger solar flare
                    __instance.trigger();
                    Reflection.InvokeInstanceMethod(__instance, decideNextSolarFlareInfo, new object[] { });
                }
            }

            return false;
        }
    }
}
