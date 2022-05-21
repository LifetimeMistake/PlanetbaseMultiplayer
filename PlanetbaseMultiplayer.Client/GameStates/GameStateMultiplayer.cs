using Planetbase;
using PlanetbaseMultiplayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.GameStates
{
	public class GameStateMultiplayer : GameState
	{
		private float mRightOffset;
		private float mAlpha;
		private GuiRenderer mGuiRenderer;
		private bool mShouldFadeIn;

		private string serverTarget;
		private string username;
		private string password;

		private GuiDefinitions.Callback windowCallback;
		private GuiWindow window;

		public GameStateMultiplayer(GameState previousState)
		{
			mShouldFadeIn = !previousState.isTitleState();
			mRightOffset = (float)Screen.width * 0.25f;
			serverTarget = "127.0.0.1:8081";
			username = "Username";
			password = "Password";
			mGuiRenderer = new GuiRenderer();
		}

		public override void onGui()
		{
			if (Input.GetKey(KeyCode.Space))
			{
				return;
			}

			if (window != null)
            {
				RenderWindow();
				return;
            }

			ResourceList instance = ResourceList.getInstance();
			TitleTextures title = instance.Title;
			Texture2D gameTitle = title.GameTitle;
			Vector2 menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
			Vector2 titleLocation = Singleton<TitleScene>.getInstance().getTitleLocation();
			Vector2 menuLocation = Singleton<TitleScene>.getInstance().getMenuLocation();
			float buttonLeftOffset = (float)Screen.width * 0.75f + (((float)Screen.width * 0.25f) - menuButtonSize.x) / 2;
			float num = (float)(Screen.height * gameTitle.height) / 1080f;
			float num2 = num * (float)gameTitle.width / (float)gameTitle.height;
			GUI.color = new Color(1f, 1f, 1f, this.mAlpha);
			GUI.DrawTexture(new Rect(titleLocation.x - num2 * 0.5f, titleLocation.y, num2, num), gameTitle);
			GUI.color = Color.white;
			Texture2D backgroundRight = title.BackgroundRight;
			float num3 = (float)(Screen.height * backgroundRight.height) / 1080f;
			float num4 = num3 * (float)backgroundRight.width / (float)backgroundRight.height;
			GUI.DrawTexture(new Rect((float)Screen.width - num4 + this.mRightOffset, ((float)Screen.height - num3) * 0.75f, num4, num3), backgroundRight);
			float num5 = menuLocation.y;
			float num6 = menuButtonSize.y * 1.3f;

			serverTarget = GUI.TextField(new Rect(buttonLeftOffset + mRightOffset, num5, menuButtonSize.x, menuButtonSize.y), serverTarget,
				21, createTextFieldStyle((int)menuButtonSize.x, (int)menuButtonSize.y));	
			num5 += num6;
			username = GUI.TextField(new Rect(buttonLeftOffset + mRightOffset, num5, menuButtonSize.x, menuButtonSize.y), username,
				21, createTextFieldStyle((int)menuButtonSize.x, (int)menuButtonSize.y));
			num5 += num6;
			password = GUI.TextField(new Rect(buttonLeftOffset + mRightOffset, num5, menuButtonSize.x, menuButtonSize.y), password,
				21, createTextFieldStyle((int)menuButtonSize.x, (int)menuButtonSize.y));

			num5 += num6 * 2;
			if (mGuiRenderer.renderTitleButton(new Rect(buttonLeftOffset + mRightOffset, num5, menuButtonSize.x, menuButtonSize.y), "Connect", FontSize.Huge, true))
			{
				DisableMultiplayer();
				ConnectServer();
			}
			num5 += num6;
			if (mGuiRenderer.renderTitleButton(new Rect(buttonLeftOffset + mRightOffset, num5, menuButtonSize.x, menuButtonSize.y), StringList.get("back"), FontSize.Huge, true))
			{
				DisableMultiplayer();
				GameManager.getInstance().setGameStateTitle();
			}
		}

		public void ShowMessageBox(GuiDefinitions.Callback callback, string title, string text)
        {
			GuiGameOverWindow window = new GuiGameOverWindow(new GuiDefinitions.Callback(WindowCallback), title, text);
			this.window = window;
			this.windowCallback = callback;
		}

		private void RenderWindow()
        {
			window.setOnCancelCallback(new GuiDefinitions.Callback(WindowCallback));
			mGuiRenderer.renderWindow(window, null);
        }
		
		private void WindowCallback(object parameter)
        {
			windowCallback?.Invoke(parameter);
			window = null;
			windowCallback = null;
		}

		public override bool isTitleState()
		{
			return true;
		}

		public override bool shouldDrawAnnouncement()
		{
			return true;
		}

		public override bool isCameraFixed()
		{
			return true;
		}

		public override bool shouldFadeIn()
		{
			return mShouldFadeIn;
		}

		public void init()
		{
			RenderSettings.fog = false;
			CameraManager.getInstance().onTitleScene();
		}

		public override void update(float timeStep)
		{
			base.update(timeStep);
			Singleton<TitleScene>.getInstance().updateOffset(ref mRightOffset);
			if (mAlpha < 1f)
			{
				mAlpha += timeStep * 0.5f;
				if (mAlpha > 1f)
					mAlpha = 1f;
			}
		}

		public GUIStyle createTextFieldStyle(int width, int height)
		{
			GUIStyle style = GuiStyles.getInstance().getBigButtonStyle(FontSize.Huge, (width * 1.2f) / height);
			style.fontSize = (int)((float)(21 * Screen.height) / 1080f);
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.MiddleCenter;
			style.active.textColor = Color.white;
			style.normal.textColor = Color.white;
			style.hover.textColor = Color.white;
			style.active.background = style.normal.background;
			style.hover.background = style.normal.background;
			return style;
		}

		public void ConnectServer()
		{
			string[] endpoint = serverTarget.Split(':');
			if(endpoint.Length != 2)
            {
				// Show message
				ShowMessageBox(null, "Failed to join game", "Invalid server endpoint provided. You need to provide a server IP address and a port. For example: 0.0.0.0:8080");
				return;
            }

			string host = endpoint[0];
			ushort port;
			if (!ushort.TryParse(endpoint[1], out port))
            {
				// Show message
				ShowMessageBox(null, "Failed to join game", "Invalid port provided.");
				return;
            }

			if (username.Length == 0)
            {
				// Show message
				ShowMessageBox(null, "Failed to join game", "Username cannot be empty.");
				return;
            }

			ConnectionOptions connectionOptions = new ConnectionOptions(host, port, username, password);
			Client client = new Client(this);
			Multiplayer.Client = client;
			client.Connect(connectionOptions);
		}

		public void DisableMultiplayer()
		{
			if (Multiplayer.Client != null)
			{
				Multiplayer.Client.Disconnect();
				Multiplayer.Client = null;
			}
		}
	}
}
