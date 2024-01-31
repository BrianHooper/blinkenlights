//import { SetModuleStatusByElement } from "./StatusModule.js";

//SetModuleStatusByElement($("#fin-status"));

interface FinanceData {
    X: number;
    Y: number;
}

const financeGraphSizes = {
    height: 150,
    graphWidth: 350,
    axisWidth: 40,
    translateTop: 0,
    axisTranslateLeft: 30
};

function drawAxis(chartYMin, chartYMax, parent: JQuery<HTMLElement>) {
    const childDiv = jQuery("<div/>", {
        "class": "finance-axis",
        "id": "abcd",
    });

    childDiv.appendTo(parent);

    const svg = d3.select("#" + "abcd")
        .append("svg")
        .attr("width", financeGraphSizes.axisWidth)
        .attr("height", financeGraphSizes.height)
        .append("g")
        .attr("transform",
            "translate(" + financeGraphSizes.axisTranslateLeft + "," + financeGraphSizes.translateTop + ")");

    const yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([financeGraphSizes.height, 0]);

    const yAxis = d3.axisLeft(yScale)
        .ticks(5, "f");

    svg.append("g")
        .attr("class", "finance-axis-g")
        .call(yAxis);
}

function drawGraph(points: FinanceData[], chartYMin: number, chartYMax: number, parent: JQuery<HTMLElement>) {
    const translate = "translate(0," + 0 + ")";

    const svg = d3.create('svg');
    svg.attr("class", "finance-graph-svg")
        .attr("width", financeGraphSizes.graphWidth)
        .attr("height", financeGraphSizes.height)
        .append("g")
        .attr("transform", translate);

    var chartXMax = Math.max(...points.map(p => p.X));
    const xScale = d3.scaleLinear()
        .domain([0, chartXMax])
        .range([0, financeGraphSizes.graphWidth]);

    const priceData: any[] = points.map(p => [p.X, p.Y]);

    const yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([financeGraphSizes.height, 0]);

    const tempColor = "#3095FF";

    const area = d3.area()
        .x(d => xScale(d[0]))
        .y0(function (d: any) { return yScale(d[0]); })
        .y1(function (d: any) { return yScale(d[1]); });

    const line = d3.line()
        .x(d => xScale(d[0]))
        .y(d => yScale(d[1]));

    svg.append("path")
        .datum(priceData)
        .attr("fill", tempColor)
        .attr("fill-opacity", 1)
        .attr("d", area)
        .attr("transform", translate);

    svg.append("path")
        .datum(priceData)
        .attr("stroke", "#333333")
        .attr("stroke-width", 4)
        .attr("fill-opacity", 0)
        .attr("d", line)
        .attr("transform", translate);

    const childDiv = jQuery("<div/>", {
        "class": "finance-graph",
    });

    childDiv.html(svg.node());
    childDiv.appendTo(parent);
}

$(".finance-block").each(function (d) {
    const modelData = $(this).attr("data-finance-graph");
    const points: FinanceData[] = JSON.parse(modelData);
    const yValues: number[] = points.map(p => p.Y);
    var chartYMin = Math.min(...yValues);
    var chartYMax = Math.max(...yValues);
    const chartYOffset = Math.abs(chartYMax - chartYMin) * 0.1;
    chartYMin -= chartYOffset;
    chartYMax += chartYOffset;

    drawAxis(chartYMin, chartYMax, $(this));
    drawGraph(points, chartYMin, chartYMax, $(this));
});