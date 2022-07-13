using System.Reflection;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlanetbaseMultiplayer.Model.Tests
{
    public class PacketTests
    {
        [NUnit.Framework.Test]
        public void PacketSerializableTest()
        {
            Assembly ass = Assembly.GetAssembly(typeof(Packet));
            List<Type> invalidTypes = new List<Type>();

            if (ass == null)
            {
                throw new Exception("Could not find assembly");
            }

            foreach (Type packetType in ass.GetTypes().Where(p => typeof(Packet).IsAssignableFrom(p)))
            {
                if (!packetType.GetCustomAttributes(true).Any(attr => attr.GetType() == typeof(SerializableAttribute)))
                {
                    invalidTypes.Add(packetType);
                    throw new Exception($"Packet type {packetType} in {packetType.Namespace} has no Serializable attribute");
                }
            }

            NUnit.Framework.Assert.That(invalidTypes.Count, NUnit.Framework.Is.EqualTo(0));
        }
    }
}