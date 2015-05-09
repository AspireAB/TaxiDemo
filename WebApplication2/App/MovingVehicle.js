var App;
(function (App) {
    var MovingVehicle = (function () {
        function MovingVehicle(id, map) {
            var _this = this;
            this.id = id;
            this.map = map;
            this.status = null;
            this.setStatus = function (status) {
                _this.status = status;
                switch (status) {
                    case App.GpsStatus.active:
                        _this.setColor("#00FF00");
                        break;
                    case App.GpsStatus.inactive:
                        _this.setColor("#000000");
                        break;
                    case App.GpsStatus.parked:
                        _this.setColor("#FF0000");
                        break;
                }
            };
            this.setPosition = function (position) {
                _this.marker.setCenter(position);
            };
            this.onClick = function () {
                _this.map.panTo(_this.position);
                console.log("Clicked: ", _this);
            };
            this.setColor = function (color) {
                _this.marker.set("fillColor", color);
                _this.marker.set("strokeColor", color);
            };
            var circleOptions = {
                strokeColor: '#FF00FF',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#FF00FF',
                fillOpacity: 0.35,
                map: this.map,
                radius: 20,
            };
            this.marker = new google.maps.Circle(circleOptions);
            this.marker.addListener('click', this.onClick);
            ko.track(this);
        }
        Object.defineProperty(MovingVehicle.prototype, "position", {
            get: function () {
                return this.marker.getCenter();
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isActive", {
            get: function () {
                return this.status === App.GpsStatus.active;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isInactive", {
            get: function () {
                return this.status === App.GpsStatus.inactive;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(MovingVehicle.prototype, "isParked", {
            get: function () {
                return this.status === App.GpsStatus.parked;
            },
            enumerable: true,
            configurable: true
        });
        return MovingVehicle;
    })();
    App.MovingVehicle = MovingVehicle;
})(App || (App = {}));
//# sourceMappingURL=MovingVehicle.js.map