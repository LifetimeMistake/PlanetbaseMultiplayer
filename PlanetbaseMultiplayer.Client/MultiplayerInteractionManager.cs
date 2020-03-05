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
    public class MultiplayerInteractionManager
    {
        private Dictionary<Guid, Interaction> interactions;
        private Client gameClient;

        public MultiplayerInteractionManager(Client gameClient)
        {
            this.gameClient = gameClient;
            interactions = new Dictionary<Guid, Interaction>();
        }

        public void AddInteraction(Interaction interaction)
        {
            Guid guid = Guid.NewGuid();
            AddInteraction_ForReal(guid, interaction, gameClient.localPlayer.IsSimulationOwner);
        }
        public void RemoveInteraction(Interaction interaction)
        {
            if (!Contains(interaction))
            {
                DestroyInteraction(interaction);
                Console.WriteLine($"Interaction destroyed locally: {interaction}");
                return;
            }

            RemoveInteraction_ForReal(Find(interaction), interaction, gameClient.localPlayer.IsSimulationOwner);
        }
        private string InteractionToXml(Interaction interaction)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode rootNode = XmlUtils.createNode(xmlDocument, xmlDocument, "root", null);
            XmlNode parentNode = XmlUtils.createNode(xmlDocument, rootNode, "interaction", interaction.GetType().Name);
            XmlUtils.serializeSelectable(xmlDocument, parentNode, "character", interaction.mCharacter);
            if (interaction.mSelectable != null)
                XmlUtils.serializeSelectable(xmlDocument, parentNode, "selectable", interaction.mSelectable);
            if (interaction is InteractionComponent)
                XmlUtils.serializeInt(xmlDocument, parentNode.LastChild, "interaction-point", (interaction as InteractionComponent).mInteractionPoint);
            else if (interaction is InteractionAirlock)
            {
                InteractionAirlock interactionAirlock = interaction as InteractionAirlock;
                XmlUtils.serializeInt(xmlDocument, parentNode, "stage", (int)interactionAirlock.mStage);
                XmlUtils.serializeFloat(xmlDocument, parentNode, "stage-progress", interactionAirlock.mStageProgress);
                XmlUtils.serializeVector3(xmlDocument, parentNode, "target", interactionAirlock.mTarget);
            }
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
        private Interaction InteractionFromXml(AddInteractionDataPackage pkg)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pkg.XmlData);
            Type type = Type.GetType($"Planetbase.{pkg.InteractionType},Assembly-CSharp");
            Interaction interaction = (Interaction)Activator.CreateInstance(type);
            System.Reflection.MethodInfo deserialize_method = type.GetMethod("deserialize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.MethodInfo postInit_method = type.GetMethod("postInit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            deserialize_method.Invoke(interaction, new object[] { doc["root"].FirstChild });
            postInit_method.Invoke(interaction, new object[] { });
            return interaction;
        }
        private Interaction Find(Guid interactionId)
        {
            return interactions.FirstOrDefault(i => i.Key == interactionId).Value;
        }
        private Guid Find(Interaction interaction)
        {
            return interactions.FirstOrDefault(i => i.Value == interaction).Key;
        }
        private bool Contains(Guid interactionId)
        {
            return (interactions.Where(i => i.Key == interactionId).Count() != 0);
        }
        private bool Contains(Interaction interaction)
        {
            return (interactions.Where(i => i.Value == interaction).Count() != 0);
        }
        private Dictionary<Guid, Interaction> GetInteractions()
        {
            return interactions;
        }
        private void AddInteraction_ForReal(Guid key, Interaction value, bool syncWithOthers)
        {
#if DEBUG
            gameClient.debug_eventList.Add($"AddInteraction_ForReal: {value.GetType().Name}");
#endif
            if (Contains(key)) { UnityEngine.Debug.LogError($"Multiplayer interaction {key} already exists."); return; }
            interactions.Add(key, value);
            if (syncWithOthers)
            {
                Character character = value.mCharacter;
                gameClient.SendPacket(new Packet(PacketType.AddInteraction, new AddInteractionDataPackage(key, value.GetType().Name, InteractionToXml(value), character.getPosition(), character.getRotation())));
            }
        }
        private void RemoveInteraction_ForReal(Guid key, Interaction value, bool syncWithOthers)
        {
            if (value == null) return;
#if DEBUG
            Console.WriteLine($"{key}, {value}, {syncWithOthers}");
            gameClient.debug_eventList.Add($"RemoveInteraction_ForReal: {value.GetType().Name}");
#endif
            if (syncWithOthers)
            {
                gameClient.SendPacket(new Packet(PacketType.RemoveInteraction, new RemoveInteractionDataPackage(key)));
            }
            DestroyInteraction(value);
            interactions.Remove(key);
        }
        private void DestroyInteraction(Interaction value)
        {
            value.destroy();
        }
        public void ProcessPacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.AddInteraction:
                    AddInteractionDataPackage addInteractionDataPackage = packet.Data as AddInteractionDataPackage;
                    Interaction interaction = InteractionFromXml(addInteractionDataPackage);
                    Console.WriteLine($"Add interaction from packet: {interaction.GetType().Name}");
                    Character character = interaction.mCharacter;
                    character.setPosition(addInteractionDataPackage.CharacterPosition);
                    character.setRotation(addInteractionDataPackage.CharacterRotation);
                    AddInteraction_ForReal(addInteractionDataPackage.InteractionId, interaction, false);
                    break;
                case PacketType.RemoveInteraction:
                    RemoveInteractionDataPackage removeInteractionDataPackage = packet.Data as RemoveInteractionDataPackage;
                    Console.WriteLine($"Remove interaction from packet: {removeInteractionDataPackage.InteractionId}");
                    RemoveInteraction_ForReal(removeInteractionDataPackage.InteractionId, Find(removeInteractionDataPackage.InteractionId), false);
                    break;
                default:
                    Console.WriteLine($"MultiplayerInteractionManager dropped packet: {packet.Type}");
                    break;
            }
        }
        public void UpdateAll(float timeStep)
        {
            foreach (Interaction interaction in Interaction.mInteractions)
                if (interaction.update(timeStep))
                    if (gameClient.localPlayer.IsSimulationOwner)
                        Interaction.mPendingDestruction.Add(interaction);

            foreach (Interaction interaction in Interaction.mPendingDestruction)
                RemoveInteraction(interaction);
            Interaction.mPendingDestruction.Clear();
        }
    }
}
