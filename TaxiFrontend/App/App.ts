module App {
    export class Application {
        private server: IPositionServer = null;
        private client: PositionClient = null;
        constructor() {
            track(this);
        }

        public init = () => {
            this.server = $.connection.positionHub.server;
				this.client = new PositionClient(this.server.onUpdateBounds);

            $.connection.positionHub.client = this.client;
            $.connection.hub.start().then(this.client.initMap);

				ko.applyBindings(this, document.body);
        }
	}

	 export function track(object: any) {
		 ko.track(object);
	 }
}

var app = new App.Application();

$(app.init);

