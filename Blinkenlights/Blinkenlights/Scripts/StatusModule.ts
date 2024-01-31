import { Convert, APIStatus, APIStatusItem } from "./ApiStatus.js"

///*
//    Unknown = 0,
//    Failed = 1,
//    Stale = 2,
//    Success = 3
//*/
function getSourceClass(source: number): string {
    if (source === 1) {
        return "api-source api-source-cache";
    } else if (source === 2) {
        return "api-source api-source-prod";

    } else if (source === 3) {
        return "api-source api-source-error";
    } else {
        return "api-source source-unknown";
    }
}

function createSourceIcon(source: number): SVGSVGElement {
    const svg = d3.create('svg');
    svg
        .attr("class", getSourceClass(source))
        .append('circle')
        .attr('cx', '50%')
        .attr('cy', '50%')
        .attr('r', 5);

    return svg.node();
}

function createDivElement(className: string, value: any, parent: JQuery<HTMLElement>): void {
    if (!parent || !className) {
        return;
    }

    const childDiv = $("<div/>", {
        "class": className,
    });

    if (value) {
        childDiv.html(value);
    }
    childDiv.appendTo(parent);
}

/*
    Unknown = 0,
    Failed = 1,
    Stale = 2,
    Success = 3
*/
function getStateClass(state: number): string {
    if (state === 1) {
        return "status-failed";
    } else if (state === 2) {
        return "status-stale";
    } else if (state === 3) {
        return "status-success";
    } else {
        return "status-unknown";
    }
}

function errorStatus(moduleName: string, errorStr: string): APIStatus {
    return Convert.Create(moduleName, errorStr, new Date().toTimeString(), 0, 0);
}

function convertStatus(moduleName: string, apiStatusStr: string): APIStatus {
    try {
        const apiStatus = Convert.toAPIStatus(apiStatusStr);
        if (apiStatus === undefined || apiStatus.Items === undefined || apiStatus.Items.length === 0) {
            return errorStatus(moduleName, "Status is empty");
        } else {
            return apiStatus;
        }
    } catch (error) {
        return errorStatus(moduleName, error);
    }
}

function createStatusRow(key: string, item: APIStatusItem): JQuery<HTMLElement> {
    var statusClass = `status-row ${getStateClass(item.State)}`;

    const statusRow = $("<div/>", {
        "class": `status-row ${statusClass}`,
        "data-key": key
    });

    const sourceIcon = createSourceIcon(item.Source);

    $("<div/>", { "class": "status-source" }).html(sourceIcon).appendTo(statusRow);

    createDivElement("status-name", item.Name, statusRow);
    createDivElement("status-lastupdate", item.LastUpdate, statusRow);
    createDivElement("status-status", item.Status, statusRow);

    return statusRow;
}

function updateStatusRow(row: JQuery<HTMLElement>, item: APIStatusItem): void {
    if (!row || row === undefined || !item || item === undefined) {
        return;
    }

    const sourceIcon = createSourceIcon(item.Source);
    row.children(".status-source").first().html(sourceIcon);
    row.children(".status-lastupdate").first().html(item.LastUpdate);
    row.children(".status-lastupdate").first().html(item.LastUpdate);
    row.children(".status-status").first().html(item.Status);
}

function Update(module: JQuery<HTMLElement>): void {
    if (!module || module === undefined || module.length === 0) {
        return;
    }

    var moduleName = module.attr("data-module-name");
    var moduleStatus = module.attr("data-api-status");
    var apiStatus = convertStatus(moduleName, moduleStatus);

    apiStatus.Items.forEach(function (item: APIStatusItem) {
        if (item) {
            const key = `${moduleName}-${item.Name}`;
            var statusRows = $("#status-body").children(`[data-key='${key}']`);
            if (!statusRows || statusRows === undefined || statusRows.length === 0) {
                const statusElement = createStatusRow(key, item);
                $("#status-body").append(statusElement);
            } else {
                updateStatusRow(statusRows.first(), item);
            }
        }
    });
}

function RefreshAll(): void {
    $(".module-status").each(function () {
        Update($(this))
    });
}

export function Refresh(key: string): void {
    Update($(`.module-status [data-module-name='${key}']`).eq(0));
}

RefreshAll();
setInterval(RefreshAll, 15 * 1000);