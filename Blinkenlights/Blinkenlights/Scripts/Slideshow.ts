var SlideshowHandler = {
    refresh: function (): void {
        $.get("/Modules/GetSlideshowModule", function (data) {
            $("#slideshow-root").html(data);
        });
    }
};

SlideshowHandler.refresh();
setInterval(SlideshowHandler.refresh, 3 * 60 * 60 * 1000);