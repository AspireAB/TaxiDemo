using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using TaxiFrontend.Hubs;
using TaxiShared;

namespace TaxiFrontend.Actors
{
	public class PresentingActor : ReceiveActor
	{
		private readonly IHubContext _chat;
		private static readonly Dictionary<string, UpdatedBounds> UserBounds = new Dictionary<string, UpdatedBounds>();

		public PresentingActor()
		{
			_chat = GlobalHost.ConnectionManager.GetHubContext<PositionHub>();
			Receive<Taxi.PositionBearing>(p => PositionChanged(p));
			Receive<Taxi.Status>(status => StatusChanged(status));
			Receive<Publisher.SourceAvailable>(s => SourceChanged(s));
			Receive<UpdatedBounds>(bounds => UserBounds[bounds.UserId] = bounds);
			Receive<Disconnected>(disconnected => UserBounds.Remove(disconnected.UserId));
		}

		private async Task StatusChanged(Taxi.Status status)
		{
			await _chat.Clients.Group(status.Source).statusChanged(status);
		}

		private async Task PositionChanged(Taxi.PositionBearing position)
		{
			var users = FindUsersWithinPosition(position);
			await _chat.Clients.Clients(users).positionChanged(position);
		}
		private async Task SourceChanged(Publisher.SourceAvailable s)
		{
			await _chat.Clients.All.sourceAdded(s.SourceName);
		}

		private List<string> FindUsersWithinPosition(Taxi.PositionBearing position)
		{
			return UserBounds.Where(b => b.Value.Contains(position))
				.Select(b => b.Key)
				.ToList();
		}

		public class Disconnected
		{
			public string UserId { get; private set; }

			public Disconnected(string userId)
			{
				UserId = userId;
			}
		}
		public class UpdatedBounds
		{
			public string UserId { get; set; }
			public UpdatedBounds(double latitudeNorthEast, double longitudeNorthEast, double latitudeSouthWest, double longitudeSouthWest)
			{
				LatitudeNorthEast = latitudeNorthEast;
				LongitudeNorthEast = longitudeNorthEast;
				LatitudeSouthWest = latitudeSouthWest;
				LongitudeSouthWest = longitudeSouthWest;
			}
			public double LatitudeNorthEast { get; private set; }
			public double LongitudeNorthEast { get; private set; }
			public double LatitudeSouthWest { get; private set; }
			public double LongitudeSouthWest { get; private set; }
		}
	}
}