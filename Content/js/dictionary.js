console.log(34566)
// ********** set date ************
const date = document.getElementById('date');
date.innerHTML = new Date().getFullYear();

// ********** close links ************
const navToggle = document.querySelector('.nav-toggle');
const linksContainer = document.querySelector('.links-container');
const links = document.querySelector('.links');

navToggle.addEventListener('click', () => {
    // linksContainer.classList.toggle('show-links')

    //add extra links
    const containerHeight = linksContainer.getBoundingClientRect().height;
    const linksHeight = links.getBoundingClientRect().height;
    if (containerHeight === 0) {
        linksContainer.style.height = `${linksHeight}px`
    } else {
        linksContainer.style.height = 0;
    }
});

const navbar = document.getElementById('nav');
const topLink = document.querySelector('.top-link');
// ********** fixed navbar ************
window.addEventListener('scroll', () => {
    const scrollHeight = window.pageYOffset;
    const navHeight = navbar.getBoundingClientRect().height;
    if (scrollHeight > navHeight) {
        navbar.classList.add('fixed-nav');
    } else {
        navbar.classList.remove('fixed-nav');
    }
    if (scrollHeight > 500) {
        topLink.classList.add('show-link')
    } else {
        topLink.classList.remove('show-link')
    }
})


// Active Menu 
function addClassElementEvent(element, className, event) {

    let elements = document.querySelectorAll(element);

    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener(event, function (event) {
            [].forEach.call(elements, function (el) {
                el.classList.remove(className);
            });
            this.classList.add(className);
        });
    }
}

addClassElementEvent('.list-group-item', 'active', 'click');



// Add Suggest Terms Form 
// Add Suggest Successfully Message
var myModal = new bootstrap.Modal(
    document.getElementById("addSuggestItem"),
    {
        keyboard: false,
    }
);
function addSuggest() {
    var successMessage = document.getElementById('successMessage');
    myModal.hide();
    successMessage.style.display = "block";
    setTimeout(() => {
        successMessage.style.display = "none";
    }, 3000);

    document.getElementById('submitForm').reset()
}

// Close Message 
function clsBtn() {
    var successMessage = document.getElementById('successMessage');
    if (successMessage.style.display = "block") {
        successMessage.style.display = "none";
    }
}


