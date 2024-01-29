var IssTrackerHandler = {
    refresh: function (): void {
        $.get("/Modules/GetIssTrackerModule", function (data) {
            $("#iss-root").html(data);
        });
    }
};

IssTrackerHandler.refresh();
setInterval(IssTrackerHandler.refresh, 3 * 60 * 60 * 1000);