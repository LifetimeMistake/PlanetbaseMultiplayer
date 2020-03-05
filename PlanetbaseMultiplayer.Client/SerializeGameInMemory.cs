using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Client
{
    public static class SerializeGameInMemory
    {
		public static string saveGame_toMemory(GameStateGame __instance)
		{
			string result;
			try
			{
				Directory.CreateDirectory(SaveData.FolderName);
				XmlNode xmlNode = beginSave_toMemory("save-game");
				Singleton<IdGenerator>.getInstance().serialize(xmlNode, "id-generator");
				Singleton<PlanetManager>.getInstance().serialize(xmlNode, "planet");
				Singleton<MilestoneManager>.getInstance().serialize(xmlNode, "milestones");
				Singleton<TechManager>.getInstance().serialize(xmlNode, "techs");
				Singleton<EnvironmentManager>.getInstance().serialize(xmlNode, "environment");
				Singleton<TerrainGenerator>.getInstance().serialize(xmlNode, "terrain");
				CameraManager.getInstance().serialize(xmlNode, "camera");
				Singleton<DisasterManager>.getInstance().serialize(xmlNode);
				Singleton<Colony>.getInstance().serialize(xmlNode, "colony");
				Singleton<LandingShipManager>.getInstance().serialize(xmlNode, "ship-manager");
				Singleton<StatsCollector>.getInstance().serialize(xmlNode, "stats");
				Singleton<VisitorEventManager>.getInstance().serialize(xmlNode, "visitor-events");
				Singleton<GameHintManager>.getInstance().serialize(xmlNode, "game-hints");
				Singleton<MeteorManager>.getInstance().serialize(xmlNode, "meteor-manager");
				Singleton<ThunderstormManager>.getInstance().serialize(xmlNode, "thunderstorm-manager");
				Singleton<ManufactureLimits>.getInstance().serialize(xmlNode, "manufacture-limits");
				Singleton<ChallengeManager>.getInstance().serialize(xmlNode, "challenge-manager");
				Construction.serializeAll(xmlNode, "constructions");
				Character.serializeAll(xmlNode, "characters");
				Resource.serializeAll(xmlNode, "resources");
				Ship.serializeAll(xmlNode, "ships");
				Interaction.serializeAll(xmlNode, "interactions");
				Serialization.saveScreenshot();
				result = endSave_toMemory();
			}
			catch (UnauthorizedAccessException e)
			{
				__instance.onSaveError(e);
				result = null;
			}
			catch (IOException e2)
			{
				__instance.onSaveError(e2);
				result = null;
			}
			return result;
		}
		public static XmlNode beginSave_toMemory(string rootNodeName)
		{
			if (Serialization.mDocument != null)
			{
				UnityEngine.Debug.LogWarning("Serialization XmlDocument is not null");
			}
			Serialization.mDocument = new XmlDocument();
			Serialization.mRootNode = Serialization.createNode(Serialization.mDocument, rootNodeName, null);
			XmlAttribute xmlAttribute = Serialization.mDocument.CreateAttribute("version");
			xmlAttribute.Value = 12.ToString();
			Serialization.mRootNode.Attributes.Append(xmlAttribute);
			return Serialization.mRootNode;
		}
		public static string endSave_toMemory()
		{
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					ConformanceLevel = ConformanceLevel.Fragment
				}))
				{
					Serialization.mDocument.WriteTo(xmlWriter);
					xmlWriter.Flush();
					Serialization.mDocument = null;
					Serialization.mPath = null;
					result = stringWriter.GetStringBuilder().ToString();
				}
			}
			return result;
		}
	}
}
