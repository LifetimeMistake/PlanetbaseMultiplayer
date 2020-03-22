using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
	[HarmonyPatch(typeof(GameStateGame), "onExitGameForReal", new[] { typeof(object) })]
	class disconnect_MultiplayerServer
	{
		static bool Prefix()
		{
			if (Globals.IsInMultiplayerMode)
			{
				Globals.LocalClient.SendDisconnect();
				Globals.IsInMultiplayerMode = false;
				Globals.LocalClient = null;
				Globals.LocalPlayer = null;
				Console.WriteLine("Disconnected.");
			}

			return true;
		}
	}
	[HarmonyPatch(typeof(GameManager), "fixedUpdate", new[] { typeof(float) })]
	class Hook_fixedUpdate
	{
		static bool Prefix(GameManager __instance)
		{
			if (!Globals.IsInMultiplayerMode) return true;
#if DEBUG
			if (Globals.LocalClient.debug_eventList.Count > 100)
				while (Globals.LocalClient.debug_eventList.Count > 100)
					Globals.LocalClient.debug_eventList.RemoveAt(0);
			Globals.LocalClient.lastTick_PacketCount = Globals.LocalClient.packetQueue.Count;
#endif
			if (Globals.LocalPlayer != null)
				if (Globals.LocalPlayer.ClientState == ClientState.ConnectedReady)
				{
					List<int> ids = MultiplayerUtil.GetAllIds();
					if (ids.Count != 0)
					{
						ids.Sort();
						IdGenerator.getInstance().mNextId = ids.Last() + 2;
					}
				}

			while (Globals.LocalClient.packetQueue.Count != 0)
			{
				if (Globals.LocalClient.packetQueue.Count == 0) return true;
				Packet packet = Globals.LocalClient.packetQueue.Dequeue();
				Globals.LocalClient.ProcessPacket(packet);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Util), "captureScreenshot", new[] { typeof(int) })]
	class override_captureScreenshot
	{
		static bool Prefix(ref byte[] __result)
		{
			if (Globals.IsInMultiplayerMode)
			{
				__result = new byte[0];
				return false;
			}
			else
				return true;
		}
	}

	[HarmonyPatch(typeof(GameStateTitle), "onGui")]
	class add_MultiplayerButton // will rewrite this later to use the harmony transpiler instead
	{
		static bool Prefix(GameStateTitle __instance)
		{
			if (Input.GetKey(KeyCode.Space))
			{
				return false;
			}
			if (__instance.mGuiRenderer == null)
			{
				__instance.mGuiRenderer = new GuiRenderer();
			}
			ResourceList instance = ResourceList.getInstance();
			TitleTextures title = instance.Title;
			Texture2D gameTitle = title.GameTitle;
			Vector2 menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
			Vector2 titleLocation = Singleton<TitleScene>.getInstance().getTitleLocation();
			Vector2 menuLocation = Singleton<TitleScene>.getInstance().getMenuLocation();
			float num = (float)(Screen.height * gameTitle.height) / 1080f;
			float num2 = num * (float)gameTitle.width / (float)gameTitle.height;
			GUI.color = new Color(1f, 1f, 1f, __instance.mAlpha);
			GUI.DrawTexture(new Rect(titleLocation.x - num2 * 0.5f, titleLocation.y, num2, num), gameTitle);
			GUI.color = Color.white;
			Texture2D backgroundRight = title.BackgroundRight;
			float num3 = (float)(Screen.height * backgroundRight.height) / 1080f;
			float num4 = num3 * (float)backgroundRight.width / (float)backgroundRight.height;
			GUI.DrawTexture(new Rect((float)Screen.width - num4 + __instance.mRightOffset, ((float)Screen.height - num3) * 0.75f, num4, num3), backgroundRight);
			float num5 = menuLocation.y * 0.95f;
			float num6 = menuButtonSize.y * 1.2f;
			menuLocation.x -= menuButtonSize.x;
			menuLocation.x += __instance.mRightOffset;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("new_game"), FontSize.Huge, true))
			{
				if (__instance.canAlreadyPlay())
				{
					GameManager.getInstance().setGameStateLocationSelection();
				}
				else
				{
					__instance.renderTutorialRequestWindow(new GuiDefinitions.Callback(__instance.onWindowCancelNewGame));
				}
			}
			GUI.enabled = __instance.mAnySavegames;
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("continue_game"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateGameContinue();
			}
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("load_game"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateLoadGame();
			}
			num5 += num6;
			GUI.enabled = true;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), "Multiplayer", FontSize.Huge, true))
			{
				GameManager.getInstance().setNewState(new GameStateMultiplayer(GameManager.getInstance().getGameState()));
			}
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("tutorial"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateGameNew(1, 0, true, null);
			}
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("challenges"), FontSize.Huge, true))
			{
				if (__instance.canAlreadyPlay())
				{
					GameManager.getInstance().setGameStateChallengeSelection();
				}
				else
				{
					__instance.renderTutorialRequestWindow(new GuiDefinitions.Callback(__instance.onWindowCancelChallenges));
				}
			}
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("settings"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateSettings();
			}
			num5 += num6;
			if (__instance.mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("quit"), FontSize.Huge, true))
			{
				Application.Quit();
			}
			if (__instance.mConfirmWindow != null)
			{
				__instance.mGuiRenderer.renderWindow(__instance.mConfirmWindow, null);
			}
			int num7 = 3;
			float num8 = menuButtonSize.y * 0.75f;
			float num9 = menuButtonSize.y * 0.25f;
			Vector2 vector = new Vector2(((float)Screen.width - (float)num7 * num8 - (float)(num7 - 1) * num9) * 0.5f, (float)Screen.height - num8 - num9 + __instance.mRightOffset * 0.5f);
			Rect rect = new Rect(vector.x, vector.y, num8, num8);
			if (__instance.mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.Credits, StringList.get("credits")), null))
			{
				GameManager.getInstance().setGameStateCredits();
			}
			rect.x += num8 + num9;
			if (__instance.mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.SwitchPlanet, StringList.get("switch_planet")), null))
			{
				Singleton<TitleScene>.getInstance().switchPlanet();
			}
			rect.x += num8 + num9;
			if (__instance.mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.Achievements, StringList.get("achievements")), null))
			{
				GameManager.getInstance().setGameStateAchievements();
			}
			rect.x += num8 + num9;
			return false;
		}
	}
}
