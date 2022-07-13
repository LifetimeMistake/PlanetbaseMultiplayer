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

            Player? player = simulationManager.GetSimulationOwner();
            if (player.HasValue && client.LocalPlayer.HasValue && client.LocalPlayer.Value != player.Value)
                return; // Not the simulation owner

            GameStateGame gameStateGame = GameManager.getInstance().getGameState() as GameStateGame;
            string xmlData = WorldSerializer.Serialize(gameStateGame);
            WorldData worldData = new WorldData();
            worldData.XmlData = xmlData;

            foreach (IPersistent persistent in context.ServiceLocator.LocateServicesOfType<IPersistent>())
            {
                if (!persistent.Save(worldData))
                {
                    // Inform the server that something went terribly wrong.
                    WorldDataRequestFailPacket worldDataRequestFailPacket = new WorldDataRequestFailPacket();
                    client.SendPacket(worldDataRequestFailPacket);

                    // TODO: Handle this by informing the user about it and not interrupting the game
                    throw new Exception($"Failed to serialize persistent manager: {persistent.GetType().Name}");
                }
            }

            WorldDataPacket worldDataPacket = new WorldDataPacket(worldData);
            client.SendPacket(worldDataPacket);
        }
    }
}
