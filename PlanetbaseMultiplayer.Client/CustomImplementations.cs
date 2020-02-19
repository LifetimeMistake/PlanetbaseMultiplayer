using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client
{
    public static class MultiplayerMethods
    {
        public static void PlaceModule(PlaceModuleDataPackage pkg)
        {
            ModuleType mType = null;
            foreach (ModuleType moduleType in TypeList<ModuleType, ModuleTypeList>.get())
                if (moduleType.getName() == pkg.ModuleType)
                {
                    mType = moduleType;
                    break;
                }
            if (mType == null) { UnityEngine.Debug.LogError($"Could not find the requested module type: {pkg.ModuleType}"); return; }
            Module module = Module.create((Vector3)pkg.Position, pkg.SizeIndex, mType);
            module.setPositionY(Singleton<TerrainGenerator>.getInstance().getFloorHeight());
            module.setRenderTop((GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
            module.onUserPlaced();
        }
        public static void PlaceConnection(PlaceConnectionDataPackage pkg)
        {
            Module m1 = null;
            Module m2 = null;
            foreach (Construction construction in Construction.mConstructions)
            {
                if (construction.mId == pkg.Module1_Id)
                    m1 = (Module)construction;
                if (construction.mId == pkg.Module2_Id)
                    m2 = (Module)construction;
            }

            if (m1 == null) { UnityEngine.Debug.LogError("Could not find m1"); return; }
            if (m2 == null) { UnityEngine.Debug.LogError("Could not find m2"); return; }
            Module.linkModules(m1, m2, (GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
        }
        public static void PlaceComponent(PlaceComponentDataPackage pkg)
        {
            Construction parentModule = null;
            ComponentType componentType = null;
            foreach (Construction construction in Construction.mConstructions)
            {
                if (construction.mId == pkg.ParentModuleId)
                {
                    parentModule = construction;
                    break;
                }
            }
            foreach (ComponentType cType in TypeList<ComponentType, ComponentTypeList>.get())
                if (cType.getName() == pkg.ComponentType)
                {
                    componentType = cType;
                    break;
                }

            if (parentModule == null) { UnityEngine.Debug.LogError("parentModule was null"); return; }
            if (componentType == null)
            { UnityEngine.Debug.LogError($"Could not find the requested component type: {pkg.ComponentType}"); return; }

            ConstructionComponent component = ConstructionComponent.create(construction: parentModule, componentType: componentType);
            component.setRotation(((Quaternion)pkg.Rotation));
            component.setPosition((Vector3)pkg.Position);
            component.setPositionY(component.getParentConstruction().getFloorPosition().y);
            parentModule.addComponent(component);
            component.onUserPlaced();
        }
        public static void CompleteProduction(ProduceResourceDataPackage pkg)
        {
            Buildable producer = null;
            if (pkg.ProducerType == ProducerType.Component)
                foreach (ConstructionComponent component in ConstructionComponent.mComponents)
                {
                    if (component.mId == pkg.ProducerId)
                    {
                        producer = component;
                        break;
                    }
                }
            else if(pkg.ProducerType == ProducerType.Module)
                foreach (Construction module in Construction.mConstructions)
                {
                    if(module.mId == pkg.ProducerId)
                    {
                        producer = module;
                        break;
                    }
                }
            if (producer == null) { UnityEngine.Debug.LogError("producer was null"); return; }

            foreach (ResourceDestructionData data in pkg.ConsumedResources)
            {
                Resource resource = null;
                foreach (Resource rsc in Resource.mResources)
                {
                    if (rsc.getId() == data.ResourceId)
                    {
                        resource = rsc;
                        break;
                    }
                }

                if (resource == null) { UnityEngine.Debug.LogWarning("consumedresources: resource was null"); continue; }
                resource.destroy();
            }
            foreach (ResourceConstructionData data in pkg.ProducedResources)
            {
                ResourceType resourceType = null;
                foreach (ResourceType type in TypeList<ResourceType, ResourceTypeList>.get())
                {
                    if (type.getName() == data.Type)
                    {
                        resourceType = type;
                        break;
                    }
                }
                if (resourceType == null) { UnityEngine.Debug.LogError($"Could not find the requested resource type: {data.Type}"); continue; }
                Resource resource = Resource.create(resourceType, data.Subtype, (Vector3)data.Position, data.Location);
                if (data.Embedded)
                {
                    // Embed the resource inside the producer's inventory.
                    if(pkg.ProducerType == ProducerType.Component)
                        (producer as ConstructionComponent).embedResource(resource);
                }
                else
                {
                    if (!data.Rotation.IsEmpty)
                        resource.setRotation((Quaternion)data.Rotation);
                    resource.drop(Resource.State.Idle);
                }
            }
            if (pkg.ProducerType == ProducerType.Component)
                (producer as ConstructionComponent).mProductionProgress.setValue(0f);
            else if (pkg.ProducerType == ProducerType.Module)
                (producer as Module).mProductionProgressIndicator.setValue(0f);
        }
    }
}
