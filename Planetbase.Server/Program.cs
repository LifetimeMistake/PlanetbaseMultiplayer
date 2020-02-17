using PlanetbaseMultiplayer.SharedLibs.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Server
{
    class Program
    {
        public static Server ServerInstance;
        static void Main(string[] args)
        {
            ServerInstance = new Server();
            Console.ReadLine();
        }
    }
}
