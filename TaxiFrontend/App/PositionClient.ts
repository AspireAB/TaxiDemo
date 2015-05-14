module App {
    export class PositionClient implements IPositionClient {
        private map: google.maps.Map = null;
        private vehicles: MovingVehicle[] = [];
        private order: (vehicle: MovingVehicle) => any;
        private searchText: string = "";

        private includedStateValues: GpsStatus[] = [GpsStatus.active];
        constructor(private onUpdatedBounds: IOnUpdateBounds) {
            track(this);
        }

        public get orderedVehicles() {
            return this.vehicles.filter(this.filter)
                .sort((a, b) => a.id > b.id ? 1 : -1);
        }

        private toggleStatus = (status: GpsStatus) => {
	        this.includedStateValues.toggleValue(status);
        }

        public isIncluded = (status: GpsStatus) => {
	        return this.includedStateValues.indexOf(status) !== -1;
        }

        private filter = (vehicle: MovingVehicle) => {
            if (this.isIncluded(vehicle.status) === false) return false;
            if (this.searchText.length > 0) {
                return vehicle.id.indexOf(this.searchText) !== -1;
            }
            return vehicle.inBounds;
        }

        public initMap = () => {
            var mapOptions = <google.maps.MapOptions>{
                zoom: 13,
                center: new google.maps.LatLng(57.70887000, 11.97456000) 
            };
            this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            google.maps.event.addListener(this.map, 'idle',() => {
					this.vehicles.forEach(v => v.viewPortChanged());
	            this.updateBounds();
            });
        }

		 private updateBounds = () => {
			 var northEast = this.map.getBounds().getNorthEast();
		    var southWest = this.map.getBounds().getSouthWest();
			 this.onUpdatedBounds({
				 LatitudeNorthEast: northEast.lat(),
				 LongitudeNorthEast: northEast.lng(),
				 LatitudeSouthWest: southWest.lat(),
				 LongitudeSouthWest: southWest.lng()
			 });
		 }

	    public positionChanged = (position: IPositionChanged) => {
            var latlng = new google.maps.LatLng(position.Latitude, position.Longitude);

            var vehicle = this.getVehicle(position.Id);
            vehicle.setPosition(position.Bearing, latlng, position.GpsStatus);
        };

        //TODO: use dictionary lookup
        public getVehicle = (id: string) => {
            var vehicles = this.vehicles.filter(d => d.id === id);
            if (vehicles.length === 0) {
                var item = new MovingVehicle(id, this.map);

                this.vehicles.push(item);
                return item;
            }
            var positionPair = vehicles[0];

            return positionPair;
        }
    }
}