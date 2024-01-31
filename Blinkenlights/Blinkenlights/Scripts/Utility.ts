import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "UtilityRoot";
const controllerEndpoint = "/Modules/GetUtilityData";

var UtilityHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};

UtilityHandler.refresh();
setInterval(UtilityHandler.refresh, 15 * 60 * 1000);