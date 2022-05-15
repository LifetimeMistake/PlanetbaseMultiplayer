using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class SessionDataPacket : Packet
    {
        public SessionData SessionData;

        public SessionDataPacket(SessionData sessionData)
        {
            SessionData = sessionData;
        }
    }
}
