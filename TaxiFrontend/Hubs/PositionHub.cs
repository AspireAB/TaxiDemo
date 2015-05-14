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
			FrontActorSystem.Presenter.Tell(updatedBounds);
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			FrontActorSystem.Presenter.Tell(new PresentingActor.Disconnected(Context.ConnectionId));
			return base.OnDisconnected(stopCalled);
		}
	}
}

