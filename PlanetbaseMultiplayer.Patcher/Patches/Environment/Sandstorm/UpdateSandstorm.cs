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

namespace PlanetbaseMultiplayer.Patcher.Patches.Environment.Sandstorm
{
    [HarmonyPatch(typeof(Planetbase.Sandstorm), "update")]
    public class UpdateSandstorm
    {
        static bool Prefix(Planetbase.Sandstorm __instance, ref float timeStep)
        {
            if (Multiplayer.Client == null)
                return true;

            Player? simulationOwner = Multiplayer.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != Multiplayer.Client.LocalPlayer)
                return false; // Player isn't the simulation owner

            Type sandstormType = __instance.GetType();
            FieldInfo mSandstormInProgress = Reflection.GetPrivateFieldOrThrow(sandstormType, "mSandstormInProgress", true);
            bool sandstormInProgress = (bool)Reflection.GetInstanceFieldValue(__instance, mSandstormInProgress);
            if(sandstormInProgress)
            {
                FieldInfo mTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstormType, "mTime", true);
                FieldInfo mSandstormTimeInfo = Reflection.GetPrivateFieldOrThrow(sandstormType, "mStandstormTime", true);
                MethodInfo onEndInfo = Reflection.GetPrivateMethodOrThrow(sandstormType, "onEnd", true);

                float mTime = (float)Reflection.GetInstanceFieldValue(__instance, mTimeInfo);
                float mSandstormTime = (float)Reflection.GetInstanceFieldValue(__instance, mSandstormTimeInfo);

                if (mTime > mSandstormTime)
                {
                    // End sandstorm
                    Reflection.InvokeInstanceMethod(__instance, onEndInfo, new object[] { });
                }
            }
            else
            {
                // Create a new sandstorm
                MethodInfo updateDetectionInfo = Reflection.GetPrivateMethodOrThrow(sandstormType, "updateDetection", true);
                MethodInfo decideNextSandstormInfo = Reflection.GetPrivateMethodOrThrow(sandstormType, "decideNextSandstorm", true);
                FieldInfo mTimeToNextSandstorminfo = Reflection.GetPrivateFieldOrThrow(sandstormType, "mTimeToNextSandstorm", true);
                float mTimeToNextSandstorm = (float)Reflection.GetInstanceFieldValue(__instance, mTimeToNextSandstorminfo);

                Reflection.InvokeInstanceMethod(sandstormType, updateDetectionInfo, new[] { mTimeToNextSandstorm, timeStep });
                mTimeToNextSandstorm -= timeStep;
                Reflection.SetInstanceFieldValue(__instance, mTimeToNextSandstorminfo, mTimeToNextSandstorm);
                if (mTimeToNextSandstorm < 0f)
                {
                    // Trigger sandstorm
                    __instance.trigger();
                    Reflection.InvokeInstanceMethod(__instance, decideNextSandstormInfo, new object[] { });
                }
            }

            return false;
        }
    }
}
