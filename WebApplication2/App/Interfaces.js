var App;
(function (App) {
    (function (GpsStatus) {
        GpsStatus[GpsStatus["inactive"] = 0] = "inactive";
        GpsStatus[GpsStatus["active"] = 1] = "active";
        GpsStatus[GpsStatus["parked"] = 2] = "parked";
    })(App.GpsStatus || (App.GpsStatus = {}));
    var GpsStatus = App.GpsStatus;
    function track(object) {
        ko.track(object);
    }
    App.track = track;
})(App || (App = {}));
//# sourceMappingURL=Interfaces.js.map