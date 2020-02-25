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
					Globals.LocalClient.OnModulePlaced(__instance.mActiveModule);
					__instance.mActiveModule.playSound(SoundList.getInstance().ConstructionPlace);
					__instance.mActiveModule.destroy();
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
				Globals.LocalClient.OnConnectionPlaced((Module)Selection.getSelected(), (Module)Selection.getLinkTarget());
				__instance.mMenuSystem.setMainMenu();
				Selection.getSelected().playSound(SoundList.getInstance().ConnectionPlace);
				__instance.clearSelection();
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(GameStateGame), "placeComponent")]
	class hook_placeComponent
	{
		static bool Prefix(GameStateGame __instance)
		{
			if (!Globals.IsInMultiplayerMode) return true;
			Construction parentConstruction = __instance.mPlacedComponent.getParentConstruction();
			if (__instance.mValidComponentPosition == null) { UnityEngine.Debug.LogError("mvalidComponentPosition was null"); }
			if (__instance.mValidComponentRotation == null) { UnityEngine.Debug.LogError("mValidComponentRotation was null"); }
			Vector3 position = (Vector3)__instance.mValidComponentPosition;
			Quaternion rotation = (Quaternion)__instance.mValidComponentRotation;
			string componentType = __instance.mPlacedComponent.getComponentType().GetType().Name;
			// original code
			__instance.mPlacedComponent.playSound(SoundList.getInstance().ComponentPlace);
			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			ComponentType componentType__ = null;
			if (flag && __instance.mPlacedComponent != null && !__instance.inTutorial())
				componentType__ = __instance.mPlacedComponent.getComponentType();
			__instance.cancelComponentPlacement();
			if (componentType__ != null)
				__instance.startPlacingComponent(componentType__);
			Globals.LocalClient.OnComponentPlaced(parentConstruction, position, rotation, componentType);
			return false;
		}
	}
}
