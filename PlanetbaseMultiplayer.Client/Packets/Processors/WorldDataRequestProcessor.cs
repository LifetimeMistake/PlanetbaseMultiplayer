using Planetbase;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.World;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Utils;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class WorldDataRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(WorldDataRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();
            WorldDataManager worldDataManager = context.ServiceLocator.LocateService<WorldDataManager>();

            Player? player = simulationManager.GetSimulationOwner();
            if (player.HasValue && client.LocalPlayer.HasValue && client.LocalPlayer.Value != player.Value)
                return; // Not the simulation owner

            WorldData worldData;
            try
            {
                worldData = worldDataManager.SaveWorldData();
            }
            catch(Exception ex)
            {
                Debug.LogWarning($"Failed to save world data: {ex}");
                // World data request failed, something went wrong
                WorldDataRequestFailPacket worldDataRequestFailPacket = new WorldDataRequestFailPacket();
                client.SendPacket(worldDataRequestFailPacket);
                return;
            }

            // Else, send world data to the server
            WorldDataPacket worldDataPacket = new WorldDataPacket(worldData);
            client.SendPacket(worldDataPacket);
        }
    }
}
