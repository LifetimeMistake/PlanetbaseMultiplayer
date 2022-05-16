using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Patcher.Patches.Core
{
	[HarmonyPatch(typeof(GameStateGame), "onExitGameForReal", new[] { typeof(object) })]
	class OnExitMultiplayerGame
	{
		static bool Prefix()
		{
			if (Multiplayer.Client != null)
			{
				if (Multiplayer.Client.LocalPlayer.HasValue)
                {
					Multiplayer.Client.RequestDisconnect();
				}
				else
                {
					Multiplayer.Client.Disconnect();
					Multiplayer.Client = null;
				}
			}

			return true;
		}
	}
}
