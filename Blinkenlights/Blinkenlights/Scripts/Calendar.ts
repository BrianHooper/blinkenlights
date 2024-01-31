import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "CalendarRoot";
const controllerEndpoint = "/Modules/GetCalendarModule";

var CalendarHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};
CalendarHandler.refresh();
setInterval(CalendarHandler.refresh, 15 * 60 * 1000);