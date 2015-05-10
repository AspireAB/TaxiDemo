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
                    case 1 /* active */:
                        _this.setColor("#00FF00");
                        break;
                    case 0 /* inactive */:
                        _this.setColor("#FF0000");
                        break;
                    case 2 /* parked */:
                        _this.setColor("#0000FF");
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
                _this.marker.set("strokeColor", "#000000");
            };
            var circleOptions = {
                strokeColor: '#FF00FF',
                strokeOpacity: 1,
                strokeWeight: 2,
                fillColor: '#FF00FF',
                fillOpacity: 1,
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
        return MovingVehicle;
    })();
    App.MovingVehicle = MovingVehicle;
})(App || (App = {}));
//# sourceMappingURL=MovingVehicle.js.map