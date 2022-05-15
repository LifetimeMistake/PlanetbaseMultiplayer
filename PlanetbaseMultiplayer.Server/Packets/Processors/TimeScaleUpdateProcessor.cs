using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    class TimeScaleUpdateProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(TimeScaleUpdatePacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            TimeScaleUpdatePacket timeScaleUpdatePacket = (TimeScaleUpdatePacket)packet;
            ServerProcessorContext processorContext = (ServerProcessorContext)context;
            SimulationManager simulationManager = processorContext.Server.SimulationManager;
            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id) 
            {
                //Deny request if client isn't the simulation owner
                return;
            }
            processorContext.Server.TimeManager.SetTimescale(timeScaleUpdatePacket.TimeScale, timeScaleUpdatePacket.IsPaused);
        }
    }
}
