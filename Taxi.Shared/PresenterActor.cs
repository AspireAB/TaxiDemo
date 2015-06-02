using System.Collections.Generic;
using Akka.Actor;

namespace TaxiShared
{
    public class PresenterActor : ReceiveActor
    {
        private readonly HashSet<IActorRef> _clients = new HashSet<IActorRef>();

        public PresenterActor()
        {
            Receive<Terminated>(t =>
            {
                _clients.RemoveWhere(c => c.Equals(t.ActorRef));
            });
            Receive<Presenter.Initialize>(i =>
            {
                _clients.Add(i.Client);
                Context.Watch(i.Client);
            });
            ReceiveAny(m =>
            {
                foreach(var client in _clients)
                    client.Tell(m);
            });
        }
    }
}
