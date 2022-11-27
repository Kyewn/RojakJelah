//Master page processes
window.addEventListener('load', () => {
    // ********** set date ************
    const date = document.getElementById('date');
    date.innerHTML = new Date().getFullYear();

    if (window.location.pathname == "/Translator" || window.location.pathname == "/") {
        document.querySelector(".nav-header").style.visibility = 'hidden';
    }

    const navLinks = document.querySelectorAll('.mobileLinks li');

    for (let i = 0; i < navLinks.length; i++) {
        // ********** remove padding for hidden links ************
        let linkDisplay = navLinks[i].querySelector('a')?.style.display
        if (linkDisplay == 'none' || !(navLinks[i].querySelector('a') instanceof HTMLAnchorElement)) {
            navLinks[i].style.padding = 0;
        }
    }
});

// ********** close links ************
const navBar = document.querySelector('.nav-center');
const body = document.querySelector('body');
const links = document.querySelector('.mobileLinks');

// Hide mobile nav menu on body click
body.addEventListener('click', (e) => {
    if (links.classList.contains('show-links') && !e.target.closest('nav')) {
        links.classList.remove('show-links');
        // Prevent postback
        e.preventDefault();
    }
});

navBar.addEventListener('click', (e) => {
    if (e.target.closest('.nav-toggle')) {
        links.classList.toggle('show-links');
        // Prevent postback
        e.preventDefault();
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