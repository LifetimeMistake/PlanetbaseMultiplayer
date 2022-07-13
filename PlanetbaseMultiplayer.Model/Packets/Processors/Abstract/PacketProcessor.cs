using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Processors.Abstract
{
    public abstract class PacketProcessor
    {
        public abstract Type GetProcessedPacketType();
        public abstract void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context);
        public static List<PacketProcessor> GetProcessors(Dictionary<Type, object> processorArguments = null)
        {
            if (processorArguments == null)
                processorArguments = new Dictionary<Type, object>();

            return Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(p => typeof(PacketProcessor).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .Select(proc =>
                {
                    ConstructorInfo[] ctors = proc.GetConstructors();
                    if (ctors.Length > 1)
                    {
                        throw new NotSupportedException($"Packet processor {proc.Name} has more than one constructor!");
                    }

                    ConstructorInfo ctor = ctors.First();

                    // Prepare arguments for constructor (if applicable):
                    object[] args = ctor.GetParameters().Select(pi =>
                    {
                        if (processorArguments.TryGetValue(pi.ParameterType, out object v))
                        {
                            return v;
                        }

                        throw new ArgumentException($"Argument value not defined for type {pi.ParameterType}! Used in {proc}");
                    }).ToArray();

                    return (PacketProcessor)ctor.Invoke(args);
                }).ToList();
        }
    }
}
