using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(TimeManager), "increaseSpeed")]
    class increaseSpeed_Patch
    {
        static bool Prefix()
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner)
            {
                if (GameManager.getInstance().getGameState() is GameStateGame)
                {
                    GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
                    gameState.addToast("Only the simulation owner can control the game speed!", 3);
                }
                return false;
            }
            TimeManager timeManager = TimeManager.getInstance();
            if (!Enum.IsDefined(typeof(GameTimeSpeed), timeManager.mScaleIndex + 1)) return false;
            Globals.LocalClient.OnTimeSpeedChanged_Locally((GameTimeSpeed)timeManager.mScaleIndex + 1, timeManager.mPaused);
            return false;
        }
    }
    [HarmonyPatch(typeof(TimeManager), "setNormalSpeed")]
    class setNormalSpeed_Patch
    {
        static bool Prefix()
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner)
            {
                if(GameManager.getInstance().getGameState() is GameStateGame)
                {
                    GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
                    gameState.addToast("Only the simulation owner can control the game speed!", 3);
                }
                return false;
            }
            TimeManager timeManager = TimeManager.getInstance();
            Globals.LocalClient.OnTimeSpeedChanged_Locally(GameTimeSpeed.Normal, timeManager.mPaused);
            return false;
        }
    }
    [HarmonyPatch(typeof(TimeManager), "decreaseSpeed")]
    class decreaseSpeed_Patch
    {
        static bool Prefix()
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner)
            {
                if (GameManager.getInstance().getGameState() is GameStateGame)
                {
                    GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
                    gameState.addToast("Only the simulation owner can control the game speed!", 3);
                }
                return false;
            }
            TimeManager timeManager = TimeManager.getInstance();
            if (!Enum.IsDefined(typeof(GameTimeSpeed), timeManager.mScaleIndex - 1)) return false;
            Globals.LocalClient.OnTimeSpeedChanged_Locally((GameTimeSpeed)timeManager.mScaleIndex - 1, timeManager.mPaused);
            return false;
        }
    }
    // temporary override. might remove later
    // this override disallows pausing/unpausing the game locally while the game is running in multiplayer mode
    [HarmonyPatch(typeof(TimeManager), "pause")]
    class pause_Patch
    {
        public static bool Prefix()
        {
            return !Globals.IsInMultiplayerMode;
        }
    }
    [HarmonyPatch(typeof(TimeManager), "unpause")]
    class unpause_Patch
    {
        public static bool Prefix()
        {
            return !Globals.IsInMultiplayerMode;
        }
    }

}
