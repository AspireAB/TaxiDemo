module App {

    export interface IPositionChanged {
        RegNr: string;
        Latitude: number;
        Longitude: number;
        Bearing: number;
        Source: string;
    }
    export interface IPositionServer {
        init: () => void;
        joinSource: (source: string) => void;
        leaveSource: (source: string) => void;
    }
    export interface IPositionClient {
        statusChanged: (taxiStatus: ITaxiStatus) => void;
        positionChanged: (position: IPositionChanged) => void;
        sourceAdded: (source: string) => void;
        initialize: (sources: string[]) => void;
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
         server: App.IPositionServer
      };
      hub: { start: any; }
   }
};
