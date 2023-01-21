var countdownHandler = {
    refresh: function () {
        $("#countdown-root .countdown-row").each(function () {
            var dateStr = $(this).attr("date");
            var itemDate = new Date(dateStr);
            var today = new Date();
            var diff = (itemDate.getTime() - today.getTime()) / 1000;

            var days = Math.floor(diff / (60 * 60 * 24));
            diff -= days * (60 * 60 * 24);
            //var hours = Math.floor(diff / 3600);
            //diff -= hours * 3600;
            //var minutes = Math.floor(diff / 60);
            //diff -= minutes * 60;
            //var seconds = Math.floor(diff);

            var name = $(this).attr("name");
            var output = `${dateStr} ${name}: ${days} days`;
            $(this).html(output);
        });
    }
};
countdownHandler.refresh();
setInterval(countdownHandler.refresh, 120 * 1000);
