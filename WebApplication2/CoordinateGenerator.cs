using System;
using System.Device.Location;
using System.Threading;
using Akka.Actor;
using GpsSimulator;
using Microsoft.AspNet.SignalR;
using Taxi.Shared;
using WebApplication2.Hubs;

namespace WebApplication2
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