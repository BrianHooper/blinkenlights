var weatherHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWeatherData", function (data) {
            var weatherData = JSON.parse(data);

            console.log(weatherData);
            if (weatherData["Error"]) {
                $("#weather-error").html(weatherData["Error"]);
                return;
            }

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