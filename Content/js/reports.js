//  List item click
//  List item click
//  List item click
const listItemContainer = document.querySelector('.listItemContainer');
listItemContainer.addEventListener('click', (e) => {
    const listItem = e.target.closest('.listItem');

    if (listItem.classList.contains("noData")) {
        return;
    } else {
        // Send selected index to server for populating action menu on the right with information
        const selectedItemId = listItem.querySelector(".topRow .itemDetail span:nth-child(2)");

        $('#txtSelectedListItem').val(selectedItemId.innerText);
        __doPostBack('txtSelectedListItem');
    }
});

//  Confirmation dialogs
//  Confirmation dialogs
//  Confirmation dialogs
const btnResolve = document.getElementById("btnResolve");
btnResolve.addEventListener('click', (e) => {
    confirmResolve(e);
});

const btnClose = document.getElementById("btnClose");
btnClose.addEventListener('click', (e) => {
    confirmClose(e);
});

const btnRestore = document.getElementById("btnRestore");
btnRestore.addEventListener('click', (e) => {
    confirmRestore(e);
});

function confirmResolve(e) {
    if (confirm("Are you sure you want to resolve this issue?")) {
        __doPostBack('btnResolve', '');
    } else {
        e.preventDefault();
    }
};

function confirmClose(e) {
    if (confirm("Are you sure you want to close this issue?")) {
        __doPostBack('btnClose', '');
    } else {
        e.preventDefault();
    }
};

function confirmRestore(e) {
    if (confirm("Are you sure you want to restore this issue for reviewing again?")) {
        __doPostBack('btnRestore', '');
    } else {
        e.preventDefault();
    }
};

//  Close Notification
//  Close Notification
//  Close Notification

function closeNotification(notification) {
    notification.css("display", "none");
}