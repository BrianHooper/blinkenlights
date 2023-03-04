import { SetModuleStatusByFields } from "./Status.js";

interface WeatherData {
    xHour: number;
    temperature: number;
    rain: number;
    humidity: number;
}

var moduleHeight = 100;

function drawWeatherAxis(divElement, chartYMin, chartYMax) {
    var margin = { top: 5, right: 5, bottom: 5, left: 30 },
        width = 0,
        height = moduleHeight;

    var svg = d3.select("#" + divElement)
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");

    var yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([height, 0]);

    var yAxis = d3.axisLeft(yScale)
        .ticks(5, "f");

    svg.append("g")
        .attr("class", "axisRed")
        .call(yAxis);
}

function drawTemperatureGraph(data, divElement, chartYMin, chartYMax) {
    var dataset: WeatherData[] = data.map(function (data: any): WeatherData {
        var dateTimeItem = new Date(0);
        dateTimeItem.setUTCSeconds(data["datetimeEpoch"]);
        return {
            xHour: dateTimeItem.getHours(),
            temperature: data["temp"],
            rain: data["precipprob"],
            humidity: data["humidity"]
        }
    });

    var margin = { top: 5, right: 5, bottom: 5, left: 5 },
        width = 70,
        height = moduleHeight;

    var svg = d3.select("#" + divElement)
        .append("svg")
        .attr("class", "weather-day-svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");

    var xScale = d3.scaleLinear()
        .domain([0, 23])
        .range([0, width]);

    var yScale = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([height, 0]);

    var rainColor = "#3095FF";
    var tempColor = "#FF9A30";
    var humidityColor = "#000000";

    const temperatureData: any[] = dataset.map(p => [p.xHour, p.temperature]);
    const humidityData: any[] = dataset.map(p => [p.xHour, p.humidity]);
    const rainData: any[] = dataset.map(p => [p.xHour, p.rain]);

    console.log(humidityData);

    const area = d3.area()
        .x(d => xScale(d[0]))
        .y0(yScale(0))
        .y1(function (d: any) { return d[1]; });

    svg.append("path")
        .datum(humidityData)
        .attr("class", "weather-humidity weather-humidity-fill")
        .attr("fill", humidityColor)
        .attr("fill-opacity", 1.0)
        .attr("d", area);

    svg.append("path")
        .datum(temperatureData)
        .attr("class", "weather-temperature weather-temperature-fill")
        .attr("fill", tempColor)
        .attr("fill-opacity", 0.75)
        .attr("d", area);

    svg.append("path")
        .datum(rainData)
        .attr("class", "weather-rain weather-rain-fill")
        .attr("fill", rainColor)
        .attr("fill-opacity", 0.35)
        .attr("d", area);

    svg.append("path")
        .datum(humidityData)
        .attr("class", "weather-humidity weather-outline")
        //.attr("fill-opacity", 0.0)
        //.attr("stroke", "#333333")
        //.attr("stroke-width", 1.5)
        .attr("d", area);

    svg.append("path")
        .datum(rainData)
        .attr("class", "weather-rain weather-outline")
        //.attr("fill-opacity", 0.0)
        //.attr("stroke", "#333333")
        //.attr("stroke-width", 1.5)
        .attr("d", area);

    svg.append("path")
        .datum(temperatureData)
        .attr("class", "weather-temperature weather-outline")
        //.attr("fill-opacity", 0.0)
        //.attr("stroke", "#333333")
        //.attr("stroke-width", 1.5)
        .attr("d", area);
}

function addCurrentBlock(parent, iconStr, resultStr) {
    var block = jQuery("<div/>", {
        "class": "weather-current-item",
    }).appendTo(parent);

    var iconBlock = jQuery("<div/>", {
        "class": "weather-current-icon",
    }).html(iconStr).appendTo(block);

    var textBlock = jQuery("<div/>", {
        "class": "weather-current-text",
    }).html(resultStr).appendTo(block);
}

