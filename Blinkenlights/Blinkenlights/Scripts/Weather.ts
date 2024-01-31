import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "WeatherRoot";
const controllerEndpoint = "/Modules/GetWeatherData";

var WeatherHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};

WeatherHandler.refresh();
setInterval(WeatherHandler.refresh, 15 * 60 * 1000);