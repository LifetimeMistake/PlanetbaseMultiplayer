using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Utils
{
    public static class DisconnectReasonUtils
    {
        public static string ReasonToString(DisconnectReason reason)
        {
            switch (reason)
            {
                case DisconnectReason.DisconnectRequest:
                    return "Disconnected";
                case DisconnectReason.KickedOut:
                    return "Kicked out";
                case DisconnectReason.ConnectionLost:
                    return "Connection lost";
                case DisconnectReason.ServerClosing:
                    return "Server closing";
                default:
                    return "Unknown reason";
            }
        }
    }
}
