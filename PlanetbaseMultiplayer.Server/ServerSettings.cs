using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server
{
    public class ServerSettings
    {
        public string Name;
        public bool PasswordProtected;
        public string Password;
        public ushort Port;
        public string SavePath;

        public ServerSettings(string name, string password, ushort port, string savePath)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PasswordProtected = true;
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Port = port;
            SavePath = savePath ?? throw new ArgumentNullException(nameof(savePath));
        }

        public ServerSettings(string name, ushort port, string savePath)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PasswordProtected = false;
            Password = null;
            Port = port;
            SavePath = savePath ?? throw new ArgumentNullException(nameof(savePath));
        }
    }
}
