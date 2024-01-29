var StatusHandler = {
    refresh: function (): void {
        $.get("/Modules/GetStatusModule", function (data) {
            $("#status-root").html(data);
        });
    }
};

StatusHandler.refresh();
setInterval(StatusHandler.refresh, 3 * 60 * 60 * 1000);