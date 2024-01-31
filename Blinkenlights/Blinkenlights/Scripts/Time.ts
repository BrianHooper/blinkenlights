var clockHandler = {
    refresh: function () {
        console.log("Refreshed Time Module");
        $.get("/Modules/GetTimeModule", function (data) {
            $("#time-root").html(data);

            $(".time-worldclock-time").each(function () {
                var offset = parseInt($(this).attr("offset"));
                var d = new Date();
                d.setTime(d.getTime() + (offset * 1000));
                
                var dateStr = d.toLocaleString("en-US", {
                    timeZone: "Etc/GMT",
                    hour: "numeric",
                    minute: "2-digit"
                });
                $(this).html(dateStr);
            });

            $(".time-countdown-remaining").each(function () {
                var dateStr = $(this).attr("endDate");
                var itemDate = new Date(dateStr);
                var today = new Date();
                var diff = (itemDate.getTime() - today.getTime()) / 1000;

                var days = Math.floor(diff / (60 * 60 * 24));
                $(this).html(days.toString() + " Days");
            });
        });
    }
};
clockHandler.refresh();
setInterval(clockHandler.refresh, 60 * 1000);
