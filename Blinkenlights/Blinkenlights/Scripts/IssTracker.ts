var IssTrackerHandler = {
    refresh: function (): void {
        console.log("Refreshed ISS Module");
        $.get("/Modules/GetIssTrackerModule", function (data) {
            $("#iss-root").html(data);
        });
    }
};

IssTrackerHandler.refresh();
setInterval(IssTrackerHandler.refresh, 30 * 1000);