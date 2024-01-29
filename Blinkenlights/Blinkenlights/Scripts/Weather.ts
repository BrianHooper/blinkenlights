import { SetModuleError } from "./StatusModule.js";

var WeatherHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWeatherData", function (data) {
            var root = $("#weather-root");
            if (!root) {
                SetModuleError("Weather", "ModuleController returned null");
            } else {
                root.html(data);
            }
        });
    }
};

WeatherHandler.refresh();
setInterval(WeatherHandler.refresh, 30 * 60 * 1000);