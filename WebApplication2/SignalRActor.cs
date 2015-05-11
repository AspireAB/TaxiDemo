using Akka.Actor;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TaxiShared;
using WebApplication2.Hubs;


namespace WebApplication2
{
    public class SignalRActor : ReceiveActor
    {
        private readonly IHubContext _chat;

        public SignalRActor()
        {
            _chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
            Receive<Taxi.PositionBearing>(p => positionChanged(p));
            Receive<Taxi.Status>(status => statusChanged(status));
            Receive<Publisher.SourceAvailable>(s => sourceChanged(s));
        }

        private async Task statusChanged(Taxi.Status status)
        {
            await _chat.Clients.All.statusChanged(status);
        }

        private async Task positionChanged(Taxi.PositionBearing position)
        {
            await _chat.Clients.Group(position.Source).statusChanged(position);
        }
        private async Task sourceChanged(Publisher.SourceAvailable s)
        {
            await _chat.Clients.All.sourceAdded(s.SourceName);
        }
    }
}