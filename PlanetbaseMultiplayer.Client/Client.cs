using Lidgren.Network;
using Planetbase;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client
{
    public class Client
    {
        public NetClient client;
        public ConcurrentQueue<Packet> packetQueue;
        public Client()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("PlanetbaseMultiplayerMod");
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            client = new NetClient(config);
            packetQueue = new ConcurrentQueue<Packet>();
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(MessageReceived));
            Globals.LocalClient = this;
        }

        public void Start()
        {
            client.Start();
            client.DiscoverLocalPeers(8081);
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

        public void ProcessPacket(Packet packet)
        {
            Console.WriteLine($"Receive - Type: {packet.Type}; DataType: {packet.Data}");
            if(packet.Type == PacketType.SetSimOwnerStatus)
            {
                if (Globals.LocalPlayer == null) return;
                Globals.LocalPlayer.IsSimulationOwner = (packet.Data as SetSimOwnerStatusDataPackage).IsSimulationOwner;
                Console.WriteLine($"LocalPlayer is sim owner: {Globals.LocalPlayer.IsSimulationOwner}");
            }
            if(packet.Type == PacketType.PrepareClient)
            {
                Globals.LocalPlayer = (packet.Data as PrepareClientPackage).Player;
                SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.WaitingForSaveData)));
                Globals.LocalPlayer.ClientState = ClientState.WaitingForSaveData;
                Console.WriteLine("Client prepared for XmlData!");
            }
            if(packet.Type == PacketType.LoadXmlSaveData)
            {
                string file = Path.GetTempFileName();
                File.WriteAllText(file, (packet.Data as SaveDataPackage).XmlData);
                SaveData save = new SaveData(file, DateTime.Now);
                while(!(GameManager.getInstance().getGameState() is GameStateTitle)) { Thread.Sleep(100); }
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
                string XmlData = gameState.saveGame_toMemory();
                SendPacket(new Packet(PacketType.LoadXmlSaveData, new SaveDataPackage(XmlData)));
            }
            if(packet.Type == PacketType.PlaceModule)
            {
                // reconstruct module
                PlaceModuleDataPackage pkg = packet.Data as PlaceModuleDataPackage;
                ModuleType mType = null;
                foreach(ModuleType moduleType in TypeList<ModuleType, ModuleTypeList>.get())
                    if(moduleType.getName() == pkg.ModuleType)
                    {
                        mType = moduleType;
                        break;
                    }
                Console.WriteLine("placeModule - found module type");
                if (mType == null) return;
                Console.WriteLine("placeModule - mType != null");
                Module module = Module.create((Vector3)pkg.Position, pkg.SizeIndex, mType);
                Console.WriteLine("placeModule - module created");
                module.setPositionY(Singleton<TerrainGenerator>.getInstance().getFloorHeight());
                Console.WriteLine("placeModule - y position set");
                module.setRenderTop((GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
                Console.WriteLine("placeModule - renderTops flag set");
                module.onUserPlaced();
                Console.WriteLine("placeModule - onUserPlaced called");
            }
            if(packet.Type == PacketType.PlaceConnection)
            {
                PlaceConnectionDataPackage pkg = packet.Data as PlaceConnectionDataPackage;
                // reconstruct modules from Id
                Module m1 = null;
                Module m2 = null;
                foreach(Construction construction in Construction.mConstructions)
                {
                    if (construction.mId == pkg.Module1_Id)
                        m1 = (Module)construction;
                    if (construction.mId == pkg.Module2_Id)
                        m2 = (Module)construction;
                }

                if (m1 == null) { UnityEngine.Debug.LogError("Could not find m1"); return; }
                if (m2 == null) { UnityEngine.Debug.LogError("Could not find m2"); return; }

                Module.linkModules(m1, m2, (GameManager.getInstance().getGameState() as GameStateGame).mRenderTops);
            }
            if(packet.Type == PacketType.PlaceComponent)
            {
                PlaceComponentDataPackage pkg = packet.Data as PlaceComponentDataPackage;
                Construction parentModule = null;
                ComponentType componentType = null;
                foreach(Construction construction in Construction.mConstructions)
                {
                    if (construction.mId == pkg.ParentModuleId)
                    {
                        parentModule = construction;
                        break;
                    }
                }
                foreach (ComponentType cType in TypeList<ComponentType, ComponentTypeList>.get())
                    if (cType.getName() == pkg.ComponentType)
                    {
                        componentType = cType;
                        break;
                    }

                if (parentModule == null) { UnityEngine.Debug.LogError("Error while processing packet type PlaceComponent: parentModule was null"); return; }
                if (componentType == null) { UnityEngine.Debug.LogError("Error while processing packet type PlaceComponent: componentType was null\r\n" +
                    $"Could not find the requested component type: {pkg.ComponentType}"); return; 
                }

                // create the component
                ConstructionComponent component = ConstructionComponent.create(construction: parentModule, componentType: componentType);
                component.setRotation(((Quaternion)pkg.Rotation));
                component.setPosition((Vector3)pkg.Position);
                component.setPositionY(component.getParentConstruction().getFloorPosition().y);
                parentModule.addComponent(component);
                component.onUserPlaced();
            }
            if(packet.Type == PacketType.IncrementNextId)
            {
                IdGenerator.getInstance().generate();
            }
            if(packet.Type == PacketType.IncrementNextBotId)
            {
                IdGenerator.getInstance().generateBot();
            }
        }

        public void OnWorldLoadingFinished()
        {
            if (Globals.LocalPlayer.ClientState != ClientState.LoadingSaveData) return;
            // For some reason the Id is incremented on each load.
            // We must  increment the Id on every other client to stay in sync.
            Send_IncrementId_Packet();
            SendPacket(new Packet(PacketType.SetClientState, new ClientStatePackage(ClientState.ConnectedReady)));
            Globals.LocalPlayer.ClientState = ClientState.ConnectedReady;
        }

        public void OnTimeSpeedChanged_Locally(GameTimeSpeed speed, bool isPaused)
        {
            SendPacket(new Packet(PacketType.SetGameTimeSpeed, new GameTimeSpeedPackage(isPaused, speed)));
        }

        public void OnModulePlaced_Locally(Module module)
        {
            SendPacket(new Packet(PacketType.PlaceModule, new PlaceModuleDataPackage(module.getPosition(), module.getSizeIndex(), module.getName())));
        }

        public void OnConnectionPlaced_Locally(Module m1, Module m2)
        {
            SendPacket(new Packet(PacketType.PlaceConnection, new PlaceConnectionDataPackage(m1.mId, m2.mId)));
        }

        public void OnComponentPlaced_Locally(Construction parentConstruction, Vector3 componentPosition, Quaternion componentRotation, string componentType)
        {
            SendPacket(new Packet(PacketType.PlaceComponent, new PlaceComponentDataPackage(parentConstruction.mId, (Quaternion_Serializable)componentRotation,
                (Vector3_Serializable)componentPosition, componentType)));
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
            Console.WriteLine($"Send - Type: {packet.Type}; DataType: {packet.Data}");
            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
