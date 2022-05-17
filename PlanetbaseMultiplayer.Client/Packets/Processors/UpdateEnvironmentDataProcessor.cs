﻿using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class UpdateEnvironmentDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateEnvironmentDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = (UpdateEnvironmentDataPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;

            Player? simulationOwner = processorContext.Client.SimulationManager.GetSimulationOwner();
            if (simulationOwner != null && sourcePlayerId == simulationOwner.Value.Id)
                return;

            processorContext.Client.EnvironmentManager.OnUpdateEnvironmentData(updateEnvironmentDataPacket.Time, updateEnvironmentDataPacket.WindLevel, updateEnvironmentDataPacket.WindDirection);
        }
    }
}