import { SetModuleStatusByElement, SetModuleStatusByStr, SetModuleStatusByFields, SetModuleError } from "./Status.js";

var layerGroup;
var mapElement;
var initialized = false;

const dataElement = $("#life360-data");
SetModuleStatusByElement(dataElement);

const apiResponse = dataElement.attr("data-response");

function CreateMap(data) {
    if (!data) {
        SetModuleError("Life360", "Failed to get data");
        return;
    }

    var apiResponse = JSON.parse(data);
    if (!apiResponse) {
        SetModuleError("Life360", "Failed to get apiResponse");
        return;
    }

    var locations = apiResponse["Models"];
    if (!locations) {
        SetModuleError("Life360", "Failed to get models - null");
        return;
    }

    if (locations.length === 0) {
        SetModuleError("Life360", "Failed to get models - empty");
        return;
    }

    var coordinates = locations.map(function (h) { return [h["Latitude"], h["Longitude"]]; })

    if (!initialized) {
        mapElement = L.map("life360-map", { attributionControl: false });
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

    $("#life360-time").html(locations[0]["TimeStr"]);
}

CreateMap(apiResponse);