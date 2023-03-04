var WWIIHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWWIIModule", function (data) {
            var root = $("#wwii-root");
            if (!root) {
                return;
            }

            root.html(data);
        });
    }
};

WWIIHandler.refresh();
setInterval(WWIIHandler.refresh, 15 * 60 * 1000);