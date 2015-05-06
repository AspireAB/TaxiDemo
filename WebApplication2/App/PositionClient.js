var App;
(function (App) {
    var ChatClient = (function () {
        function ChatClient() {
            var _this = this;
            this.map = null;
            this.positionDictionary = [];
            this.initMap = function () {
                var mapOptions = {
                    zoom: 13,
                    center: new google.maps.LatLng(59.273525, 15.212679)
                };
                _this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            };
            this.setMarker = function (id, position) {
                var markers = _this.positionDictionary.filter(function (d) {
                    return d.id === id;
                });
                if (markers.length === 0) {
                    var circleOptions = {
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: '#FF0000',
                        fillOpacity: 0.35,
                        map: _this.map,
                        center: position,
                        radius: 20
                    };

                    // Add the circle for this city to the map.
                    var newMarker = new google.maps.Circle(circleOptions);

                    _this.positionDictionary.push({
                        id: id,
                        marker: newMarker
                    });
                } else {
                    var positionPair = markers[0];
                    positionPair.marker.setCenter(position);
                }
            };
            this.positionChanged = function (objectPosition) {
                var latitude = objectPosition.Latitude;
                var longitude = objectPosition.Longitude;

                var latlng = new google.maps.LatLng(latitude, longitude);

                _this.setMarker(objectPosition.Id, latlng);
            };
            this.initMap();
        }
        return ChatClient;
    })();
    App.ChatClient = ChatClient;
})(App || (App = {}));
//# sourceMappingURL=PositionClient.js.map
