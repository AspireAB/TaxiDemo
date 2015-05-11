var App;
(function (App) {
    var MovingVehicle = (function () {
        function MovingVehicle(id, map) {
            var _this = this;
            this.id = id;
            this.map = map;
            this.status = null;
            this.positions = [];
            this.drawingLine = false;
            this.showingInfo = false;
            this.expanded = false;
            //TODO: obsolete this.. send all state in position msg instead
            this.setStatus = function (status) {
                _this.status = status;
                switch (status) {
                    case 1 /* active */:
                        _this.icon.fillColor = "#00FF00";
                        _this.icon.path = google.maps.SymbolPath.FORWARD_CLOSED_ARROW;
                        _this.marker.set("icon", _this.icon);
                        break;
                    case 0 /* inactive */:
                        _this.icon.fillColor = "#FF0000";
                        _this.icon.path = google.maps.SymbolPath.CIRCLE;
                        _this.marker.set("icon", _this.icon);
                        break;
                    case 2 /* parked */:
                        _this.icon.fillColor = "#0000FF";
                        _this.icon.path = google.maps.SymbolPath.CIRCLE;
                        _this.marker.set("icon", _this.icon);
                        break;
                }
            };
            this.setPosition = function (bearing, position, status) {
                //   this.positions.push(new PositionReport(position));
                _this.position = position;
                _this.viewPortChanged();
                switch (status) {
                    case 1 /* active */:
                        _this.icon.fillColor = "#00FF00";
                        _this.icon.path = google.maps.SymbolPath.FORWARD_CLOSED_ARROW;
                        break;
                    case 0 /* inactive */:
                        _this.icon.fillColor = "#FF0000";
                        _this.icon.path = google.maps.SymbolPath.CIRCLE;
                        break;
                    case 2 /* parked */:
                        _this.icon.fillColor = "#0000FF";
                        _this.icon.path = google.maps.SymbolPath.CIRCLE;
                        break;
                }
                _this.marker.setPosition(position);
                _this.icon.rotation = bearing;
                _this.marker.set("icon", _this.icon);
                if (_this.drawingLine) {
                    _this.updateLines(position);
                }
                if (_this.showingInfo) {
                    _this.info.setPosition(position);
                }
            };
            this.updateLines = function (position) {
                var path = _this.line.getPath();
                path.push(position);
                _this.line.setPath(path);
            };
            this.onMapClick = function () {
                _this.toggleShowInfo();
            };
            this.onClick = function () {
                _this.expanded = !_this.expanded;
            };
            this.toggleShowInfo = function () {
                _this.map.panTo(_this.position);
                _this.showingInfo = !_this.showingInfo;
                if (_this.showingInfo === false) {
                    _this.info.close();
                }
                else {
                    _this.info.open(_this.map, _this.marker);
                    _this.info.setPosition(_this.position);
                }
            };
            this.toggleDrawLine = function () {
                _this.drawingLine = !_this.drawingLine;
                if (_this.drawingLine) {
                    _this.line = new google.maps.Polyline({
                        path: _this.positions.map(function (p) { return p.position; }),
                        geodesic: true,
                        strokeColor: '#FF0000',
                        strokeOpacity: 1.0,
                        strokeWeight: 2
                    });
                    _this.line.setMap(_this.map);
                }
                else {
                    _this.line.setMap(null);
                }
            };
            this.setColor = function (color) {
                _this.icon.fillColor = color;
                _this.marker.set("icon", _this.icon);
            };
            this.viewPortChanged = function () {
                if (_this.isInBounds()) {
                    if (_this.marker.getMap() === null) {
                        _this.marker.setMap(_this.map);
                    }
                }
                else {
                    if (_this.marker.getMap() !== null) {
                        _this.marker.setMap(null);
                    }
                }
            };
            this.icon = {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 4,
                fillColor: "#FF0000",
                fillOpacity: 1,
                strokeWeight: 1,
                rotation: 0 //this is how to rotate the pointer
            };
            var markerOptions = {
                map: this.map,
                icon: this.icon,
            };
            this.marker = new google.maps.Marker(markerOptions);
            this.info = new google.maps.InfoWindow({
                content: this.id
            });
            this.marker.addListener('click', this.onMapClick);
            App.track(this);
        }
        Object.defineProperty(MovingVehicle.prototype, "isActive", {
            get: function () {
                return this.status === 1 /* active */;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isInactive", {
            get: function () {
                return this.status === 0 /* inactive */;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isParked", {
            get: function () {
                return this.status === 2 /* parked */;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isNoState", {
            get: function () {
                return this.status === null;
            },
            enumerable: true,
            configurable: true
        });
        MovingVehicle.prototype.isInBounds = function () {
            return this.map.getBounds().contains(this.position);
        };
        return MovingVehicle;
    })();
    App.MovingVehicle = MovingVehicle;
    var PositionReport = (function () {
        function PositionReport(position) {
            this.position = position;
            this.date = new Date();
        }
        return PositionReport;
    })();
})(App || (App = {}));
//# sourceMappingURL=MovingVehicle.js.map