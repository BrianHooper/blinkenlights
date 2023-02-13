import { SetModuleStatusByStr, SetModuleStatusByFields } from "./Status.js";

var MehHandler = {
    refresh: function (): void {
        $.get("/Modules/GetMehData", function (data) {
            if (!data) {
                SetModuleStatusByFields("Meh", "Data response is null", null, 1, 0);
                return;
            }

            var apiResponse = JSON.parse(data);
            if (!apiResponse) {
                SetModuleStatusByFields("Meh", "API response is null", null, 1, 0);
                return;
            }

            var apiStatus: string = apiResponse["Status"];
            if (apiStatus) {
                SetModuleStatusByStr(apiStatus);
            }
            else {
                //TODO this should use constants, avoid re-defining key/name/state enum
                SetModuleStatusByFields("Meh", "API status response is null", null, 1, 0);
                return;
            }

            var apiData = apiResponse["ApiData"];
            if (!apiData) {
                SetModuleStatusByFields("Meh", "API data response is null", null, 1, 0);
                return;
            }

            var meh = JSON.parse(apiData);
            if (!meh) {
                SetModuleStatusByFields("Meh", "Failed to parse API result", null, 1, 0);
                return;
            }

            var root = $("#meh-root");
            if (!root) {
                SetModuleStatusByFields("Meh", "Failed to find meh root on page", null, 1, 0);
                return;
            }

            $("#meh-item").html(meh["deal"]["title"]);

            $("#meh-price").html("$" + meh["deal"]["items"][0]["price"]);

            var imgBlockHeight = $("#meh-image").height() - 20;
            $("#meh-image").empty();
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