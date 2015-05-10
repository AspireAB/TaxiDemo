module App {
    export class Application {
        private server: IChatServer;
        private client: PositionClient;
        constructor() {

            this.client = new PositionClient()
            track(this);
        }

        public init = () => {
            this.server = $.connection.positionHub.server;

            $.connection.positionHub.client = this.client;

            $.connection.hub.start();

            this.client.initMap();
        }
    }
}