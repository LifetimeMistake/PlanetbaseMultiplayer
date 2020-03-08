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
        RecycleSelectable = 11,
        CharacterStartWalking = 12,
        CharacterLoadResource = 13,
        CharacterUnloadResource = 14,
        RemoveInteraction = 15,
        AddInteraction = 16,
        AddConstructionMaterial = 17,
        CharacterStoreResource = 18,
        CharacterEmbedResource = 19,
        CharacterDestroyResource = 20,
        ExtractResource = 21,
        BuildableBuilt = 22,
        ConstructionSetPriority = 23,
        BuildableSetEnabled = 24,
        DecideNextSandstorm = 25,
        EndSandstorm = 26,
        TriggerSandstorm = 27,
        AddResource = 28,
        UpdateResource = 29
    }
}
