using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace TaxiBackend
{
   class Program
   {
      static void Main(string[] args)
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
