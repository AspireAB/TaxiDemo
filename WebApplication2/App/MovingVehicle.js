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
            this.setPosition = function (bearing, position) {
                _this.marker.setPosition(position);
                _this.icon.rotation = bearing;
                _this.marker.set("icon", _this.icon);
            };
            this.onClick = function () {
                _this.map.panTo(_this.position);
                console.log("Clicked: ", _this);
            };
            this.setColor = function (color) {
                _this.icon.fillColor = color;
                _this.marker.set("icon", _this.icon);
                //this.marker.set("icon.fillColor", color);
                //    this.marker.set("icon.fillColor", "#000000");
            };
            this.icon = {
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                scale: 4,
                fillColor: "#ff5050",
                fillOpacity: 1,
                strokeWeight: 1,
                rotation: 0 //this is how to rotate the pointer
            };
            var markerOptions = {
                map: this.map,
                icon: this.icon,
            };
            this.marker = new google.maps.Marker(markerOptions);
            this.marker.addListener('click', this.onClick);
            this.marker.setTitle(id);
            ko.track(this);
        }
        Object.defineProperty(MovingVehicle.prototype, "position", {
            get: function () {
                return this.marker.getPosition();
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