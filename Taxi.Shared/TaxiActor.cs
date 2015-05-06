using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Shared
{
    public class TaxiActor : ReceiveActor
    {
        private readonly Queue<Position> _positions = new Queue<Position>(); 
        private Position _position;
        private bool _idle;
        private ICancelable _idleTimer;

        public TaxiActor ()
        {

            Receive<Idle>(_ =>
            {
                _idle = true;
                //TODO: notify server?
            });

            Receive<GpsPosition>(p =>
            {                
                _position = new Position()
                {
                    //TODO: uppdatera
                };
                _positions.Enqueue(_position);
                
                if (_idleTimer != null)
                    _idleTimer.Cancel();

                _idleTimer = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromMinutes(1),Self,new Idle(),Self);
                _idle = false;
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
