using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(GameStateGame), "placeModule")]
    class hook_placeModule
    {
        static bool Prefix(GameStateGame __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
			if (__instance.mActiveModule != null)
			{
				if (__instance.mActiveModule.isValidPosition())
				{
					__instance.mActiveModule.onUserPlaced();
					Globals.ConstructionManager.AddBuildable(__instance.mActiveModule);
					__instance.mActiveModule.playSound(SoundList.getInstance().ConstructionPlace);
					__instance.onModulePlacementEnd();
					return false;
				}
				Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ButtonClickWrong, null);
				__instance.mActiveModule.destroy();
				__instance.onModulePlacementEnd();
				__instance.addToast(StringList.get("hint_placement_invalid"), 3f);
			}
            return false;
        }
    }

	[HarmonyPatch(typeof(GameStateGame), "onButtonLink", new[] { typeof(object) })]
	class hook_placeConnection
	{
		static bool Prefix(GameStateGame __instance)
		{
			if (!Globals.IsInMultiplayerMode) return true;
			if(__instance.mMode == GameStateGame.Mode.Idle && Selection.isLinkable())
			{
				Connection connection = linkModules((Module)Selection.getSelected(), (Module)Selection.getLinkTarget(), __instance.mRenderTops);
				Globals.ConstructionManager.AddBuildable(connection);
				__instance.mMenuSystem.setMainMenu();
				Selection.getSelected().playSound(SoundList.getInstance().ConnectionPlace);
				__instance.clearSelection();
			}
			return false;
		}

		static Connection linkModules(Module m1, Module m2, bool renderTops)
		{
			if (Connection.canLink(m1, m2))
			{
				Connection connection = Connection.create(m1, m2);
				connection.onUserPlaced();
				connection.setRenderTop(renderTops);
				m1.recycleLinkComponents();
				m2.recycleLinkComponents();
				return connection;
			}

			return null;
		}
	}

	[HarmonyPatch(typeof(GameStateGame), "placeComponent")]
	class hook_placeComponent
	{
		static bool Prefix(GameStateGame __instance)
		{
			if (!Globals.IsInMultiplayerMode) return true;
			__instance.mPlacedComponent.setPositionY(__instance.mPlacedComponent.getParentConstruction().getFloorPosition().y);
			__instance.mActiveModule.addComponent(__instance.mPlacedComponent);
			__instance.mPlacedComponent.onUserPlaced();
			Globals.ConstructionManager.AddBuildable(__instance.mPlacedComponent);
			__instance.mPlacedComponent.playSound(SoundList.getInstance().ComponentPlace);
			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			ComponentType componentType = null;
			if (flag && __instance.mPlacedComponent != null && !__instance.inTutorial())
			{
				componentType = __instance.mPlacedComponent.getComponentType();
			}
			__instance.onComponentPlacementEnd();
			if (componentType != null)
			{
				__instance.startPlacingComponent(componentType);
			}
			return false;
		}
	}
}
