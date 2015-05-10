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
        private ActorSystem _system;

        protected void Application_Start()
        {            
            _system = ActorSystem.Create("TaxiSystem");
            var signalRactor = _system.ActorOf(Props.Create(() => new SignalRActor()));
            _system.ActorSelection("akka.tcp://TaxiBackend@localhost:8080/user/publisher")
                .Tell(new Publisher.Initialize(signalRactor));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}