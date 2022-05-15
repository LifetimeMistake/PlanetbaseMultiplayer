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
        public PlayerPermissions Permissions;
        public PlayerState State;

        public Player(Guid id, string name, PlayerPermissions permissions, PlayerState state)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Permissions = permissions;
            State = state;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Player))
                return false;

            return Id == ((Player)obj).Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Player p1, Player p2) => p1.Id == p2.Id;
        public static bool operator !=(Player p1, Player p2) => p1.Id != p2.Id;
    }
}
