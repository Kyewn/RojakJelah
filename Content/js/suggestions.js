const listItemContainer = document.querySelector('.listItemContainer');
listItemContainer.addEventListener('click', (e) => {
    const listItem = e.target.closest('.listItem');

    //Populate action menu on the right with information
});


// Modal window
// Modal window
// Modal window
const modalWindow = document.getElementById('editModalWindow');
const body = document.querySelector('body');
modalWindow.addEventListener('click', (e) => {
    body.style.pointerEvents = 'none';
})
