using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using PlanetbaseMultiplayer.Server.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class TimeScaleUpdateProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(TimeScaleUpdatePacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            TimeScaleUpdatePacket timeScaleUpdatePacket = (TimeScaleUpdatePacket)packet;
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();
            TimeManager timeManager = context.ServiceLocator.LocateService<TimeManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id) 
            {
                //Deny request if client isn't the simulation owner
                return;
            }

            if(timeManager.IsTimeLocked())
            {
                // The server is holding onto the current timescale
                // Inform the client, maybe?
                return;
            }

            timeManager.SetTimescale(timeScaleUpdatePacket.TimeScale, timeScaleUpdatePacket.IsPaused);
        }
    }
}
