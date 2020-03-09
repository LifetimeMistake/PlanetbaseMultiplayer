using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Client
{
    public class MultiplayerResourceManager
    {
        private Dictionary<Guid, Resource> resources;
        private Client gameClient;

        public MultiplayerResourceManager(Client gameClient)
        {
            this.gameClient = gameClient;
            resources = new Dictionary<Guid, Resource>();
        }

        public Resource Find(Guid resourceId)
        {
            return resources.FirstOrDefault(i => i.Key == resourceId).Value;
        }
        public Guid Find(Resource resource)
        {
            return resources.FirstOrDefault(i => i.Value == resource).Key;
        }
        public bool Contains(Guid resourceId)
        {
            return (resources.Where(i => i.Key == resourceId).Count() != 0);
        }
        public bool Contains(Resource resource)
        {
            return (resources.Where(i => i.Value == resource).Count() != 0);
        }
        public Dictionary<Guid, Resource> GetResources()
        {
            return resources;
        }

        public void AppendMultiplayerId(Resource resource, XmlNode parent)
        {
            if (!Contains(resource)) return;
            Serialization.serializeString(parent, "multiplayer-id", Find(resource).ToString());
        }

        public void AddResource(Resource resource)
        {
            Guid guid = Guid.NewGuid();
            AddResource_ForReal(guid, resource, gameClient.localPlayer.IsSimulationOwner);
        }

        public void RemoveResource(Resource resource)
        {
            if (!Contains(resource))
            {
                DestroyResource(resource);
                Console.WriteLine($"Resource destroyed locally: {resource}");
                return;
            }

            RemoveResource_ForReal(Find(resource), resource, gameClient.localPlayer.IsSimulationOwner);
        }

        public void StoreResource(Character character, Module module)
        {
            if (character.getLoadedResource() == null) return;
            if (!Contains(character.getLoadedResource()))
            {
                character.MP_storeResource(module);
                Console.WriteLine($"Resource stored locally: {character.getLoadedResource()}");
                return;
            }

            StoreResource_ForReal(Find(character.getLoadedResource()), character.getLoadedResource(), character, module, gameClient.localPlayer.IsSimulationOwner);
        }

        public void EmbedResource(Character character, ConstructionComponent component, Resource.State state)
        {
            if (character.getLoadedResource() == null) return;
            if (!Contains(character.getLoadedResource()))
            {
                character.MP_embedResource(component, state);
                Console.WriteLine($"Resource embedded locally: {character.getLoadedResource()}");
                return;
            }

            EmbedResource_ForReal(Find(character.getLoadedResource()), character.getLoadedResource(), character, component, state, gameClient.localPlayer.IsSimulationOwner);
        }

        public void ExtractResource(Resource resource)
        {
            if (resource == null) return;
            if(!Contains(resource))
            {
                resource.getGameObject().name = resource.getResourceType().getName();
                resource.getGameObject().SetActive(true);
                resource.mContainer.remove(resource);
                resource.mContainer = null;
                Console.WriteLine($"Resource extracted locally: {resource}");
            }

            ExtractResource_ForReal(Find(resource), resource, gameClient.localPlayer.IsSimulationOwner);
        }

        public void LoadResource(Resource resource, Character character)
        {
            if (!Contains(resource))
            {
                character.MP_loadResource(resource);
                Console.WriteLine($"Resource loaded locally: {resource}");
                return;
            }

            LoadResource_ForReal(Find(resource), resource, character, gameClient.localPlayer.IsSimulationOwner);
        }

        public void UnloadResource(Character character, Resource.State state)
        {
            if (character.getLoadedResource() == null) return;
            if (!Contains(character.getLoadedResource()))
            {
                character.MP_unloadResource(state);
                Console.WriteLine($"Resource unloaded locally: {character.getLoadedResource()}");
                return;
            }

            UnloadResource_ForReal(Find(character.getLoadedResource()), character.getLoadedResource(), character, state, gameClient.localPlayer.IsSimulationOwner);
        }

        public void AddConstructionMaterial(Buildable buildable, Resource resource)
        {
            if (!Contains(resource))
            {
                if (buildable.mPendingConstructionCosts.containsResourceType(resource.getResourceType()))
                {
                    if (buildable.mConstructionMaterials == null)
                    {
                        buildable.mConstructionMaterials = new ResourceArray();
                    }
                    buildable.mConstructionMaterials.add(resource);
                    buildable.mPendingConstructionCosts.remove(resource.getResourceType(), 1);
                }
                Console.WriteLine($"Construction material added locally: {resource}");
                return;
            }

            AddConstructionMaterial_ForReal(Find(resource), resource, buildable, gameClient.localPlayer.IsSimulationOwner);
        }

        private string ResourceToXml(Resource resource)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode rootNode = XmlUtils.createNode(xmlDocument, xmlDocument, "root", null);
            XmlNode parentNode = XmlUtils.createNode(xmlDocument, rootNode, "resource", resource.mResourceType.GetType().Name);
            XmlUtils.serializeInt(xmlDocument, parentNode, "id", resource.mId);
            XmlUtils.serializeInt(xmlDocument, parentNode, "trader-id", resource.mTraderId);
            XmlUtils.serializeVector3(xmlDocument, parentNode, "position", resource.getPosition());
            XmlUtils.serializeQuaternion(xmlDocument, parentNode, "orientation", resource.mObject.transform.localRotation);
            XmlUtils.serializeInt(xmlDocument, parentNode, "state", (int)resource.mState);
            XmlUtils.serializeInt(xmlDocument, parentNode, "location", (int)resource.mLocation);
            XmlUtils.serializeInt(xmlDocument, parentNode, "subtype", (int)resource.mSubtype);
            XmlUtils.serializeFloat(xmlDocument, parentNode, "condition", resource.mConditionIndicator.getValue());
            XmlUtils.serializeFloat(xmlDocument, parentNode, "durability", (float)resource.mDurability);
            if (Contains(resource))
                XmlUtils.serializeString(xmlDocument, parentNode, "multiplayer-id", Find(resource).ToString());
            string xmlData = "";
            using (StringWriter writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
                { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment }))
                {
                    xmlDocument.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                    xmlData = writer.GetStringBuilder().ToString();
                }
            }
            return xmlData;
        }

        private Resource ResourceFromXml(AddResourceDataPackage pkg)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pkg.XmlData);
            Resource resource = Resource.create(doc["root"].FirstChild);
            return resource;
        }

        private void AddResource_ForReal(Guid key, Resource value, bool syncWithOthers)
        {
#if DEBUG
            gameClient.debug_eventList.Add($"AddResource_ForReal: {value.GetType().Name}");
#endif
            if (Contains(key)) { UnityEngine.Debug.LogError($"Multiplayer resource {key} already exists."); return; }
            resources.Add(key, value);
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.AddResource, new AddResourceDataPackage(key, value.mResourceType.GetType().Name, ResourceToXml(value))));
            }
        }

        private void RemoveResource_ForReal(Guid key, Resource value, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"RemoveResource_ForReal: {value.mResourceType.GetType().Name}");
