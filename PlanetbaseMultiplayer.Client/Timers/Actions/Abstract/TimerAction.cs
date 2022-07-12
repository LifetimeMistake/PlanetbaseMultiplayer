using PlanetbaseMultiplayer.Model.Packets.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Timers.Actions.Abstract
{
    public abstract class TimerAction
    {
        public abstract void ProcessAction(ulong currentTick, ProcessorContext context);
    }
}
