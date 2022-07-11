using PlanetbaseMultiplayer.Client.Environment;
using PlanetbaseMultiplayer.Client.Environment.Disasters;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.Timers.Actions.Abstract;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Timers.Actions
{
    public class UpdateDisasterAction : TimerAction
    {
        public override void ProcessAction(ulong currentTick, ClientProcessorContext context)
        {
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();
            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != context.Client.LocalPlayer)
                return;

            DisasterManager disasterManager = context.ServiceLocator.LocateService<DisasterManager>();
            if (!disasterManager.AnyDisasterInProgress())
                return;

            // Pull the current time from our disaster proxy
            IDisasterProxy disasterProxy = disasterManager.GetCurrentDisasterProxy();
            disasterManager.UpdateDisaster(disasterProxy.Time);
        }
    }
}
