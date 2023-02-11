import { Convert, APIStatus, ApiStatusItem } from "./ApiStatusSerializer.js"

function createDivElement(className: string, value: any, parent: JQuery<HTMLElement>): void {
    if (!parent || !value || !className) {
        return;
    }

    const childDiv = jQuery("<div/>", {
        "class": "status-name",
    });
    childDiv.html(value);
    childDiv.appendTo(parent);
}

export function SetModuleStatus(module: JQuery<HTMLElement>) {
    const root = $("#status-root");

    if (!module) {
        return;
    }
    const data = module.attr("data-api-status");
    if (!data) {
        return;
    }
    
    if (!root) {
        console.log("Failed to find status-root");
        return;
    }

    const apiStatus = Convert.toAPIStatus(data);

    if (!apiStatus) {
        console.log("Dict is null");
        return;
    }

    if (!apiStatus.Items || apiStatus.Items.length === 0) {
        console.log("Items is null or empty");
        return;
    }

    apiStatus.Items.forEach(function (item: ApiStatusItem) {
        var statusRow = $("div[data-status-key='" + item.Key + "']");
        if (!statusRow || statusRow.html() === undefined) {
            statusRow = jQuery("<div/>", {
                "class": "status-row",
                "data-status-key": item.Key
            });
        }

        createDivElement("status-State", item.State, statusRow);
        createDivElement("status-Name", item.Name, statusRow);
        createDivElement("status-LastUpdate", item.LastUpdate, statusRow);
        createDivElement("status-Status", item.Status, statusRow);

        statusRow.appendTo(root);
    });
};