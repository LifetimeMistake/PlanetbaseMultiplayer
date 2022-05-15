using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.UI
{
    public static class MessageLog
    {
        public static bool Show(string description, Texture2D icon, MessageLogFlags flags)
        {
            if (!(GameManager.getInstance().getGameState() is GameStateGame))
                return false;

            Message message = new Message(description, icon, (int)flags);
            Planetbase.MessageLog.getInstance().addMessage(message);
            return true;
        }
    }
}
