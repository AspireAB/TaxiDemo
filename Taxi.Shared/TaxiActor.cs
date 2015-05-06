using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Taxi.Shared
{
   public class TaxiActor : ReceiveActor
   {
      private readonly IActorRef signalR;
      private readonly string regNr;
      ICancelable idleTimer = null;

      public TaxiActor(IActorRef signalR, string regNr)
      {
         this.signalR = signalR;
         this.regNr = regNr;

         Become(Active);
      }

      public void Active()
      {
         Receive<Idle>(_ =>
         {
            Become(Inactive);
            signalR.Tell(new TaxiStatus(GpsStatus.Inactive, regNr));
         });

         Receive<GpsPosition>(p =>
         {
            ScheduleIdleTimer();

            signalR.Tell(new PositionChanged(p.Longitude, p.Latitude, regNr));
         });
      }

      public void Inactive()
      {
         Receive<GpsPosition>(p =>
         {
            Become(Active);
            signalR.Tell(new TaxiStatus(GpsStatus.Active, regNr));
            
            ScheduleIdleTimer();

            signalR.Tell(new PositionChanged(p.Longitude, p.Latitude, regNr));
         });
      }

      private void ScheduleIdleTimer()
      {
         if (idleTimer != null)
            idleTimer.Cancel();

         idleTimer = Context.System.Scheduler
            .ScheduleTellOnceCancelable(TimeSpan.FromSeconds(1), Self, new Idle(), Self);
      }
   }

   public enum GpsStatus
   {
      Inactive = 0,
      Active = 1
   }

   public class Idle
   {

   }
}
