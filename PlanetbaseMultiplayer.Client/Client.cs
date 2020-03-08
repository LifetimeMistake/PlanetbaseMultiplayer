using Lidgren.Network;
using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client
{
    public class Client
    {
        public NetClient client;
        public Player localPlayer;
        public ConcurrentQueue<Packet> packetQueue;
#if DEBUG
        public List<string> debug_eventList;
        public int lastTick_PacketCount;
#endif
        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("PlanetbaseMultiplayerMod");
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
#if DEBUG
            debug_eventList = new List<string>();
#endif
            packetQueue = new ConcurrentQueue<Packet>();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(MessageReceived));
            Globals.LocalClient = this;
        }

        public void Start(string host, int port)
        {
            client.Start();
            client.Connect(host: host, port: port);
        }

        public void MessageReceived(object p)
        {
            NetPeer peer = p as NetPeer;
            NetIncomingMessage msg = peer.ReadMessage();
            switch(msg.MessageType)
            {
                case NetIncomingMessageType.DiscoveryResponse:
                    string serverType = msg.ReadString();
                    if(serverType != "PlanetbaseMultiplayer.Server")
                    {
                        Console.WriteLine("Received response was not from a valid server type");
                        return;
                    }
                    Console.WriteLine($"Found server of type {serverType} at {msg.SenderEndPoint}! Connecting!");
                    client.Connect(msg.SenderEndPoint);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    if(client.ConnectionStatus == NetConnectionStatus.Connected)
                    {
                        Console.WriteLine("Client connected!");
                        Globals.IsInMultiplayerMode = true;
                        SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.ConnectedWaitingForAuth)));
                        Globals.LocalPlayer.ClientState = ClientState.ConnectedWaitingForAuth;
                    }
                    if(client.ConnectionStatus == NetConnectionStatus.Disconnected)
                    {
                        Console.WriteLine("Lost connection with the game server!");
                        if(GameManager.getInstance().getGameState() is GameStateGame)
                        {
                            GameStateGame state = GameManager.getInstance().getGameState() as GameStateGame;
                            GuiConfirmWindow window = new GuiConfirmWindow("Lost connection with the game server.", new GuiDefinitions.Callback(state.onExitGameForReal), null, null, 0);
                            state.mGameGui.setWindow(null);
                            state.mGameGui.setWindow(window);
                        }
                    }
                    break;
                case NetIncomingMessageType.Data:
                    try
                    {
                        Packet packet = Packet.Deserialize(msg.ReadBytes(msg.LengthBytes));
                        packetQueue.Enqueue(packet);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while deserializing: {e.Message}");
                    }
                    break;
                case NetIncomingMessageType.DebugMessage:
                    Console.WriteLine($"DEBUG: {msg.ReadString()}");
                    break;
                default:
                    Console.WriteLine($"Unhandled packet received: {msg.MessageType}, Data: {Encoding.UTF8.GetString(msg.ReadBytes(msg.LengthBytes))}");
                    break;
            }

            client.Recycle(msg);
        }

        public void OnExtractResource(int resourceId)
        {
            SendPacket(new Packet(PacketType.ExtractResource, new ExtractResourceDataPackage(resourceId)));
        }

        public void OnCharacterStoreResource(int characterId, int moduleId)
        {
            SendPacket(new Packet(PacketType.CharacterStoreResource, new CharacterStoreResourceDataPackage(characterId, moduleId)));
        }

        public void OnCharacterEmbedResource(int characterId, int componentId, Resource.State resourceState)
        {
            SendPacket(new Packet(PacketType.CharacterEmbedResource, new CharacterEmbedResourceDataPackage(characterId, componentId, resourceState)));
        }

        public void OnCharacterDestroyResource(int characterId)
        {
            SendPacket(new Packet(PacketType.CharacterDestroyResource, new CharacterDestroyResourceDataPackage(characterId)));
        }

        public void OnAddConstructionMaterial(int buildableId, int resourceId)
        {
            SendPacket(new Packet(PacketType.AddConstructionMaterial, new AddConstructionMaterialDataPackage(buildableId, resourceId)));
        }

        public void OnBuildableBuilt(int buildableId)
        {
            SendPacket(new Packet(PacketType.BuildableBuilt, new BuildableBuiltDataPackage(buildableId)));
        }

        public void ProcessPacket(Packet packet)
        {
            if(packet == null) { Console.WriteLine("[WARNING] Attempted to process a null reference packet"); return; }
            //Console.WriteLine($"Receive - Type: {packet.Type}; DataType: {packet.Data}");
#if DEBUG
            Globals.LocalClient.debug_eventList.Add($"Receive - {packet.Type}");
#endif
            if(packet.Type == PacketType.SetSimOwnerStatus)
            {
                if (Globals.LocalPlayer == null) return;
                Globals.LocalPlayer.IsSimulationOwner = (packet.Data as SetSimOwnerStatusDataPackage).IsSimulationOwner;
                Console.WriteLine($"LocalPlayer is sim owner: {Globals.LocalPlayer.IsSimulationOwner}");
            }
            if(packet.Type == PacketType.PrepareClient)
            {
                Globals.LocalPlayer = (packet.Data as PrepareClientPackage).Player;
                localPlayer = Globals.LocalPlayer;
                SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.WaitingForSaveData)));
                Globals.LocalPlayer.ClientState = ClientState.WaitingForSaveData;
                Console.WriteLine("Client prepared for XmlData!");
            }
            if(packet.Type == PacketType.LoadXmlSaveData)
            {
                string file = Path.GetTempFileName();
                SaveDataPackage pkg = packet.Data as SaveDataPackage;
                Globals.IdSync_NextId = pkg.NextId;
                Globals.IdSync_NextBotId = pkg.NextBotId;
                File.WriteAllText(file, pkg.XmlData);
                SaveData save = new SaveData(file, DateTime.Now);
                GameManager.getInstance().setNewState(new GameStateGame(save.getPath(), save.getPlanetIndex(), null));
                SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.LoadingSaveData)));
                Globals.LocalPlayer.ClientState = ClientState.LoadingSaveData;
                UnityEngine.Debug.Log("Loaded remote world data!");
            }
            if(packet.Type == PacketType.SetGameTimeSpeed)
            {
                GameTimeSpeedPackage package = (packet.Data) as GameTimeSpeedPackage;
                TimeManager manager = TimeManager.getInstance();
                manager.mScaleIndex = (int)package.GameSpeed;
                manager.mPaused = package.Paused;
                manager.onTimeScaleChanged();
            }
            if(packet.Type == PacketType.RequestXmlSaveData)
            {
                if (!Globals.LocalPlayer.IsSimulationOwner) return;
                if (!(GameManager.getInstance().getGameState() is GameStateGame)) return;
                GameStateGame gameState = GameManager.getInstance().getGameState() as GameStateGame;
                string XmlData = SerializeGameInMemory.saveGame_toMemory(GameManager.getInstance().getGameState() as GameStateGame);
                SendPacket(new Packet(PacketType.LoadXmlSaveData, new SaveDataPackage(XmlData, IdGenerator.getInstance().mNextId, IdGenerator.getInstance().mNextBotId)));
            }
            if(packet.Type == PacketType.PlaceModule)
            {
                PlaceModuleDataPackage pkg = packet.Data as PlaceModuleDataPackage;
                MultiplayerMethods.PlaceModule(pkg);
            }
            if(packet.Type == PacketType.PlaceConnection)
            {
                PlaceConnectionDataPackage pkg = packet.Data as PlaceConnectionDataPackage;
                MultiplayerMethods.PlaceConnection(pkg);
            }
            if(packet.Type == PacketType.PlaceComponent)
            {
                PlaceComponentDataPackage pkg = packet.Data as PlaceComponentDataPackage;
                MultiplayerMethods.PlaceComponent(pkg);
            }
            if(packet.Type == PacketType.IncrementNextId)
            {
                IdGenerator.getInstance().generate();
            }
            if(packet.Type == PacketType.IncrementNextBotId)
            {
                IdGenerator.getInstance().generateBot();
            }
            if (packet.Type == PacketType.RecycleSelectable)
            {
                RecycleSelectableDataPackage pkg = packet.Data as RecycleSelectableDataPackage;
                MultiplayerMethods.RecycleSelectable(pkg);
            }
            if(packet.Type == PacketType.CharacterStartWalking)
            {
                CharacterStartWalkingDataPackage pkg = packet.Data as CharacterStartWalkingDataPackage;
                MultiplayerMethods.CharacterStartWalking(pkg);
            }
            if (packet.Type == PacketType.CharacterLoadResource)
            {
                CharacterLoadResourceDataPackage pkg = packet.Data as CharacterLoadResourceDataPackage;
                MultiplayerMethods.CharacterLoadResource(pkg);
            }
            if (packet.Type == PacketType.CharacterUnloadResource)
            {
                CharacterUnloadResourceDataPackage pkg = packet.Data as CharacterUnloadResourceDataPackage;
                MultiplayerMethods.CharacterUnloadResource(pkg);
            }

            if (packet.Type == PacketType.AddConstructionMaterial)
            {
                AddConstructionMaterialDataPackage pkg = packet.Data as AddConstructionMaterialDataPackage;
                MultiplayerMethods.AddConstructionMaterial(pkg);
            }
            if (packet.Type == PacketType.CharacterStoreResource)
            {
                CharacterStoreResourceDataPackage pkg = packet.Data as CharacterStoreResourceDataPackage;
                MultiplayerMethods.CharacterStoreResource(pkg);
            }
            if (packet.Type == PacketType.CharacterEmbedResource)
            {
                CharacterEmbedResourceDataPackage pkg = packet.Data as CharacterEmbedResourceDataPackage;
                MultiplayerMethods.CharacterEmbedResource(pkg);
            }
            if (packet.Type == PacketType.CharacterDestroyResource)
            {
                CharacterDestroyResourceDataPackage pkg = packet.Data as CharacterDestroyResourceDataPackage;
                MultiplayerMethods.CharacterDestroyResource(pkg);
            }
            if (packet.Type == PacketType.ExtractResource)
            {
                ExtractResourceDataPackage pkg = packet.Data as ExtractResourceDataPackage;
                MultiplayerMethods.ExtractResource(pkg);
            }
            if (packet.Type == PacketType.BuildableBuilt)
            {
                BuildableBuiltDataPackage pkg = packet.Data as BuildableBuiltDataPackage;
                MultiplayerMethods.BuildableBuilt(pkg);
            }
            if(packet.Type == PacketType.AddInteraction || packet.Type == PacketType.RemoveInteraction)
            {
                Globals.InteractionManager.ProcessPacket(packet);
            }
            if(packet.Type == PacketType.AddResource || packet.Type == PacketType.UpdateResource)
            {
                Globals.ResourceManager.ProcessPacket(packet);
            }
            if (packet.Type == PacketType.BuildableSetEnabled)
            {
                BuildableSetEnabledDataPackage pkg = packet.Data as BuildableSetEnabledDataPackage;
                MultiplayerMethods.BuildableSetEnabled(pkg);
            }
            if (packet.Type == PacketType.ConstructionSetPriority)
            {
                ConstructionSetPriorityDataPackage pkg = packet.Data as ConstructionSetPriorityDataPackage;
                MultiplayerMethods.ConstructionSetPriority(pkg);
            }
            if (packet.Type == PacketType.DecideNextSandstorm)
            {
                DecideNextSandstormDataPackage pkg = packet.Data as DecideNextSandstormDataPackage;
                MultiplayerMethods.DecideNextSandstorm(pkg);
            }
            if (packet.Type == PacketType.EndSandstorm)
            {
                MultiplayerMethods.EndSandstorm();
            }
            if (packet.Type == PacketType.TriggerSandstorm)
            {
                TriggerSandstormDataPackage pkg = packet.Data as TriggerSandstormDataPackage;
                MultiplayerMethods.TriggerSandstorm(pkg);
            }
        }

        public void OnWorldLoadingFinished()
        {
            if (Globals.LocalPlayer.ClientState != ClientState.LoadingSaveData) return;
            if (Globals.IdSync_NextId != IdGenerator.getInstance().mNextId) Globals.IdSyncRequired = true;
            Console.WriteLine(Globals.IdSync_NextId);
            Console.WriteLine("Client started");
            if (Globals.LocalPlayer.IsSimulationOwner)
                Console.WriteLine("IsSimulationOwner");
            if(Globals.IdSyncRequired)
            {
                Console.WriteLine("IdSyncRequired");
                Globals.IdSyncRequired = false;
                Send_IncrementId_Packet();
            }
                
            SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.ConnectedReady)));
            Globals.LocalPlayer.ClientState = ClientState.ConnectedReady;
        }

        public void OnTimeSpeedChanged(GameTimeSpeed speed, bool isPaused)
        {
            SendPacket(new Packet(PacketType.SetGameTimeSpeed, new GameTimeSpeedPackage(isPaused, speed)));
        }

        public void OnModulePlaced(Module module)
        {
            SendPacket(new Packet(PacketType.PlaceModule, new PlaceModuleDataPackage(module.getPosition(), module.getSizeIndex(), module.getModuleType().GetType().Name)));
        }

        public void OnConnectionPlaced(Module m1, Module m2)
        {
            SendPacket(new Packet(PacketType.PlaceConnection, new PlaceConnectionDataPackage(m1.mId, m2.mId)));
        }

        public void OnComponentPlaced(Construction parentConstruction, Vector3 componentPosition, Quaternion componentRotation, string componentType)
        {
            SendPacket(new Packet(PacketType.PlaceComponent, new PlaceComponentDataPackage(parentConstruction.mId, (Quaternion_Serializable)componentRotation,
                (Vector3_Serializable)componentPosition, componentType)));
        }

        public void OnCharacterLoadResource(Character character, Resource resource)
        {
            SendPacket(new Packet(PacketType.CharacterLoadResource, new CharacterLoadResourceDataPackage(character.getId(), resource.getId())));
        }

        public void OnCharacterUnloadResource(Character character, Resource.State resourceState)
        {
            SendPacket(new Packet(PacketType.CharacterUnloadResource, new CharacterUnloadResourceDataPackage(character.getId(), resourceState)));
        }

        public void OnSandstormTrigger(Sandstorm sandstorm)
        {
            SendPacket(new Packet(PacketType.TriggerSandstorm, new TriggerSandstormDataPackage(sandstorm.mSandstormTime)));
        }

        public void OnSandstormEnd()
        {
            SendPacket(new Packet(PacketType.EndSandstorm, null));
        }

        public void OnSandstormDecideNext(Sandstorm sandstorm)
        {
            SendPacket(new Packet(PacketType.DecideNextSandstorm, new DecideNextSandstormDataPackage(sandstorm.mTimeToNextSandstorm)));
        }

        public void OnCharacterStartWalking(Character character, Target target, Selectable[] indirectTargets)
        {
            List<int> indirectTargets_ids = new List<int>();
            if(indirectTargets != null)
            {
                foreach (Selectable selectable in indirectTargets)
                    indirectTargets_ids.Add(selectable.getId());
            }
            
            CharacterStartWalkingDataPackage pkg = new CharacterStartWalkingDataPackage(character.getId(), target.mFlags, target.mRadius, target.mLocation, (Vector3_Serializable)target.mPosition,
                target.mRotation == null ? new Quaternion_Serializable() : (Quaternion_Serializable)target.mRotation, target.mSelectable == null ? -1 : target.mSelectable.getId(), indirectTargets_ids.ToArray(),
                (Vector3_Serializable)character.getPosition(), (Quaternion_Serializable)character.getRotation());
            SendPacket(new Packet(PacketType.CharacterStartWalking, pkg));
        }

        public void OnBuildableEnable(Buildable buildable)
        {
            SendPacket(new Packet(PacketType.BuildableSetEnabled, new BuildableSetEnabledDataPackage(buildable.getId(), true)));
        }
        public void OnBuildableDisable(Buildable buildable)
        {
            SendPacket(new Packet(PacketType.BuildableSetEnabled, new BuildableSetEnabledDataPackage(buildable.getId(), false)));
        }
        public void OnConstructionPriorityUp(Construction construction)
        {
            SendPacket(new Packet(PacketType.ConstructionSetPriority, new ConstructionSetPriorityDataPackage(construction.getId(), true)));
        }
        public void OnConstructionPriorityDown(Construction construction)
        {
            SendPacket(new Packet(PacketType.ConstructionSetPriority, new ConstructionSetPriorityDataPackage(construction.getId(), false)));
        }

        public void OnSelectableRecycled(Selectable selectable)
        {
            SendPacket(new Packet(PacketType.RecycleSelectable, new RecycleSelectableDataPackage(selectable.getId())));
        }
        // A fix for Id desync
        public void Send_IncrementId_Packet()
        {
            SendPacket(new Packet(PacketType.IncrementNextId, null));
        }
        // A fix for Id desync
        public void Send_IncrementBotId_Packet()
        {
            SendPacket(new Packet(PacketType.IncrementNextBotId, null));
        }

        public void SendPacket(Packet packet)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            msg.Write(packet.Serialize());
            //Console.WriteLine($"Send - Type: {packet.Type}; DataType: {packet.Data}");
#if DEBUG
            Globals.LocalClient.debug_eventList.Add($"Send - {packet.Type}");
#endif
            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendDisconnect()
        {
            if (client.ConnectionStatus == NetConnectionStatus.Connected)
                client.Disconnect("bye");
        }
    }
}
