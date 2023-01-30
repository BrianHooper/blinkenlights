var MehHandler = {
    refresh: function (): void {
        $.get("/Modules/GetMehData", function (data) {
            var meh = JSON.parse(data);
            if (meh["Error"]) {
                $("#meh-root").html(meh["Error"]);
                return;
            }

            var root = $("#meh-root");
            if (!root) {
                return;
            }

            $(root).attr("report", "This is a status report");


            $("#meh-title").html(meh["deal"]["title"]);

            $("#meh-price").html("$" + meh["deal"]["items"][0]["price"]);

            var textBlock = jQuery("<img/>", {
                "height": 150,
                "width": 150,
                "src": meh["deal"]["photos"][0],
            }).appendTo("#meh-image");
        });
    }
};

MehHandler.refresh();
setInterval(MehHandler.refresh, 120 * 60 * 1000);