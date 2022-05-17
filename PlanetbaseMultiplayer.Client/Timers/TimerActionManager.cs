using PlanetbaseMultiplayer.Client.Timers.Actions.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Timers
{
    public class TimerActionManager
    {
        private ulong tickCounter;
        private ClientProcessorContext processorContext;
        private Dictionary<TimerAction, uint> timerActions;

        public TimerActionManager(ClientProcessorContext processorContext)
        {
            this.processorContext = processorContext;
            timerActions = new Dictionary<TimerAction, uint>();
        }

        public void RegisterAction(TimerAction action, uint activationInterval)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

#if DEBUG
            Console.WriteLine($"Registered timer action {action.GetType().FullName} with activation interval {activationInterval}");
#endif

            timerActions.Add(action, activationInterval);
        }

        public void OnTick()
        {
            foreach(KeyValuePair<TimerAction, uint> kvp in timerActions)
            {
                if (tickCounter % kvp.Value == 0)
                    kvp.Key.ProcessAction(tickCounter, processorContext);
            }

            tickCounter++;
        }
    }
}
