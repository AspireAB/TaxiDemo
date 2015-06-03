using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

namespace TaxiShared
{
    public static class Presenter
    {
        public class Sources
        {
            public Sources()
            {
            }

            public Sources(string[] sources)
            {
                Items = sources;
            }

            public string[] Items { get; private set; }
        }

        public class SourceAvailable
        {
            public SourceAvailable(string sourceName)
            {
                SourceName = sourceName;
            }

            public string SourceName { get; set; }
        }

        public class Initialize
        {
            public Initialize(IActorRef client)
            {
                Client = client;
            }

            public IActorRef Client { get; private set; }
        }

        public class Position
        {
            public Position(double longitude, double latitude, string id, string source)
            {
                Longitude = longitude;
                Latitude = latitude;
                Id = id;
                Source = source;
            }

            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
            public string Id { get; private set; }
            public string Source { get; private set; }
        }
    }

    public class PublisherActor : ReceiveActor
    {
        private readonly Dictionary<string, IActorRef> _idToVehicleLookup;
        private readonly ILoggingAdapter _log = Context.GetLogger(new SerilogLogMessageFormatter());
        private readonly IActorRef _presenter;

        public PublisherActor(IActorRef presenter)
        {
            _presenter = presenter;
            _idToVehicleLookup = new Dictionary<string, IActorRef>();

            Become(Active);
        }

        public void Active()
        {
            Receive<Presenter.Sources>(s =>
            {
                var sources = new string[_idToVehicleLookup.Keys.Count];
                _idToVehicleLookup.Keys.CopyTo(sources, 0);
                Sender.Tell(new Presenter.Sources(new[] {"Vehicles"}), Self);
            });

            //remove actors that have died
            Receive<Terminated>(t =>
            {
                var key = _idToVehicleLookup.FirstOrDefault(_ => _.Value.Equals(t.ActorRef)).Key;
                _idToVehicleLookup.Remove(key);
                _log.Info("Creating new Taxi {VehicleId}", key);
                _log.Info("Tracking {VehicleCount} objects", _idToVehicleLookup.Count);
            });

            //forward positions to taxis
            Receive<Presenter.Position>(p =>
            {
                var id = p.Id;
                if (_idToVehicleLookup.ContainsKey(id) == false)
                {
                    var taxiCarActor = Context.ActorOf(Props.Create(() => new TaxiActor(_presenter, id, p.Source)));
                    _idToVehicleLookup.Add(id, taxiCarActor);
                    _log.Info("Creating new Taxi {VehicleId} from source {VehicleSource}", id, p.Source);
                    _log.Info("Tracking {VehicleCount} objects", _idToVehicleLookup.Count);
                }
                var actor = _idToVehicleLookup[id];
                var position = new Taxi.Position(p.Longitude, p.Latitude);
                actor.Tell(position);
            });
        }
    }
}