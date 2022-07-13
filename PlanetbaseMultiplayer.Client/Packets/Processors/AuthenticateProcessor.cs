using Planetbase;
using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class AuthenticateProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(AuthenticatePacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            AuthenticatePacket authenticatePacket = (AuthenticatePacket)packet;
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();
            SimulationManager simulationManager = context.ServiceLocator.LocateService<SimulationManager>();

            if(!authenticatePacket.AuthenticationSuccessful)
            {
                switch (authenticatePacket.ErrorReason.Value)
                {
                    case AuthenticationErrorReason.UsernameTaken:
                        MessageBoxOk.Show(null, "Failed to join game", "Failed to connect to the server: Username is already taken");
                        break;
                    case AuthenticationErrorReason.IncorrectPassword:
                        MessageBoxOk.Show(null, "Failed to join game", "Failed to connect to the server: Incorrect password");
                        break;
                    case AuthenticationErrorReason.IllegalUsername:
                        MessageBoxOk.Show(null, "Failed to join game",  "Failed to connect to the server: Username contains disallowed characters");
                        break;
                }

                client.RequestDisconnect();
                return;
            }

            client.LocalPlayer = authenticatePacket.LocalPlayer.Value;
            simulationManager.OnSimulationOwnerUpdated(authenticatePacket.SimulationOwner);
            foreach (Player player in authenticatePacket.Players)
                playerManager.OnPlayerAdded(player); // Sync players

            Debug.Log("Sending world data request");
            WorldDataRequestPacket worldDataRequestPacket = new WorldDataRequestPacket();
            client.SendPacket(worldDataRequestPacket);
        }
    }
}
