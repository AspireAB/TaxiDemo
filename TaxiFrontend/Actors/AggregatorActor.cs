using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using TaxiShared;

namespace TaxiFrontend.Actors
{
    public class AggregatedData
    {
        public AggregatedData(int vehicleCount, double latitude, double longitude)
        {
            VehicleCount = vehicleCount;
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public int VehicleCount { get; private set; }
    }

    public class AggregatorActor : ReceiveActor
    {
        public AggregatorActor(IActorRef presenter)
        {
            var set = new Dictionary<string, Taxi.PositionBearing>();
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(5),Self,"foo",Self  );

            Receive<string>(_ =>
            {
                var groups = set.Values.GroupBy(p => new
                {
                    Longitude = 0.5 + (int) p.Longitude,
                    Latitude = 0.5 + (int)p.Latitude
                }).ToList();

                foreach (var group in groups)
                {
                    presenter.Tell(new AggregatedData(group.Count(),group.Key.Latitude,group.Key.Longitude));
                }

                set.Clear();
            });
            Receive<Taxi.PositionBearing>(p =>
            {
                set.Remove(p.Id);
                set.Add(p.Id,p);
            });
        }
    }
}