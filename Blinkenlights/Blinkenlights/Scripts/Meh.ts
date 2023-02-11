import { SetModuleStatusByStr, SetModuleStatusByFields } from "./Status.js";

var MehHandler = {
    refresh: function (): void {
        $.get("/Modules/GetMehData", function (data) {
            if (!data) {
                SetModuleStatusByFields("Meh", 1, "Meh", null, "Data response is null");
                return;
            }

            var apiResponse = JSON.parse(data);
            if (!apiResponse) {
                SetModuleStatusByFields("Meh", 1, "Meh", null, "API response is null");
                return;
            }

            var apiStatus: string = apiResponse["Status"];
            if (apiStatus) {
                SetModuleStatusByStr(apiStatus);
            }
            else {
                //TODO this should use constants, avoid re-defining key/name/state enum
                SetModuleStatusByFields("Meh", 1, "Meh", null, "API status response is null");
                return;
            }

            var apiData = apiResponse["ApiData"];
            if (!apiData) {
                SetModuleStatusByFields("Meh", 1, "Meh", null, "API data response is null");
                return;
            }

            var meh = JSON.parse(apiData);
            if (!meh) {
                SetModuleStatusByFields("Meh", 1, "Meh", null, "Failed to parse API result");
                return;
            }

            var root = $("#meh-root");
            if (!root) {
                SetModuleStatusByFields("Meh", 1, "Meh", null, "Failed to find meh root on page");
                return;
            }

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