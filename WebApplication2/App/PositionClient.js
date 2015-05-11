var App;
(function (App) {
    var PositionClient = (function () {
        function PositionClient() {
            var _this = this;
            this.map = null;
            this.vehicles = [];
            this.searchText = "";
            this.includedStateValues = [1 /* active */];
            this.toggleStatus = function (status) {
                var index = _this.includedStateValues.indexOf(status);
                if (index === -1) {
                    _this.includedStateValues.push(status);
                }
                else {
                    _this.includedStateValues.splice(index, 1);
                }
            };
            this.isIncluded = function (status) {
                return _this.includedStateValues.indexOf(status) !== -1;
            };
            this.filter = function (vehicle) {
                if (_this.isIncluded(vehicle.status) === false)
                    return false;
                if (_this.searchText.length > 0) {
                    return vehicle.id.indexOf(_this.searchText) !== -1;
                }
                return true;
            };
            this.initMap = function () {
                var mapOptions = {
                    zoom: 13,
                    center: new google.maps.LatLng(34.049678, -118.259469) //"Latitude":34.049678,"Longitude":-118.259469
                };
                _this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
                google.maps.event.addListener(_this.map, 'idle', function () {
                    _this.vehicles.forEach(function (v) {
                        v.viewPortChanged();
                    });
                });
            };
            this.positionChanged = function (position) {
                var latlng = new google.maps.LatLng(position.Latitude, position.Longitude);
                var vehicle = _this.getVehicle(position.RegNr);
                vehicle.setPosition(position.Bearing, latlng, position.GpsStatus);
            };
            //TODO: use dictionary lookup
            this.getVehicle = function (regNr) {
                var markers = _this.vehicles.filter(function (d) { return d.id === regNr; });
                if (markers.length === 0) {
                    var item = new App.MovingVehicle(regNr, _this.map);
                    _this.vehicles.push(item);
                    return item;
                }
                var positionPair = markers[0];
                return positionPair;
            };
            this.statusChanged = function (taxiStatus) {
                var vehicle = _this.getVehicle(taxiStatus.RegNr);
                vehicle.setStatus(taxiStatus.GpsStatus);
            };
            App.track(this);
        }
        Object.defineProperty(PositionClient.prototype, "orderedVehicles", {
            get: function () {
                return this.vehicles.filter(this.filter).sort(function (a, b) { return a.id > b.id ? 1 : -1; });
            },
            enumerable: true,
            configurable: true
        });
        return PositionClient;
    })();
    App.PositionClient = PositionClient;
})(App || (App = {}));
//# sourceMappingURL=PositionClient.js.map