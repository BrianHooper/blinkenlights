import { SetModuleStatusByElement } from "./StatusModule.js";

interface WeatherAxis {
    yAxisMin: number;
    yAxisMax: number;
}

interface WeatherData {
    xHour: number;
    temperature: number;
    rain: number;
    humidity: number;
}

const graphSizes = {
    height: 150,
    graphWidth: 90,
    axisWidth: 30,
    translateTop: 10,
    axisTranslateLeft: 30
};

function drawWeatherAxis(divElement, chartYMin, chartYMax) {
    const svg = d3.select("#" + divElement)
        .append("svg")
        .attr("width", graphSizes.axisWidth)
        .attr("height", graphSizes.height + 20)
        .append("g")
        .attr("transform",
            "translate(" + graphSizes.axisTranslateLeft + "," + graphSizes.translateTop + ")");

    const yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([graphSizes.height, 0]);

    const yAxis = d3.axisLeft(yScale)
        .ticks(5, "f");
    svg.style("background-color", "red");

    svg.append("g")
        .attr("class", "axisRed")
        .call(yAxis);
}

function drawDayGraph(points: WeatherData[], chartYMin: number, chartYMax: number, parent: JQuery<HTMLElement>) {
    const translate = "translate(0," + graphSizes.translateTop + ")";

    const svg = d3.create('svg');
    svg.attr("class", "weather-day-svg")
        .attr("width", graphSizes.graphWidth)
        .attr("height", graphSizes.height + 10)
        .append("g")
        .attr("transform", translate);

    const xScale = d3.scaleLinear()
        .domain([0, 23])
        .range([0, graphSizes.graphWidth]);

    const yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([graphSizes.height, 0]);

    const rainColor = "#3095FF";
    const tempColor = "#FF9A30";

    const temperatureData: any[] = points.map(p => [p.xHour, p.temperature]);
    const rainData: any[] = points.map(p => [p.xHour, p.rain]);

    const area = d3.area()
        .x(d => xScale(d[0]))
        .y0(yScale(0))
        .y1(function (d: any) { return yScale(d[1]); });

    const line = d3.line()
        .x(d => xScale(d[0]))
        .y(d => yScale(d[1]));

    svg.append("path")
        .datum(temperatureData)
        .attr("fill", tempColor)
        .attr("fill-opacity", 1)
        .attr("d", area)
        .attr("transform", translate);

    svg.append("path")
        .datum(temperatureData)
        .attr("stroke", "#333333")
        .attr("stroke-width", 2)
        .attr("fill-opacity", 0)
        .attr("d", line)
        .attr("transform", translate);

    svg.append("path")
        .datum(rainData)
        .attr("stroke", rainColor)
        .attr("stroke-width", 4)
        .attr("fill-opacity", 0)
        .attr("d", line)
        .attr("transform", translate);

    parent.html(svg.node());
}

const dataElement = $("#weather-data");
SetModuleStatusByElement(dataElement);

const weatherAxisModel: WeatherAxis = JSON.parse($("#weather-axis").attr("data-weather-axis"));
drawWeatherAxis("weather-axis", weatherAxisModel.yAxisMin, weatherAxisModel.yAxisMax);

$(".weather-day-graph").each(function (d) {
    const modelData = $(this).attr("data-weather-day");
    const points: WeatherData[] = JSON.parse(modelData);
    drawDayGraph(points, weatherAxisModel.yAxisMin, weatherAxisModel.yAxisMax, $(this));
});