using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Taxi.Shared
{
    public class TaxiActor : ReceiveActor
    {
        private readonly Queue<Position> _positions = new Queue<Position>();
        private bool _idle;

        private readonly ILoggingAdapter _log = Context.GetLogger();

        public TaxiActor (IActorRef reportBackTo,string regNr)
        {
            ICancelable idleTimer = null;

            Receive<Idle>(_ =>
            {
                _idle = true;               
            });

            Receive<GpsPosition>(p =>
            {                
                var position = new Position()
                {
                    //TODO: uppdatera
                };
                _positions.Enqueue(position);
                
                if (idleTimer != null)
                    idleTimer.Cancel();

                idleTimer = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromMinutes(1),Self,new Idle(),Self);
                _idle = false;

                _log.Info("Taxi {0} new position {1} {2}",regNr,position.X,position.Y);

                reportBackTo.Tell(new PositionChanged(position.X,position.Y));
            });
        }
    }

    public class Idle
    {
        
    }

    public class Position
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
    }
}
