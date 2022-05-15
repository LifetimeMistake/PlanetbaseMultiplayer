using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace PlanetbaseMultiplayer.Model.Utils
{
	public static class XmlSerializerUtils
	{
		public static XmlNode createNode(XmlDocument document, XmlNode parentNode, string name, string typeName = null)
		{
			XmlElement xmlElement = document.CreateElement(name);
			if (typeName != null)
			{
				XmlAttribute xmlAttribute = document.CreateAttribute("type");
				xmlAttribute.Value = typeName;
				xmlElement.Attributes.Append(xmlAttribute);
			}
			parentNode.AppendChild(xmlElement);
			return xmlElement;
		}
		public static void serializeSelectable(XmlDocument document, XmlNode parent, string name, Selectable selectable)
		{
			XmlAttribute xmlAttribute = document.CreateAttribute("id");
			xmlAttribute.Value = selectable.getId().ToString();
			XmlAttribute xmlAttribute2 = document.CreateAttribute("type-name");
			xmlAttribute2.Value = selectable.GetType().Name;
			XmlElement xmlElement = document.CreateElement(name);
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			parent.AppendChild(xmlElement);
		}
		public static void serializeString(XmlDocument document, XmlNode parent, string name, string value)
		{
			XmlAttribute xmlAttribute = document.CreateAttribute("value");
			xmlAttribute.Value = value;
			XmlElement xmlElement = document.CreateElement(name);
			xmlElement.Attributes.Append(xmlAttribute);
			parent.AppendChild(xmlElement);
		}
		public static void serializeFloat(XmlDocument document, XmlNode parent, string name, float value)
		{
			serializeString(document, parent, name, value.ToString());
		}
		public static void serializeDouble(XmlDocument document, XmlNode parent, string name, double value)
		{
			serializeString(document, parent, name, value.ToString());
		}
		public static void serializeInt(XmlDocument document, XmlNode parent, string name, int value)
		{
			serializeString(document, parent, name, value.ToString());
		}
		public static void serializeQuaternion(XmlDocument document, XmlNode parent, string name, Quaternion q)
		{
			serializeVector3(document, parent, name, q.eulerAngles);
		}
		public static void serializeBool(XmlDocument document, XmlNode parent, string name, bool value)
		{
			serializeString(document, parent, name, value.ToString());
		}
		public static void serializeVector3(XmlDocument document, XmlNode parent, string name, Vector3 v)
		{
			XmlAttribute xmlAttribute = document.CreateAttribute("x");
			xmlAttribute.Value = v.x.ToString();
			XmlAttribute xmlAttribute2 = document.CreateAttribute("y");
			xmlAttribute2.Value = v.y.ToString();
			XmlAttribute xmlAttribute3 = document.CreateAttribute("z");
			xmlAttribute3.Value = v.z.ToString();
			XmlElement xmlElement = document.CreateElement(name);
			xmlElement.Attributes.Append(xmlAttribute);
			xmlElement.Attributes.Append(xmlAttribute2);
			xmlElement.Attributes.Append(xmlAttribute3);
			parent.AppendChild(xmlElement);
		}
	}
}
