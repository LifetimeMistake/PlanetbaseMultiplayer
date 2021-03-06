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
        SetSimOwnerStatus = 8,
        IncrementNextId = 9,
        IncrementNextBotId = 10,
        RecycleSelectable = 11,
        CharacterStartWalking = 12,
        RemoveInteraction = 13,
        AddInteraction = 14,
        BuildableBuilt = 15,
        ConstructionSetPriority = 16,
        BuildableSetEnabled = 17,
        DecideNextSandstorm = 18,
        EndSandstorm = 19,
        TriggerSandstorm = 20,
        AddResource = 21,
        UpdateResource = 22,
        AddBuildable = 23,
        UpdateBuildable = 24
    }
}
