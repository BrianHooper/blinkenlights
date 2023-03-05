import { SetModuleError } from "./Status.js";

var Life360Handler = {
    refresh: function (): void {
        $.get("/Modules/GetLife360Module", function (data) {
            var root = $("#life360-root");
            if (!root) {
                SetModuleError("Life360", "ModuleController returned null");
            } else {
                root.html(data);
            }
        });
    }
};

Life360Handler.refresh();
setInterval(Life360Handler.refresh, 2 * 60 * 1000);