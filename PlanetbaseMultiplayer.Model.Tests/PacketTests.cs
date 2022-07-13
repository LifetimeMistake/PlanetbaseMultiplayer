using System.Reflection;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets;
using NUnit.Framework;

namespace PlanetbaseMultiplayer.Model.Tests
{
    public class PacketTests
    {
        [Test]
        public void PacketSerializableTest()
        {
            Assembly? ass = Assembly.GetAssembly(typeof(Packet));
            List<Type> invalidTypes = new List<Type>();

            if (ass == null)
            {
                throw new Exception("Could not find assembly");
            }

            foreach (Type packetType in ass.GetTypes().Where(p => p.IsAssignableTo(typeof(Packet))))
            {
                if (packetType.GetCustomAttribute(typeof(SerializableAttribute)) == null)
                {
                    invalidTypes.Add(packetType);
                    throw new Exception($"Packet type {packetType} in {packetType.Namespace} has no Serializable attribute");
                }
            }

            Assert.That(invalidTypes.Count, Is.EqualTo(0));
        }
    }
}