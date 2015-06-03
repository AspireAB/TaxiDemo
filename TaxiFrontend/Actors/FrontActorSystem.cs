using Akka.Actor;

namespace TaxiFrontend.Actors
{
    public class FrontActorSystem
    {
        public static ActorSystem System;
        public static ActorSelection Presenter;
        public static IActorRef SignalRActor;

        static FrontActorSystem()
        {
            System = ActorSystem.Create("TaxiSystem");
            SignalRActor = System.ActorOf(Props.Create(() => new PresentingActor()));
            Presenter = System.ActorSelection("akka.tcp://TaxiBackend@localhost:8080/user/presenter");
        }
    }
}