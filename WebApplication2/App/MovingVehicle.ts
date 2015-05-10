module App {
    export class MovingVehicle {
        private info: google.maps.InfoWindow;
        private marker: google.maps.Marker;
        private line: google.maps.Polyline;
        private icon: any;

        public status: GpsStatus = null;
        private positions: PositionReport[] = [];
        private drawingLine: boolean = false;
        private showingInfo: boolean = false;
        private expanded: boolean = false;

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

            this.info = new google.maps.InfoWindow({
                content: this.id
            });

            this.marker.addListener('click',this.onMapClick);

            track(this);
        }

        public get position() {
            return this.marker.getPosition();
        }

        public setStatus = (status: GpsStatus) => {
            this.status = status;
            switch (status) {
                case GpsStatus.active:
                    this.icon.fillColor = "#00FF00";
                    this.icon.path = google.maps.SymbolPath.FORWARD_CLOSED_ARROW;
                    this.marker.set("icon", this.icon);
                    break;
                case GpsStatus.inactive:
                    this.icon.fillColor = "#FF0000";
                    this.icon.path = google.maps.SymbolPath.CIRCLE;
                    this.marker.set("icon", this.icon);
                    break;
                case GpsStatus.parked:
                    this.icon.fillColor = "#0000FF";
                    this.icon.path = google.maps.SymbolPath.CIRCLE;
                    this.marker.set("icon", this.icon);
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

        public get isNoState() {
            return this.status === null;
        }

        public setPosition = (bearing: number, position: google.maps.LatLng) => {
         //   this.positions.push(new PositionReport(position));
            this.marker.setPosition(position);
            this.icon.rotation = bearing;
            this.marker.set("icon", this.icon);
            if (this.drawingLine) {
                this.updateLines(position);
            }
            if (this.showingInfo) {
                this.info.setPosition(position);
            }
        }

        private updateLines = (position: google.maps.LatLng) => {
            var path = this.line.getPath();
            path.push(position);
            this.line.setPath(path);
        }

        public onMapClick = () => {
            this.toggleShowInfo();
        }
        public onClick = () => {
            this.expanded = !this.expanded;
        }
        private toggleShowInfo = () => {
            this.map.panTo(this.position);

            this.showingInfo = !this.showingInfo;

            if (this.showingInfo === false) {
                this.info.close();
            } else {
                this.info.open(this.map, this.marker);
                this.info.setPosition(this.position);
            }
        }

        public toggleDrawLine = () => {
            this.drawingLine = !this.drawingLine;

            if (this.drawingLine) {
                this.line = new google.maps.Polyline({
                    path: this.positions.map(p => p.position),
                    geodesic: true,
                    strokeColor: '#FF0000',
                    strokeOpacity: 1.0,
                    strokeWeight: 2
                });

                this.line.setMap(this.map);
            } else {
                this.line.setMap(null);
            }
        }

        private setColor = (color: string) => {
            this.icon.fillColor = color;
            this.marker.set("icon", this.icon);
        }
    }

    class PositionReport {
        public date: Date;
        constructor(public position: google.maps.LatLng) {
            this.date = new Date();
        }
    }
}