using PlanetbaseMultiplayer.Client.Timers.Actions.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Timers.Actions
{
    public class ProcessPacketsAction : TimerAction
    {
        public override void ProcessAction(ulong currentTick, ClientProcessorContext context)
        {
            context.Client.ProcessPackets();
        }
    }
}
