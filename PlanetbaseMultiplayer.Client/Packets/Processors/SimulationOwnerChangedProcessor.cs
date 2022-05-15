using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class SimulationOwnerChangedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(SimulationOwnerChangedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            SimulationOwnerChangedPacket simulationOwnerChangedPacket = (SimulationOwnerChangedPacket)packet;
            ClientProcessorContext processorContext = (ClientProcessorContext)context;
            PlayerManager playerManager = processorContext.Client.PlayerManager;
            SimulationManager simulationManager = processorContext.Client.SimulationManager;
            if(simulationOwnerChangedPacket.PlayerId != null)
            {
                Player player = playerManager.GetPlayer(simulationOwnerChangedPacket.PlayerId.Value);
                simulationManager.OnSimulationOwnerUpdated(player);
            }
            else
            {
                // No simulation owner
                simulationManager.OnSimulationOwnerUpdated(null);
            }
        }
    }
}
