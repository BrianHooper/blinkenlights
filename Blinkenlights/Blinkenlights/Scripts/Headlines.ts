var HeadlinesHandler = {
    refresh: function (): void {
        console.log("Refreshed Headlines Module");
        $.get("/Modules/GetHeadlinesModule", function (data) {
            $("#headlines-root").html(data);
        });
    }
};

HeadlinesHandler.refresh();
setInterval(HeadlinesHandler.refresh, 3 * 60 * 60 * 1000);