var CalendarHandler = {
    refresh: function (): void {
        $.get("/Modules/GetCalendarModule", function (data) {
            var root = $("#calendar-root");
            if (!root) {
                return;
            }

            root.html(data);
        });
    }
};

CalendarHandler.refresh();
setInterval(CalendarHandler.refresh, 15 * 60 * 1000);