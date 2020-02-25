using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static void RecycleComponent(RecycleComponentDataPackage pkg)
        {
            ConstructionComponent component = ConstructionComponent.find(pkg.ComponentId);
            foreach(ResourceConstructionData data in pkg.CreatedResources)
            {
                ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find(data.Type);
                if (resourceType == null) { UnityEngine.Debug.LogError($"Could not find the requested resource type: {data.Type}"); continue; }
                Resource resource = Resource.create(resourceType, data.Subtype, (Vector3)data.Position, data.Location);
                if (!data.Rotation.IsEmpty)
                    resource.setRotation((Quaternion)data.Rotation);
                resource.drop(Resource.State.Idle);
            }
            foreach(ResourceDestructionData data in pkg.DestroyedResources)
            {
                Resource resource = Resource.find(data.ResourceId);
                if (resource == null) { UnityEngine.Debug.LogWarning("recyclecomponent: resource was null"); continue; }
                resource.destroy();
            }
            Console.WriteLine($"{pkg.CreatedResources.Length}   {pkg.DestroyedResources.Length}");
            if (component != null)
                component.destroy();
        }

        public static void RecycleSelectable(RecycleSelectableDataPackage pkg)
        {
            Selectable selectable = Resource.find(pkg.SelectableId);
            if (selectable == null) selectable = Construction.find(pkg.SelectableId);
            if (selectable == null) { UnityEngine.Debug.LogError($"Could not find selectable object with Id: {pkg.SelectableId}"); return; }
            foreach(ResourceConstructionData data in pkg.CreatedResources)
            {
                ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find(data.Type);
                if (resourceType == null) { UnityEngine.Debug.LogError($"Could not find the requested resource type: {data.Type}"); continue; }
                Resource resource = Resource.create(resourceType, data.Subtype, (Vector3)data.Position, data.Location);
                if (!data.Rotation.IsEmpty)
                    resource.setRotation((Quaternion)data.Rotation);
                resource.drop(Resource.State.Idle);
            }
            if (selectable != null)
                selectable.destroy();
        }

        public static void CharacterStartWalking(CharacterStartWalkingDataPackage pkg)
        {
            Character character = Character.find(pkg.CharacterId);
            if(character == null) { UnityEngine.Debug.LogError("CharacterStartWalking: character was null"); return; }
            Target target = new Target((Vector3)pkg.TargetPosition, pkg.TargetLocation);
            Selectable selectable = MultiplayerUtil.FindSelectableFromId(pkg.TargetSelectableId);
            if (selectable != null) target.mSelectable = selectable;
            target.mRadius = pkg.TargetRadius;
            if(!pkg.TargetRotation.IsEmpty)
                target.mRotation = (Quaternion)pkg.TargetRotation;
            target.mFlags = pkg.TargetFlags;

            List<Selectable> indirectTargets_list = new List<Selectable>();
            foreach(int indirectTarget_id in pkg.IndirectTargetIds)
            {
                Selectable s = MultiplayerUtil.FindSelectableFromId(indirectTarget_id);
                if (s != null) indirectTargets_list.Add(s);
                else UnityEngine.Debug.LogWarning("CharacterStartWalking: s was null");
            }

            character.MP_startWalking(target, indirectTargets_list.Count == 0 ? null : indirectTargets_list.ToArray());
        }
    }
}