var weatherHandler = {
    refresh: function () {
        $.get("/Modules/GetWeatherData", function (weatherApiResponse) {
            //SetModuleStatusByFields("Weather", 1, "Weather", null, "Data response is null");
            if (!weatherApiResponse) {
                $("#weather-error").html("Invalid API response");
                return;
            }

            var weatherData = JSON.parse(weatherApiResponse);

            if (weatherData["Error"]) {
                $("#weather-error").html(weatherData["Error"]);
                return;
            }

            var parent = $("#weather-temp");
            parent.html("");

            var days = weatherData["days"].slice(0, 5);
            var minTemperature = Math.min(...days.map(d => Math.min(...d["hours"].map(h => h["temp"]))));
            var maxTemperature = Math.max(...days.map(d => Math.max(...d["hours"].map(h => h["temp"]))));

            var chartYMin = minTemperature < 0 ? minTemperature - 10 : 0;
            var chartYMax = maxTemperature > 100 ? maxTemperature + 10 : 100;

            var xAxisDiv = jQuery("<div/>", {
                "id": "weather-day-yAxis",
                "class": "weather-day-wrapper",
            }).appendTo(parent);

            drawWeatherAxis("weather-day-yAxis", chartYMin, chartYMax);
            xAxisDiv.css("margin-left", -1 * (xAxisDiv.width() / 2) + "px");

            var forecastDiv = jQuery("<div/>", {
                "id": "weather-forecast",
            }).appendTo(parent);

            days.map((day, idx) => {
                var divId = "weather-day-" + idx;
                var dayDiv = jQuery("<div/>", {
                    "id": divId,
                    "class": "weather-day-wrapper",
                }).appendTo(forecastDiv);

                drawTemperatureGraph(day["hours"], divId, chartYMin, chartYMax);

                var labelDiv = jQuery("<div/>", {
                    "class": "weather-day-label",
                }).appendTo(dayDiv);

                var dateTime = new Date(0);
                dateTime.setUTCSeconds(day["datetimeEpoch"]);
                labelDiv.html(dateTime.toLocaleString("en-us", { weekday: "long" }));

                var icon = day["icon"];
                var iconPath = `/images/weather/${icon}.svg`;
                var iconImg = `<img src="${iconPath}" width="40px;" height="40px;" />`
                var iconDiv = jQuery("<div/>", {
                    "class": "weather-day-icon",
                }).appendTo(dayDiv);
                iconDiv.html(iconImg);
            });

            var xAxisDivRight = jQuery("<div/>", {
                "id": "weather-day-current",
                "class": "weather-day-wrapper",
            }).appendTo(parent);


            // TODO Build these in WeatherModule.cshtml
            var chanceRain = parseInt(weatherData["currentConditions"]["precipprob"]) + "%";
            addCurrentBlock(xAxisDivRight, "☂", chanceRain);

            var temperature = parseInt(weatherData["currentConditions"]["temp"]) + "° F";
            addCurrentBlock(xAxisDivRight, "🌡", temperature);

            var humidity = parseInt(weatherData["currentConditions"]["humidity"]) + "%";
            addCurrentBlock(xAxisDivRight, "H", humidity);

            var moonphase = weatherData["currentConditions"]["moonphase"];
            addCurrentBlock(xAxisDivRight, "☾", moonphase);

            var sunsetTime = new Date(0);
            sunsetTime.setUTCSeconds(weatherData["currentConditions"]["sunsetEpoch"]);
            var sunsetTimeStr = sunsetTime.toLocaleString("en-us", { hour: "numeric", minute: "numeric" });
            addCurrentBlock(xAxisDivRight, "🌇", sunsetTimeStr);

            var windspeed = weatherData["currentConditions"]["windspeed"] + " mph";
            addCurrentBlock(xAxisDivRight, "🍃", windspeed);

            SetModuleStatusByFields("VisualCrossingWeather", "Success", "", 3, 2);

            $("#weather-humidity-button").hover(function (d) {
                $(".weather-temperature").addClass("weather-hidden");
            });




            $("#weather-humidity-button").mouseenter(function (d) {
                $(".weather-temperature").addClass("weather-hidden");
                $(".weather-rain").addClass("weather-hidden");
                $(".weather-humidity").addClass("weather-highlight");
            }).mouseleave(function (d) {
                $(".weather-temperature").removeClass("weather-hidden");
                $(".weather-rain").removeClass("weather-hidden");
                $(".weather-humidity").removeClass("weather-highlight");
            });

            $("#weather-rain-button").mouseenter(function (d) {
                $(".weather-temperature").addClass("weather-hidden");
                $(".weather-rain").addClass("weather-highlight");
                $(".weather-humidity").addClass("weather-hidden");
            }).mouseleave(function (d) {
                $(".weather-temperature").removeClass("weather-hidden");
                $(".weather-rain").removeClass("weather-highlight");
                $(".weather-humidity").removeClass("weather-hidden");
            });

            $("#weather-temperature-button").mouseenter(function (d) {
                $(".weather-temperature").addClass("weather-highlight");
                $(".weather-rain").addClass("weather-hidden");
                $(".weather-humidity").addClass("weather-hidden");
            }).mouseleave(function (d) {
                $(".weather-temperature").removeClass("weather-highlight");
                $(".weather-rain").removeClass("weather-hidden");
                $(".weather-humidity").removeClass("weather-hidden");
            });
        });
    }
};

weatherHandler.refresh();
setInterval(weatherHandler.refresh, 60 * 1000);