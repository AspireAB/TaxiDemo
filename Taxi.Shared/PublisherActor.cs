using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace TaxiShared
{
	public static class Presenter
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
		private readonly Dictionary<string, Dictionary<string, IActorRef>> _idToVehicleLookup;
		private readonly IActorRef _presenter;
		private readonly ILoggingAdapter _log = Context.GetLogger();

        public PublisherActor(IActorRef presenter)
        {
            _presenter = presenter;
            _idToVehicleLookup = new Dictionary<string, Dictionary<string, IActorRef>>();

            Become(Active);
        }

		public void Active()
		{
			Receive<Presenter.Sources>(s =>
			{
				var sources = new string[_idToVehicleLookup.Keys.Count];
				_idToVehicleLookup.Keys.CopyTo(sources, 0);
				Sender.Tell(new Presenter.Sources(sources), Self);
			});
			//remove actors that have died
			Receive<Terminated>(t =>
			{
				if (_idToVehicleLookup.ContainsKey(t.ActorRef.Path.Name))
				{
					_idToVehicleLookup.Remove(t.ActorRef.Path.Name);
				}
			});

			//forward positions to taxis
			Receive<Presenter.Position>(p =>
			{
				var actor = GetVehicleBySourceAndId(p.Source, p.Id);
				var position = new Taxi.Position(p.Longitude, p.Latitude);
				actor.Tell(position);
			});
		}

		private IActorRef GetVehicleBySourceAndId(string source, string id)
		{
			if (_idToVehicleLookup.ContainsKey(source) == false)
			{
				_idToVehicleLookup.Add(source, new Dictionary<string, IActorRef>());
				_presenter.Tell(new Presenter.SourceAvailable(source));
			}
			var dictionary = _idToVehicleLookup[source];

			if (dictionary.ContainsKey(id) == false)
			{
				var taxiCarActor = Context.ActorOf(Props.Create(() => new TaxiActor(_presenter, id, source)));
				dictionary.Add(id, taxiCarActor);
				_log.Info("Creating new Taxi {0}", id);
				_log.Info("Tracking {0} objects", dictionary.Count);
			}
			return dictionary[id];
		}
	}
}