#endif
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.Destroy, null)));
            }

            DestroyResource(value);
            resources.Remove(key);
        }

        private void LoadResource_ForReal(Guid key, Resource value, Character character, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"LoadResource_ForReal: {value.mResourceType.GetType().Name}");
#endif
            if(syncWithOthers)
            {
                ResourceData data = new ResourceData();
                data.CharacterId = character.getId();
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.Load, data)));
            }

            character.MP_loadResource(value);
        }

        private void UnloadResource_ForReal(Guid key, Resource value, Character character, Resource.State state, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{Find(value)}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"UnloadResource_ForReal: {value.mResourceType.GetType().Name}");
#endif
            if(syncWithOthers)
            {
                ResourceData data = new ResourceData();
                data.CharacterId = character.getId();
                data.State = state;
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(Find(value), ResourceAction.Unload, data)));
            }

            character.MP_unloadResource(state);
        }

        private void AddConstructionMaterial_ForReal(Guid key, Resource value, Buildable buildable, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"AddConstructionmaterial_ForReal: {value.mResourceType.GetType()}");
#endif
            if(syncWithOthers)
            {
                ResourceData data = new ResourceData();
                data.SelectableId = buildable.getId();
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.AddConstructionMaterial, data)));
            }

            if (buildable.mPendingConstructionCosts.containsResourceType(value.getResourceType()))
            {
                if (buildable.mConstructionMaterials == null)
                {
                    buildable.mConstructionMaterials = new ResourceArray();
                }
                buildable.mConstructionMaterials.add(value);
                buildable.mPendingConstructionCosts.remove(value.getResourceType(), 1);
            }
        }

        private void StoreResource_ForReal(Guid key, Resource value, Character character, Module module, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"StoreResource_ForReal: {value.mResourceType.GetType()}");
#endif
            if(syncWithOthers)
            {
                ResourceData data = new ResourceData();
                data.SelectableId = module.getId();
                data.CharacterId = character.getId();
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.Store, data)));
            }

            character.MP_storeResource(module);
        }

        private void ExtractResource_ForReal(Guid key, Resource value, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"ExtractResource_ForReal: {value.mResourceType.GetType()}");
