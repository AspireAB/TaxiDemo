using System;
using System.Collections.Generic;
using System.Collections;
using Akka.Actor;
using Akka.Event;

namespace TaxiShared
{
    public static class Publisher
    {
        public class Sources
        {
            public string[] Items { get; private set; }

            public Sources()
            {

            }
            public Sources(string[] sources)
            {
                Items = sources;
            }
        }
        public class SourceAvailable
        {
            public string SourceName { get; set; }
            public SourceAvailable(string sourceName)
            {
                SourceName = sourceName;
            }
        }
        public class Initialize
        {
            public Initialize(IActorRef presenter)
            {
                Presenter = presenter;
            }

            public IActorRef Presenter { get; private set; }
        }

        public class Position
        {
            public Position(double longitude, double latitude, string regNr, string source)
            {
                Longitude = longitude;
                Latitude = latitude;
                Id = regNr;
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
        private readonly Dictionary<string, Dictionary<string, IActorRef>> _regNrToTaxiLookup;
        private IActorRef _presenter;
        private ILoggingAdapter _log = Context.GetLogger();

        public PublisherActor()
        {
            _regNrToTaxiLookup = new Dictionary<string, Dictionary<string, IActorRef>>();

            Become(Initializing);

        }

        public void Initializing()
        {
            Receive<Publisher.Initialize>(s =>
            {
                Console.WriteLine("Publisher started!");
                _presenter = s.Presenter;
                Become(Active);
            });
        }

        public void Active()
        {
            Receive<Publisher.Sources>(s =>
            {
                var sources = new string[_regNrToTaxiLookup.Keys.Count];
                _regNrToTaxiLookup.Keys.CopyTo(sources, 0);
                Sender.Tell(new Publisher.Sources(sources), Self);
            });
            //remove actors that have died
            Receive<Terminated>(t =>
            {
                if (_regNrToTaxiLookup.ContainsKey(t.ActorRef.Path.Name))
                {
                    _regNrToTaxiLookup.Remove(t.ActorRef.Path.Name);
                }
            });

            //forward positions to taxis
            Receive<Publisher.Position>(p =>
            {
                var actor = GetVehicleBySourceAndId(p.Source, p.Id);
                var position = new Taxi.Position(p.Longitude, p.Latitude);
                actor.Tell(position);
            });
        }

        private IActorRef GetVehicleBySourceAndId(string source, string id)
        {
            if (_regNrToTaxiLookup.ContainsKey(source) == false)
            {
                _regNrToTaxiLookup.Add(source, new Dictionary<string, IActorRef>());
                _presenter.Tell(new Publisher.SourceAvailable(source));
            }
            var dictionary = _regNrToTaxiLookup[source];

            if (dictionary.ContainsKey(id) == false)
            {
                var taxiCarActor = Context.ActorOf(Props.Create(() =>
                    new TaxiActor(_presenter, id, source)));
                dictionary.Add(id, taxiCarActor);
                _log.Info("Creating new Taxi {0}", id);
                _log.Info("Tracking {0} objects", dictionary.Count);
            }
            return dictionary[id];
        }
    }
}