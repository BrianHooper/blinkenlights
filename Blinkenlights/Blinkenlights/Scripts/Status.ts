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

export function SetModuleStatusByFields(key: string, state: number, name: string, lastUpdate: string, status: string): void {
    const root = $("#status-root");
    if (!root) {
        console.log("SetModuleStatus: Failed to find root element");
        return;
    }

    if (!key) {
        return;
    }

    var statusRow = $("div[data-status-key='" + key + "']");
    if (!statusRow || statusRow.html() === undefined) {
        statusRow = jQuery("<div/>", {
            "class": "status-row",
            "data-status-key": key
        });
    }

    // TODO State should render an icon
    createDivElement("status-State", state, statusRow);
    createDivElement("status-Name", name, statusRow);
    createDivElement("status-LastUpdate", lastUpdate, statusRow);
    createDivElement("status-Status", status, statusRow);

    if (statusRow.children.length === 0) {
        return;
    }

    statusRow.appendTo(root);
}

export function SetModuleStatusByObject(apiStatus: APIStatus): void {
    const root = $("#status-root");
    if (!root) {
        console.log("SetModuleStatus: Failed to find root element");
        return;
    }

    if (!apiStatus) {
        console.log("SetModuleStatus: Dict is null");
        return;
    }

    if (!apiStatus.Items || apiStatus.Items.length === 0) {
        console.log("SetModuleStatus: Items is null or empty");
        return;
    }

    apiStatus.Items.forEach(function (item: ApiStatusItem) {
        if (item) {
            SetModuleStatusByFields(item.Key, item.State, item.Name, item.LastUpdate, item.Status);
        }

        //var statusRow = $("div[data-status-key='" + item.Key + "']");
        //if (!statusRow || statusRow.html() === undefined) {
        //    statusRow = jQuery("<div/>", {
        //        "class": "status-row",
        //        "data-status-key": item.Key
        //    });
        //}

        //createDivElement("status-State", item.State, statusRow);
        //createDivElement("status-Name", item.Name, statusRow);
        //createDivElement("status-LastUpdate", item.LastUpdate, statusRow);
        //createDivElement("status-Status", item.Status, statusRow);

        //statusRow.appendTo(root);
    });
}

export function SetModuleStatusByStr(data: string): void {
    if (!data) {
        console.log("SetModuleStatus: data is null");
        return;
    }

    const apiStatus = Convert.toAPIStatus(data);
    SetModuleStatusByObject(apiStatus);    
}

export function SetModuleStatusByElement(module: JQuery<HTMLElement>): void {
    if (!module) {
        console.log("SetModuleStatus: Module not found");
        return;
    }

    const data = module.attr("data-api-status");
    SetModuleStatusByStr(data);
};