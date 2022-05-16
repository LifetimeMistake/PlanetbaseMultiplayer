using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Environment;
using PlanetbaseMultiplayer.Server.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    class UpdateEnvironmentDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateEnvironmentDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = (UpdateEnvironmentDataPacket)packet;
            ServerProcessorContext processorContext = (ServerProcessorContext)context;
            SimulationManager simulationManager = processorContext.Server.SimulationManager;
            EnvironmentManager environmentManager = processorContext.Server.EnvironmentManager;

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id)
                return;

            environmentManager.UpdateEnvironmentData(updateEnvironmentDataPacket.Time, updateEnvironmentDataPacket.WindLevel);
        }
    }
}
