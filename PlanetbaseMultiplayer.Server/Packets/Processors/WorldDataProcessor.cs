﻿using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using PlanetbaseMultiplayer.Server.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class WorldDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(WorldDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            ServerProcessorContext processorContext = (ServerProcessorContext)context;
            WorldDataPacket worldDataPacket = (WorldDataPacket)packet;
            SimulationManager simulationManager = processorContext.ServiceLocator.LocateService<SimulationManager>();
            WorldStateManager worldStateManager = processorContext.ServiceLocator.LocateService<WorldStateManager>();

            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || sourcePlayerId != simulationOwner.Value.Id)
            {
                //Deny request if client isn't the simulation owner
                return;
            }

            worldStateManager.OnWorldDataReceived(worldDataPacket.World);
        }
    }
}
