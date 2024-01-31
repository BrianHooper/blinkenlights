var SlideshowHandler = {
    refresh: function (): void {
        console.log("Refreshed Slideshow Module");
        $.get("/Modules/GetSlideshowModule", function (data) {
            $("#slideshow-root").html(data);
        });
    }
};

SlideshowHandler.refresh();
setInterval(SlideshowHandler.refresh, 3 * 60 * 60 * 1000);