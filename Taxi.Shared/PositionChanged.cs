using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taxi.Shared
{
   public class PositionChanged
   {
      public double Longitude { get; set; }
      public double Latitude { get; set; }
      public string RegNr { get; set; }

      public PositionChanged(double longitude, double latitude, string regNr)
      {
         Longitude = longitude;
         Latitude = latitude;
         RegNr = regNr;
      }
   }
}
