using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace TaxiShared
{
    public static class Taxi
    {
        public class Idle
        {
        }

        public class Position : IEquatable<Position>
        {
            public Position(double longitude, double latitude)
            {
                Longitude = longitude;
                Latitude = latitude;
            }

            public bool Equals(Position other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Position) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Latitude.GetHashCode()*397) ^ Longitude.GetHashCode();
                }
            }

            public static bool operator ==(Position left, Position right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Position left, Position right)
            {
                return !Equals(left, right);
            }

            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
        }

        public class Status
        {
            public GpsStatus GpsStatus { get; set; }
            public string RegNr { get; set; }

            public Status(GpsStatus gpsStatus, string regNr)
            {
                GpsStatus = gpsStatus;
                RegNr = regNr;
            }
        }
    }



    public class TaxiActor : ReceiveActor
    {
        private readonly string _regNr;
        private readonly IActorRef _signalR;
        private ICancelable _idleTimer;
        private readonly Queue<Taxi.Position> _positions = new Queue<Taxi.Position>();

        public TaxiActor(IActorRef signalR, string regNr)
        {
            _signalR = signalR;
            _regNr = regNr;

            Become(Disconnected);
        }

        public void Driving()
        {
            _signalR.Tell(new Taxi.Status(GpsStatus.Active, _regNr));

            ReceiveIdle();

            Receive<Taxi.Position>(p =>
            {
                RememberPosition(p);
                if (_positions.All(p2 => p2 == p))
                {
                    Become(Parked);
                }

                ScheduleIdleTimer();

                _signalR.Tell(new Publisher.Position(p.Longitude, p.Latitude, _regNr));
            });
        }

        private void RememberPosition(Taxi.Position p)
        {
            _positions.Enqueue(p);
            if (_positions.Count > 10)
            {
                _positions.Dequeue();
            }
        }

        private void ReceiveIdle()
        {
            Receive<Taxi.Idle>(_ => {
                Become(Disconnected);
            });
        }

        public void Parked()
        {
            _signalR.Tell(new Taxi.Status(GpsStatus.Parked, _regNr));

            ReceiveIdle();

            Receive<Taxi.Position>(p =>
            {
                RememberPosition(p);
                if (_positions.Any(p2 => p2 != p))
                {
                    Become(Driving);
                }

                ScheduleIdleTimer();

                _signalR.Tell(new Publisher.Position(p.Longitude, p.Latitude, _regNr));
            });
        }

        public void Disconnected()
        {
            _signalR.Tell(new Taxi.Status(GpsStatus.Inactive, _regNr));

            Receive<Taxi.Position>(p =>
            {
                Become(Driving);
                
                //we are waking up from a period of silence
                _positions.Clear();

                ScheduleIdleTimer();
                
                _signalR.Tell(new Publisher.Position(p.Longitude, p.Latitude, _regNr));
            });
        }

        private void ScheduleIdleTimer()
        {
            if (_idleTimer != null)
                _idleTimer.Cancel();

            _idleTimer = Context.System.Scheduler
                .ScheduleTellOnceCancelable(TimeSpan.FromMinutes(1), Self, new Taxi.Idle(), Self);
        }
    }

    public enum GpsStatus
    {
        Inactive = 0,
        Active = 1,
        Parked = 2,
    }
}