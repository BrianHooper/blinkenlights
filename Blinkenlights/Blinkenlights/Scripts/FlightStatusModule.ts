interface FlightData {
    origin: string;
    destination: string;
    departureRelative: string;
    arrivalRelative: string;
    duration: string;
    aircraft: string;
    airline: string;
    fnia: string;
}

function SetFlightDataRow(key: string, value: string, container: JQuery<HTMLElement>): void {
    const row = $("<div/>", { "class": "flightstatus-overlay-row" });
    $("<div/>", { "class": "flightstatus-overlay-key" }).html(key).appendTo(row);
    $("<div/>", { "class": "flightstatus-overlay-value" }).html(value).appendTo(row);
    row.appendTo(container);
}

function SetFlightData(flightData: FlightData, container: JQuery<HTMLElement>): void {
    SetFlightDataRow("Airline:", flightData.airline, container);
    SetFlightDataRow("Origin:", flightData.origin, container);
    SetFlightDataRow("Destination:", flightData.destination, container);
    SetFlightDataRow("Departed:", flightData.departureRelative, container);
    SetFlightDataRow("Arriving:", flightData.arrivalRelative, container);
    SetFlightDataRow("Duration:", flightData.duration, container);
    SetFlightDataRow("Aircraft:", flightData.aircraft, container);
}

export function AddEventHandlers() {
    $(".flightstatus-plane").each(function (d) {
        $(this).mouseenter(function () {
            const flightName = $(this).attr("data-flight-name");
            const flightIcon = $(`.flightstatus-row[data-flight-name='${flightName}']`).first();
            if (!flightIcon || flightIcon === null) {
                return;
            }
            $(this).addClass("flightstatus-plane-highlight");
            flightIcon.addClass("flightstatus-row-highlight");
        })
            .mouseleave(function () {
                const flightName = $(this).attr("data-flight-name");
                const flightIcon = $(`.flightstatus-row[data-flight-name='${flightName}']`).first();
                if (!flightIcon || flightIcon === null) {
                    return;
                }
                $(this).removeClass("flightstatus-plane-highlight");
                flightIcon.removeClass("flightstatus-row-highlight");
            });
    });

    $(".flightstatus-row").each(function (d) {
        const flightName = $(this).attr("data-flight-name");
        const overlaycontainer = $(this).find(".flightstatus-overlay-container").first();
        const overlay = $(this).find(".flightstatus-overlay").first();

        $(this).mouseenter(function () {
            overlaycontainer.addClass("flightstatus-overlay-show");
            const flightIcon = $(`.flightstatus-plane[data-flight-name='${flightName}']`).first();
            if (!flightIcon || flightIcon === null) {
                return;
            }
            $(this).addClass("flightstatus-row-highlight");
            flightIcon.addClass("flightstatus-plane-highlight");
        })
        .mouseleave(function () {
            overlaycontainer.removeClass("flightstatus-overlay-show");
            const flightIcon = $(`.flightstatus-plane[data-flight-name='${flightName}']`).first();
            if (!flightIcon || flightIcon === null) {
                return;
            }
            $(this).removeClass("flightstatus-row-highlight");
            flightIcon.removeClass("flightstatus-plane-highlight");
        });

        
        $.ajax({
            url: `/Modules/GetSingleFlightData`,
            type: "GET",
            data: { fid: flightName },
            success: function (flightDataJson) {
                overlay.html("");
                const flightData: FlightData = JSON.parse(flightDataJson);
                SetFlightData(flightData, overlay);
            },
            error: function (error) {
                overlay.html(error.responseText);
            },
            timeout: 30000
        });
    });

}
