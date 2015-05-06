using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Actor.Dsl;
using Taxi.Shared;

namespace TaxiBackend
{
   public class PublisherActor : ReceiveActor
   {
      private IActorRef presenter;
      private Dictionary<string, IActorRef> dictionary;

      public PublisherActor()
      {
         dictionary = new Dictionary<string, IActorRef>();

         Become(Startup);
      }

      public void Startup()
      {
         Receive<StartupMessage>(s =>
         {
            Console.WriteLine("Igång!");
            presenter = s.Presenter;
            Become(Active);
         });
      }

      public void Active()
      {
         Receive<PositionChanged>(p =>
         {
            if (presenter == null) return;

            if (dictionary.ContainsKey(p.RegNr) == false)
            {
               var taxiCarActor = Context.ActorOf(Props.Create(() =>
                  new TaxiActor(presenter, p.RegNr)));
               dictionary.Add(p.RegNr, taxiCarActor);
            }
            var actor = dictionary[p.RegNr];
            var position = new GpsPosition(p.Longitude, p.Latitude);
            actor.Tell(position);
         });
      }
   }
}