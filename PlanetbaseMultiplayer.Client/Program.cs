using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client
{
    public class Program
    {
        // only for debug
        public static void Main()
        {
            Client c = new Client();
            c.Start(host: "127.0.0.1", 8080);
            Console.ReadLine();
        }
    }
}
