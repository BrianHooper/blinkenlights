var clockHandler = {
    refresh: function () {

        $.get("/Modules/GetWorldClockModule", function (data) {
            $("#worldclock-root").html(data);
            $(".clock-time").each(function (i, divElement) {
                var offset = parseInt(divElement.getAttribute("offset"));
                var d = new Date();
                d.setTime(d.getTime() + (offset * 1000));
                var dateStr = d.toLocaleString("en-US", {
                    timeZone: "Etc/GMT",
                    hour: "2-digit",
                    minute: "2-digit"
                });
                divElement.innerHTML = dateStr;
            });
        });
    }
};
clockHandler.refresh();
setInterval(clockHandler.refresh, 60 * 1000);
