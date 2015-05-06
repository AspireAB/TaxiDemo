module App {
   export class ChatClient {
      private map: google.maps.Map = null;
      private positionDictionary: IPositionMarker[] = []

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


      public setMarker = (regNr: string, position: google.maps.LatLng) => {
         var marker = this.getMarker(regNr);

         marker.marker.setCenter(position);

      }

      public onClick = (position: IPositionMarker) => {
         console.log(position);
      }

      public positionChanged = (objectPosition: IPositionChanged) => {

         var latitude = objectPosition.Latitude;
         var longitude = objectPosition.Longitude;

         var latlng = new google.maps.LatLng(latitude, longitude);

         this.setMarker(objectPosition.RegNr, latlng);

      };

      public getMarker = (regNr: string) => {
         var markers = this.positionDictionary.filter(d => d.id === regNr);
         if (markers.length === 0) {
            var circleOptions = <google.maps.CircleOptions>{
               strokeColor: '#FF0000',
               strokeOpacity: 0.8,
               strokeWeight: 2,
               fillColor: '#FF0000',
               fillOpacity: 0.35,
               map: this.map,
               radius: 20,

            };
            var newMarker = new google.maps.Circle(circleOptions);

            var item = {
               id: regNr,
               marker: newMarker,
            };

            newMarker.addListener('click', () => this.onClick(item));

            this.positionDictionary.push(item);
            return item;
         }
         var positionPair = markers[0];

         return positionPair;
      }

      public statusChanged = (taxiStatus: ITaxiStatus) => {
         var marker = this.getMarker(taxiStatus.RegNr);

         if (taxiStatus.GpsStatus === GpsStatus.inactive) {
            marker.marker.set("fillColor", "#000000");
         } else {
            marker.marker.set("fillColor", '#FF0000');
         }
      }
   }

   export interface IPositionMarker {
      id: string;
      marker: google.maps.Circle;
   }
}