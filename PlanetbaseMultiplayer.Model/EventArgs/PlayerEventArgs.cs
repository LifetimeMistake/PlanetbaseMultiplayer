using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.EventArgs
{
    public class PlayerEventArgs : System.EventArgs
    {
        public Guid PlayerId;

        public PlayerEventArgs(Guid playerId)
        {
            PlayerId = playerId;
        }
    }
}
