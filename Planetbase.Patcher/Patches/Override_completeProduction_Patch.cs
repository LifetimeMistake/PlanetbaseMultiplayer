using Harmony;
using Planetbase;
using PlanetbaseMultiplayer.Client;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetbaseMultiplayer.Patcher.Patches
{
    [HarmonyPatch(typeof(ConstructionComponent), "completeProduction")]
    class Override_completeProduction_Patch
    {
        static bool Prefix(ConstructionComponent __instance)
        {
            if (!Globals.IsInMultiplayerMode) return true;
            if (!Globals.LocalPlayer.IsSimulationOwner) return false; // only the simulation owner can send this packet type
            __instance.mProductionProgress.setValue(0f); // fixes duplicate production
            List<ProductionItem> production = __instance.mComponentType.getProduction();
            List<ResourceType> resourceConsumption = __instance.mComponentType.getResourceConsumption();
            Recipe recipe = __instance.findRecipee();

            List<ResourceConstructionData> producedResources = new List<ResourceConstructionData>();
            List<ResourceDestructionData> consumedResources = new List<ResourceDestructionData>();

            // this is basically a copy paste of the original function
            if(recipe != null)
            {
                if(recipe.getIngredients()[0] != ResourceSubtype.None)
                {
                    foreach (Resource resource in getConsumedResources(__instance, recipe))
                        consumedResources.Add(new ResourceDestructionData(resource.getId()));
                }
                else
                {
                    foreach (Resource resource in getConsumedResources(__instance, resourceConsumption))
                        consumedResources.Add(new ResourceDestructionData(resource.getId()));
                }
                for(int i = 0; i < Mathf.Max(recipe.getIngredients().Length, 1); i++)
                {
                    ResourceConstructionData? data = produceResource(__instance, production[0].getResourceType(), recipe.getMeal(), __instance.shouldEmbedProduction());
                    if (data.HasValue)
                        producedResources.Add(data.Value);
                }
            }
            else
            {
                if(resourceConsumption != null)
                {
                    foreach (Resource resource in getConsumedResources(__instance, resourceConsumption))
                        consumedResources.Add(new ResourceDestructionData(resource.getId()));
                }
                if(__instance.hasFlag(131072))
                {
                    ResourceConstructionData? data = produceItem(__instance, production[__instance.mProducedItemIndex]);
                    if (data.HasValue)
                        producedResources.Add(data.Value);
                }
                else
                {
                    for(int j = 0; j <production.Count; j++)
                    {
                        ResourceConstructionData? data = produceItem(__instance, production[j]);
                        if (data.HasValue)
                            producedResources.Add(data.Value);
                    }
                }
            }
            // send packet
            UnityEngine.Debug.Log($"{__instance.getComponentType().getName()} produced {producedResources.Count} resources, consumed {consumedResources.Count} resources");
            Globals.LocalClient.OnProductionCompleted_Locally(__instance, ProducerType.Component, producedResources.ToArray(), consumedResources.ToArray());
            return false;
        }
        // This function is not yet implemented. The original can be located at Planetbase.ConstructionComponent.produceItem
        static ResourceConstructionData? produceItem(ConstructionComponent __instance, ProductionItem item)
        {
            if(item.getResourceType() != null)
            {
                return produceResource(__instance, item.getResourceType(), item.getSubtype(), __instance.shouldEmbedProduction());
            }
            // this case will spawn a bot. not yet implemented so we return null
            return null;
        }
        // This function is a modified copy of the original. The original can be located at Planetbase.ConstructionComponent.produceResource
        static ResourceConstructionData? produceResource(ConstructionComponent __instance, ResourceType resourceType, ResourceSubtype subtype, bool embedded)
        {
            if(embedded)
                return new ResourceConstructionData(resourceType.GetType().Name, subtype, (Vector3_Serializable)__instance.getPosition(), new Quaternion_Serializable(), Location.Interior, true);

            List<Vector2> resourcePositions = __instance.mParentConstruction.getResourcePositions();
            if(resourcePositions != null)
            {
                List<Vector3> list = new List<Vector3>();
                Vector3 position = __instance.getPosition();
                Transform transform = __instance.mParentConstruction.getTransform();
                float num = __instance.mParentConstruction.getRadius() * 0.75f;
                foreach (Vector2 vector in resourcePositions)
                {
                    Vector3 vector2 = transform.position + transform.right * vector.x + transform.forward * vector.y;
                    if ((position - vector2).magnitude < num)
                    {
                        list.Add(vector2);
                    }
                }
                __instance.mNextProductionPosition = (__instance.mNextProductionPosition + 1) % list.Count;
                Vector3 position2 = list[__instance.mNextProductionPosition];
                Vector3 vector3;
                if (PhysicsUtil.findFloor(position2, out vector3, 5120) && vector3.y - __instance.getPosition().y < 2f)
                {
                    return new ResourceConstructionData(resourceType.GetType().Name, subtype, (Vector3_Serializable)position2,
                        (Quaternion_Serializable)__instance.mObject.transform.rotation, Location.Interior, embedded);
                }
            }
            else
            {
                Vector3 position = __instance.getPosition() + (__instance as Selectable).getTransform().forward * 2.25f;
                Quaternion rotation = (__instance as Selectable).getTransform().rotation;
                return new ResourceConstructionData(resourceType.GetType().Name, subtype, (Vector3_Serializable)position, (Quaternion_Serializable)rotation, Location.Interior, embedded);
            }
            return null;
        }
        // This function returns the resources that would be consumed without actually consuming them
        static Resource[] getConsumedResources(ConstructionComponent __instance, Recipe recipe)
        {
            ResourceSubtype[] ingredients = recipe.getIngredients();
            List<ResourceSubtype> usedSubtypes = new List<ResourceSubtype>();

            List<Resource> consumedResources = new List<Resource>();
            for(int i = 0; i < ingredients.Length; i++)
            {
                ResourceSubtype subtype = ingredients[i];
                Resource consumedResource = null;
                foreach (Resource resource in __instance.mResourceContainer.mResources)
                {
                    ResourceSubtype subtype2 = resource.getSubtype();
                    if (subtype == ResourceSubtype.AnyVegetable && resource.getResourceType() == ResourceTypeList.VegetablesInstance && !usedSubtypes.Contains(subtype2))
                    {
                        usedSubtypes.Add(subtype2);
                        consumedResource = resource;
                        break;
                    }
                    if (subtype == ResourceSubtype.AnyMeat && resource.getResourceType() == ResourceTypeList.VitromeatInstance && !usedSubtypes.Contains(subtype2))
                    {
                        usedSubtypes.Add(subtype2);
                        consumedResource = resource;
                        break;
                    }
                    if (resource.getSubtype() == subtype || subtype == ResourceSubtype.None)
                    {
                        consumedResource = resource;
                        break;
                    }
                }
                if(consumedResource != null)
                {
                    // add resource to the list
                    consumedResources.Add(consumedResource);
                }
            }

            return consumedResources.ToArray();
        }
        // This function returns the resources that would be consumed without actually consuming them
        static Resource[] getConsumedResources(ConstructionComponent __instance, List<ResourceType> resourceConsumption)
        {
            List<Resource> consumedResources = new List<Resource>();
            if(resourceConsumption != null)
            {
                int highestCountResourceIndex = Resource.getHighestCountResourceIndex(resourceConsumption);
                for (int i = 0; i < resourceConsumption.Count; i++)
                {
                    ResourceType resourceType = resourceConsumption[(i + highestCountResourceIndex) % resourceConsumption.Count];
                    if (__instance.mResourceContainer.contains(resourceType))
                    {
                        Resource consumedResource = null;
                        foreach(Resource resource in __instance.mResourceContainer.mResources)
                        {
                            if(resource.getResourceType() == resourceType)
                            {
                                consumedResource = resource;
                                break;
                            }
                        }
                        if (consumedResource != null)
                        {
                            // add resource to the list
                            consumedResources.Add(consumedResource);
                        }
                        if (__instance.mComponentType.hasFlag(262144))
                        {
                            break;
                        }
                    }
                }
            }
            return consumedResources.ToArray();
        }
    }
}
