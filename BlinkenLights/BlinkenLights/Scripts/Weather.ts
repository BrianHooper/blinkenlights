var weatherHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWeatherData", function (data) {
            var weatherData = JSON.parse(data);
            var hourlyTimestamps = weatherData["hourly"].map(function (h) { return h["dt"]; });
            var hourlyChangeOfRain = weatherData["hourly"].map(function (h) { return h["pop"]; });
            // parse the date / time
            var parseTime = d3.timeParse("%d-%b-%y");
            //debugger;
        });
    }
};

weatherHandler.refresh();
setInterval(weatherHandler.refresh, 60 * 1000);