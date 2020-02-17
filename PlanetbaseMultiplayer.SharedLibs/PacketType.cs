using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs
{
    public enum PacketType : int
    {
        LoadXmlSaveData = 0,
        PrepareClient = 1,
        SetGameTimeSpeed = 2,
        SetClientState = 3,
        RequestXmlSaveData = 4,
        PlaceModule = 5,
        PlaceComponent = 6,
        PlaceConnection = 7,
        SetSimOwnerStatus = 8
    }
}
