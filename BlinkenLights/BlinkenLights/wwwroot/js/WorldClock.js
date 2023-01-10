
function calcTime(timezoneStr) {
    var date = new Date();
    var options = {
        
    };

    return date.toLocaleTimeString("en-us", options);
}

var refreshClocks = function () {
    //$("#clock-seattle").html(calcTime("Atlantic/South_Georgia"));

    $('.clock-time').each(function (i, divElement) {
        var offset = parseInt(divElement.getAttribute("offset"));
        var d = new Date();
        d.setTime(d.getTime() + (offset * 1000));
        const dateStr = d.toLocaleString('en-US', {
            timeZone: "Etc/GMT",
            hour: "2-digit",
            minute: "2-digit"
        })
        divElement.innerHTML = dateStr;
    });
    //console.log(x);
};

$(document).ready(function () {
    setInterval(refreshModules, 60 * 1000);
    refreshClocks();
});