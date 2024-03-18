export function RefreshLife360Utility() {
    var container = $("#util-l360-tdelta");
    var timeStr = container.attr("data-time-delta");
    var time = new Date(timeStr);

    var dateStr = time.toLocaleString("en-US", {
        timeZone: "America/Los_Angeles",
        hour: "numeric",
        minute: "2-digit"
    });

    var now = new Date();
    var diff = Math.round((now.getTime() - time.getTime()) / 1000);

    var hours = 0;
    if (diff > 3600) {
        hours = Math.floor(diff / 3600);
    }
    diff = diff % 3600;

    var minutes = 0;
    if (diff > 60) {
        minutes = Math.floor(diff / 60);
    }

    container.html(`${("0" + hours).slice(-2)}:${("0" + minutes).slice(-2)}, ${dateStr}`);
}