using TaxiShared;

namespace TaxiFrontend.Actors
{
	public static class Extensions
	{
        public static bool Contains(this ViewPort viewPort, Taxi.PositionBearing position)
		{
			return viewPort.LatitudeNorthEast > position.Latitude
					 && viewPort.LatitudeSouthWest < position.Latitude
					 && viewPort.LongitudeNorthEast > position.Longitude
					 && viewPort.LongitudeSouthWest < position.Longitude;
		}
	}
}