using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Akka.Actor;
using Taxi.Shared;

namespace WebApplication2
{
    public class MvcApplication : System.Web.HttpApplication
    {
       private ActorSystem mySystem;
       public static IActorRef signalRactor;

       protected void Application_Start()
        {
           mySystem = ActorSystem.Create("TaxiSystem");
           var hub = "";
           signalRactor = mySystem.ActorOf(Props.Create(() => new SignalRActor()));
          var publisher = mySystem.ActorOf(Props.Create(() => new PublisherActor()));
           var coordinateGenerator = new CoordinateGenerator(publisher);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }

   public class PublisherActor : ReceiveActor
   {
      public PublisherActor()
      {
         var dictionary = new Dictionary<string, IActorRef>();
         Receive<PositionChanged>(p =>
         {
            if (dictionary.ContainsKey(p.RegNr) == false)
            {
               var taxiCarActor = Context.ActorOf(Props.Create(() =>
                  new TaxiActor(MvcApplication.signalRactor, p.RegNr)));
               dictionary.Add(p.RegNr, taxiCarActor);
            }
            var actor = dictionary[p.RegNr];
            var position = new GpsPosition(p.Longitude, p.Latitude);
            actor.Tell(position);
         });
      }
   }

}
