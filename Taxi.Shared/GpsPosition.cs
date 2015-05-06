using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Shared
{
    public class GpsPosition
    {
       public double Longitude { get; set; }
       public double Latitude { get; set; }

       public GpsPosition(double longitude, double latitude)
       {
          Longitude = longitude;
          Latitude = latitude;
       }
    }
}
