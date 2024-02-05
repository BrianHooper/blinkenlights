//import { SetModuleStatusByElement } from "./StatusModule.js";
//setModuleStatusByElement($("#slideshow-status"));

var SlideshowRunner = {
    refresh: async function (): Promise<void> {
        const frames: JQuery<HTMLDivElement> = $("#stage");
        const children = frames.children();
        const count = children.length;

            for (var i = 0; i < count; i++) {

                /*
    
                wait for slideshow length
                set current z-index to 10
                set next to visible
                set next z-index to 20;
                start fading in opacity of next
                start fading out opacity of current
                wait for opacity animation
                set current to hidden
                */
                var nextI = i + 1;
                if (nextI >= count) {
                    nextI = 0;
                }

                const current = children[i];
                const next = children[nextI];

                children[nextI].classList.remove("slideshow_hide");
                children[nextI].classList.add("slideshow_show");

                children[i].classList.remove("slideshow_ztwenty");
                children[i].classList.add("slideshow_zten");

                children[nextI].classList.remove("slideshow_zten");
                children[nextI].classList.add("slideshow_ztwenty");

                children[nextI].classList.remove("slideshow_fade_out");
                children[nextI].classList.add("slideshow_fade_in");

                children[i].classList.remove("slideshow_fade_in");
                children[i].classList.add("slideshow_fade_out");

                await new Promise(resolve => setTimeout(resolve, 10000));

                children[i].classList.remove("slideshow_show");
                children[i].classList.add("slideshow_hide");


        }
    }
};

SlideshowRunner.refresh();
setInterval(SlideshowRunner.refresh, 20 * 1000);