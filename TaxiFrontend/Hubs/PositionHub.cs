using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using TaxiFrontend.Actors;

namespace TaxiFrontend.Hubs
{
	public class PositionHub : Hub
	{
		public void OnUpdateBounds(PresentingActor.UpdatedBounds updatedBounds)
		{
			updatedBounds.UserId = Context.ConnectionId;
			FrontActorSystem.SignalRActor.Tell(updatedBounds);
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			FrontActorSystem.SignalRActor.Tell(new PresentingActor.Disconnected(Context.ConnectionId));
			return base.OnDisconnected(stopCalled);
		}
	}
}

