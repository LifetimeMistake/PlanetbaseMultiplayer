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
    public class MultiplayerConstructionManager
    {
        private Dictionary<Guid, Buildable> buildables;
        private Client gameClient;

        public MultiplayerConstructionManager(Client gameClient)
        {
            this.gameClient = gameClient;
            buildables = new Dictionary<Guid, Buildable>();
        }

        public Buildable Find(Guid buildableId)
        {
            return buildables.FirstOrDefault(i => i.Key == buildableId).Value;
        }
        public Guid Find(Buildable buildable)
        {
            return buildables.FirstOrDefault(i => i.Value == buildable).Key;
        }
        public bool Contains(Guid buildableId)
        {
            return (buildables.Where(i => i.Key == buildableId).Count() != 0);
        }
        public bool Contains(Buildable buildable)
        {
            return (buildables.Where(i => i.Value == buildable).Count() != 0);
        }
        public Dictionary<Guid, Buildable> GetConstructions()
        {
            return buildables;
        }

        public void AppendMultiplayerId(Buildable buildable, XmlNode parent)
        {
            if (!Contains(buildable)) return;
            Serialization.serializeString(parent, "multiplayer-id", Find(buildable).ToString());
        }

        private string BuildableToXml(Buildable buildable)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode rootNode = XmlUtils.createNode(xmlDocument, xmlDocument, "root", null);
            if (buildable is Module)
                SerializeModule(xmlDocument, rootNode, "construction", (Module)buildable, false);
            else if (buildable is Connection)
                SerializeConnection(xmlDocument, rootNode, "construction", (Connection)buildable, false);
            else if (buildable is ConstructionComponent)
            {
                SerializeConstructionComponent(xmlDocument, rootNode, "component", (ConstructionComponent)buildable);
                XmlUtils.serializeString(xmlDocument, rootNode.FirstChild, "parent-multiplayer-id", Find((buildable as ConstructionComponent).mParentConstruction).ToString());
            }
            else if (buildable is Construction)
                SerializeConstruction(xmlDocument, rootNode, "construction", (Construction)buildable, false);
            else if (buildable is Buildable)
                SerializeBuildable(xmlDocument, rootNode, "buildable", buildable);
            if (Contains(buildable))
                XmlUtils.serializeString(xmlDocument, rootNode.LastChild, "multiplayer-id", Find(buildable).ToString());
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

        private Buildable BuildableFromXml(AddBuildableDataPackage pkg)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pkg.XmlData);
            XmlNode data = doc["root"].FirstChild;
            Buildable buildable;
            switch (data.Name)
            {
                case "construction":
                    string a = Serialization.deserializeType(data);
                    if (a == typeof(Module).Name)
                    {
                        buildable = Module.create(data);
                    }
                    else if (a == typeof(Connection).Name)
                    {
                        List<Construction> list = Construction.deserializeLinks(data);
                        if (!(list[0] is Module))
                        {
                            UnityEngine.Debug.LogError("Link is not module: " + list[0].getId());
                        }
                        if (!(list[1] is Module))
                        {
                            UnityEngine.Debug.LogError("Link is not module: " + list[1].getId());
                        }
                        buildable = Connection.create((Module)list[0], (Module)list[1], data);
                    }
                    else
                        buildable = null;
                    break;
                case "component":
                    if (data["parent-multiplayer-id"] == null)
                    {
                        buildable = null;
                        break;
                    }
                    Construction parent = Find(new Guid(data["parent-multiplayer-id"].Attributes[0].Value)) as Construction;
                    buildable = ConstructionComponent.create(parent, data);
                    break;
                default:
                    buildable = null;
                    break;
            }
            return buildable;
        }

        public void AddBuildable(Buildable buildable)
        {
            Guid guid = Guid.NewGuid();
            AddBuildable_ForReal(guid, buildable, true);
        }

        public void RemoveBuildable(Buildable buildable)
        {
            if (!Contains(buildable))
            {
                DestroyBuildable(buildable);
                Console.WriteLine($"Buildable destroyed locally: {buildable}");
                return;
            }

            RemoveBuildable_ForReal(Find(buildable), buildable, true);
        }

        public void SetEnabled(Buildable buildable, bool enabled)
        {
            if (!Contains(buildable))
            {
                buildable.setEnabled(enabled);
                Console.WriteLine($"Enabled set locally: {buildable}");
                return;
            }

            SetEnabled_ForReal(Find(buildable), buildable, enabled, true);
        }

        public void SetPriority(Construction construction, bool highPriority)
        {
            if (!Contains(construction))
            {
                construction.setHighPriority(highPriority);
                Console.WriteLine($"Priority set locally: {construction}");
                return;
            }

            SetPriority_ForReal(Find(construction), construction, highPriority, true);
        }

        private void DestroyBuildable(Buildable buildable)
        {

        }

        private void SetPriority_ForReal(Guid key, Construction value, bool highPriority, bool syncWithOthers)
        {
#if DEBUG
            gameClient.debug_eventList.Add($"SetPriority_ForReal: {value.GetType().Name}");
#endif
            if (syncWithOthers)
            {
                BuildableData data = new BuildableData();
                data.HighPriority = highPriority;
                gameClient.SendPacket(new Packet(PacketType.UpdateBuildable, new UpdateBuildableDataPackage(key, BuildableAction.SetPriority, data)));
            }

            value.setHighPriority(highPriority);
        }

        private void SetEnabled_ForReal(Guid key, Buildable value, bool enabled, bool syncWithOthers)
        {
#if DEBUG
            gameClient.debug_eventList.Add($"SetEnabled_ForReal: {value.GetType().Name}");
#endif
            if (syncWithOthers)
            {
                BuildableData data = new BuildableData();
                data.Enabled = enabled;
                gameClient.SendPacket(new Packet(PacketType.UpdateBuildable, new UpdateBuildableDataPackage(key, BuildableAction.SetEnabled, data)));
            }

            value.setEnabled(enabled);
        }

        private void AddBuildable_ForReal(Guid key, Buildable value, bool syncWithOthers)
        {
#if DEBUG
            gameClient.debug_eventList.Add($"AddBuildable_ForReal: {value.GetType().Name}");
#endif
            if (Contains(key)) { UnityEngine.Debug.LogError($"Multiplayer buildable {key} already exists."); return; }
            buildables.Add(key, value);
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.AddBuildable, new AddBuildableDataPackage(key, BuildableToXml(value))));
            }
        }

        private void RemoveBuildable_ForReal(Guid key, Buildable value, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            gameClient.debug_eventList.Add($"RemoveBuildable_ForReal: {value.GetType().Name}");
#endif
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.UpdateBuildable, new UpdateBuildableDataPackage(key, BuildableAction.Destroy, null)));
            }
        }
        #region Utility methods for deserializing different types of buildables
        public Buildable DeserializeBuildable(XmlNode node, Buildable buildable)
        {
            // Buildable.deserialize
            buildable.mEnabled = Serialization.deserializeBool(node["enabled"]);
            buildable.mState = (BuildableState)Serialization.deserializeInt(node["state"]);
            buildable.mBuildProgress.setValue(Serialization.deserializeFloat(node["build-progress"]));
            if (node["construction-materials"] != null)
            {
                buildable.mConstructionMaterials = new ResourceArray();
                buildable.mConstructionMaterials.deserialize(node["construction-materials"]);
            }
            if (node["pending-construction-costs"] != null)
            {
                buildable.mPendingConstructionCosts = new ResourceAmounts(name: null);
                buildable.mPendingConstructionCosts.deserialize(node["pending-construction-costs"]);
            }
            return buildable;
        }
        public Construction DeserializeConstruction(XmlNode node, Construction construction)
        {
            // Construction.deserialize
            construction.setPosition(Serialization.deserializeVector3(node["position"]));
            construction.mObject.transform.localRotation = Serialization.deserializeQuaternion(node["orientation"]);
            construction.mId = Serialization.deserializeId(node["id"]);
            construction.mOxygenIndicator.setValue(Serialization.deserializeFloat(node["oxygen"]));
            construction.mConditionIndicator.setValue(Serialization.deserializeFloat(node["condition"]));
            construction.mEnabled = Serialization.deserializeBool(node["enabled"]);
            construction.mLocked = Serialization.deserializeBool(node["locked"]);
            construction.mHighPriority = Serialization.deserializeBool(node["high-priority"]);
            construction.mTimeBuilt = (double)Serialization.deserializeFloat(node["time-built"]);
            XmlNode xmlNode = node["components"];
            if (xmlNode != null)
            {
                foreach (object obj in xmlNode.ChildNodes)
                {
                    XmlNode node2 = (XmlNode)obj;
                    ConstructionComponent item = ConstructionComponent.create(construction, node2);
                    if (node2["multiplayer-id"] != null)
                        AddBuildable_ForReal(new Guid(node2["multiplayer-id"].Attributes[0].Value), item, false);
                    else
                    {
                        if (!gameClient.localPlayer.IsSimulationOwner) continue;
                        // Assign a new multiplayer Id to the component
                        UnityEngine.Debug.LogWarning("Component multiplayer Id was null. Creating a new one");
                        Guid guid = Guid.NewGuid();
                        AddBuildable_ForReal(guid, item, false);
                    }
                    construction.mComponents.Add(item);
                }
            }
            return construction;
        }
        public Module DeserializeModule(XmlNode node, Module module)
        {
            module.mSizeIndex = Serialization.deserializeInt(node["size-index"]);
            module.mModuleType = TypeList<ModuleType, ModuleTypeList>.find(Serialization.deserializeString(node["module-type"]));
            module.createIndicators();
            XmlNode xmlNode = node["mobile-rotation"];
            if (xmlNode != null)
            {
                module.mMobileRotation = Serialization.deserializeQuaternion(xmlNode);
            }
            XmlNode xmlNode2 = node["resource-storage"];
            if (xmlNode2 != null)
            {
                module.mResourceStorage = new ResourceStorage();
                module.mResourceStorage.deserialize(xmlNode2);
            }
            if (module.mProductionProgressIndicator != null)
            {
                module.mProductionProgressIndicator.setValue(Serialization.deserializeFloat(node["production-progress"]));
            }
            if (module.mPowerStorageIndicator != null)
            {
                module.mPowerStorageIndicator.setValue(Serialization.deserializeFloat(node["power-storage"]));
            }
            if (module.mLaserChargeIndicator != null)
            {
                module.mLaserChargeIndicator.setValue(Serialization.deserializeFloat(node["laser-charge"]));
            }
            if (module.mWaterStorageIndicator != null)
            {
                module.mWaterStorageIndicator.setValue(Serialization.deserializeFloat(node["water-storage"]));
            }
            return module;
        }
        #endregion
        #region Utility methods for serializing different types of buildables
        public void SerializeBuildable(XmlDocument xmlDoc, XmlNode parent, string name, Buildable buildable)
        {
            XmlNode parent2 = XmlUtils.createNode(xmlDoc, parent, name, buildable.GetType().Name);
            XmlUtils.serializeBool(xmlDoc, parent2, "enabled", buildable.mEnabled);
            XmlUtils.serializeInt(xmlDoc, parent2, "state", (int)buildable.mState);
            XmlUtils.serializeFloat(xmlDoc, parent2, "build-progress", buildable.mBuildProgress.getValue());
            if (buildable.mConstructionMaterials != null)
            {
                SerializeResourceArray(xmlDoc, parent2, "construction-materials", buildable.mConstructionMaterials);
            }
            if (buildable.mPendingConstructionCosts != null)
            {
                SerializeResourceAmounts(xmlDoc, parent2, "pending-construction-costs", buildable.mPendingConstructionCosts);
            }
        }
        public void SerializeResourceArray(XmlDocument xmlDoc, XmlNode parent, string name, ResourceArray resourceArray)
        {
            XmlNode parent2 = XmlUtils.createNode(xmlDoc, parent, name, null);
            if (resourceArray.mResources != null)
            {
                foreach (Resource resource in resourceArray.mResources)
                {
                    Serialization.serializeInt(parent2, "id", resource.getId());
                }
            }
        }
        public void SerializeResourceAmounts(XmlDocument xmlDoc, XmlNode parent, string name, ResourceAmounts resourceAmounts)
        {
            XmlNode parent2 = XmlUtils.createNode(xmlDoc, parent, name, null);
            XmlUtils.serializeString(xmlDoc, parent2, "container-name", resourceAmounts.getName());
            foreach (ResourceAmount resourceAmount in resourceAmounts.mResourceAmounts)
            {
                SerializeResourceAmount(xmlDoc, parent2, "amount", resourceAmount);
            }
        }
        public void SerializeResourceAmount(XmlDocument xmlDoc, XmlNode parent, string name, ResourceAmount amount)
        {
            XmlNode parent2 = XmlUtils.createNode(xmlDoc, parent, name, null);
            XmlUtils.serializeString(xmlDoc, parent2, "resource-type", amount.getResourceType().GetType().Name);
            XmlUtils.serializeInt(xmlDoc, parent2, "amount", amount.getAmount());
        }
        public void SerializeConstruction(XmlDocument xmlDoc, XmlNode parent, string name, Construction construction, bool serializeComponents)
        {
            SerializeBuildable(xmlDoc, parent, name, construction);
            XmlNode lastChild = parent.LastChild;
            XmlUtils.serializeFloat(xmlDoc, lastChild, "condition", construction.mConditionIndicator.getValue());
            XmlUtils.serializeFloat(xmlDoc, lastChild, "oxygen", construction.mOxygenIndicator.getValue());
            XmlUtils.serializeInt(xmlDoc, lastChild, "id", construction.mId);
            XmlUtils.serializeVector3(xmlDoc, lastChild, "position", construction.mObject.transform.position);
            XmlUtils.serializeQuaternion(xmlDoc, lastChild, "orientation", construction.mObject.transform.localRotation);
            XmlUtils.serializeBool(xmlDoc, lastChild, "enabled", construction.mEnabled);
            XmlUtils.serializeDouble(xmlDoc, lastChild, "time-built", construction.mTimeBuilt);
            XmlUtils.serializeBool(xmlDoc, lastChild, "locked", construction.mLocked);
            XmlUtils.serializeBool(xmlDoc, lastChild, "high-priority", construction.mHighPriority);
            if (construction.mComponents != null && construction.mComponents.Count > 0 && serializeComponents)
            {
                XmlNode parent2 = XmlUtils.createNode(xmlDoc, lastChild, "components", null);
                int count = construction.mComponents.Count;
                for (int i = 0; i < count; i++)
                {
                    ConstructionComponent constructionComponent = construction.mComponents[i];
                    SerializeConstructionComponent(xmlDoc, parent2, "component", constructionComponent);
                }
            }
        }
        public void SerializeConstructionComponent(XmlDocument xmlDoc, XmlNode parent, string name, ConstructionComponent component)
        {
            SerializeBuildable(xmlDoc, parent, name, component);
            XmlNode lastChild = parent.LastChild;
            XmlUtils.serializeInt(xmlDoc, lastChild, "id", component.mId);
            XmlUtils.serializeString(xmlDoc, lastChild, "component-type", component.mComponentType.GetType().Name);
            XmlUtils.serializeVector3(xmlDoc, lastChild, "position", component.mObject.transform.position);
            XmlUtils.serializeQuaternion(xmlDoc, lastChild, "orientation", component.mObject.transform.localRotation);
            XmlUtils.serializeFloat(xmlDoc, lastChild, "condition", component.mConditionIndicator.getValue());
            XmlUtils.serializeFloat(xmlDoc, lastChild, "production-progress", component.mProductionProgress.getValue());
            XmlUtils.serializeFloat(xmlDoc, lastChild, "time", component.mTime);
            XmlUtils.serializeBool(xmlDoc, lastChild, "enabled", component.mEnabled);
            XmlUtils.serializeInt(xmlDoc, lastChild, "produced-item-index", component.mProducedItemIndex);
            if (component.mResourceContainer != null)
            {
                XmlNode parent2 = XmlUtils.createNode(xmlDoc, lastChild, "resource-container");
                XmlUtils.serializeInt(xmlDoc, parent2, "capacity", component.mResourceContainer.getCapacity());
            }
        }
        public void SerializeConnection(XmlDocument xmlDoc, XmlNode parent, string name, Connection connection, bool serializeComponents)
        {
            SerializeConstruction(xmlDoc, parent, name, connection, serializeComponents);
            XmlNode lastChild = parent.LastChild;
            XmlNode parent2 = XmlUtils.createNode(xmlDoc, lastChild, "links", null);
            foreach (Construction construction in connection.mLinks)
            {
                XmlUtils.serializeInt(xmlDoc, parent2, "id", construction.getId());
            }
        }
        public void SerializeModule(XmlDocument xmlDoc, XmlNode parent, string name, Module module, bool serializeComponents)
        {
            SerializeConstruction(xmlDoc, parent, name, module, serializeComponents);
            XmlNode lastChild = parent.LastChild;
            XmlUtils.serializeString(xmlDoc, lastChild, "module-type", module.mModuleType.GetType().Name);
            XmlUtils.serializeFloat(xmlDoc, lastChild, "size-index", (float)module.mSizeIndex);
            if (module.mObjectMobile != null)
            {
                XmlUtils.serializeQuaternion(xmlDoc, lastChild, "mobile-rotation", module.mObjectMobile.transform.localRotation);
            }
            if (module.mResourceStorage != null)
            {
                module.mResourceStorage.serialize(lastChild, "resource-storage");
            }
            if (module.mProductionProgressIndicator != null)
            {
                XmlUtils.serializeFloat(xmlDoc, lastChild, "production-progress", module.mProductionProgressIndicator.getValue());
            }
            if (module.mPowerStorageIndicator != null)
            {
                XmlUtils.serializeFloat(xmlDoc, lastChild, "power-storage", module.mPowerStorageIndicator.getValue());
            }
            if (module.mWaterStorageIndicator != null)
            {
                XmlUtils.serializeFloat(xmlDoc, lastChild, "water-storage", module.mWaterStorageIndicator.getValue());
            }
            if (module.mLaserChargeIndicator != null)
            {
                XmlUtils.serializeFloat(xmlDoc, lastChild, "laser-charge", module.mLaserChargeIndicator.getValue());
            }
        }
        #endregion
        public void ProcessPacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.AddBuildable:
                    AddBuildableDataPackage addBuildableDataPackage = packet.Data as AddBuildableDataPackage;
                    Buildable buildable = BuildableFromXml(addBuildableDataPackage);
                    Console.WriteLine($"Add resource from packet: {buildable.GetType().Name}");
                    AddBuildable_ForReal(addBuildableDataPackage.BuildableId, buildable, false);
                    break;
                case PacketType.UpdateBuildable:
                    UpdateBuildableDataPackage updateResourceDataPackage = packet.Data as UpdateBuildableDataPackage;
                    switch (updateResourceDataPackage.Action)
                    {
                        case BuildableAction.SetEnabled:
                            SetEnabled_ForReal(updateResourceDataPackage.BuildableId, Find(updateResourceDataPackage.BuildableId), updateResourceDataPackage.Data.Enabled, false);
                            break;
                        case BuildableAction.SetPriority:
                            SetPriority_ForReal(updateResourceDataPackage.BuildableId, (Construction)Find(updateResourceDataPackage.BuildableId), updateResourceDataPackage.Data.HighPriority, false);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine($"MultiplayerConstructionManager dropped packet: {packet.Type}");
                    break;
            }
        }
        public Module DeserializeModule(XmlNode node)
        {
            Module module = new Module();
            DeserializeBuildable(node, module);
            DeserializeConstruction(node, module);
            DeserializeModule(node, module);
            module.postInit();
            if (module.mState == BuildableState.Built)
            {
                module.onPlaced();
                module.onBuilt();
            }
            else
            {
                module.onPlaced();
            }
            return module;
        }

        public void DeserializeModules()
        {
            foreach (object obj in Construction.mXmlNode.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                if (xmlNode.Name == "construction")
                {
                    string a = Serialization.deserializeType(xmlNode);
                    if (a == typeof(Module).Name)
                    {
                        Module module = DeserializeModule(xmlNode);
                        if (xmlNode["multiplayer-id"] != null)
                            AddBuildable_ForReal(new Guid(xmlNode["multiplayer-id"].Attributes[0].Value), module, false);
                        else
                        {
                            if (!gameClient.localPlayer.IsSimulationOwner) continue;
                            // Assign a new multiplayer Id to the module
                            UnityEngine.Debug.LogWarning("Module multiplayer Id was null. Creating a new one");
                            Guid guid = Guid.NewGuid();
                            AddBuildable_ForReal(guid, module, false);
                        }
                    }
                }
            }
        }

        public void DeserializeConnections()
        {
            foreach (object obj in Construction.mXmlNode.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                if (xmlNode.Name == "construction")
                {
                    string a = Serialization.deserializeType(xmlNode);
                    if (a == typeof(Connection).Name)
                    {
                        List<Construction> list = Construction.deserializeLinks(xmlNode);
                        if (!(list[0] is Module))
                        {
                            UnityEngine.Debug.LogError("Link is not module: " + list[0].getId());
                        }
                        if (!(list[1] is Module))
                        {
                            UnityEngine.Debug.LogError("Link is not module: " + list[1].getId());
                        }
                        Connection connection = Connection.create((Module)list[0], (Module)list[1], xmlNode);
                        if (xmlNode["multiplayer-id"] != null)
                            AddBuildable_ForReal(new Guid(xmlNode["multiplayer-id"].Attributes[0].Value), connection, false);
                        else
                        {
                            if (!gameClient.localPlayer.IsSimulationOwner) continue;
                            // Assign a new multiplayer Id to the connection
                            UnityEngine.Debug.LogWarning("Connection multiplayer Id was null. Creating a new one");
                            Guid guid = Guid.NewGuid();
                            AddBuildable_ForReal(guid, connection, false);
                        }
                    }
                }
            }
        }
    }
}
