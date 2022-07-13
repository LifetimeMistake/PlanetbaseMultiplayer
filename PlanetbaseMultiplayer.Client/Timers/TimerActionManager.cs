using PlanetbaseMultiplayer.Client.Timers.Actions.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Timers
{
    public class TimerActionManager
    {
        private ulong tickCounter;
        private ProcessorContext context;
        private Dictionary<TimerAction, uint> timerActions;

        public TimerActionManager(ProcessorContext context)
        {
            this.context = context;
            timerActions = new Dictionary<TimerAction, uint>();
        }

        public void RegisterAction(TimerAction action, uint activationInterval)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

#if DEBUG
            Debug.Log($"Registered timer action {action.GetType().FullName} with activation interval {activationInterval}");
#endif

            timerActions.Add(action, activationInterval);
        }

        public void OnTick()
        {
            foreach(KeyValuePair<TimerAction, uint> kvp in timerActions)
            {
                if (tickCounter % kvp.Value == 0)
                    kvp.Key.ProcessAction(tickCounter, context);
            }

            tickCounter++;
        }
    }
}
