function GetModuleStatus(handle) {
    var moduleName = $(".module-name", handle).attr("name");
    var statusHandler = $(".status-report", handle);
    if (statusHandler) {
        var status = statusHandler.attr("report");
        if (status) {
            return `${moduleName} - ${status}`;
        }
    }
    return moduleName;
}

$(".index-module").each(function () {
    $(this).hover(
        function () {
            var status = GetModuleStatus(this);
            $(".module-name", this).html(status);
            $(".module-name", this).removeClass("module-name-hide");
        }, function () {
            $(".module-name", this).addClass("module-name-hide");
        }
    );
});