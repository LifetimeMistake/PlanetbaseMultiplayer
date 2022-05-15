using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Session
{
    [Serializable]
    public struct SessionData
    {
        public string ServerName;
        public bool PasswordProtected;
        public int PlayerCount;

        public SessionData(string serverName, bool passwordProtected, int playerCount)
        {
            ServerName = serverName ?? throw new ArgumentNullException(nameof(serverName));
            PasswordProtected = passwordProtected;
            PlayerCount = playerCount;
        }
    }
}
