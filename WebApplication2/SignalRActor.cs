using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Taxi.Shared;
using WebApplication2.Hubs;

namespace WebApplication2
{
   public class SignalRActor : ReceiveActor
   {
      public SignalRActor()
      {
         Receive<PositionChanged>(p =>
         {
            var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
            chat.Clients.All.positionChanged(p);
         });
         Receive<TaxiStatus>(status =>
         {
            var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
            chat.Clients.All.statusChanged(status);
         });
      }
   }
}