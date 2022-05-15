using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Session
{
    public enum AuthenticationErrorReason
    {
        UsernameTaken = 0,
        IncorrectPassword = 1,
        IllegalUsername = 2
    }
}
