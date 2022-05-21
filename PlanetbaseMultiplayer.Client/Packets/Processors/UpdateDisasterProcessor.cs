using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class UpdateDisasterProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateDisasterProcessor);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            UpdateDisasterPacket updateDisasterPacket = (UpdateDisasterPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;

            Player? simulationOwner = processorContext.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner != null && sourcePlayerId == simulationOwner.Value.Id)
                return;

            processorContext.Client.DisasterManager.OnUpdateDisaster(updateDisasterPacket.CurrentTime);
        }
    }
}
