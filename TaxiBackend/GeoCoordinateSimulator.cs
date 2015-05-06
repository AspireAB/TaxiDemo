// GpsCoordinateSimulator.cs:
// WPF: Requires reference to System.Device.dll

using System;
using System.Device.Location;
using System.Timers;
using System.Windows;

namespace TaxiBackend
{
   /// <summary>
   /// GeoPositionWatcher for more accurately simulating a GPS,
   /// including change in accuracy, heading, and speed.
   /// </summary>
   public class GeoCoordinateSimulator
   {
      private readonly int id;
      private static Random randomizer = new Random();
      private Timer timer;
      DateTime dateTime = DateTime.Now;

      /// <summary>
      /// Initializes a new instance of the <see cref="GeoCoordinateSimulator" /> class.
      /// </summary>
      public GeoCoordinateSimulator(int id)
      {
         this.id = id;
         timer = new Timer()
         {
            Interval = 100
         };
         timer.Elapsed += timer_Tick;
         Status = GeoPositionStatus.NoData;
         // Default start location
         StartLatitude = 59.273525;
         StartLongitude = 15.212679;
      }

      /// <summary>
      /// Called when the position timer triggers, and calculates the next position based on current speed and heading,
      /// and adds a little randomization to current heading, speed and accuracy.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void timer_Tick(object sender, EventArgs e)
      {

         Status = GeoPositionStatus.Ready;
         var oldPosition = Position;
         if (oldPosition == null)
         {
            oldPosition = new GeoPosition<GeoCoordinate>(
               new DateTimeOffset(DateTime.Now.AddSeconds(1)), new GeoCoordinate()
               {
                  Latitude = StartLatitude,
                  Longitude = StartLongitude,
                  Altitude = StartAltitude,
                  Speed = 50,
                  Course = 0,
                  HorizontalAccuracy = 20,
                  VerticalAccuracy = 0
               });

         }
         var now = DateTime.Now;
         TimeSpan timeParsed = oldPosition.Timestamp - now;
         double acceleration = randomizer.NextDouble() * 5 - 2.5;
         double deltaSpeed = acceleration * timeParsed.TotalSeconds;
         double newSpeed = Math.Max(0, deltaSpeed + oldPosition.Location.Speed);
         double deltaCourse = randomizer.NextDouble() * 30 - 15;
         double newCourse = deltaCourse + oldPosition.Location.Course;
         while (newCourse < 0) newCourse += 360;
         while (newCourse >= 360) newCourse -= 360;
         double distanceTravelled = (newSpeed + oldPosition.Location.Speed) * .5 * timeParsed.TotalSeconds;
         double accuracy = Math.Min(500, Math.Max(20, oldPosition.Location.HorizontalAccuracy + (randomizer.NextDouble() * 100 - 50)));
         var pos = GetPointFromHeadingGeodesic(new Point(oldPosition.Location.Longitude, oldPosition.Location.Latitude), distanceTravelled, newCourse - 180);
         var newPosition = new GeoPosition<GeoCoordinate>(
            new DateTimeOffset(now), new GeoCoordinate()
            {
               Latitude = pos.Y,
               Longitude = pos.X,
               Altitude = oldPosition.Location.Altitude + oldPosition.Location.Altitude + randomizer.NextDouble() * 20,
               Speed = newSpeed,
               Course = newCourse,
               HorizontalAccuracy = accuracy,
               VerticalAccuracy = randomizer.NextDouble() * 300,
            });
         Position = newPosition;
      }

