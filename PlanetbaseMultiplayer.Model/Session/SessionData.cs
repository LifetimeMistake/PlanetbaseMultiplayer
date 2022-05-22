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
        private string serverName;
        private bool passwordProtected;
        private int playerCount;

        public string ServerName { get => serverName; set => serverName = value; }
        public bool PasswordProtected { get => passwordProtected; set => passwordProtected = value; }
        public int PlayerCount { get => playerCount; set => playerCount = value; }

        public SessionData(string serverName, bool passwordProtected, int playerCount)
        {
            this.serverName = serverName ?? throw new ArgumentNullException(nameof(serverName));
            this.passwordProtected = passwordProtected;
            this.playerCount = playerCount;
        }
    }
}
