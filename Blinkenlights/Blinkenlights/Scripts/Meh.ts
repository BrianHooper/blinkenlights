import { SetModuleError } from "./Status.js";

var MehHandler = {
    refresh: function (): void {
        $.get("/Modules/GetMehData", function (data) {
            var root = $("#meh-root");
            if (!root) {
                SetModuleError("Meh", "ModuleController returned null");
            } else {
                root.html(data);
            }
        });
    }
};

MehHandler.refresh();
setInterval(MehHandler.refresh, 120 * 60 * 1000);