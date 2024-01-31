import { Convert, APIStatus, APIStatusItem } from "./ApiStatus.js"

function getModuleHeader(moduleKey: string): JQuery<HTMLElement> {
    return $(`.index-module[data-module-name='${moduleKey}']`).find(".module-header").first();
}

function setModuleStatusField(header: JQuery<HTMLElement>, className: string, styleClass: string, innerHtml: string): void {
    if (!header || header === undefined || header.length === 0) {
        return;
    }

    var fieldElement = header.find(`.${className}`).first();
    if (!fieldElement || fieldElement === undefined || fieldElement.length === 0) {
        fieldElement = $("<div/>");
        fieldElement.appendTo(header);
    }
    fieldElement.attr("class", `status-field ${className} ${styleClass}`);

    fieldElement.html(innerHtml);
}

function setModuleStatus(key: string, styleClass: string): void {
    const header = getModuleHeader(key);

    if (!header || header === undefined || header.length === 0) {
        return;
    }

    setModuleStatusField(header, "module-title", styleClass, key);
}

function SetModuleStatusSuccess(moduleKey: string) {
    setModuleStatus(moduleKey, "module-success");
}

function SetModuleStatusFailed(moduleKey: string) {
    setModuleStatus(moduleKey, "module-failed");
}

function SetModuleStatusLoading(moduleKey: string) {
    setModuleStatus(moduleKey, "module-loading");
}

function SetModuleResultMessage(key: string, msg: string) {
    const header = getModuleHeader(key);

    if (!header || header === undefined || header.length === 0) {
        return;
    }

    setModuleStatusField(header, "module-result", "", msg);
}

function GetModuleResultData(key: string): any {
    return $(`.index-module[data-module-name='${key}']`).find(".module-result").attr("data-api-result");
}

function SetApiResult(moduleName: string, apiStatusStr: string): void {
    try {
        const apiStatus = Convert.toAPIStatus(apiStatusStr);
        if (apiStatus === undefined || apiStatus.Items === undefined || apiStatus.Items.length === 0) {
            SetModuleResultMessage(moduleName, "Empty");
        } else {
            SetModuleResultMessage(moduleName, apiStatus.Items.length.toString());;
        }
    } catch (error) {
        SetModuleResultMessage(moduleName, "Exception");
    }
}

export function UpdateResult(key: string) {
    const data = GetModuleResultData(key);
    console.log(data);

    if (!data || data === undefined || data.length === 0) {
        SetModuleResultMessage(key, "Failed");
        return;
    }

    SetApiResult(key, data);
}

export function RefreshModule(key: string, endpoint: string) {
    SetModuleStatusLoading(key);

    $.ajax({
        url: endpoint,
        success: function (data) {
            var root = $(`.index-module[data-module-name='${key}']`).find(".module-body").first();
            if (!root) {
                return;
            }

            SetModuleStatusSuccess(key);
            root.html(data);
        },
        error: function () {
            SetModuleStatusFailed(key);
        },
        timeout: 5000
    });
}

export function StartModule(key: string, endpoint: string, interval: number): void {
    RefreshModule(key, endpoint);
    setInterval(RefreshModule, interval, key, endpoint);
}