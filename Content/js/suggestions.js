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
const btnEditCancel = document.getElementById("btnEditCancel");
btnEditCancel.addEventListener('click', (e) => {
    confirmCancelEdit(e);
});

const btnAccept = document.getElementById("btnAccept");
btnAccept.addEventListener('click', (e) => {
    confirmApprove(e);
});

const btnReject = document.getElementById("btnReject");
btnReject.addEventListener('click', (e) => {
    confirmReject(e);
});

function confirmCancelEdit(e) {
    if (confirm("Are you sure you want to cancel editing?")) {
        __doPostBack('btnEditCancel', '');
    } else {
        e.preventDefault();
    }
};

function confirmApprove(e) {
    if (confirm("Are you sure you want to approve this suggestion?")) {
        __doPostBack('btnAccept', '');
    } else {
        e.preventDefault();
    }
};

function confirmReject(e) {
    if (confirm("Are you sure you want to reject this suggestion?")) {
        __doPostBack('btnReject', '');
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