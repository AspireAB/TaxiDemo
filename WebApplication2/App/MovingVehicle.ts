module App {
    export class MovingVehicle {
        private marker: google.maps.Marker;
        private status: GpsStatus = null;
        private icon: any;
        constructor(public id: string, private map: google.maps.Map) {

            this.icon = {
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                scale: 4,
                fillColor: "#ff5050",
                fillOpacity: 1,
                strokeWeight: 1,
                rotation: 0 //this is how to rotate the pointer
            };

            var markerOptions = <google.maps.MarkerOptions>{               
                map: this.map,                
                icon: this.icon,
            };

            
                                 
            this.marker = new google.maps.Marker(markerOptions);

            this.marker.addListener('click', this.onClick);            

            this.marker.setTitle(id);

            ko.track(this);
        }

        public get position() {
            return this.marker.getPosition();
        }

        public setStatus = (status: GpsStatus) => {
            this.status = status;
            switch (status) {
                case GpsStatus.active:
                    this.setColor("#00FF00");
                    break;
                case GpsStatus.inactive:
                    this.setColor("#FF0000");
                    break;
                case GpsStatus.parked:
                    this.setColor("#0000FF");
                    break;
            }
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

        public setPosition = (bearing: number, position: google.maps.LatLng) => {
            
            this.marker.setPosition(position);
            this.icon.rotation = bearing;
            this.marker.set("icon", this.icon);
        }

        public onClick = () => {
            this.map.panTo(this.position);
            console.log("Clicked: ", this);
        }

        private setColor = (color: string) => {
            this.icon.fillColor = color;
            this.marker.set("icon", this.icon);
            //this.marker.set("icon.fillColor", color);
            //    this.marker.set("icon.fillColor", "#000000");
        }
    }
}