export interface Life360JSONModel {
    Name: string;
    TimeStr: string;
    Latitude: number;
    Longitude: number;
}

function CreateMap(mapElement: L.Map, layerGroup: L.LayerGroup<any>): void {
    const userModels = $(".life360-user-model").map(function (d): Life360JSONModel {
        const userModel = $(this).attr("data-model-info");
        return JSON.parse(userModel);
    }).get();

    if (userModels.length === 0) {
        return;
    }

    layerGroup.clearLayers();

    var coordinates = userModels.map(function (h) { return L.marker([h.Latitude, h.Longitude]); });
    var group = L.featureGroup(coordinates).addTo(mapElement);
    mapElement.fitBounds(group.getBounds());

    userModels.forEach(function (userModel) {
        var marker = L.marker([userModel.Latitude, userModel.Longitude],).addTo(layerGroup);
        marker.bindTooltip(userModel.Name).openTooltip();
    });
}

export function RefreshLife360Module() {
    const mapElement = L.map("life360-map", { attributionControl: false });
    const layerGroup = L.layerGroup().addTo(mapElement);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
    }).addTo(mapElement);

    CreateMap(mapElement, layerGroup);
}
