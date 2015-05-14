using Akka.Actor;

namespace TaxiFrontend.Actors
{
	public class FrontActorSystem
	{
		public static ActorSystem System;
		public static ActorSelection Publisher;
		public static IActorRef Presenter;

		static FrontActorSystem()
		{
			System = ActorSystem.Create("TaxiSystem");
			Presenter = System.ActorOf(Props.Create(() => new PresentingActor()));
			Publisher = System.ActorSelection("akka.tcp://TaxiBackend@localhost:8080/user/publisher");
		}
	}
}