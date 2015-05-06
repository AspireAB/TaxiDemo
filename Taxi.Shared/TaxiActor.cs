using System;
using Akka.Actor;

namespace TaxiShared
{
    public static class Taxi
    {
        public class Idle
        {
        }

        public class Position
        {
            public Position(double longitude, double latitude)
            {
                Longitude = longitude;
                Latitude = latitude;
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

        public TaxiActor(IActorRef signalR, string regNr)
        {
            _signalR = signalR;
            _regNr = regNr;

            Become(Active);
        }

        public void Active()
        {
            Receive<Taxi.Idle>(_ =>
            {
                Become(Inactive);
                _signalR.Tell(new Taxi.Status(GpsStatus.Inactive, _regNr));
            });

            Receive<Taxi.Position>(p =>
            {
                ScheduleIdleTimer();

                _signalR.Tell(new Publisher.Position(p.Longitude, p.Latitude, _regNr));
            });
        }

        public void Inactive()
        {
            Receive<Taxi.Position>(p =>
            {
                Become(Active);
                _signalR.Tell(new Taxi.Status(GpsStatus.Active, _regNr));

                ScheduleIdleTimer();

                _signalR.Tell(new Publisher.Position(p.Longitude, p.Latitude, _regNr));
            });
        }

        private void ScheduleIdleTimer()
        {
            if (_idleTimer != null)
                _idleTimer.Cancel();

            _idleTimer = Context.System.Scheduler
                .ScheduleTellOnceCancelable(TimeSpan.FromSeconds(1), Self, new Taxi.Idle(), Self);
        }
    }

    public enum GpsStatus
    {
        Inactive = 0,
        Active = 1
    }
}