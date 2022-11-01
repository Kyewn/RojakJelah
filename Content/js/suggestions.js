const listItemContainer = document.querySelector('.listItemContainer');
listItemContainer.addEventListener('click', (e) => {
    const listItem = e.target.closest('.listItem');
    console.log(listItem);
});

const modalWindow = document.getElementById('modalWindow');
modalWindow.addEventListener('click', (e) => {
    console.log("test");
    const body = document.querySelector('body');
    body.style.pointerEvents = 'none';
})