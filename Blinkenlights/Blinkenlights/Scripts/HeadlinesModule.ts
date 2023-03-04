import { SetModuleStatusByElement } from "./Status.js";

$(".headlines-status").each(function () {
    SetModuleStatusByElement($(this));
});
