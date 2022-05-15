using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Players
{
    public enum PlayerState
    {
        ConnectedUnauthenticated = 0, // Player connection has been accepted, the client should send an AuthenticatePacket to progress
        ConnectedMainMenu = 1, // The authentication ticket was accepted, the client is waiting for game data (player is in the main menu)
        ConnectedLoadingData = 2, // The client has received the game data, now loading the scene (player is on the loading screen)
        ConnectedReady = 3 // The client has finished initializing the world (player is in game)
    }
}
