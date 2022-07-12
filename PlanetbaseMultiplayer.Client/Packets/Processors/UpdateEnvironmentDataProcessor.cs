﻿using PlanetbaseMultiplayer.Client.Environment;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Packets.Processors;
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

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = (UpdateEnvironmentDataPacket)packet;
            EnvironmentManager environmentManager = context.ServiceLocator.LocateService<EnvironmentManager>();
            environmentManager.OnUpdateEnvironmentData(updateEnvironmentDataPacket.Time, updateEnvironmentDataPacket.WindLevel, updateEnvironmentDataPacket.WindDirection);
        }
    }
}