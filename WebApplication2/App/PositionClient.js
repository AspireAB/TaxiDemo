var App;
(function (App) {
    var PositionClient = (function () {
        function PositionClient() {
            var _this = this;
            this.map = null;
            this.vehicles = [];
            this.searchText = "";
            this.includedStateValues = [1 /* active */];
            this.joinedSources = [];
            this.sources = [];
            this.initialize = function (sources) {
                _this.sources = sources;
            };
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
                //google.maps.event.addListener(this.map, 'idle',() => {
                //    var lat0 = this.map.getBounds().getNorthEast().lat();
                //    var lng0 = this.map.getBounds().getNorthEast().lng();
                //    var lat1 = this.map.getBounds().getSouthWest().lat();
                //    var lng1 = this.map.getBounds().getSouthWest().lng();
                //    console.log("lat0:" + lat0 + " lng0:" + lng0 + "lat1:" + lat1 + " lng1:" + lng1);
                //});
            };
            this.positionChanged = function (position) {
                var latlng = new google.maps.LatLng(position.Latitude, position.Longitude);
                var vehicle = _this.getVehicle(position.RegNr);
                vehicle.setPosition(position.Bearing, latlng);
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
            this.sourceAdded = function (source) {
                _this.sources.push(source);
            };
            this.toggleSource = function (source) {
                var sourceIndex = _this.joinedSources.indexOf(source);
                if (sourceIndex !== -1) {
                    _this.leaveSource(source);
                    _this.joinedSources.splice(sourceIndex, 1);
                }
                else {
                    _this.joinSource(source);
                    _this.joinedSources.unshift(source);
                }
            };
            this.isSourceJoined = function (source) {
                return _this.joinedSources.indexOf(source) !== -1;
            };
            this.joinSource = function (sourceName) {
            };
            this.leaveSource = function (sourceName) {
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