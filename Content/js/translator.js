function closeNotification(notification) {
    notification.css("display", "none");
}

function showNotification(icon, title, message) {
    $("#MainContent_notificationIcon").attr("class", icon);
    $("#MainContent_notificationTitle").text(title);
    $("#MainContent_notificationMessage").text(message);
    $("#MainContent_notification").css("display", "block");
}