using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Players
{
    [Serializable]
    public struct Player
    {
        public Guid Id;
        public string Name;
        public bool IsSimulationOwner;
        public PlayerPermissions Permissions;
        public PlayerState State;

        public Player(Guid id, string name, bool isSimulationOwner, PlayerPermissions permissions, PlayerState state)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsSimulationOwner = isSimulationOwner;
            Permissions = permissions;
            State = state;
        }
    }
}
