using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Server.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class AuthenticateRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(AuthenticateRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, IProcessorContext context)
        {
            ServerProcessorContext processorContext = (ServerProcessorContext)context;
            AuthenticateRequestPacket authenticateRequestPacket = (AuthenticateRequestPacket)packet;
            ServerSettings serverSettings = processorContext.Server.Settings;
            PlayerManager playerManager = processorContext.ServiceLocator.LocateService<PlayerManager>();
            SimulationManager simulationManager = processorContext.ServiceLocator.LocateService<SimulationManager>();

            AuthenticatePacket authenticateResponsePacket;
            if (playerManager.PlayerExists(sourcePlayerId))
                return; // Player already authenticated

            // Will fail if the requested username contains disallowed characters/does not meet length requirements/etc.
            if (!playerManager.IsUsernameAllowed(authenticateRequestPacket.Username))
            {
                authenticateResponsePacket = new AuthenticatePacket(false, AuthenticationErrorReason.IllegalUsername, null, null, null);
                Console.WriteLine($"Player {sourcePlayerId} attempted joining with an illegal username");
            }
            // Will fail if a player with the same nickname is already connected.
            else if(playerManager.IsUsernameTaken(authenticateRequestPacket.Username))
            {
                authenticateResponsePacket = new AuthenticatePacket(false, AuthenticationErrorReason.UsernameTaken, null, null, null);
                Console.WriteLine($"Player {sourcePlayerId} attempted joining with an already taken username: {authenticateRequestPacket.Username}");
            }
            // Will fail if the authenticating client provided an incorrect server password.
            else if (serverSettings.PasswordProtected && serverSettings.Password != authenticateRequestPacket.Password)
            {
                authenticateResponsePacket = new AuthenticatePacket(false, AuthenticationErrorReason.IncorrectPassword, null, null, null);
                Console.WriteLine($"Player {sourcePlayerId} attempted joining with an incorrect password");
            }
            // Approve authentication request and let other players know that a player joined.
            else
            {
                Player client = playerManager.CreatePlayer(sourcePlayerId, authenticateRequestPacket.Username, state: PlayerState.ConnectedMainMenu);
                Player[] players = playerManager.GetPlayers().ToArray();
                Player? simulationOwner = simulationManager.GetSimulationOwner();
                authenticateResponsePacket = new AuthenticatePacket(true, null, client, players, simulationOwner);
                Console.WriteLine($"Player {sourcePlayerId} successfully authenticated!");
            }

            processorContext.Server.SendPacketToPlayer(authenticateResponsePacket, sourcePlayerId);
        }
    }
}
