using PlanetbaseMultiplayer.Client.Environment;
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
    public class UpdateDisasterProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(UpdateDisasterPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            UpdateDisasterPacket updateDisasterPacket = (UpdateDisasterPacket)packet;
            DisasterManager disasterManager = context.ServiceLocator.LocateService<DisasterManager>();
            disasterManager.OnUpdateDisaster(updateDisasterPacket.CurrentTime);
        }
    }
}
