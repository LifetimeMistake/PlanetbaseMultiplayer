using PlanetbaseMultiplayer.Client.Time;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class TimeScaleUpdateProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(TimeScaleUpdatePacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            TimeScaleUpdatePacket timeScaleUpdatedPacket = (TimeScaleUpdatePacket)packet;
            TimeManager timeManager = context.ServiceLocator.LocateService<TimeManager>();
            timeManager.OnTimeScaleUpdated(timeScaleUpdatedPacket.TimeScale, timeScaleUpdatedPacket.IsPaused);
        }
    }
}
