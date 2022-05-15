using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class AuthenticateRequestPacket : Packet
    {
        public string Username;
        public string Password;

        public AuthenticateRequestPacket(string username, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password;
        }
    }
}
