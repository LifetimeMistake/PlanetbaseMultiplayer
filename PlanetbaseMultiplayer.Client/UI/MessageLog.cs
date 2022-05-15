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
        public static void Show(string description, Texture2D icon, MessageLogFlags flags)
        {
            Message message = new Message(description, icon, (int)flags);
            Planetbase.MessageLog.getInstance().addMessage(message);
        }
    }
}
