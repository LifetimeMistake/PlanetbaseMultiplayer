using PlanetbaseMultiplayer.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client
{
    public static class Globals
    {
        public static Client LocalClient;
        public static Player LocalPlayer;
        public static MultiplayerInteractionManager InteractionManager;
        public static MultiplayerResourceManager ResourceManager;
        public static MultiplayerConstructionManager ConstructionManager;
        public static bool IsInMultiplayerMode;
        public static bool IdSyncRequired;
        public static int IdSync_NextId;
        public static int IdSync_NextBotId;
    }
}
