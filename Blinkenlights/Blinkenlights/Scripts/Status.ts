import { Convert, APIStatus, APIStatusItem } from "./ApiStatus.js"

function createDivElement(className: string, value: any, parent: JQuery<HTMLElement>): void {
    if (!parent || !className) {
        return;
    }

    const childDiv = jQuery("<div/>", {
        "class": className,
    });
    if (value) {
        childDiv.html(value);
    }
    childDiv.appendTo(parent);
}

export function SetModuleStatusByFields(Name: string, Status: string, LastUpdate: string, State: number, Source: number): void {
    const root = $("#status-root");
    if (!root) {
        console.log("SetModuleStatus: Failed to find root element");
        return;
    }

    if (!Name) {
        return;
    }

    var statusRow = $("div[data-status-key='" + Name + "']");
    if (!statusRow || statusRow.html() === undefined) {
        statusRow = jQuery("<div/>", {
            "class": "status-row",
            "data-status-key": Name
        });
    }

    statusRow.empty();

    // TODO State & Source should render an icon
    createDivElement("status-state", State, statusRow);
    createDivElement("status-state", Source, statusRow);
    createDivElement("status-name", Name, statusRow);
    createDivElement("status-lastupdate", LastUpdate, statusRow);
    createDivElement("status-status", Status, statusRow);

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

    apiStatus.Items.forEach(function (item: APIStatusItem) {
        if (item) {
            SetModuleStatusByFields(item.Name, item.Status, item.LastUpdate, item.State, item.Source);
        }
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