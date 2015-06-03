using System;
using Akka.Actor;

namespace TaxiHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("TaxiHost"))
            {
                Console.ReadLine();
            }
        }
    }
}