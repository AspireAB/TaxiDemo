namespace Taxi.Shared
{
   public class TaxiStatus
   {
      public GpsStatus GpsStatus { get; set; }
      public string RegNr { get; set; }

      public TaxiStatus(GpsStatus gpsStatus, string regNr)
      {
         GpsStatus = gpsStatus;
         RegNr = regNr;
      }
   }
}