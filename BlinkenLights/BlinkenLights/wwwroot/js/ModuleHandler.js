var refreshModules = function () {
    $("#wwii").load("/Root/GetWWIIModule");
    $("#weather").load("/Root/GetWeatherModule");
    $("#wikipedia").load("/Root/GetWikipediaModule");
};

$(document).ready(function () {
    setInterval(refreshModules, 60 * 1000);
    refreshModules();
    $("#worldclock").load("/Root/GetWorldClockModule");
    $("#life360").load("/Root/GetLife360Module");
});