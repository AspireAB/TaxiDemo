module App {
    export class Application {
        private server: IPositionServer;
        private client: PositionClient;
        constructor() {

            this.client = new PositionClient();
            track(this);
        }

        public init = () => {
            this.server = $.connection.positionHub.server;

            this.client.joinSource = this.server.joinSource;
            this.client.leaveSource = this.server.leaveSource;

            $.connection.positionHub.client = this.client;

            $.connection.hub.start().then(() => this.server.init());

            this.client.initMap();
        }
    }
}