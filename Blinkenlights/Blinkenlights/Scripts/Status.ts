import { Convert, APIStatus, APIStatusItem } from "./ApiStatus.js"

/*
    Unknown = 0,
    Failed = 1,
    Stale = 2,
    Success = 3
*/
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

function createSourceElement(className: string, source: number, parent: JQuery<HTMLElement>): void {
    if (!parent || !className) {
        return;
    }

        var cicleClass = getSourceClass(source);

    const svg = d3.create('svg');
    svg
        .attr("class", getSourceClass(source))
        .append('circle')
        .attr('cx', '50%')
        .attr('cy', '50%')
        .attr('r', 5);

    const childDiv = jQuery("<div/>", {
        "class": className,
    });

    childDiv.html(svg.node());

    childDiv.appendTo(parent);
}

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

export function SetModuleStatusByFields(Name: string, Status: string, LastUpdate: string, State: number, Source: number): void {
    const root = $("#status-root");
    if (!root) {
        console.log("SetModuleStatus: Failed to find root element");
        return;
    }

    if (!Name) {
        return;
    }

    var statusClass = `status-row ${getStateClass(State)}`;
    var statusRow = $("div[data-status-key='" + Name + "']");
    if (!statusRow || statusRow.html() === undefined) {
        statusRow = jQuery("<div/>", {
            "class": statusClass,
            "data-status-key": Name
        });
    } else {
        statusRow.attr("class", statusClass);
    }

    statusRow.empty();

    createSourceElement("status-source", Source, statusRow);
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
        console.log(data);
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
    if (data.length === 0) {
        const elementId = module.attr("id");
        console.log(elementId);
    }
    SetModuleStatusByStr(data);
};

export function SetModuleError(Name: string, Status: string): void {
    SetModuleStatusByFields(Name, Status, null, 1, 0);
};