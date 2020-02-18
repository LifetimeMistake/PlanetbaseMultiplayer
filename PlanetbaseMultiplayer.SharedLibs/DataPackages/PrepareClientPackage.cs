using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class PrepareClientPackage : IDataPackage
    {
        public Player Player;

        public PrepareClientPackage(Player player)
        {
            Player = player;
        }
    }
}
