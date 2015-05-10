var App;
(function (App) {
    var PositionClient = (function () {
        function PositionClient() {
            var _this = this;
            this.map = null;
            this.positionDictionary = [];
            this.initMap = function () {
                var mapOptions = {
                    zoom: 13,
                    center: new google.maps.LatLng(34.049678, -118.259469) //"Latitude":34.049678,"Longitude":-118.259469
                };
                _this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            };
            this.setMarker = function (regNr, bearing, position) {
                var vehicle = _this.getVehicle(regNr);
                vehicle.setPosition(bearing, position);
            };
            this.positionChanged = function (position) {
                var latlng = new google.maps.LatLng(position.Latitude, position.Longitude);
                var bearing = position.Bearing;
                _this.setMarker(position.RegNr, bearing, latlng);
            };
            this.getVehicle = function (regNr) {
                var markers = _this.positionDictionary.filter(function (d) { return d.id === regNr; });
                if (markers.length === 0) {
                    var item = new App.MovingVehicle(regNr, _this.map);
                    _this.positionDictionary.push(item);
                    return item;
                }
                var positionPair = markers[0];
                return positionPair;
            };
            this.statusChanged = function (taxiStatus) {
                var vehicle = _this.getVehicle(taxiStatus.RegNr);
                vehicle.setStatus(taxiStatus.GpsStatus);
            };
            ko.track(this);
        }
        return PositionClient;
    })();
    App.PositionClient = PositionClient;
})(App || (App = {}));
//# sourceMappingURL=PositionClient.js.map