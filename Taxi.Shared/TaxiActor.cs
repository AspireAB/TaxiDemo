using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using NExtra.Geo;

namespace TaxiShared
{
	public static class Taxi
	{
		public class Idle
		{
		}

		public class PositionBearing
		{
			public PositionBearing(double longitude, double latitude, double bearing, GpsStatus status, string id, string source)
			{
				Bearing = bearing;
				Latitude = latitude;
				Longitude = longitude;
				Id = id;
				GpsStatus = status;
				Source = source;
			}

			public double Longitude { get; set; }
			public double Latitude { get; set; }
			public double Bearing { get; set; }
			public string Id { get; set; }
			public string Source { get; set; }

			public GpsStatus GpsStatus { get; set; }
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
				return Equals((Position)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
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
	}

	public class TaxiActor : ReceiveActor
	{
		private const int TailLength = 20;
		private readonly string _id;
		private readonly string _source;
		private readonly IActorRef _presenter;
		private ICancelable _idleTimer;
		private readonly Queue<Taxi.Position> _positions = new Queue<Taxi.Position>();

		public TaxiActor(IActorRef presenter, string id, string source)
		{
			//HACK: status kan inte sättas innan pos
			//   _presenter.Tell(new Publisher.Position(0,0, _regNr));
			_presenter = presenter;
			_id = id;
			_source = source;

			Become(Disconnected);
		}

		public void Driving()
		{
			ReceiveIdle();

			Receive<Taxi.Position>(p =>
			{
                ScheduleIdleTimer();
				RememberPosition(p);
				//TODO: this makes all vehicles become parked the first tick
				if (_positions.All(p2 => p2 == p))
				{
                    _presenter.Tell(new Taxi.PositionBearing(p.Longitude, p.Latitude, Bearing(), GpsStatus.Parked, _id, _source));
					Become(Parked);
				}
				else
				{
                    _presenter.Tell(new Taxi.PositionBearing(p.Longitude, p.Latitude, Bearing(), GpsStatus.Active, _id, _source));
				}
			});
		}

		private void RememberPosition(Taxi.Position p)
		{
			_positions.Enqueue(p);
			if (_positions.Count > TailLength)
			{
				_positions.Dequeue();
			}
		}

		private void ReceiveIdle()
		{
			Receive<Taxi.Idle>(_ =>
			{
				Become(Disconnected);
			});
		}

		public void Parked()
		{
			ReceiveIdle();

			Receive<Taxi.Position>(p =>
			{
                ScheduleIdleTimer();
				RememberPosition(p);
			    if (_positions.Any(p2 => p2 != p))
			    {
			        _presenter.Tell(new Taxi.PositionBearing(p.Longitude, p.Latitude, Bearing(), GpsStatus.Active, _id, _source));
			        Become(Driving);
			    }
			    else
			    {
			        _presenter.Tell(new Taxi.PositionBearing(p.Longitude, p.Latitude, Bearing(), GpsStatus.Parked, _id, _source));
			    }
			});
		}

		public void Disconnected()
		{
			Receive<Taxi.Position>(p =>
			{
                ScheduleIdleTimer();
				Become(Driving);

				//we are waking up from a period of silence
				_positions.Clear();

				

                //tell the world we are not driving again
				_presenter.Tell(new Taxi.PositionBearing(p.Longitude, p.Latitude, Bearing(), GpsStatus.Active, _id, _source));
			});
		}

		private void ScheduleIdleTimer()
		{
			if (_idleTimer != null)
				_idleTimer.Cancel();

			_idleTimer = Context.System.Scheduler
				 .ScheduleTellOnceCancelable(TimeSpan.FromSeconds(7), Self, new Taxi.Idle(), Self);
		}

		private double Bearing()
		{
			if (_positions.Count < 2)
				return 0;

			var lasts = _positions.Take(TailLength / 2).ToList();
			var firsts = _positions.Skip(lasts.Count).Take(TailLength / 2).ToList();
			var p1 = new Position(lasts.Sum(p => p.Latitude) / lasts.Count, lasts.Sum(p => p.Longitude) / lasts.Count);
			var p2 = new Position(firsts.Sum(p => p.Latitude) / firsts.Count, firsts.Sum(p => p.Longitude) / firsts.Count);

			//   var p2 = _positions.Last();
			//   var p1 = _positions.First();
			var c = new PositionBearingCalculator(new AngleConverter());
			var bearing = c.CalculateBearing(new Position(p1.Latitude, p1.Longitude), new Position(p2.Latitude, p2.Longitude));
			return bearing;
		}


	}

	public enum GpsStatus
	{
		Inactive = 0,
		Active = 1,
		Parked = 2,
	}


}

//http://stackoverflow.com/questions/6800613/rotating-image-marker-image-on-google-map-v3
/*
 // Convert to radians.
        var lat1 = from.latRadians();
        var lon1 = from.lngRadians();
        var lat2 = to.latRadians();
        var lon2 = to.lngRadians();
        // Compute the angle.
        var angle = - Math.atan2( Math.sin( lon1 - lon2 ) * Math.cos( lat2 ), Math.cos( lat1 ) * Math.sin( lat2 ) - Math.sin( lat1 ) * Math.cos( lat2 ) * Math.cos( lon1 - lon2 ) );
        if ( angle < 0.0 )
            angle  += Math.PI * 2.0;
        if (angle == 0) {angle=1.5;}
*/