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

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.Blizzard
{
    [HarmonyPatch(typeof(Planetbase.Blizzard), "update")]
    public class UpdateBlizzard
    {
        static bool Prefix(Planetbase.Blizzard __instance, ref float timeStep)
        {
            if (Multiplayer.Client == null)
                return true;

            Client.Simulation.SimulationManager simulationManager = Multiplayer.Client.ServiceLocator.LocateService<Client.Simulation.SimulationManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            Type blizzardType = __instance.GetType();
            FieldInfo mBlizzardInProgress = Reflection.GetPrivateFieldOrThrow(blizzardType, "mBlizzardInProgress", true);
            bool blizzardInProgress = (bool)Reflection.GetInstanceFieldValue(__instance, mBlizzardInProgress);
            if (blizzardInProgress)
            {
                FieldInfo mTimeInfo = Reflection.GetPrivateFieldOrThrow(blizzardType, "mTime", true);
                FieldInfo mBlizzardTimeInfo = Reflection.GetPrivateFieldOrThrow(blizzardType, "mBlizzardTime", true);
                MethodInfo onEndInfo = Reflection.GetPrivateMethodOrThrow(blizzardType, "onEnd", true);

                float mTime = (float)Reflection.GetInstanceFieldValue(__instance, mTimeInfo) + timeStep;
                Reflection.SetInstanceFieldValue(__instance, mTimeInfo, mTime);
                float mBlizzardTime = (float)Reflection.GetInstanceFieldValue(__instance, mBlizzardTimeInfo);

                if (mTime > mBlizzardTime)
                {
                    // End blizzard
                    Reflection.InvokeInstanceMethod(__instance, onEndInfo, new object[] { });
                }
            }
            else
            {
                // Create a new blizzard
                MethodInfo updateDetectionInfo = Reflection.GetPrivateMethodOrThrow(blizzardType, "updateDetection", true);
                MethodInfo decideNextBlizzardInfo = Reflection.GetPrivateMethodOrThrow(blizzardType, "decideNextBlizzard", true);
                FieldInfo mTimeToNextBlizzardInfo = Reflection.GetPrivateFieldOrThrow(blizzardType, "mTimeToNextBlizzard", true);
                float mTimeToNextBlizzard = (float)Reflection.GetInstanceFieldValue(__instance, mTimeToNextBlizzardInfo);

                Reflection.InvokeInstanceMethod(__instance, updateDetectionInfo, new object[] { mTimeToNextBlizzard, timeStep });
                mTimeToNextBlizzard -= timeStep;
                Reflection.SetInstanceFieldValue(__instance, mTimeToNextBlizzardInfo, mTimeToNextBlizzard);
                if (mTimeToNextBlizzard < 0f)
                {
                    // Trigger blizzard
                    __instance.trigger();
                    Reflection.InvokeInstanceMethod(__instance, decideNextBlizzardInfo, new object[] { });
                }
            }

            return false;
        }
    }
}
