var App;
(function (App) {
    var Application = (function () {
        function Application() {
            var _this = this;
            this.init = function () {
                _this.server = $.connection.positionHub.server;
                _this.client.joinSource = _this.server.joinSource;
                _this.client.leaveSource = _this.server.leaveSource;
                $.connection.positionHub.client = _this.client;
                $.connection.hub.start().then(function () { return _this.server.init(); });
                _this.client.initMap();
            };
            this.client = new App.PositionClient();
            App.track(this);
        }
        return Application;
    })();
    App.Application = Application;
})(App || (App = {}));
//# sourceMappingURL=App.js.map