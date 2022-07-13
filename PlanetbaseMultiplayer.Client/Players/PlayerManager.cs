using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Server.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Players
{
    public class PlayerManager : IManager
    {
        private Client client;
        private Dictionary<Guid, Player> connectedPlayers;

        public bool IsInitialized { get; private set; }
        //public event EventHandler<PlayerEventArgs> PlayerAdded;
        //public event EventHandler<PlayerEventArgs> PlayerUpdated;
        //public event EventHandler<PlayerEventArgs> PlayerRemoved;

        public PlayerManager(Client client)
        {
            this.client = client;
            connectedPlayers = new Dictionary<Guid, Player>();
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void OnPlayerAdded(Player player)
        {
            connectedPlayers.Add(player.Id, player);
            //PlayerEventArgs playerEventArgs = new PlayerEventArgs(player.Id);
            //PlayerAdded?.Invoke(this, playerEventArgs);
        }

        public bool OnPlayerRemoved(Guid playerId)
        {
            //PlayerEventArgs playerEventArgs = new PlayerEventArgs(playerId);
            //PlayerRemoved?.Invoke(this, playerEventArgs);

            return connectedPlayers.Remove(playerId);
        }

        public bool OnPlayerUpdated(Guid playerId, Player player)
        {
            bool flag = connectedPlayers.ContainsKey(playerId);
            if (flag)
                connectedPlayers[playerId] = player;

            //PlayerEventArgs playerEventArgs = new PlayerEventArgs(playerId);
            //PlayerUpdated?.Invoke(this, playerEventArgs);

            return flag;
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
    }
}
