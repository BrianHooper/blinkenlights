import { SetModuleStatus } from "./Status.js";

var MehHandler = {
    refresh: function (): void {
        $.get("/Modules/GetMehData", function (data) {
            var meh = JSON.parse(data);
            if (meh["Error"]) {
                $("#meh-root").html(meh["Error"]);
                return;
            }

            var root = $("#meh-root");
            if (!root) {
                return;
            }

            $(root).attr("report", "This is a status report");

            $("#meh-item").html(meh["deal"]["title"]);

            $("#meh-price").html("$" + meh["deal"]["items"][0]["price"]);

            var imgBlockHeight = $("#meh-image").height() - 20;

            var textBlock = jQuery("<img/>", {
                "height": imgBlockHeight,
                "width": imgBlockHeight,
                "src": meh["deal"]["photos"][0],
            }).appendTo("#meh-image");
        });
    }
};

MehHandler.refresh();
setInterval(MehHandler.refresh, 120 * 60 * 1000);