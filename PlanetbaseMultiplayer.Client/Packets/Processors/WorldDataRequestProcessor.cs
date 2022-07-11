﻿using Planetbase;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Utils;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class WorldDataRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(WorldDataRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            ClientProcessorContext clientProcessor = (ClientProcessorContext)context;
            SimulationManager simulationManager = clientProcessor.ServiceLocator.LocateService<SimulationManager>();

            Player? player = simulationManager.GetSimulationOwner();
            if (player.HasValue && clientProcessor.Client.LocalPlayer.HasValue && clientProcessor.Client.LocalPlayer.Value != player.Value)
                return; // Not the simulation owner

            GameStateGame gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;

            string xmlData = WorldSerializer.Serialize(gameStateGame);
            WorldStateData worldStateData = new WorldStateData(xmlData);

            WorldDataPacket worldDataPacket = new WorldDataPacket(worldStateData);
            clientProcessor.Client.SendPacket(worldDataPacket);
        }
    }
}
