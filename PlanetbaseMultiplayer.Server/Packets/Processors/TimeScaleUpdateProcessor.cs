using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Time;
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
            processorContext.Server.TimeManager.SetSpeed(timeScaleUpdatePacket.TimeScale);
            processorContext.Server.TimeManager.SetPausedState(timeScaleUpdatePacket.IsPaused);
        }
    }
}
