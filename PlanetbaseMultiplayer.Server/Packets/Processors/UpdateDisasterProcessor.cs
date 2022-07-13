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
    public class UpdateDisasterProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateDisasterPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            UpdateDisasterPacket updateDisasterPacket = (UpdateDisasterPacket)packet;
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();
            DisasterManager disasterManager = context.ServiceLocator.LocateService<DisasterManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id)
            {
                //Deny request if client isn't the simulation owner
                return;
            }

            disasterManager.UpdateDisaster(updateDisasterPacket.CurrentTime);
        }
    }
}
