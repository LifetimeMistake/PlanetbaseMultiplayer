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
            ModuleType mType = TypeList<ModuleType, ModuleTypeList>.find(pkg.ModuleType);
            if (mType == null) { UnityEngine.Debug.LogError($"Could not find the requested module type: {pkg.ModuleType}"); return; }
            Module module = Module.create((Vector3)pkg.Position, pkg.SizeIndex, mType);
            module.setPositionY(Singleton<TerrainGenerator>.getInstance().getFloorHeight());
            module.setRenderTop((GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
            module.onUserPlaced();
        }
        public static void PlaceConnection(PlaceConnectionDataPackage pkg)
        {
            Module m1 = (Module)Construction.find(pkg.Module1_Id);
            Module m2 = (Module)Construction.find(pkg.Module2_Id);

            if (m1 == null) { UnityEngine.Debug.LogError("Could not find m1"); return; }
            if (m2 == null) { UnityEngine.Debug.LogError("Could not find m2"); return; }
            Module.linkModules(m1, m2, (GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
        }
        public static void PlaceComponent(PlaceComponentDataPackage pkg)
        {
            Construction parentModule = Construction.find(pkg.ParentModuleId);
            ComponentType componentType = TypeList<ComponentType, ComponentTypeList>.find(pkg.ComponentType);

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
                producer = ConstructionComponent.find(pkg.ProducerId);
            else if (pkg.ProducerType == ProducerType.Module)
                producer = Construction.find(pkg.ProducerId);
            if (producer == null) { UnityEngine.Debug.LogError("producer was null"); return; }

            foreach (ResourceDestructionData data in pkg.ConsumedResources)
            {
                Resource resource = Resource.find(data.ResourceId);
                if (resource == null) { UnityEngine.Debug.LogWarning("consumedresources: resource was null"); continue; }
                resource.destroy();
            }
            foreach (ResourceConstructionData data in pkg.ProducedResources)
            {
                ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find(data.Type);
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

        public static void RecycleColonyShip(RecycleColonyShipDataPackage pkg)
        {
            ColonyShip colonyShip = (ColonyShip)Ship.find(pkg.ColonyShipId);
            foreach(ResourceUpdateData data in pkg.ExtractedResources)
            {
                Resource resource = Resource.find(data.ResourceId);
                switch(data.UpdateAction)
                {
                    case ResourceAction.Extract:
                        resource.onExtract();
                        resource.setPosition((Vector3)data.Position);
                        resource.drop(Resource.State.Idle);
                        break;
                    default:
                        break;
                }
            }
            foreach(ResourceConstructionData data in pkg.CreatedResources)
            {
                ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find(data.Type);
                if (resourceType == null) { UnityEngine.Debug.LogError($"Could not find the requested resource type: {data.Type}"); continue; }
                Resource resource = Resource.create(resourceType, data.Subtype, (Vector3)data.Position, data.Location);
                if (!data.Rotation.IsEmpty)
                    resource.setRotation((Quaternion)data.Rotation);
                resource.drop(Resource.State.Idle);
            }
            if (colonyShip != null)
                colonyShip.destroy();
        }
    }
}
