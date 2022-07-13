using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Packets.Processors;
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
    public class UpdateEnvironmentDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateEnvironmentDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = (UpdateEnvironmentDataPacket)packet;
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();
            EnvironmentManager environmentManager = context.ServiceLocator.LocateService<EnvironmentManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id)
            {
                //Deny request if client isn't the simulation owner
                return;
            }

            environmentManager.UpdateEnvironmentData(updateEnvironmentDataPacket.Time, updateEnvironmentDataPacket.WindLevel, updateEnvironmentDataPacket.WindDirection);
        }
    }
}
