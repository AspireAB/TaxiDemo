module App {
   export class PositionClient {
      private map: google.maps.Map = null;
      private positionDictionary: MovingVehicle[] = []

      constructor() {
          ko.track(this);
      }

      public initMap = () => {
         var mapOptions = <google.maps.MapOptions>{
            zoom: 13,
            center: new google.maps.LatLng(34.049678, -118.259469) //"Latitude":34.049678,"Longitude":-118.259469
         };
         this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
      }

      public setMarker = (regNr: string,bearing:number, position: google.maps.LatLng) => {
         var vehicle = this.getVehicle(regNr);
         vehicle.setPosition(bearing,position);
      }

      public positionChanged = (position: IPositionChanged) => {
         var latlng = new google.maps.LatLng(position.Latitude, position.Longitude);
          var bearing = position.Bearing;
         this.setMarker(position.RegNr, bearing, latlng);
      };

      public getVehicle = (regNr: string) => {
         var markers = this.positionDictionary.filter(d => d.id === regNr);
         if (markers.length === 0) {
            
            var item = new MovingVehicle(regNr, this.map);

            this.positionDictionary.push(item);
            return item;
         }
         var positionPair = markers[0];

         return positionPair;
      }

      public statusChanged = (taxiStatus: ITaxiStatus) => {
         var vehicle = this.getVehicle(taxiStatus.RegNr);
         vehicle.setStatus(taxiStatus.GpsStatus);
      }
   }
}