using PlanetbaseMultiplayer.SharedLibs;
using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;

namespace PlanetbaseMultiplayer.Server
{
    public class TimeManager
    {
        public GameTimeSpeed GameSpeed;
        public bool Paused;
        private Server server;
        public TimeManager(Server s)
        {
            server = s ?? throw new ArgumentNullException(nameof(s));
            Paused = false;
            GameSpeed = GameTimeSpeed.Normal;
        }
        public void SetGameSpeed(GameTimeSpeed speed, bool paused)
        {
            Console.WriteLine($"World simulation speed changed: {GameSpeed} => {speed}");
            GameSpeed = speed;
            Paused = paused;
            onGameSpeedChanged();
        }
        public void SetGameSpeed(GameTimeSpeed speed)
        {
            Console.WriteLine($"World simulation speed changed: {GameSpeed} => {speed}");
            GameSpeed = speed;
            onGameSpeedChanged();
        }
        public void Pause()
        {
            Paused = true;
            Console.WriteLine("World simulation paused.");
            onGameSpeedChanged();
        }
        public void Unpause()
        {
            Paused = false;
            Console.WriteLine("World simulation unpaused.");
            onGameSpeedChanged();
        }
        private void onGameSpeedChanged()
        {
            Packet setGameSpeedPacket = new Packet(PacketType.SetGameTimeSpeed, new GameTimeSpeedPackage(Paused, GameSpeed));
            server.SendPacketToAll(setGameSpeedPacket);
        }
    }
}