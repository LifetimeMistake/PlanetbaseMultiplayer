using Lidgren.Network;
using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Server
{
	public class Server
	{
		public NetServer server;
		public List<Player> ConnectedPlayers;
		public string XmlWorld;
		public TimeManager TimeManager;

		public Server()
		{
			XmlWorld = File.ReadAllText("save.sav");

			TimeManager = new TimeManager(this);

			ConnectedPlayers = new List<Player>();
			NetPeerConfiguration config = new NetPeerConfiguration("PlanetbaseMultiplayerMod");
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			config.EnableMessageType(NetIncomingMessageType.Data);
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 8081;
			server = new NetServer(config);
			server.RegisterReceivedCallback(new SendOrPostCallback(MessageReceived));
			server.Start();
			Console.WriteLine($"Server running on port {server.Port}");
		}

		public void MessageReceived(object peerObj)
		{
			NetPeer peer = peerObj as NetPeer;
			NetIncomingMessage msg = peer.ReadMessage();
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.StatusChanged:
					if(msg.SenderConnection.Status == NetConnectionStatus.Disconnected)
					{
						Player dcdplayer = ConnectedPlayers.First(player => player.UniqueId == msg.SenderConnection.RemoteUniqueIdentifier);
						if (dcdplayer == null) return;
						if (dcdplayer.IsSimulationOwner)
							Console.WriteLine($"Client disconnected - ID: {dcdplayer.UniqueId} (simulation owner)");
						else
							Console.WriteLine($"Client disconnected - ID: {dcdplayer.UniqueId}");

						ConnectedPlayers.Remove(dcdplayer);
						if(ConnectedPlayers.Count == 0)
						{
							Console.WriteLine("All players have left. Simulation paused.");
							return;
						}
						if(ConnectedPlayers.Where(player => player.IsSimulationOwner).Count() == 0)
						{
							Player newSimOwner = ConnectedPlayers.First();
							newSimOwner.IsSimulationOwner = true;
							Console.WriteLine($"New simulation owner: {newSimOwner.UniqueId}");
							SendPacket(newSimOwner, new Packet(PacketType.SetSimOwnerStatus, new SetSimOwnerStatusDataPackage(true)));
							return;
						}
					}
					break;
				case NetIncomingMessageType.DiscoveryRequest:
					NetOutgoingMessage response = server.CreateMessage();
					response.Write("PlanetbaseMultiplayer.Server");
					server.SendDiscoveryResponse(response, msg.SenderEndPoint);
					break;
				case NetIncomingMessageType.ConnectionApproval:
					Console.WriteLine($"Client connected - ID: {msg.SenderConnection.RemoteUniqueIdentifier}");
					Player p = new Player(msg.SenderConnection.RemoteUniqueIdentifier, ClientState.ConnectedUnauthorized, (ConnectedPlayers.Count == 0));
					if(p.IsSimulationOwner)
						Console.WriteLine($"New simulation owner: {p.UniqueId}");
					ConnectedPlayers.Add(p);
					msg.SenderConnection.Approve();
					break;
				case NetIncomingMessageType.Data:
					try
					{
						Packet packet = Packet.Deserialize(msg.ReadBytes(msg.LengthBytes));
						ProcessPacket(ConnectedPlayers.First(player => player.UniqueId == msg.SenderConnection.RemoteUniqueIdentifier), packet);
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
					Console.WriteLine($"Unhandled packet received: {msg.MessageType}");
					break;
			}

			server.Recycle(msg);
		}

		public Player GetSimulationOwner()
		{
			return ConnectedPlayers.First(p => p.IsSimulationOwner);
		}

		public void ProcessPacket(Player sender, Packet packet)
		{
			Console.WriteLine($"Receive - Type: {packet.Type}; DataType: {packet.Data}");
			if(packet.Type == PacketType.SetClientState)
			{
				sender.ClientState = ((packet.Data) as ClientStatePackage).State;
				switch(sender.ClientState)
				{
					case ClientState.ConnectedWaitingForAuth:
						Console.WriteLine("Sending config to remote client!");
						SendPacket(sender, new Packet(PacketType.PrepareClient, new PrepareClientPackage(sender)));
						break;
					case ClientState.WaitingForSaveData:
						TimeManager.Pause();
						Task.Run(() =>
						{
							Thread.Sleep(2500);
							if (ConnectedPlayers.Count == 1)
							{
								if (sender.IsSimulationOwner)
								{
									// The simulation owner is not ready. This forces us to use the server's world data.
									Console.WriteLine("Simulation owner not ready. Using server's world state instead");
									SendPacket(sender, new Packet(PacketType.LoadXmlSaveData, new SaveDataPackage(XmlWorld)));
								}
								else
								{
									throw new Exception("Weird situation. Possible desync?");
								}
							}
							else
							{
								// Ask the simulation owner to provide the server with the latest world data.
								Console.WriteLine("Requesting the latest world state from the simulation owner");
								SendPacket(GetSimulationOwner(), new Packet(PacketType.RequestXmlSaveData, null));
							}
						});
						break;
					case ClientState.ConnectedReady:
						// If there are no remaining players in the loading queue, unpause the game
						if (ConnectedPlayers.Where(p => p.ClientState != ClientState.ConnectedReady).Count() == 0)
							TimeManager.Unpause();
						break;
				}
			}
			if(packet.Type == PacketType.LoadXmlSaveData)
			{
				Console.WriteLine("Received a new world state from the simulation owner");
				SaveDataPackage package = packet.Data as SaveDataPackage;
				XmlWorld = package.XmlData;
				File.WriteAllText("received_data.xml", XmlWorld);
				// Forward the data to every player in the loading queue (if there are any)
				foreach (Player player in ConnectedPlayers.Where(p => p.ClientState == ClientState.WaitingForSaveData))
					SendPacket(player, new Packet(PacketType.LoadXmlSaveData, new SaveDataPackage(XmlWorld)));
			}
			if(packet.Type == PacketType.SetGameTimeSpeed)
			{
				// Remote client has requested a game speed change
				if (!sender.IsSimulationOwner)
				{
					Console.WriteLine($"Client {sender.UniqueId} has requested a game speed change and was denied. Reason: Player was not the simulation owner.");
					return;
				}

				if(sender.ClientState != ClientState.ConnectedReady)
				{
					Console.WriteLine($"Client {sender.UniqueId} (simulation owner) has requested a game speed change and was denied. Reason: Client was in an incorrect state. " +
						$"Expected ConnectedReady, but was {sender.ClientState} instead.");
					return;
				}

				GameTimeSpeedPackage pkg = (packet.Data) as GameTimeSpeedPackage;
				TimeManager.SetGameSpeed(pkg.GameSpeed, pkg.Paused);
			}
			if(packet.Type == PacketType.PlaceModule)
			{
				PlaceModuleDataPackage pkg = packet.Data as PlaceModuleDataPackage;
				Console.WriteLine($"Build module: {pkg.ModuleType}, size {pkg.SizeIndex}");
				SendPacketToAll(packet);
			}
			if(packet.Type == PacketType.PlaceConnection)
			{
				PlaceConnectionDataPackage pkg = packet.Data as PlaceConnectionDataPackage;
				Console.WriteLine($"Build connection: Link1_Id: {pkg.Module1_Id}, Link2_Id: {pkg.Module2_Id}");
				SendPacketToAll(packet);
			}
			if(packet.Type == PacketType.PlaceComponent)
			{
				PlaceComponentDataPackage pkg = packet.Data as PlaceComponentDataPackage;
				Console.WriteLine($"Build component: Parent module Id: {pkg.ParentModuleId}, Position: {pkg.Position.ToString()}, Rotation: {pkg.Rotation.ToString()}, ComponentType: {pkg.ComponentType}");
				SendPacketToAll(packet);
			}
			if(packet.Type == PacketType.IncrementNextId)
			{
				SendPacketToAllExcept(sender, packet);
			}
			if(packet.Type == PacketType.IncrementNextBotId)
			{
				SendPacketToAllExcept(sender, packet);
			}
			if(packet.Type == PacketType.ProduceResource)
			{
				SendPacketToAll(packet);
			}
			if(packet.Type == PacketType.RecycleColonyShip)
			{
				SendPacketToAll(packet);
			}
			if(packet.Type == PacketType.RecycleComponent)
			{
				SendPacketToAll(packet);
			}
		}

		public bool SendPacket(Player recipient, Packet packet)
		{
			if (server.Connections.Where(p => p.RemoteUniqueIdentifier == recipient.UniqueId).Count() == 0) { Console.WriteLine("Send error: Recipient not found."); return false; }
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write(packet.Serialize());
			Console.WriteLine($"Send - Type: {packet.Type}; DataType: {packet.Data}");
			server.SendMessage(msg, server.Connections.First(p => p.RemoteUniqueIdentifier == recipient.UniqueId), NetDeliveryMethod.ReliableOrdered);
			return true;
		}

		public bool SendPacketToAll(Packet packet)
		{
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write(packet.Serialize());
			Console.WriteLine($"Send - Type: {packet.Type}; DataType: {packet.Data}");
			server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
			return true;
		}

		public bool SendPacketToAllExcept(Player except, Packet packet)
		{
			if (server.Connections.Where(p => p.RemoteUniqueIdentifier == except.UniqueId).Count() == 0) { Console.WriteLine("Send error: Excluded connection not found."); return false; }
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write(packet.Serialize());
			Console.WriteLine($"Send - Type: {packet.Type}; DataType: {packet.Data}");
			server.SendToAll(msg, server.Connections.First(p => p.RemoteUniqueIdentifier == except.UniqueId), NetDeliveryMethod.ReliableOrdered, 1);
			return true;
		}
	}
}
