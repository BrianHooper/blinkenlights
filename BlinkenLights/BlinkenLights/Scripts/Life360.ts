var layerGroup;
var mapElement;
var initialized = false;

var gpsHandler = {
    refresh: function (): void {
        $.get("/Root/GetLife360Locations", function (data) {
            var locations = JSON.parse(data);
            var coordinates = locations.map(function (h) { return [h["Latitude"], h["Longitude"]]; })

            if (!initialized) {
                mapElement = L.map("map", { attributionControl: false });
                layerGroup = L.layerGroup().addTo(mapElement);
                initialized = true;
            }
            layerGroup.clearLayers();

            var bounds = new L.LatLngBounds(coordinates);
            mapElement.fitBounds(bounds);
            L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
            }).addTo(mapElement);

            locations.forEach(function (value) {
                var marker = L.marker([value["Latitude"], value["Longitude"]],).addTo(layerGroup);
                marker.bindTooltip(value["Name"]).openTooltip();
            });

            $("#life360-time").html(locations[1]["TimeStr"]);
        });
    }
};

gpsHandler.refresh();
setInterval(gpsHandler.refresh, 120 * 1000);