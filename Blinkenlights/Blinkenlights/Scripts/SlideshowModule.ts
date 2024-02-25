//import { SetModuleStatusByElement } from "./StatusModule.js";
//setModuleStatusByElement($("#slideshow-status"));

var slideshowframes: JQuery<HTMLElement> = $("#slideshow-root").children();

let curIdx = slideshowframes.length - 1;
let nxtIdx = 0;

var SlideshowRunner = {
    refresh: async function (): Promise<void> {
        slideshowframes = $("#slideshow-root").children();

        // set z-index of next to 1
        slideshowframes[nxtIdx].classList.add("stacked");

        // start fading in next to visible
        slideshowframes[nxtIdx].classList.add("visible");
        await new Promise(resolve => setTimeout(resolve, 1500));

        // start fading out previous
        slideshowframes[curIdx].classList.remove("visible");
        slideshowframes[curIdx].classList.add("visible-end");
        await new Promise(resolve => setTimeout(resolve, 1500));

        // after fades are complete, set current to invisible
        slideshowframes[curIdx].classList.remove("visible-end");

        // set z-index of next to 0
        slideshowframes[nxtIdx].classList.remove("stacked");

        curIdx++;
        nxtIdx++;

        if (curIdx >= slideshowframes.length) {
            curIdx = 0;
        }

        if (nxtIdx >= slideshowframes.length) {
            nxtIdx = 0;
        }
    }
};

if (slideshowframes.length > 1) {
    SlideshowRunner.refresh();
    setInterval(SlideshowRunner.refresh, 10 * 1000);
} else if (slideshowframes.length == 1) {
    slideshowframes[0].classList.add("visible");
}