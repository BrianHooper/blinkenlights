import { RefreshModule } from "./ModuleStatusCommon.js";

const moduleKey = "FinanceAnswerRoot";
const controllerEndpoint = "/Modules/GetFinanceAnswerModule";

var FinanceAnswerHandler = {
    refresh: function (): void {
        RefreshModule(moduleKey, controllerEndpoint)
    }
};
FinanceAnswerHandler.refresh();
setInterval(FinanceAnswerHandler.refresh, 3 * 60 * 60 * 1000);