var App;
(function (App) {
    var Application = (function () {
        function Application() {
            var _this = this;
            this.server = null;
            this.client = null;
            this.init = function () {
                _this.server = $.connection.positionHub.server;
                _this.client = new App.PositionClient(_this.server.onUpdateBounds);
                $.connection.positionHub.client = _this.client;
                $.connection.hub.start().then(_this.client.initMap);
                ko.applyBindings(_this, document.body);
            };
            track(this);
        }
        return Application;
    })();
    App.Application = Application;
    function track(object) {
        ko.track(object);
    }
    App.track = track;
})(App || (App = {}));
var app = new App.Application();
$(app.init);
