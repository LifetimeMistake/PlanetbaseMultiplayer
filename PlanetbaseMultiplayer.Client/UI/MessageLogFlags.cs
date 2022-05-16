using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.UI
{
    public enum MessageLogFlags
    {
        MessageSoundNormal = 0,
        MessageSoundAlert = 1,
        MessageSoundPowerDown = 2,
        LongMessageDuration = 4,
        VeryLongMessageDuration = 8
    }
}
