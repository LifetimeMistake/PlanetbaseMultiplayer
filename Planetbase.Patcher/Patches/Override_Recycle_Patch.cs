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
    [HarmonyPatch(typeof(GameStateGame), "onRecycle")]
    class Override_Recycle_Patch
    {
        static bool Prefix(GameStateGame __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            Selectable selected = Selection.getSelected();
            if (selected != null)
                selected.playSound(SoundList.getInstance().ConstructionRecycle);

            // recycle
            if(selected != null)
            {
                Console.WriteLine($"Recycled: {selected.GetType()}");
                if (selected is ColonyShip)
                    Recycle_ColonyShip(selected as ColonyShip);
                else if (selected is ConstructionComponent)
                    Recycle_Component(selected as ConstructionComponent);
                else
                    Recycle_Selectable(selected);
            }
            Selection.clear();
            // recycle

            if (__instance.mMode == GameStateGame.Mode.EditingModule)
                __instance.mMenuSystem.setEditMenu(__instance.mActiveModule);
            else
                __instance.mMenuSystem.setMainMenu();
            __instance.mInfoPanel = null;
            __instance.mGameGui.setWindow(null);
            return false;
		}

        private static void Recycle_Selectable(Selectable selectable)
        {
            List<ResourceConstructionData> created = new List<ResourceConstructionData>();
            ResourceAmounts resourceAmounts = selectable.calculateRecycleAmounts();
            if (resourceAmounts != null)
            {
                foreach (ResourceAmount resourceAmount in resourceAmounts)
                {
                    for (int i = 0; i < resourceAmount.getAmount(); i++)
                    {
                        created.Add(new ResourceConstructionData(resourceAmount.getResourceType().GetType().Name, MultiplayerUtil.GetDefaultResourceSubtype(resourceAmount.getResourceType()),
                            (Vector3_Serializable)(selectable.getPosition() + MathUtil.randFlatVector(selectable.getRadius())), new Quaternion_Serializable(), Location.Exterior, false));
                    }
                }
            }
            Globals.LocalClient.OnSelectableRecycled(selectable, created.ToArray());
        }

        static void Recycle_Component(ConstructionComponent component)
        {
            List<ResourceConstructionData> createdResources = new List<ResourceConstructionData>();
            List<ResourceDestructionData> destroyedResources = new List<ResourceDestructionData>();
            ResourceAmounts resourceAmounts = CalculateBasicAmounts(component);
            if(component.mResourceContainer != null)
            {
                foreach (Resource resource in component.mResourceContainer.getResources())
                {
                    resourceAmounts.add(resource.getResourceType(), 1);
                    destroyedResources.Add(new ResourceDestructionData(resource.getId()));
                }
            }
            if(resourceAmounts != null)
            {
                int num = 0;
                foreach(ResourceAmount resourceAmount in resourceAmounts)
                {
                    for (int i = 0; i < resourceAmount.getAmount(); i++)
                    {
                        Vector3 b = new Vector3((float)(num % 2) - 0.5f, 0f, (float)(num / 2 % 2) - 0.5f);
                        createdResources.Add(new ResourceConstructionData(resourceAmount.getResourceType().GetType().Name, MultiplayerUtil.GetDefaultResourceSubtype(resourceAmount.getResourceType()),
                            (Vector3_Serializable)(component.getPosition() + b), (Quaternion_Serializable)component.getTransform().rotation, Location.Interior, false));
                        num++;
                    }
                }
            }
            Globals.LocalClient.OnComponentRecycled(component, createdResources.ToArray(), destroyedResources.ToArray());
        }

        static ResourceAmounts CalculateBasicAmounts(ConstructionComponent component)
        {
            if(component.isBuilt())
            {
                ResourceAmounts resourceAmounts = component.calculateCost();
                foreach (ResourceAmount resourceAmount in resourceAmounts)
                {
                    resourceAmount.setAmount(Mathf.RoundToInt((float)resourceAmount.getAmount() * 0.666f));
                }
                return resourceAmounts;
            }
            return null;
        }

        static void Recycle_ColonyShip(ColonyShip colonyShip)
        {
            List<ResourceUpdateData> extracted = new List<ResourceUpdateData>();
            List<ResourceConstructionData> created = new List<ResourceConstructionData>();
            foreach (Resource resource in colonyShip.mResourceContainer.getResources())
            {
                extracted.Add(new ResourceUpdateData(resource.getId(), ResourceAction.Extract, (Vector3_Serializable)(colonyShip.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior));
            }
            ResourceType metalType = TypeList<ResourceType, ResourceTypeList>.find<Metal>();
            ResourceType bioplasticType = TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>();
            for (int i = 0; i < 15; i++)
            {
                created.Add(new ResourceConstructionData(metalType.GetType().Name, ResourceSubtype.None, (Vector3_Serializable)(colonyShip.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior, false));
            }
            for (int i = 0; i < 10; i++)
            {
                created.Add(new ResourceConstructionData(bioplasticType.GetType().Name, ResourceSubtype.None, (Vector3_Serializable)(colonyShip.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f),
                    new Quaternion_Serializable(), Location.Exterior, false));
            }
            Globals.LocalClient.OnColonyShipRecycled(colonyShip, created.ToArray(), extracted.ToArray());
        }
    }
}
