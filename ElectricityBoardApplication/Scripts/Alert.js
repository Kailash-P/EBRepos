function showNotification(text, type, title, sticky) {

    var icon = "fa fa-2x fa-check";
    if (type === "info" || type === "details") {
        icon = "fa fa-2x fa-info";
    }
    if (type === "error") {
        icon = "fa fa-2x fa-exclamation";

    }

    var detailsClass = "";

    if (type === "details") {
        type = "info";
        detailsClass = "detailsMsg";
    }

    if (sticky == true) {
        new PNotify({
            title: title,
            text: text,
            type: type,
            icon: icon,
            hide: false,
            nonblock: false,
            nonblock_opacity: .2,
            styling: "bootstrap3",
            mouse_reset: false,
            addclass: detailsClass,
        });
        return;
    }

    new PNotify({
        title: title,
        nonblock: false,
        nonblock_opacity: .2,
        text: text,
        delay: 2000,
        type: type,
        icon: icon,
        styling: "bootstrap3",
        mouse_reset: false,
        addclass: detailsClass,

    });
}

function hideNotification() {
    PNotify.removeAll();
}