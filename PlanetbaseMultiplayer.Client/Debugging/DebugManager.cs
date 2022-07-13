using PlanetbaseMultiplayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Debugging
{
    public class DebugManager : IManager
    {
        private Client client;
        private List<string> debugLog;
        private int logCapacity;
        public bool IsInitialized { get; private set; }
        public DebugManager(Client client)
        {
            this.client = client;
            this.debugLog = new List<string>();
            this.logCapacity = 50;
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public int GetLogCapacity()
        {
            return logCapacity;
        }

        public void SetLogCapacity(int capacity)
        {
            logCapacity = capacity;
        }

        public void AddMessage(string message)
        {
            while (debugLog.Count > logCapacity + 1)
                debugLog.RemoveAt(0);

            debugLog.Add(message);
        }

        public List<string> GetLog()
        {
            return debugLog;
        }
    }
}
