$(document).ready(function () {
    // ********** Tooltip **********
    var savedTimeoutId;
    $("#lnkViewSavedTranslations").mouseenter(function () {
        reportTimeoutId = setTimeout(function () {
            $("#tooltipSavedTranslations").css("visibility", "visible");
        }, 700)
    });
    $("#lnkViewSavedTranslations").mouseleave(function () {
        clearTimeout(reportTimeoutId);
        $("#tooltipSavedTranslations").css("visibility", "hidden");
    });

    var historyTimeoutId;
    $("#lnkViewTranslationHistory").mouseenter(function () {
        reportTimeoutId = setTimeout(function () {
            $("#tooltipTranslationHistory").css("visibility", "visible");
        }, 700)
    });
    $("#lnkViewTranslationHistory").mouseleave(function () {
        clearTimeout(reportTimeoutId);
        $("#tooltipTranslationHistory").css("visibility", "hidden");
    });

    var reportTimeoutId;
    $("#lnkMakeReport").mouseenter(function () {
        reportTimeoutId = setTimeout(function () {
            $("#tooltipReport").css("visibility", "visible");
        }, 700)
    });
    $("#lnkMakeReport").mouseleave(function () {
        clearTimeout(reportTimeoutId);
        $("#tooltipReport").css("visibility", "hidden");
    });

    var saveTimeoutId;
    $("#lnkSaveTranslation").mouseenter(function () {
        reportTimeoutId = setTimeout(function () {
            $("#tooltipSave").css("visibility", "visible");
        }, 700)
    });
    $("#lnkSaveTranslation").mouseleave(function () {
        clearTimeout(reportTimeoutId);
        $("#tooltipSave").css("visibility", "hidden");
    });
});

function closeModal(selectedModal) {
    selectedModal.css("display", "none");
    $(document.body).css("overflow-y", "auto");
}

function closeNotification(notification) {
    notification.css("display", "none");
}

function confirmDelete(event, controlID) {
    if (confirm("This is an irreversible action. Are you sure?")) {
        __doPostBack(controlID, "");
    } else {
        event.preventDefault();
    }
}