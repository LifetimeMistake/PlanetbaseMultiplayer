using Planetbase;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;

namespace PlanetbaseMultiplayer.Model.World
{
	public static class WorldSerializer
	{
		public static string Serialize(GameStateGame gameStateGame)
		{
			string result;
			XmlNode xmlNode = BeginSerialize("save-game");
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
			//Serialization.saveScreenshot();
			result = EndSerialize();
			return result;
		}
		private static XmlNode BeginSerialize(string rootNodeName)
		{
			FieldInfo mDocumentInfo = Reflection.GetPrivateFieldOrThrow(typeof(Serialization), "mDocument", false);
			FieldInfo mRootNodeInfo = Reflection.GetPrivateFieldOrThrow(typeof(Serialization), "mRootNode", false);

			Reflection.SetStaticFieldValue(mDocumentInfo, new XmlDocument());
			XmlDocument mDocument = Reflection.GetStaticFieldValue(mDocumentInfo) as XmlDocument;
			XmlNode mRootNode = Serialization.createNode(mDocument, rootNodeName, null);
			Reflection.SetStaticFieldValue(mRootNodeInfo, mRootNode);

			XmlAttribute xmlAttribute = mDocument.CreateAttribute("version");
			xmlAttribute.Value = 12.ToString();
			mRootNode.Attributes.Append(xmlAttribute);
			return mRootNode;
		}
		private static string EndSerialize()
		{
			FieldInfo mDocumentInfo = Reflection.GetPrivateFieldOrThrow(typeof(Serialization), "mDocument", false);
			FieldInfo mPathInfo = Reflection.GetPrivateFieldOrThrow(typeof(Serialization), "mPath", false);

			XmlDocument mDocument = Reflection.GetStaticFieldValue(mDocumentInfo) as XmlDocument;

			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					ConformanceLevel = ConformanceLevel.Fragment
				}))
				{
					mDocument.WriteTo(xmlWriter);
					xmlWriter.Flush();
					Reflection.SetStaticFieldValue(mDocumentInfo, null);
					Reflection.SetStaticFieldValue(mPathInfo, null);
					result = stringWriter.GetStringBuilder().ToString();
				}
			}
			return result;
		}
	}
}
