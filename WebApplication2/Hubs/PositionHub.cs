using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TaxiShared;
using Akka.Actor;

namespace WebApplication2.Hubs
{
   public class PositionHub : Hub
   {
       public async Task Init()
       {
           var publisher = MvcApplication._publisher;
           var sources = await publisher.Ask<Publisher.Sources>(new Publisher.Sources());
           await Clients.Caller.initialize(sources.Items);
       }
       public async Task JoinSource(string sourceName)
       {
           await Groups.Add(Context.ConnectionId, sourceName);
       }

       public async Task LeaveSource(string sourceName)
       {
           await Groups.Remove(Context.ConnectionId, sourceName);
       }
   }
}

