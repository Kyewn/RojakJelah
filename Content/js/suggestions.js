// Main view
// Main view
// Main view
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

// Modal window
// Modal window
// Modal window
const modalWindow = document.getElementById('editModalWindow');
const body = document.querySelector('body');
modalWindow.addEventListener('click', (e) => {
    body.style.pointerEvents = 'none';
})

function closeNotification(notification) {
    notification.css("display", "none");
}