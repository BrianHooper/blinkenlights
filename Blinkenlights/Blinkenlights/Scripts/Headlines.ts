import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "HeadlinesRoot";
const controllerEndpoint = "/Modules/GetHeadlinesModule";

var HeadlinesHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};
HeadlinesHandler.refresh();
setInterval(HeadlinesHandler.refresh, 1 * 60 * 60 * 1000);