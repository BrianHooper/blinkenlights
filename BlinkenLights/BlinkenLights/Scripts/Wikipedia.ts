var WikiHandler = {
    refresh: function (): void {
        $.get("/Modules/GetWikipediaData", function (data) {
            var wiki = JSON.parse(data);
            if (wiki["Error"]) {
                $("#wiki-root").html(wiki["Error"]);
                return;
            }
            $("#wiki-root").html(data);
        });
    }
};

WikiHandler.refresh();
setInterval(WikiHandler.refresh, 3 * 60 * 60 * 1000);