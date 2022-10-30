// ********** set date ************
const date = document.getElementById('date');
date.innerHTML = new Date().getFullYear();

// ********** close links ************
const navToggle = document.querySelector('.nav-toggle');
const linksContainer = document.querySelector('.links-container');
const links = document.querySelector('.links');


document.addEventListener('load', () => {
    if (window.location.pathname == "/Translator") {
        document.querySelector(".nav-header").style.visibility = 'hidden';
    }
});

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

// Top Show Input Value 
function getTextBoxValue() {
    var result;
    result = document.getElementById("inputValue").value;
    //alert(st);
    document.getElementById("inputValueResult").value = result;
}

// Bottom Show Input Value 
function getTextBoxValue2() {
    var result2;
    result2 = document.getElementById("inputValue2").value;
    //alert(st);
    document.getElementById("inputValueResult2").value = result2;
}