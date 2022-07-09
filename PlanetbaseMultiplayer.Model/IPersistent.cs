using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PlanetbaseMultiplayer.Model
{
    public interface IPersistent
    {
        bool Save(WorldData world); // Dumps the persistence data into the given WorldData object
        bool Load(WorldData world); // Loads the persistence data from the given WorldData object
    }
}
