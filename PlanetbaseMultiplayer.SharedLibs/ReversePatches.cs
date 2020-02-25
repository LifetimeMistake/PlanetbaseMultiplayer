using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.SharedLibs
{
    public static class ReversePatches
    {
        public static void MP_startWalking(this Character character, Target target, Selectable[] indirectTargets = null)
        {
			if (character.isWaitingForAirlock())
			{
				character.mInteractions[0].destroy();
			}
			if (character.anyInteractions() && !character.isWaitingForAirlock())
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Starting to walk when interacting: ",
					character.getName(),
					": ",
					character.mInteractions[0]
				}));
			}
			if (!character.canStartWalking(target))
			{
				character.setIdle(null, CharacterAnimation.PlayMode.CrossFade);
				return;
			}
			character.setIndirectTargets(indirectTargets);
			if (character.mState == Character.State.Ko)
			{
				UnityEngine.Debug.LogWarning("Character " + character.getName() + ", is being given orders to walk while KO");
			}
			character.setTarget(target);
			character.mImmediateTarget = null;
			character.mNavigationPath = null;
			character.mState = Character.State.Walking;
			character.playAnimation(new CharacterAnimation(CharacterAnimationType.Idle, 1f), WrapMode.Loop, CharacterAnimation.PlayMode.CrossFade);
		}
    }
}
