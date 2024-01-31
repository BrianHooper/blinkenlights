//import { SetModuleError } from "./StatusModule.js";

var UtilityHandler = {
    refresh: function (): void {
        console.log("Refreshed Utility Module");
        $.get("/Modules/GetUtilityData", function (data) {
            var root = $("#utility-root");
            if (!root) {
                //SetModuleError("Utility", "ModuleController returned null");
            } else {
                root.html(data);
            }
        });
    }
};

UtilityHandler.refresh();
setInterval(UtilityHandler.refresh, 3 * 60 * 60 * 1000);