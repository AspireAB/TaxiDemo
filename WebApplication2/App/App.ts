module App {
   export class Application {
      private server: IChatServer;

      constructor() {

         this.server = $.connection.positionHub.server;

         $.connection.positionHub.client = new ChatClient();

         $.connection.hub.start().done(this.init);
      }

       public init = () => {
      }
    }
}