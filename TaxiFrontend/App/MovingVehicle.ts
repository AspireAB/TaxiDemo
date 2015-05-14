module App {
    export class MovingVehicle {
        private info: google.maps.InfoWindow;
        private marker: google.maps.Marker;
        private line: google.maps.Polyline;
        private icon: any;

        public status: GpsStatus = null;
        private showingInfo: boolean = false;
		  public inBounds: boolean = false;
        private position: google.maps.LatLng;

        constructor(public id: string, private map: google.maps.Map) {
            this.icon = {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 4,
                fillColor: "#FF0000",
                fillOpacity: 1,
                strokeWeight: 1,
                rotation: 0 //this is how to rotate the pointer
            };

            this.marker = new google.maps.Marker({ map: this.map, icon: this.icon });

            this.info = new google.maps.InfoWindow({ content: this.id });

            this.marker.addListener('click', this.onClick);

            track(this);
        }

        //TODO: obsolete this.. send all state in position msg instead
        public setStatus = (status: GpsStatus) => {
            this.updateStatus(status);
				this.marker.set("icon", this.icon);
        }

        public get isActive() {
            return this.status === GpsStatus.active;
        }

        public get isInactive() {
            return this.status === GpsStatus.inactive;
        }

        public get isParked() {
            return this.status === GpsStatus.parked;
        }

        public get isNoState() {
            return this.status === null;
        }

        public setPosition = (bearing: number, position: google.maps.LatLng,status: GpsStatus) => {
            this.position = position;
            this.viewPortChanged();
	         this.updateStatus(status);
            this.marker.setPosition(position);
            this.icon.rotation = bearing;
            this.marker.set("icon", this.icon);
            if (this.showingInfo) {
                this.info.setPosition(position);
            }
        }

		 private updateStatus = (status: GpsStatus) => {
			 this.status = status;
			 switch (status) {
				 case GpsStatus.active:
					 this.icon.fillColor = "#00FF00";
					 this.icon.path = google.maps.SymbolPath.FORWARD_CLOSED_ARROW;
					 break;
				 case GpsStatus.inactive:
					 this.icon.fillColor = "#FF0000";
					 this.icon.path = google.maps.SymbolPath.CIRCLE;
					 break;
				 case GpsStatus.parked:
					 this.icon.fillColor = "#0000FF";
					 this.icon.path = google.maps.SymbolPath.CIRCLE;
					 break;
			 }
		 }

        private isInBounds() {
            if(this.position)
                return this.map.getBounds().contains(this.position);
            return false;
        }

        public onClick = () => {
            this.map.panTo(this.position);

            this.showingInfo = !this.showingInfo;

            if (this.showingInfo) {
					this.info.open(this.map, this.marker);
					this.info.setPosition(this.position);
            } else {
					this.info.close();
            }
        }

        private setColor = (color: string) => {
            this.icon.fillColor = color;
            this.marker.set("icon", this.icon);
        }

        public viewPortChanged = () => {
			  this.inBounds = this.isInBounds();

			  if (this.inBounds) {
                if (this.marker.getMap() === null) {
                    this.marker.setMap(this.map);
                }
            } else {
                if (this.marker.getMap() !== null) {
                    this.marker.setMap(null);
                }
            }
        }
    }
}