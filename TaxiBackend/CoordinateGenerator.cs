using Akka.Actor;
using Taxi.Shared;

namespace TaxiBackend
{
   public class CoordinateGenerator
   {
      public CoordinateGenerator(IActorRef publisher)
      {
         for (int i = 0; i < 100; i++)
         {
            var geoCoordinateSimulator = new GeoCoordinateSimulator(i);
            geoCoordinateSimulator.Start();
            geoCoordinateSimulator.PositionChanged += (sender, args) => publisher.Tell(new PositionChanged(args.Longitude, args.Latitude, args.Id));
         }
      }

   }
}