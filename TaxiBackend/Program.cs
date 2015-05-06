using System;
using Akka.Actor;
using TaxiShared;

namespace TaxiBackend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("TaxiBackend"))
            {
                var publisher = system.ActorOf(Props.Create(() => new PublisherActor()), "publisher");
                new CoordinateGenerator(publisher);

                Console.ReadLine();
            }
        }
    }
}