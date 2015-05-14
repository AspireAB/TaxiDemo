using TaxiShared;

namespace TaxiFrontend.Actors
{
	public static class Extensions
	{
		public static bool Contains(this PresentingActor.UpdatedBounds bounds, Taxi.PositionBearing position)
		{
			return bounds.LatitudeNorthEast > position.Latitude
					 && bounds.LatitudeSouthWest < position.Latitude
					 && bounds.LongitudeNorthEast > position.Longitude
					 && bounds.LongitudeSouthWest < position.Longitude;
		}
	}
}