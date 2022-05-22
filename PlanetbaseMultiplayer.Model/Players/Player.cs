using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Players
{
    [Serializable]
    public struct Player
    {
        private Guid id;
        private string name;
        private PlayerPermissions permissions;
        private PlayerState state;

        public Guid Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public PlayerPermissions Permissions { get => permissions; set => permissions = value; }
        public PlayerState State { get => state; set => state = value; }

        public Player(Guid id, string name, PlayerPermissions permissions, PlayerState state)
        {
            this.id = id;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.permissions = permissions;
            this.state = state;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Player))
                return false;

            return id == ((Player)obj).id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Player p1, Player p2) => p1.id == p2.id;
        public static bool operator !=(Player p1, Player p2) => p1.id != p2.id;
    }
}
