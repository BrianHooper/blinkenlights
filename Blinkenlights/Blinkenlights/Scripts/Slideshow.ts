import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "SlideshowRoot";
const controllerEndpoint = "/Modules/GetSlideshowModule";

var SlideshowHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};
SlideshowHandler.refresh();
setInterval(SlideshowHandler.refresh, 3 * 60 * 60 * 1000);