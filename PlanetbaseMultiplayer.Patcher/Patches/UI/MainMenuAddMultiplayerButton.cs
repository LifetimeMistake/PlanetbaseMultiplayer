using HarmonyLib;
using Planetbase;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Client.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches.UI
{
	[HarmonyPatch(typeof(GameStateTitle), "onGui")]
	class MainMenuAddMultiplayerButton // will rewrite this later to use the harmony transpiler instead
	{
		static bool Prefix(GameStateTitle __instance)
		{
			if (Input.GetKey(KeyCode.Space))
			{
				return false;
			}

			FieldInfo mGuiRendererInfo;
			FieldInfo mAlphaInfo;
			FieldInfo mRightOffsetInfo;
			FieldInfo mConfirmWindowInfo;
			FieldInfo mAnySavegamesInfo;
			MethodInfo canAlreadyPlayInfo;
			MethodInfo renderTutorialRequestWindowInfo;

			if (!Reflection.TryGetPrivateField(__instance.GetType(), "mGuiRenderer", true, out mGuiRendererInfo))
			{
				Debug.LogError("Failed to find \"mGuiRenderer\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateField(__instance.GetType(), "mAlpha", true, out mAlphaInfo))
			{
				Debug.LogError("Failed to find \"mAlpha\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateField(__instance.GetType(), "mRightOffset", true, out mRightOffsetInfo))
			{
				Debug.LogError("Failed to find \"mRightOffset\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateField(__instance.GetType(), "mConfirmWindow", true, out mConfirmWindowInfo))
			{
				Debug.LogError("Failed to find \"mConfirmWindow\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateField(__instance.GetType(), "mAnySavegames", true, out mAnySavegamesInfo))
			{
				Debug.LogError("Failed to find \"mAnySavegames\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateMethod(__instance.GetType(), "canAlreadyPlay", true, out canAlreadyPlayInfo))
			{
				Debug.LogError("Failed to find \"canAlreadyPlay\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			if (!Reflection.TryGetPrivateMethod(__instance.GetType(), "renderTutorialRequestWindow", true, out renderTutorialRequestWindowInfo))
			{
				Debug.LogError("Failed to find \"renderTutorialRequestWindow\", disabling MainMenuAddMultiplayerButton patch");
				return true;
			}

			GuiRenderer mGuiRenderer = (GuiRenderer)Reflection.GetInstanceFieldValue(__instance, mGuiRendererInfo);

			if (mGuiRenderer == null)
			{
				mGuiRenderer = new GuiRenderer();
				Reflection.SetInstanceFieldValue(__instance, mGuiRendererInfo, mGuiRenderer);
			}
			ResourceList instance = ResourceList.getInstance();
			TitleTextures title = instance.Title;
			Texture2D gameTitle = title.GameTitle;
			Vector2 menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
			Vector2 titleLocation = Singleton<TitleScene>.getInstance().getTitleLocation();
			Vector2 menuLocation = Singleton<TitleScene>.getInstance().getMenuLocation();
			float num = (float)(Screen.height * gameTitle.height) / 1080f;
			float num2 = num * (float)gameTitle.width / (float)gameTitle.height;
			GUI.color = new Color(1f, 1f, 1f, (float)Reflection.GetInstanceFieldValue(__instance, mAlphaInfo));
			GUI.DrawTexture(new Rect(titleLocation.x - num2 * 0.5f, titleLocation.y, num2, num), gameTitle);
			GUI.color = Color.white;
			Texture2D backgroundRight = title.BackgroundRight;
			float num3 = (float)(Screen.height * backgroundRight.height) / 1080f;
			float num4 = num3 * (float)backgroundRight.width / (float)backgroundRight.height;
			GUI.DrawTexture(new Rect((float)Screen.width - num4 + (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo), ((float)Screen.height - num3) * 0.75f, num4, num3), backgroundRight);
			float num5 = menuLocation.y * 0.95f;
			float num6 = menuButtonSize.y * 1.2f;
			menuLocation.x -= menuButtonSize.x;
			menuLocation.x += (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo);
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("new_game"), FontSize.Huge, true))
			{
				if ((bool)Reflection.InvokeInstanceMethod(__instance, canAlreadyPlayInfo))
				{
					GameManager.getInstance().setGameStateLocationSelection();
				}
				else
				{
					Reflection.InvokeInstanceMethod(__instance, renderTutorialRequestWindowInfo, new GuiDefinitions.Callback(__instance.onWindowCancelNewGame));
				}
			}
			GUI.enabled = (bool)Reflection.GetInstanceFieldValue(__instance, mAnySavegamesInfo);
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("continue_game"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateGameContinue();
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("load_game"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateLoadGame();
			}
			num5 += num6;
			GUI.enabled = true;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), "Multiplayer", FontSize.Huge, true))
			{
				GameManager.getInstance().setNewState(new GameStateMultiplayer(GameManager.getInstance().getGameState()));
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("tutorial"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateGameNew(1, 0, true, null);
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("challenges"), FontSize.Huge, true))
			{
				if ((bool)Reflection.InvokeInstanceMethod(__instance, canAlreadyPlayInfo))
				{
					GameManager.getInstance().setGameStateChallengeSelection();
				}
				else
				{
					Reflection.InvokeInstanceMethod(__instance, renderTutorialRequestWindowInfo, new GuiDefinitions.Callback(__instance.onWindowCancelChallenges));
				}
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("settings"), FontSize.Huge, true))
			{
				GameManager.getInstance().setGameStateSettings();
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(menuLocation.x, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("quit"), FontSize.Huge, true))
			{
				Application.Quit();
			}
			if ((GuiConfirmWindow)Reflection.GetInstanceFieldValue(__instance, mConfirmWindowInfo) != null)
			{
				mGuiRenderer.renderWindow((GuiConfirmWindow)Reflection.GetInstanceFieldValue(__instance, mConfirmWindowInfo), null);
			}
			int num7 = 3;
			float num8 = menuButtonSize.y * 0.75f;
			float num9 = menuButtonSize.y * 0.25f;
			Vector2 vector = new Vector2(((float)Screen.width - (float)num7 * num8 - (float)(num7 - 1) * num9) * 0.5f, (float)Screen.height - num8 - num9 + (float)Reflection.GetInstanceFieldValue(__instance, mRightOffsetInfo) * 0.5f);
			Rect rect = new Rect(vector.x, vector.y, num8, num8);
			if (mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.Credits, StringList.get("credits")), null))
			{
				GameManager.getInstance().setGameStateCredits();
			}
			rect.x += num8 + num9;
			if (mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.SwitchPlanet, StringList.get("switch_planet")), null))
			{
				Singleton<TitleScene>.getInstance().switchPlanet();
			}
			rect.x += num8 + num9;
			if (mGuiRenderer.renderButton(rect, new GUIContent(null, instance.Icons.Achievements, StringList.get("achievements")), null))
			{
				GameManager.getInstance().setGameStateAchievements();
			}
			rect.x += num8 + num9;
			return false;
		}
	}
}
