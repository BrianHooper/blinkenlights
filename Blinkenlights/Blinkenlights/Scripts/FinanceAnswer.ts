var FinanceAnswerHandler = {
    refresh: function (): void {
        $.get("/Modules/GetFinanceAnswerModule", function (data) {
            $("#fin-root").html(data);
        });
    }
};

FinanceAnswerHandler.refresh();
setInterval(FinanceAnswerHandler.refresh, 3 * 60 * 60 * 1000);