using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class SimulationOwnerChangedProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(SimulationOwnerChangedPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            SimulationOwnerChangedPacket simulationOwnerChangedPacket = (SimulationOwnerChangedPacket)packet;
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();

            if(simulationOwnerChangedPacket.PlayerId != null)
            {
                Debug.Log($"Setting new simulation owner to {simulationOwnerChangedPacket.PlayerId.Value}");
                Player player = playerManager.GetPlayer(simulationOwnerChangedPacket.PlayerId.Value);
                simulationManager.OnSimulationOwnerUpdated(player);
                MessageLog.Show($"New simulation owner: {player.Name}", null, MessageLogFlags.MessageSoundNormal);
            }
            else
            {
                // No simulation owner
                simulationManager.OnSimulationOwnerUpdated(null);
                Debug.Log("Setting new simulation owner to none");
            }
        }
    }
}