#endif
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.Extract, null)));
            }

            value.getGameObject().name = value.getResourceType().getName();
            value.getGameObject().SetActive(true);
            value.mContainer.remove(value);
            value.mContainer = null;
        }

        private void EmbedResource_ForReal(Guid key, Resource value, Character character, ConstructionComponent component, Resource.State state, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"EmbedResource_ForReal: {value.mResourceType.GetType()}");
#endif
            if(syncWithOthers)
            {
                ResourceData data = new ResourceData();
                data.CharacterId = character.getId();
                data.SelectableId = component.getId();
                data.State = state;
                gameClient.SendPacket(new Packet(PacketType.UpdateResource, new UpdateResourceDataPackage(key, ResourceAction.Embed, data)));
            }

            character.MP_embedResource(component, state);
        }

        private void DestroyResource(Resource resource)
        {
            Resource.mResourceDictionary.Remove(resource.mObject);
            Resource.mResources.Remove(resource);
            Resource.mTypeResources[resource.mResourceType].Remove(resource);
            resource.end();
        }

        public void ProcessPacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.AddResource:
                    AddResourceDataPackage addResourceDataPackage = packet.Data as AddResourceDataPackage;
                    Resource resource = ResourceFromXml(addResourceDataPackage);
                    Console.WriteLine($"Add resource from packet: {resource.mResourceType.GetType().Name}");
                    AddResource_ForReal(addResourceDataPackage.ResourceId, resource, false);
                    break;
                case PacketType.UpdateResource:
                    UpdateResourceDataPackage updateResourceDataPackage = packet.Data as UpdateResourceDataPackage;
                    switch (updateResourceDataPackage.Action)
                    {
                        case ResourceAction.Destroy:
                            Console.WriteLine($"Remove resource from packet: {updateResourceDataPackage.ResourceId}");
                            RemoveResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), false);
                            break;
                        case ResourceAction.Load:
                            Console.WriteLine($"Load resource from packet: {updateResourceDataPackage.ResourceId}");
                            LoadResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), Character.find(updateResourceDataPackage.Data.CharacterId), false);
                            break;
                        case ResourceAction.Unload:
                            Console.WriteLine($"Unload resource from packet: {updateResourceDataPackage.ResourceId}");
                            UnloadResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), Character.find(updateResourceDataPackage.Data.CharacterId), updateResourceDataPackage.Data.State, false);
                            break;
                        case ResourceAction.AddConstructionMaterial:
                            Console.WriteLine($"Add construction material from packet: {updateResourceDataPackage.ResourceId}");
                            AddConstructionMaterial_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId),
                                (Buildable)MultiplayerUtil.FindSelectableFromId(updateResourceDataPackage.Data.SelectableId), false);
                            break;
                        case ResourceAction.Store:
                            Console.WriteLine($"Store resource from packet: {updateResourceDataPackage.ResourceId}");
                            StoreResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), 
                                Character.find(updateResourceDataPackage.Data.CharacterId), (Module)Construction.find(updateResourceDataPackage.Data.SelectableId), false);
                            break;
                        case ResourceAction.Embed:
                            Console.WriteLine($"Embed resource from packet: {updateResourceDataPackage.ResourceId}");
                            EmbedResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), Character.find(updateResourceDataPackage.Data.CharacterId),
                                ConstructionComponent.find(updateResourceDataPackage.Data.SelectableId), updateResourceDataPackage.Data.State, false);
                            break;
                        case ResourceAction.Extract:
                            Console.WriteLine($"Extract resource from packet: {updateResourceDataPackage.ResourceId}");
                            ExtractResource_ForReal(updateResourceDataPackage.ResourceId, Find(updateResourceDataPackage.ResourceId), false);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine($"MultiplayerResourceManager dropped packet: {packet.Type}");
                    break;
            }
        }
        public void DeserializeAll(XmlNode node)
        {
            foreach (object obj in node.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                if (xmlNode.Name == "resource")
                {
                    Resource resource = Resource.create(xmlNode);
                    if (xmlNode["multiplayer-id"] != null)
                    {
                        // Add resource to the list
                        AddResource_ForReal(new Guid(xmlNode["multiplayer-id"].Attributes[0].Value), resource, false);
                    }
                    else
                    {
                        if (!gameClient.localPlayer.IsSimulationOwner) continue;
                        // Assign a new multiplayer Id to the resource
                        UnityEngine.Debug.LogWarning("Resource multiplayer Id was null. Creating a new one");
                        Guid guid = Guid.NewGuid();
                        AddResource_ForReal(guid, resource, false);
                    }
                }
            }
            Resource.mInmaterialResources.deserialize(node["inmaterial-resources"]);
            Resource.mTotalAmounts.add(Resource.mInmaterialResources);
        }
    }
}
