module App {

    export interface IPositionChanged {
        RegNr: string;
        Latitude: number;
        Longitude: number;
        Bearing: number;
        GpsStatus: GpsStatus;
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
        active = 1,
        parked = 2,
    }
    export function track(object: any) {
        ko.track(object);
    }
}

declare var ko: {
    track: (Function) => void;
    applyBindings: (Function) => void;
}
declare var $: {
   connection: {
      positionHub: {
         client: App.PositionClient;
         server: App.IChatServer
      };
      hub: { start: any; }
   }
};
