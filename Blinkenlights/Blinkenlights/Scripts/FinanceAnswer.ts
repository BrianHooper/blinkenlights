var FinanceAnswerHandler = {
    refresh: function (): void {
        console.log("Refreshed Finance Module");
        $.get("/Modules/GetFinanceAnswerModule", function (data) {
            $("#fin-root").html(data);
        });
    }
};

FinanceAnswerHandler.refresh();
setInterval(FinanceAnswerHandler.refresh, 3 * 60 * 60 * 1000);