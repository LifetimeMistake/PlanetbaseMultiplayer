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
            Player? simulationOwner = context.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner != null && simulationOwner.Value == context.Client.LocalPlayer)
            {
                Planetbase.DisasterManager disasterManager = Planetbase.DisasterManager.getInstance();
            }
        }
    }
}
