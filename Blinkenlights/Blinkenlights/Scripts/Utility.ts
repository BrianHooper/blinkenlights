import { SetModuleError } from "./StatusModule.js";

var UtilityHandler = {
    refresh: function (): void {
        $.get("/Modules/GetUtilityData", function (data) {
            var root = $("#utility-root");
            if (!root) {
                SetModuleError("Utility", "ModuleController returned null");
            } else {
                root.html(data);
            }
        });
    }
};

UtilityHandler.refresh();
setInterval(UtilityHandler.refresh, 1 * 30 * 1000);