using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Session
{
    public enum DisconnectReason
    {
        DisconnectRequest, // Used for signaling the disconnect reason to other players
        DisconnectRequestResponse, // Used internally in the graceful disconnect mechanism
        KickedOut, // Used for signaling the disconnect reason to other players
        ConnectionLost, // Used for signaling the disconnect reason to other players
        ServerClosing
    }
}
