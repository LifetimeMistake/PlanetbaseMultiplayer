using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client
{
    public class ConnectionOptions
    {
        public string Host;
        public ushort Port;
        public string Username;
        public string Password;

        public ConnectionOptions(string host, ushort port, string username, string password)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            Port = port;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
