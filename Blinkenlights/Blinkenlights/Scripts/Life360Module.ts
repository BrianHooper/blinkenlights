import { create } from "d3";
import { SetModuleStatusByElement, SetModuleStatusByStr, SetModuleStatusByFields, SetModuleError } from "./Status.js";

export interface Life360JSONModel {
    Name: string;
    TimeStr: string;
    Latitude: number;
    Longitude: number;
}

var layerGroup;
var mapElement;
var initialized = false;

function CreateMap() {
    const userModels = $(".life360-user-model").map(function (d): Life360JSONModel {
        const userModel = $(this).attr("data-model-info");
        return JSON.parse(userModel);
    }).get();

    if (userModels.length === 0) {
        SetModuleError("Life360", "Failed to get models - empty");
        return;
    }

    if (!initialized) {
        mapElement = L.map("life360-map", { attributionControl: false });
        layerGroup = L.layerGroup().addTo(mapElement);
        initialized = true;
    }
    layerGroup.clearLayers();

    var coordinates = userModels.map(function (h) { return L.marker([h.Latitude, h.Longitude]); });
    var group = L.featureGroup(coordinates).addTo(mapElement);
    mapElement.fitBounds(group.getBounds());

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
    }).addTo(mapElement);

    userModels.forEach(function (userModel) {
        var marker = L.marker([userModel.Latitude, userModel.Longitude],).addTo(layerGroup);
        marker.bindTooltip(userModel.Name).openTooltip();
    });
}

SetModuleStatusByElement($("#life360-data"));
CreateMap();