//Master page processes
window.addEventListener('load', () => {
    const navLinks = document.querySelectorAll('.links li a');
    if (window.location.pathname == "/Translator" || window.location.pathname == "/") {
        document.querySelector(".nav-header").style.visibility = 'hidden';
        navLinks[0].classList.add('active');
    } else if (window.location.pathname == "/Dictionary") {
        navLinks[1].classList.add('active');
    } else if (window.location.pathname == "/About") {
        navLinks[2].classList.add('active');
    }
});

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
    if (window.location.pathname == "/Translator") {
        if (scrollHeight > navHeight) {
            document.querySelector(".nav-header").style.visibility = 'visible';
        } else {
            document.querySelector(".nav-header").style.visibility = 'hidden';
        }
    }
    if (scrollHeight > 500) {
        topLink.classList.add('show-link');
    } else {
        topLink.classList.remove('show-link');
    }
})