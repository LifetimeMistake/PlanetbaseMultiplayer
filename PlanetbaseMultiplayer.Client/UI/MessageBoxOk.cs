using Planetbase;
using PlanetbaseMultiplayer.Client.GameStates;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Client.UI
{
    public static class MessageBoxOk
    {
        public static bool Show(GuiDefinitions.Callback callback, string title, string text)
        {
            GameState gameState = GameManager.getInstance().getGameState();
            if (gameState is GameStateGame)
            {
                GuiGameOverWindow window = new GuiGameOverWindow(callback, title, text);
                GameStateGame gameStateGame = (GameStateGame)gameState;
                FieldInfo mGameGuiInfo = Reflection.GetPrivateFieldOrThrow(gameStateGame.GetType(), "mGameGui", true);
                GameGui mGameGui = (GameGui)Reflection.GetInstanceFieldValue(gameStateGame, mGameGuiInfo);
                mGameGui.setWindow(window);
                return true;
            }

            if (gameState is GameStateMultiplayer)
            {
                GameStateMultiplayer gameStateMultiplayer = (GameStateMultiplayer)gameState;
                gameStateMultiplayer.ShowMessageBox(callback, title, text);
                return true;
            }

            // Not supported
            return false;
        }
    }
}
