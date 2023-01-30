var HeadlinesHandler = {
    refresh: function (): void {
        $.get("/Modules/GetHeadlinesModule", function (data) {
            $("#headlines-root").html(data);
        });
    }
};

HeadlinesHandler.refresh();
setInterval(HeadlinesHandler.refresh, 3 * 60 * 60 * 1000);