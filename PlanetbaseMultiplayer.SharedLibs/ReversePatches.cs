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
		public static void MP_loadResource(this Character character, Resource resource)
		{
			if (character.mLoadedResource != null)
			{
				Debug.LogError("Trying to load resource while loaded: " + resource.getName());
			}
			character.mLoadedResource = resource;
			character.mLoadedResource.load();
			character.mLoadedResource.attach(character.getAnchorPoint());
			character.updateExoskeleton();
		}
		public static void MP_unloadResource(this Character character, Resource.State resourceState)
		{
			character.mLoadedResource.detach();
			character.mLoadedResource.drop(resourceState);
			character.mLoadedResource = null;
			character.updateExoskeleton();
		}
		public static void MP_embedResource(this Character character, ConstructionComponent component, Resource.State resourceState)
		{
			character.mLoadedResource.detach();
			character.mLoadedResource.setState(resourceState);
			component.embedResource(character.mLoadedResource);
			character.mLoadedResource = null;
			character.updateExoskeleton();
		}
		public static void MP_storeResource(this Character character, Module module)
		{
			character.mLoadedResource.detach();
			StorageSlot storageSlot = module.findStorageSlot(character.getPosition());
			if (storageSlot != null)
			{
				storageSlot.addResource(character.mLoadedResource);
			}
			else
			{
				character.mLoadedResource.drop(Resource.State.Idle);
			}
			character.mLoadedResource = null;
			character.updateExoskeleton();
		}
		public static void MP_destroyResource(this Character character)
		{

			if (character.mLoadedResource != null)
			{
				character.mLoadedResource.destroy();
				character.mLoadedResource = null;
			}
			character.updateExoskeleton();
		}
		public static void MP_buildableOnBuilt(this Buildable buildable)
		{
			buildable.mState = BuildableState.Built;
			if (buildable.mConstructionMaterials != null)
			{
				buildable.mConstructionMaterials.destroyAll();
				buildable.mConstructionMaterials = null;
			}
			buildable.mIndicators.Remove(buildable.mBuildProgress);
			buildable.mBuildProgress.setValue(-1f);
			buildable.mPendingConstructionCosts = null;
		}
	}
}
