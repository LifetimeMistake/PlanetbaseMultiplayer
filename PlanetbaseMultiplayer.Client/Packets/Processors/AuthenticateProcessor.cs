using Planetbase;
using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
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

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            ClientProcessorContext processorContext = (ClientProcessorContext)context;
            AuthenticatePacket authenticatePacket = (AuthenticatePacket)packet;
            PlayerManager playerManager = processorContext.Client.PlayerManager;

            if(!authenticatePacket.AuthenticationSuccessful)
            {
                switch (authenticatePacket.ErrorReason.Value)
                {
                    case AuthenticationErrorReason.UsernameTaken:
                        processorContext.Client.ShowMessageBox(null, "Failed to join game", "Failed to connect to the server: Username is already taken");
                        break;
                    case AuthenticationErrorReason.IncorrectPassword:
                        processorContext.Client.ShowMessageBox(null, "Failed to join game", "Failed to connect to the server: Incorrect password");
                        break;
                    case AuthenticationErrorReason.IllegalUsername:
                        processorContext.Client.ShowMessageBox(null, "Failed to join game",  "Failed to connect to the server: Username contains disallowed characters");
                        break;
                }

                processorContext.Client.RequestDisconnect();
                return;
            }

            processorContext.Client.LocalPlayer = authenticatePacket.LocalPlayer.Value;
            processorContext.Client.SimulationManager.OnSimulationOwnerUpdated(authenticatePacket.SimulationOwner);
            foreach (Player player in authenticatePacket.Players)
                playerManager.OnPlayerAdded(player); // Sync players

            Debug.Log("Sending world data request");
            WorldDataRequestPacket worldDataRequestPacket = new WorldDataRequestPacket();
            processorContext.Client.SendPacket(worldDataRequestPacket);
        }
    }
}
