import { SetModuleStatusByElement } from "./Status.js";

var currentIndex = 0;
SetModuleStatusByElement($("#slideshow-status"));

var SlideshowRunner = {
    refresh: function (): void {
        const frames: JQuery<HTMLDivElement> = $(".slideshow-image");

        if (!frames || frames.length === 0) {
            return;
        }

        const prevElement = frames[currentIndex];
        if (!prevElement) {
            return;
        }
        prevElement.classList.remove("slideshow-image-fade-visible");
        setTimeout(function () {
            prevElement.classList.remove("slideshow-image-visible");
            currentIndex++;
            if (currentIndex >= frames.length) {
                currentIndex = 0;
            }

            const nextElement = frames[currentIndex];
            if (!nextElement) {
                return;
            }
            nextElement.classList.add("slideshow-image-visible");
            setTimeout(function () {
                nextElement.classList.add("slideshow-image-fade-visible");
            }, 1000);

        }, 1000);


    }
};

SlideshowRunner.refresh();
setInterval(SlideshowRunner.refresh, 5 * 1000);