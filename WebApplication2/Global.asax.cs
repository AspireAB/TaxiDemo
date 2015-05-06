using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Akka.Actor;
using TaxiBackend;

namespace WebApplication2
{
   public class MvcApplication : HttpApplication
   {
      private ActorSystem mySystem;
      public static IActorRef signalRactor;

      protected void Application_Start()
      {
         mySystem = ActorSystem.Create("TaxiSystem");
         signalRactor = mySystem.ActorOf(Props.Create(() => new SignalRActor()));
         mySystem.ActorSelection("akka.tcp://TaxiBackend@localhost:8080/user/publisher")
            .Tell(new StartupMessage(signalRactor));

         AreaRegistration.RegisterAllAreas();
         RouteConfig.RegisterRoutes(RouteTable.Routes);
      }
   }
}
