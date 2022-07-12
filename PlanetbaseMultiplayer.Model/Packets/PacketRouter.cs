using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets
{
    public class PacketRouter
    {
        private ProcessorContext processorContext;
        private Dictionary<Type, PacketProcessor> registeredProcessors;

        public PacketRouter(ProcessorContext processorContext, IEnumerable<PacketProcessor> packetProcessors)
        {
            this.processorContext = processorContext;
            registeredProcessors = new Dictionary<Type, PacketProcessor>();

            foreach(PacketProcessor processor in packetProcessors)
                RegisterPacketProcessor(processor);
        }

        public bool ProcessPacket(Guid sourcePlayerId, Packet packet)
        {
#if DEBUG
            Console.WriteLine($"Handling packet {packet.GetType().Name} from {sourcePlayerId}");
#endif
            Type packetType = packet.GetType();
            if (!registeredProcessors.ContainsKey(packetType))
                return false;

            PacketProcessor processor = registeredProcessors.First(p => p.Key == packetType).Value;
            processor.ProcessPacket(sourcePlayerId, packet, processorContext);
            return true;
        }

        // Autodetects the packet type that should be passed to the processor 
        public void RegisterPacketProcessor(PacketProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            RegisterPacketProcessor(processor.GetProcessedPacketType(), processor);
        }

        // Can be used to force a processor to handle packets it officially shouldn't handle
        // Might need this in the future idk
        public void RegisterPacketProcessor(Type packetType, PacketProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            if (registeredProcessors.ContainsKey(packetType))
                throw new Exception("A packet processor for this packet type is already registered");

#if DEBUG
            Console.WriteLine($"Registered processor {processor.GetType().FullName} for packet {packetType.Name}");
#endif

            registeredProcessors.Add(packetType, processor);
        }
    }
}
