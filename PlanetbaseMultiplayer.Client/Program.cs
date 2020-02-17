using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.Client
{
    public class Program
    {
        public static void Main()
        {
            Client c = new Client();
            c.Start();
            Console.ReadLine();
        }
    }
}
