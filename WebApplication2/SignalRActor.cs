using Akka.Actor;
using Microsoft.AspNet.SignalR;
using TaxiShared;
using WebApplication2.Hubs;


namespace WebApplication2
{
   public class SignalRActor : ReceiveActor
   {
      public SignalRActor()
      {
         var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
         Receive<Taxi.PositionBearing>(p =>
         {
            chat.Clients.All.positionChanged(p);
         });
         Receive<Taxi.Status>(status =>
         {
            chat.Clients.All.statusChanged(status);
         });
      }
   }
}