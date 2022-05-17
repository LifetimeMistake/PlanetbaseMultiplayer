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
    internal class SyncEnvironmentDataAction : TimerAction
    {
        public override void ProcessAction(ulong currentTick, ClientProcessorContext context)
        {
            Player? simulationOwner = context.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner != null && simulationOwner.Value == context.Client.LocalPlayer)
            {
                FieldInfo mTimeIndicator;
                FieldInfo mWindIndicator;
                FieldInfo mWindDirection;

                Planetbase.EnvironmentManager environmentManager = Planetbase.EnvironmentManager.getInstance();
                if (!Reflection.TryGetPrivateField(environmentManager.GetType(), "mTimeIndicator", true, out mTimeIndicator))
                {
                    Debug.LogError("Failed to find \"mTimeIndicator\"");
                    return;
                }

                if (!Reflection.TryGetPrivateField(environmentManager.GetType(), "mWindIndicator", true, out mWindIndicator))
                {
                    Debug.LogError("Failed to find \"mWindIndicator\"");
                    return;
                }

                if (!Reflection.TryGetPrivateField(environmentManager.GetType(), "mWindDirection", true, out mWindDirection))
                {
                    Debug.LogError("Failed to find \"mWindDirection\"");
                    return;
                }

                Indicator timeIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mTimeIndicator);
                Indicator windIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mWindIndicator);

                float time = timeIndicator.getValue();
                float windLevel = windIndicator.getValue();
                Vector3 windDirection = (Vector3)Reflection.GetInstanceFieldValue(environmentManager, mWindDirection);

                context.Client.EnvironmentManager.UpdateEnvironmentData(time, windLevel, windDirection);
            }
        }
    }
}
