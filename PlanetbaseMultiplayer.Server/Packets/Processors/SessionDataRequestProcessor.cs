using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Server.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Packets.Processors
{
    public class SessionDataRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(SessionDataRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Server server = context.ServiceLocator.LocateService<Server>();
            PlayerManager playerManager = context.ServiceLocator.LocateService<PlayerManager>();
            ServerSettings serverSettings = server.Settings;

            SessionData sessionData = new SessionData(serverSettings.Name, serverSettings.PasswordProtected, playerManager.GetPlayerCount());
            SessionDataPacket sessionDataPacket = new SessionDataPacket(sessionData);
            server.SendPacketToPlayer(sessionDataPacket, sourcePlayerId);
        }
    }
}
