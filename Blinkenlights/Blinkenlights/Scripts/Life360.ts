import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "Life360Root";
const controllerEndpoint = "/Modules/GetLife360Module";

var Life360Handler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};

Life360Handler.refresh();
setInterval(Life360Handler.refresh, 2 * 60 * 1000);