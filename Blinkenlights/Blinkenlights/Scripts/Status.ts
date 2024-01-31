var StatusHandler = {
    refresh: function (): void {
        console.log("Refreshed Status Module");
        $.get("/Modules/GetStatusModule", function (data) {
            $("#status-root").html(data);
        });
    }
};

StatusHandler.refresh();
setInterval(StatusHandler.refresh, 3 * 60 * 60 * 1000);