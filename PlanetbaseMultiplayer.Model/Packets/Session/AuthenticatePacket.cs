using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class AuthenticatePacket : Packet
    {
        public bool AuthenticationSuccessful;
        public AuthenticationErrorReason? ErrorReason;
        public Player? LocalPlayer;
        public Player[] Players;

        public AuthenticatePacket(bool authenticationSuccessful, AuthenticationErrorReason? errorReason, Player? localPlayer, Player[] players)
        {
            AuthenticationSuccessful = authenticationSuccessful;
            ErrorReason = errorReason;
            LocalPlayer = localPlayer;
            Players = players;
        }
    }
}
