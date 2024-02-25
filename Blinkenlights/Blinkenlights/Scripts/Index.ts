import { RefreshModule, GetModuleRoot } from "./ModuleStatusCommon.js";

function RefreshData(event) {
    var name = event.data.Name;
    var endpoint = event.data.Endpoint;

    if (!name || name === undefined || name.length === 0) {
        return;
    }

    var root = GetModuleRoot(name);
    if (root) {
        root.addClass("module-loading");
    }
    $.ajax({
        url: `/Modules/GetData`,
        type: "GET",
        data: { id: event.data.Name },
        success: function (data) {
            if (!endpoint || endpoint === undefined || endpoint.length === 0) {
                return;
            }
            console.log(`Refreshed module: ${data}`);
            RefreshModule(name, endpoint);
            if (root) {
                root.removeClass("module-loading");
            }
        },
        error: function (data) {
            console.log(`Error refreshing module: ${data}`);
            if (root) {
                root.removeClass("module-loading");
            }
        },
        timeout: 30000
    });
}

$(".module-refresh-block").each(function () {
    var name = $(this).attr("data-module-name");
    var endpoint = $(this).attr("data-module-endpoint");

    $(this).bind('click', { Name: name, Endpoint: endpoint }, RefreshData);
});