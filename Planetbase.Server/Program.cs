using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PlanetbaseMultiplayer.Server
{
    class Program
    {
        public static Server ServerInstance;
        static void Main(string[] args)
        {
            ServerInstance = new Server();
            while(true)
            {
                string cmd = Console.ReadLine();
                if(cmd == "save")
                {
                    ServerInstance.Save();
                }
            }
            
        }
    }
}
