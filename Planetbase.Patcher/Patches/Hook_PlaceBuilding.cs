using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
					Globals.LocalClient.OnModulePlaced_Locally(__instance.mActiveModule);
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
				Globals.LocalClient.OnConnectionPlaced_Locally((Module)Selection.getSelected(), (Module)Selection.getLinkTarget());
				__instance.mMenuSystem.setMainMenu();
				Selection.getSelected().playSound(SoundList.getInstance().ConnectionPlace);
				__instance.clearSelection();
			}
			return false;
		}
	}
}
