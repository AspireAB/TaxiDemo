using System;
using System.Collections.Generic;
using Akka.Actor;

namespace TaxiShared
{
    public static class Publisher
    {
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
            public Position(double longitude, double latitude, string regNr)
            {
                Longitude = longitude;
                Latitude = latitude;
                RegNr = regNr;
            }

            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
            public string RegNr { get; private set; }
        }
    }

    public class PublisherActor : ReceiveActor
    {
        private readonly Dictionary<string, IActorRef> _regNrToTaxiLookup;
        private IActorRef _presenter;

        public PublisherActor()
        {
            _regNrToTaxiLookup = new Dictionary<string, IActorRef>();

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
                //if the specific taxi does not exist, create it
                EnsureTaxiExists(p.RegNr);

                var actor = _regNrToTaxiLookup[p.RegNr];
                var position = new Taxi.Position(p.Longitude, p.Latitude);
                actor.Tell(position);
            });
        }

        private void EnsureTaxiExists(string regNr)
        {
            if (_regNrToTaxiLookup.ContainsKey(regNr) == false)
            {
                var taxiCarActor = Context.ActorOf(Props.Create(() =>
                    new TaxiActor(_presenter, regNr)));
                _regNrToTaxiLookup.Add(regNr, taxiCarActor);
            }
        }
    }
}