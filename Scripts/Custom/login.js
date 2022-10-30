$(document).ready(function () {
    // Hide navbar and footer
    $(".navbar").css("display", "none");
    $("footer").css("display", "none");
});

function closeNotification(notification) {
    notification.css("display", "none");
}