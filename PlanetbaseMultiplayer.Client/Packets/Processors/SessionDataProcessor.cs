using Planetbase;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class SessionDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(SessionDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            SessionDataPacket sessionDataPacket = (SessionDataPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;
            processorContext.Client.OnSessionDataReceived(sessionDataPacket.SessionData);
        }
    }
}
