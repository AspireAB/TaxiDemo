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
         Receive<Publisher.Position>(p =>
         {
            var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
            chat.Clients.All.positionChanged(p);
         });
         Receive<Taxi.Status>(status =>
         {
            var chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
            chat.Clients.All.statusChanged(status);
         });
      }
   }
}