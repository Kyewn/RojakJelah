const listItemContainer = document.querySelector('.listItemContainer');
listItemContainer.addEventListener('click', (e) => {
    const listItem = e.target.closest('.listItem');
    console.log(listItem);
});