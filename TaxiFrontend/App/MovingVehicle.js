var App;
(function (App) {
    var MovingVehicle = (function () {
        function MovingVehicle(id, map) {
            var _this = this;
            this.id = id;
            this.map = map;
            this.status = null;
            this.showingInfo = false;
            this.inBounds = false;
            this.setStatus = function (status) {
                _this.updateStatus(status);
                _this.marker.set("icon", _this.icon);
            };
            this.setPosition = function (bearing, position, status) {
                _this.position = position;
                _this.viewPortChanged();
                _this.updateStatus(status);
                _this.marker.setPosition(position);
                _this.icon.rotation = bearing;
                _this.marker.set("icon", _this.icon);
                if (_this.showingInfo) {
                    _this.info.setPosition(position);
                }
            };
            this.updateStatus = function (status) {
                _this.status = status;
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
            };
            this.onClick = function () {
                _this.map.panTo(_this.position);
                _this.showingInfo = !_this.showingInfo;
                if (_this.showingInfo) {
                    _this.info.open(_this.map, _this.marker);
                    _this.info.setPosition(_this.position);
                }
                else {
                    _this.info.close();
                }
            };
            this.setColor = function (color) {
                _this.icon.fillColor = color;
                _this.marker.set("icon", _this.icon);
            };
            this.viewPortChanged = function () {
                _this.inBounds = _this.isInBounds();
                if (_this.inBounds) {
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
                rotation: 0
            };
            this.marker = new google.maps.Marker({ map: this.map, icon: this.icon });
            this.info = new google.maps.InfoWindow({ content: this.id });
            this.marker.addListener('click', this.onClick);
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
            if (this.position)
                return this.map.getBounds().contains(this.position);
            return false;
        };
        return MovingVehicle;
    })();
    App.MovingVehicle = MovingVehicle;
})(App || (App = {}));