      /// <summary>
      /// Gets a point on the globe based on a location, a heading and a distance.
      /// </summary>
      /// <param name="start">The start.</param>
      /// <param name="distance">The distance.</param>
      /// <param name="heading">The heading.</param>
      /// <returns></returns>
      private static Point GetPointFromHeadingGeodesic(Point start, double distance, double heading)
      {
         double brng = heading / 180 * Math.PI;
         double lon1 = start.X / 180 * Math.PI;
         double lat1 = start.Y / 180 * Math.PI;
         double dR = distance / 6378137; //Angular distance in radians
         double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
         double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
         double lon = lon2 / Math.PI * 180;
         double lat = lat2 / Math.PI * 180;
         while (lon < -180) lon += 360;
         while (lat < -90) lat += 180;
         while (lon > 180) lon -= 360;
         while (lat > 90) lat -= 180;
         return new Point(lon, lat);
      }

      /// <summary>
      /// Gets or sets the start latitude.
      /// </summary>
      /// <value>
      /// The start latitude.
      /// </value>
      public double StartLatitude { get; set; }

      /// <summary>
      /// Gets or sets the start longitude.
      /// </summary>
      /// <value>
      /// The start longitude.
      /// </value>
      public double StartLongitude { get; set; }

      /// <summary>
      /// Gets or sets the start altitude.
      /// </summary>
      /// <value>
      /// The start altitude.
      /// </value>
      public double StartAltitude { get; set; }

      #region IGeoPositionWatcher<GeoCoordinate>

      /// <summary>
      /// Initiate the acquisition of location data.
      /// </summary>
      public void Start()
      {
         Start(true);
      }

      /// <summary>
      /// Start acquiring location data, specifying whether or not to suppress prompting
      /// for permissions. This method returns synchronously.
      /// </summary>
      /// <param name="suppressPermissionPrompt">
      /// If true, do not prompt the user to enable location providers and only start
      /// if location data is already enabled. If false, a dialog box may be displayed
      /// to prompt the user to enable location sensors that are disabled.
      /// </param>
      public void Start(bool suppressPermissionPrompt)
      {
         TryStart(true, TimeSpan.MaxValue);
      }

      /// <summary>
      /// Start acquiring location data, specifying an initialization timeout. This
      /// method returns synchronously.
      /// </summary>
      /// <param name="suppressPermissionPrompt">
      /// If true, do not prompt the user to enable location providers and only start
      /// if location data is already enabled. If false, a dialog box may be displayed
      /// to prompt the user to enable location sensors that are disabled.
      /// </param>
      /// <param name="timeout">Time in milliseconds to wait for initialization to complete.</param>
      /// <returns>true if succeeded, false if timed out.</returns>
      public bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout)
      {
         timer.Start();
         Status = GeoPositionStatus.Initializing;
         return true;
      }

      /// <summary>
      /// Stop acquiring location data.
      /// </summary>
      public void Stop()
      {
         timer.Stop();
         Status = GeoPositionStatus.NoData;
         Position = null;
      }

      private GeoPosition<GeoCoordinate> m_Position;

      public GeoPosition<GeoCoordinate> Position
      {
         get { return m_Position; }
         private set
         {
            if (m_Position != value)
            {
               if (dateTime < DateTime.Now)
               {
                  if (randomizer.Next(100) == 1)
                  {
                     dateTime = DateTime.Now.AddSeconds(randomizer.Next(5, 10));
                  }

                  m_Position = value;
                  if (PositionChanged != null)
                     PositionChanged(this, new ObjectPosition(id.ToString(), m_Position.Location));
               }
            }
         }
      }

      public event EventHandler<ObjectPosition> PositionChanged;

      private GeoPositionStatus m_Status;

      public GeoPositionStatus Status
      {
         get { return m_Status; }
         set
         {
            if (m_Status != value)
            {
               m_Status = value;
               if (StatusChanged != null)
                  StatusChanged(this, new GeoPositionStatusChangedEventArgs(value));
            }
         }
      }

      public event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;
      #endregion
   }

   public class ObjectPosition
   {
      public ObjectPosition(string id, GeoCoordinate position)
      {
         Id = id;

         Latitude = position.Latitude;
         Longitude = position.Longitude;
      }

      public double Longitude { get; set; }

      public double Latitude { get; set; }

      public string Id { get; set; }
   }
}