using PlanetbaseMultiplayer.Client.World;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Packets.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class WorldDataProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(WorldDataPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            WorldDataPacket worldDataPacket = (WorldDataPacket)packet;
            WorldStateManager worldStateManager = context.ServiceLocator.LocateService<WorldStateManager>();
            worldStateManager.UpdateWorldData(worldDataPacket.World);

            Debug.Log("Informing the server that we've started loading the world data");
            ClientLoadingStartedPacket clientLoadingStartedPacket = new ClientLoadingStartedPacket();
            client.SendPacket(clientLoadingStartedPacket);
        }
    }
}
