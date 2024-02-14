//import { SetModuleStatusByElement } from "./StatusModule.js";
//setModuleStatusByElement($("#slideshow-status"));

const slideshowframes: JQuery<HTMLDivElement> = $("#slideshow-root");
const children = slideshowframes.children();

let curIdx = children.length - 1;
let nxtIdx = 0;

var SlideshowRunner = {
    refresh: async function (): Promise<void> {
        // set z-index of next to 1
        children[nxtIdx].classList.add("stacked");

        // start fading in next to visible
        children[nxtIdx].classList.add("visible");
        await new Promise(resolve => setTimeout(resolve, 1500));

        // start fading out previous
        children[curIdx].classList.remove("visible");
        children[curIdx].classList.add("visible-end");
        await new Promise(resolve => setTimeout(resolve, 1500));

        // after fades are complete, set current to invisible
        children[curIdx].classList.remove("visible-end");

        // set z-index of next to 0
        children[nxtIdx].classList.remove("stacked");

        curIdx++;
        nxtIdx++;

        if (curIdx >= children.length) {
            curIdx = 0;
        }

        if (nxtIdx >= children.length) {
            nxtIdx = 0;
        }
    }
};

if (children.length > 1) {
    SlideshowRunner.refresh();
    setInterval(SlideshowRunner.refresh, 10 * 1000);
} else if (children.length == 1) {
    children[0].classList.add("visible");
}