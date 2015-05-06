using System.Device.Location;
using System.Threading;
using GpsSimulator;
using Microsoft.AspNet.SignalR;
using WebApplication2.Hubs;

namespace WebApplication2
{
   public class CoordinateGenerator
   {
      private GeoCoordinateSimulator geoCoordinateSimulator;
     
      public CoordinateGenerator()
      {
         for (int i = 0; i < 100; i++)
         {
            var geoCoordinateSimulator = new GeoCoordinateSimulator(i);
            geoCoordinateSimulator.Start();
            geoCoordinateSimulator.PositionChanged += (sender, args) => OnPositionChanged(args);
            Thread.Sleep(50);
         }
      }

      private void OnPositionChanged(ObjectPosition objectPosition)
      {
         var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
         chat.Clients.All.positionChanged(objectPosition);
      }
   }
}