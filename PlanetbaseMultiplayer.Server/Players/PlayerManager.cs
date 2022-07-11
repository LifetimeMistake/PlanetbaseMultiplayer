using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using PlanetbaseMultiplayer.Server.EventArgs;
using PlanetbaseMultiplayer.Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Players
{
    public class PlayerManager : IManager
    {
        private Server server;
        private Dictionary<Guid, Player> connectedPlayers;

        public bool IsInitialized { get; private set; }
        public event EventHandler<PlayerEventArgs> PlayerCreated;
        public event EventHandler<PlayerEventArgs> PlayerUpdated;
        public event EventHandler<PlayerEventArgs> PlayerRemoved;

        public PlayerManager(Server server)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            connectedPlayers = new Dictionary<Guid, Player>();
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public Player CreatePlayer(string username, PlayerPermissions permissions = PlayerPermissions.Standard, PlayerState state = PlayerState.ConnectedUnauthenticated)
        {
            Guid playerId = Guid.NewGuid();
            return CreatePlayer(playerId, username, permissions, state);
        }

        public Player CreatePlayer(Guid playerId, string username, PlayerPermissions permissions = PlayerPermissions.Standard, PlayerState state = PlayerState.ConnectedUnauthenticated)
        {
            if (connectedPlayers.Values.Any(p => p.Name == username))
                throw new UsernameTakenException();

            Player player = new Player(playerId, username, permissions, state);
            connectedPlayers.Add(playerId, player);
            Console.WriteLine($"Player created: {playerId}");

            PlayerJoinedPacket playerJoinedPacket = new PlayerJoinedPacket(player);
            server.SendPacketToAllExcept(playerJoinedPacket, playerId);

            PlayerEventArgs playerEventArgs = new PlayerEventArgs(playerId);
            PlayerCreated?.Invoke(this, playerEventArgs);

            return player;
        }
        
        public bool UpdatePlayer(Player player)
        {
            return UpdatePlayer(player.Id, player);
        }

        public bool UpdatePlayer(Guid playerId, Player player)
        {
            bool flag = connectedPlayers.ContainsKey(playerId);
            if (flag)
                connectedPlayers[playerId] = player;

            Console.WriteLine($"Player updated: {playerId}");

            PlayerDataUpdatedPacket playerDataUpdatedPacket = new PlayerDataUpdatedPacket(playerId, player);
            server.SendPacketToAll(playerDataUpdatedPacket);

            PlayerEventArgs playerEventArgs = new PlayerEventArgs(playerId);
            PlayerUpdated?.Invoke(this, playerEventArgs);

            return flag;
        }

        public bool DestroyPlayer(Player player, DisconnectReason reason)
        {
            return DestroyPlayer(player.Id, reason);
        }

        public bool DestroyPlayer(Guid playerId, DisconnectReason reason)
        {
            bool removed = connectedPlayers.Remove(playerId);
            Console.WriteLine($"Player destroyed: {playerId}");

            PlayerDisconnectedPacket playerDisconnectedPacket = new PlayerDisconnectedPacket(playerId, reason);
            server.SendPacketToAll(playerDisconnectedPacket);

            PlayerEventArgs playerEventArgs = new PlayerEventArgs(playerId);
            PlayerRemoved?.Invoke(this, playerEventArgs);
            return removed;
        }

        public Player GetPlayer(Guid playerId)
        {
            return connectedPlayers[playerId];
        }

        public List<Player> GetPlayers()
        {
            return connectedPlayers.Values.ToList();
        }

        public bool PlayerExists(Guid playerId)
        {
            return connectedPlayers.ContainsKey(playerId);
        }

        public bool PlayerExists(Player player)
        {
            return PlayerExists(player.Id);
        }

        public int GetPlayerCount()
        {
            return connectedPlayers.Count;
        }

        public bool IsUsernameAllowed(string username)
        {
            // Empty or null usernames are disallowed
            if (string.IsNullOrEmpty(username))
                return false;

            // Usernames consisting of only whitespace are disallowed
            if (username.Trim().Length == 0)
                return false;

            return true;
        }

        public bool IsUsernameTaken(string username)
        {
            return connectedPlayers.Values.Any(player => player.Name == username);
        }
    }
}
