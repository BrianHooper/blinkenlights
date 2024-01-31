import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "WWIIRoot";
const controllerEndpoint = "/Modules/GetWWIIModule";

var WWIIHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};

WWIIHandler.refresh();
setInterval(WWIIHandler.refresh, 15 * 60 * 1000);