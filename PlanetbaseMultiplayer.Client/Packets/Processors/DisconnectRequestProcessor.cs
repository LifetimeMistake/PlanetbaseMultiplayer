using Planetbase;
using PlanetbaseMultiplayer.Client.UI;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Packets.Processors
{
    public class DisconnectRequestProcessor : PacketProcessor
    {
        public override Type GetProcessedPacketType()
        {
            return typeof(DisconnectRequestPacket);
        }

        public override void ProcessPacket(Guid sourcePlayerId, Packet packet, ProcessorContext context)
        {
            Client client = context.ServiceLocator.LocateService<Client>();
            DisconnectRequestPacket disconnectRequestPacket = (DisconnectRequestPacket)packet;

            void OnExitConfirm(object parameter)
            {
                GameManager.getInstance().setGameStateTitle();
                client.Disconnect();
            }

            GuiDefinitions.Callback callback = new GuiDefinitions.Callback(OnExitConfirm);

            switch (disconnectRequestPacket.Reason)
            {
                case DisconnectReason.DisconnectRequestResponse:
                    Debug.Log("Graceful disconnect response received, disconnecting.");
                    OnExitConfirm(null);
                    break;
                case DisconnectReason.KickedOut:
                    if(!MessageBoxOk.Show(callback, "Disconnected from server", "You have been kicked out of the game."))
                        OnExitConfirm(null); // Failed to show window
                    break;
                case DisconnectReason.ServerClosing:
                    if(!MessageBoxOk.Show(callback, "Disconnected from server", "Server is shutting down."))
                        OnExitConfirm(null); // Failed to show window
                    break;
                default:
                    if (!MessageBoxOk.Show(callback, "Disconnected from server", "Unknown reason."))
                        OnExitConfirm(null); // Failed to show window
                    break;
            }
        }
    }
}
