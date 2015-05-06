module App {

   export interface ObjectPosition {
      Id: number;
      Latitude: number;
      Longitude: number;
   }
   export interface IChatServer {

   }
   export interface IPosition {
      Location: {
         Latitude: number;
         Longitude: number;
      }
   }
}

declare var $: {
   connection: {
      positionHub: {
         client: App.ChatClient;
         server: App.IChatServer
      };
      hub: { start: any; }
   }
};
