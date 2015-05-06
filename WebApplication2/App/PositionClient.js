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
            this.setMarker = function (regNr, position) {
                var marker = _this.getMarker(regNr);

                marker.marker.setCenter(position);
            };
            this.onClick = function (position) {
                var panoramaOptions = {
                    position: position,
                    pov: {
                        heading: 34,
                        pitch: 10
                    }
                };

                var panorama = new google.maps.StreetViewPanorama(document.getElementById('pano'), panoramaOptions);
                _this.map.setStreetView(panorama);
            };
            this.positionChanged = function (objectPosition) {
                var latitude = objectPosition.Latitude;
                var longitude = objectPosition.Longitude;

                var latlng = new google.maps.LatLng(latitude, longitude);

                _this.setMarker(objectPosition.RegNr, latlng);
            };
            this.getMarker = function (regNr) {
                var markers = _this.positionDictionary.filter(function (d) {
                    return d.id === regNr;
                });
                if (markers.length === 0) {
                    var circleOptions = {
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: '#FF0000',
                        fillOpacity: 0.35,
                        map: _this.map,
                        radius: 20
                    };

                    var newMarker = new google.maps.Circle(circleOptions);
                    newMarker.addListener('click', function () {
                        var position = newMarker.getCenter();
                        _this.onClick(newMarker.getCenter());
                    });
                    var item = {
                        id: regNr,
                        marker: newMarker
                    };
                    _this.positionDictionary.push(item);
                    return item;
                }
                var positionPair = markers[0];

                return positionPair;
            };
            this.statusChanged = function (taxiStatus) {
                var marker = _this.getMarker(taxiStatus.RegNr);

                if (taxiStatus.GpsStatus === 0 /* inactive */) {
                    marker.marker.set("fillColor", "#000000");
                } else {
                    marker.marker.set("fillColor", '#FF0000');
                }
            };
            this.initMap();
        }
        return ChatClient;
    })();
    App.ChatClient = ChatClient;
})(App || (App = {}));
//# sourceMappingURL=PositionClient.js.map
