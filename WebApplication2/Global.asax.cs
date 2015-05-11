using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using TaxiShared;

namespace WebApplication2
{
    public class MvcApplication : HttpApplication
    {
        public static ActorSystem _system;
        public static ActorSelection _publisher;
        public static IActorRef _signalRactor;

        protected void Application_Start()
        {            
            _system = ActorSystem.Create("TaxiSystem");
            _signalRactor = _system.ActorOf(Props.Create(() => new SignalRActor()));
            _publisher = _system.ActorSelection("akka.tcp://TaxiBackend@localhost:8080/user/publisher");
            _publisher.Tell(new Publisher.Initialize(_signalRactor));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}