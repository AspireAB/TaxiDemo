module App {

   export interface IPositionChanged {
      RegNr: string;
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

   export interface ITaxiStatus {
      RegNr: string;
      GpsStatus: GpsStatus;
   }
   export enum GpsStatus {
      inactive = 0,
      active = 1
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
