var moduleHeight = 100;

function drawWeatherAxis(data, divElement, chartYMin, chartYMax) {
    var dataset = data.map(function (d) {
        var dateTimeItem = new Date(0);
        dateTimeItem.setUTCSeconds(d["datetimeEpoch"]);
        return {
            x: dateTimeItem.getHours(),
            temp: d["temp"],
            pop: d["precipprob"]
        }
    });

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

    var x = d3.scaleLinear()
        .domain([0, 23])
        .range([0, width]);
    //var xAxisLabels = ["12:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00"];
    //var xAxis = d3.axisBottom(x)
    //    .ticks(6, "f")
    //    .tickFormat((d, i) => xAxisLabels[d]);
    //svg.append("g")
    //    .attr("transform", "translate(0," + height + ")")
    //    .call(xAxis);

    var y = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([height, 0]);
    var yAxis = d3.axisLeft(y)
        .ticks(5, "f");
    svg.append("g")
        .attr("class", "axisRed")
        .call(yAxis);
}

function drawTemperatureGraph(data, divElement, chartYMin, chartYMax) {
    var dataset = data.map(function (d) {
        var dateTimeItem = new Date(0);
        dateTimeItem.setUTCSeconds(d["datetimeEpoch"]);
        return {
            x: dateTimeItem.getHours(),
            temp: d["temp"],
            pop: d["precipprob"],
            precip: d["humidity"]
        }
    });

    var margin = { top: 5, right:5, bottom: 5, left: 5 },
        width = 70,
        height = moduleHeight;

    var svg = d3.select("#" + divElement)
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");

    var x = d3.scaleLinear()
        .domain([0, 23])
        .range([0, width]);
    //var xAxisLabels = ["12:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00"];
    //var xAxis = d3.axisBottom(x)
    //    .ticks(6, "f")
    //    .tickFormat((d, i) => xAxisLabels[d]);
    //svg.append("g")
    //    .attr("transform", "translate(0," + height + ")")
    //    .call(xAxis);

    var y = d3.scaleLinear()
        .domain([chartYMin, chartYMax])
        .range([height, 0]);
    //var yAxis = d3.axisLeft(y)
    //    .ticks(5, "f");
    //svg.append("g")
    //    .call(yAxis);

    // "#E77818"
    var rainColor = "#3095FF";

    // "#1887E7"
    var tempColor = "#FF9A30";

    svg.append("path")
        .datum(dataset)
        .attr("fill", "#333333")
        .attr("fill-opacity", 0.15)
        .attr("stroke", "#333333")
        .attr("stroke-width", 1.5)
        .attr("stroke-opacity", 0.35)
        .attr("d", d3.area()
            .x(d => x(d.x))
            .y0(y(0))
            .y1(d => y(d.precip)));

    svg.append("path")
        .datum(dataset)
        .attr("fill", tempColor)
        .attr("fill-opacity", 0.75)
        .attr("stroke", "#333333")
        .attr("stroke-width", 1.5)
        .attr("d", d3.area()
            .x(d => x(d.x))
            .y0(y(0))
            .y1(d => y(d.temp)));

    svg.append("path")
        .datum(dataset)
        .attr("fill", rainColor)
        .attr("fill-opacity", 0.75)
        .attr("stroke", "#333333")
        .attr("stroke-width", 1.5)
        .attr("d", d3.area()
            .x(d => x(d.x))
            .y0(y(0))
            .y1(d => y(d.pop)));

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

            drawWeatherAxis(days[0]["hours"], "weather-day-yAxis", chartYMin, chartYMax);
            xAxisDiv.css("margin-left", -1*(xAxisDiv.width() / 2) + "px");

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
        });
    }
};

weatherHandler.refresh();
//setInterval(weatherHandler.refresh, 60 * 1000);