module App {
    export class MovingVehicle {
        private marker: google.maps.Circle;
        private status: GpsStatus = null;
        constructor(public id: string, private map: google.maps.Map) {
            var circleOptions = <google.maps.CircleOptions>{
                strokeColor: '#FF00FF',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#FF00FF',
                fillOpacity: 0.35,
                map: this.map,
                radius: 20,
            };
            this.marker = new google.maps.Circle(circleOptions);

            this.marker.addListener('click',this.onClick);

            ko.track(this);
        }

        public get position() {
            return this.marker.getCenter();
        }

        public setStatus = (status: GpsStatus) => {
            this.status = status;
            switch (status) {
                case GpsStatus.active:
                    this.setColor("#00FF00");
                    break;
                case GpsStatus.inactive:
                    this.setColor("#000000");
                    break;
                case GpsStatus.parked:
                    this.setColor("#FF0000");
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

        public setPosition = (position: google.maps.LatLng) => {
            this.marker.setCenter(position);
        }

        public onClick = () => {
            this.map.panTo(this.position);
            console.log("Clicked: ", this);
        }

        private setColor = (color: string) => {
            this.marker.set("fillColor", color);
            this.marker.set("strokeColor", color);
        }
    }
}