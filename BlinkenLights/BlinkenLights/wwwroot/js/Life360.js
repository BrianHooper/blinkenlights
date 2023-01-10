var refreshModules = function () {

    $.get("/Root/GetLife360Locations", function (data) {
        $("#locations").html(data);
    });
}

$(document).ready(function () {
    setInterval(refreshModules, 60 * 1000);
    refreshModules();
});