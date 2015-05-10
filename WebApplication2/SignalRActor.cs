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
            chat.Clients.Group(p.Source).positionChanged(p);
         });
         Receive<Taxi.Status>(status =>
         {
            chat.Clients.Group(status.Source).statusChanged(status);
         });
         Receive<Publisher.SourceAvailable>(s =>
         {
             chat.Clients.All.sourceAdded(s);
         });
      }
   }
}