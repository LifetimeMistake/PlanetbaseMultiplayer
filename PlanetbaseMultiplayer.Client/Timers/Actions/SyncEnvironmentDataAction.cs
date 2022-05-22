using Planetbase;
using PlanetbaseMultiplayer.Client.Environment;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.Timers.Actions.Abstract;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Timers.Actions
{
    public class SyncEnvironmentDataAction : TimerAction
    {
        public override void ProcessAction(ulong currentTick, ClientProcessorContext context)
        {
            Player? simulationOwner = context.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != context.Client.LocalPlayer)
                return;

            Planetbase.EnvironmentManager environmentManager = Planetbase.EnvironmentManager.getInstance();
            Type environmentManagerType = environmentManager.GetType();

            FieldInfo mTimeIndicator = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mTimeIndicator", true);
            FieldInfo mWindIndicator = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mWindIndicator", true);
            FieldInfo mWindDirection = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mWindDirection", true);

            Indicator timeIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mTimeIndicator);
            Indicator windIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mWindIndicator);

            float time = timeIndicator.getValue();
            float windLevel = windIndicator.getValue();
            Vector3 windDirection = (Vector3)Reflection.GetInstanceFieldValue(environmentManager, mWindDirection);

            context.Client.EnvironmentManager.UpdateEnvironmentData(time, windLevel, windDirection);
        }
    }
}
