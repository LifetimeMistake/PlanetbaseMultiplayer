using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
        SetSimOwnerStatus = 8,
        IncrementNextId = 9,
        IncrementNextBotId = 10,
        ProduceResource = 11,
        RecycleColonyShip = 12,
        RecycleComponent = 13,
        RecycleSelectable = 14,
        CharacterStartWalking = 15,
        CharacterLoadResource = 16,
        CharacterUnloadResource = 17,
        RemoveInteraction = 18,
        AddInteraction = 19,
        AddConstructionMaterial = 20,
        CharacterStoreResource = 21,
        CharacterEmbedResource = 22,
        CharacterDestroyResource = 23,
        ExtractResource = 24,
        BuildableBuilt = 25,
        ConstructionSetPriority = 26,
        BuildableSetEnabled = 27,
        DecideNextSandstorm = 28,
        EndSandstorm = 29,
        TriggerSandstorm = 30
    }
}
