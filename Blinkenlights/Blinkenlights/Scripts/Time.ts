import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "TimeRoot";
const controllerEndpoint = "/Modules/GetTimeModule";

var clockHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};
clockHandler.refresh();
setInterval(clockHandler.refresh, 60 * 1000);