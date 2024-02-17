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

function createSourceIcon(sourceClass: string): SVGSVGElement {
    const svg = d3.create('svg');
    svg
        .attr("class", sourceClass)
        .append('circle')
        .attr('cx', '50%')
        .attr('cy', '50%')
        .attr('r', 5)
        .attr("transform",
            "translate(0, 1.5)");

    return svg.node();
}

function getModuleHeader(moduleKey: string): JQuery<HTMLElement> {
    return $(`.index-module[data-module-name='${moduleKey}']`).find(".module-header").first();
}

function SetModuleResultMessage(key: string, msg: string) {
    const header = getModuleHeader(key);

    if (!header || header === undefined || header.length === 0) {
        return;
    }
}

function GetModuleResultElement(key: string): JQuery<HTMLElement> {
    return $(`.module-result[data-module-name='${key}']`);
}

function GetModuleResultData(resultElement: JQuery<HTMLElement>): string {
    return resultElement.attr("data-api-result");
}

function ApiStatusItemToElement(item: APIStatusItem): JQuery<HTMLElement> {
    const container = jQuery("<div/>", { "class": "api-status", "title": item.Status });

    const sourceIcon = createSourceIcon(getSourceClass(item.Source));
    $("<div/>", { "class": "status-source" }).html(sourceIcon).appendTo(container);
    $("<div/>", { "class": "status-name" }).html(item.Name).appendTo(container);
    $("<div/>", { "class": "status-lastupdate" }).html(item.LastUpdate).appendTo(container);
    return container;
}

function SetApiStatus(key: string, status: APIStatus): void {
    if (!status || status === undefined) {
        return;
    }

    const header = getModuleHeader(key);

    if (!header || header === undefined || header.length === 0) {
        return;
    }

    var container = header.find(".module-api-result").first();
    if (!container || container === undefined || container.length === 0) {
        container = jQuery("<div/>", { "class": "module-api-result" });
        container.appendTo(header);
    } else {
        container.html("");
    }

    status.Items.forEach((item) => ApiStatusItemToElement(item).appendTo(container));
}

function SetModuleStatus(key: string, status: string): void {
    if (!status || status === undefined) {
        return;
    }

    const header = getModuleHeader(key);

    if (!header || header === undefined || header.length === 0) {
        return;
    }

    var container = header.find(".module-status-result").first();
    if (!container || container === undefined || container.length === 0) {
        container = jQuery("<div/>", { "class": "module-status-result" });
        container.appendTo(header);
    } else {
        container.html("");
    }

    const sourceIcon = createSourceIcon(status);
    $("<div/>", { "class": "module-status" }).html(sourceIcon).appendTo(container);
    $("<div/>", { "class": "module-title" }).html(key).appendTo(container);

    var d = new Date();

    var dateStr = d.toLocaleString("en-US", {
        hour: "numeric",
        minute: "2-digit"
    });
    $("<div/>", { "class": "module-time" }).html(dateStr).appendTo(container);
}

function SetApiResult(moduleName: string, apiStatusStr: string): void {
    try {
        const apiStatus = Convert.toAPIStatus(apiStatusStr);
        if (apiStatus === undefined || apiStatus.Items === undefined || apiStatus.Items.length === 0) {
            SetModuleResultMessage(moduleName, "Empty");
        } else {
            SetApiStatus(moduleName, apiStatus)
        }
    } catch (error) {
        SetModuleResultMessage(moduleName, "Exception");
    }
}

export function UpdateResult(key: string) {
    const result = GetModuleResultElement(key);
    const data = GetModuleResultData(result);

    if (!data || data === undefined || data.length === 0) {
        SetModuleResultMessage(key, "Failed");
        return;
    }

    SetApiResult(key, data);
}

export function RefreshModule(key: string, endpoint: string) {
    SetModuleStatus(key, "module-status-icon module-loading");

    $.ajax({
        url: endpoint,
        success: function (data) {
            var root = $(`.index-module[data-module-name='${key}']`).find(".module-body").first();
            if (!root) {
                return;
            }

            SetModuleStatus(key, "module-status-icon module-success");
            root.html(data);
        },
        error: function () {
            SetModuleStatus(key, "module-status-icon module-failed");
        },
        timeout: 30000
    });
}

export function StartModule(key: string, endpoint: string, interval: number): void {
    RefreshModule(key, endpoint);
    setInterval(RefreshModule, interval, key, endpoint);
}