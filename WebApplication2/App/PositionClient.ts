module App {
   export class ChatClient {
      private map: google.maps.Map = null;
      private positionDictionary: { id: number; marker: google.maps.Circle }[] = []

      constructor() {
         this.initMap();
      }

      public initMap = () => {
         var mapOptions = <google.maps.MapOptions>{
            zoom: 13,
            center: new google.maps.LatLng(59.273525, 15.212679)
         };
         this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
      }

      public setMarker = (id: number, position: google.maps.LatLng) => {
         var markers = this.positionDictionary.filter(d => d.id === id);
         if (markers.length === 0) {
            var circleOptions = <google.maps.CircleOptions>{
               strokeColor: '#FF0000',
               strokeOpacity: 0.8,
               strokeWeight: 2,
               fillColor: '#FF0000',
               fillOpacity: 0.35,
               map: this.map,
               center: position,
               radius: 20
            };
            // Add the circle for this city to the map.
            var newMarker = new google.maps.Circle(circleOptions);

            this.positionDictionary.push({
               id: id,
               marker: newMarker
            });
         } else {
            var positionPair = markers[0];
            positionPair.marker.setCenter(position);
         }
      }

      public positionChanged = (objectPosition: ObjectPosition) => {
         var latitude = objectPosition.Latitude;
         var longitude = objectPosition.Longitude;

         var latlng = new google.maps.LatLng(latitude, longitude);

         this.setMarker(objectPosition.Id, latlng);

      };
   }
}