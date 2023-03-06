var AstronomyHandler = {
    refresh: function (): void {
        $.get("/Modules/GetAstronomyModule", function (data) {
            $("#astronomy-root").html(data);
        });
    }
};

AstronomyHandler.refresh();
setInterval(AstronomyHandler.refresh, 3 * 60 * 60 * 1000);