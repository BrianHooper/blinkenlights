var WWIIHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWWIIModule", function (data) {
            $("#wwii-root").html(data);
        });
    }
};

WWIIHandler.refresh();
setInterval(WWIIHandler.refresh, 15 * 60 * 1000);