var App;
(function (App) {
    var Application = (function () {
        function Application() {
            this.init = function () {
            };
            this.server = $.connection.positionHub.server;

            $.connection.positionHub.client = new App.ChatClient();

            $.connection.hub.start().done(this.init);
        }
        return Application;
    })();
    App.Application = Application;
})(App || (App = {}));
//# sourceMappingURL=App.js.map
