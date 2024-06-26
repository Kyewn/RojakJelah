﻿$(document).ready(function () {
    // Unset overflow hidden from body
    $('body').css("overflow-y", "unset");

    // ********** Tooltip **********
    var timeoutId;
    $("#btnReport").mouseenter(function () {
        timeoutId = setTimeout(function () {
            $("#tooltipReport").css("visibility", "visible");
        }, 700)
    });
    $("#btnReport").mouseleave(function () {
        clearTimeout(timeoutId);
        $("#tooltipReport").css("visibility", "hidden");
    });

    // ********** Trigger search on txtSearch enter **********
    $("#txtSearch").keypress(function (e) {
        if (e.keyCode == 13) {
            $("#lnkSearch")[0].click();
            return false;
        }
    });

    // ********** Maintain scroll position **********
    var dictionaryContainer = $("#divDictionary");
    $(dictionaryContainer).scroll(function () {
        $("#hfScrollPosition").val($(dictionaryContainer)[0].scrollTop);
    });
});


// ********** Set Date ************
const date = document.getElementById('date');
date.innerHTML = new Date().getFullYear();



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



// ********** Custom Dropdown **********
var x, i, j, l, ll, selElmnt, a, b, c;

// Look for any elements with the class "dropdown"
x = document.getElementsByClassName("dropdown");
l = x.length;

for (i = 0; i < l; i++) {
    selElmnt = x[i].getElementsByTagName("select")[0];
    ll = selElmnt.length;

    // For each element, create a new DIV that will act as the selected item
    a = document.createElement("DIV");
    a.setAttribute("class", "dropdown-selected");
    a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
    x[i].appendChild(a);

    // For each element, create a new DIV that will contain the option list
    b = document.createElement("DIV");
    b.setAttribute("class", "dropdown-items dropdown-hide");

    for (j = 1; j < ll; j++) {
        /* For each option in the original select element,
        create a new DIV that will act as an option item */
        c = document.createElement("DIV");
        c.innerHTML = selElmnt.options[j].innerHTML;

        c.addEventListener("click", function (e) {
            /* When an item is clicked, update the original select box,
            and the selected item: */
            var y, i, k, s, h, sl, yl;
            s = this.parentNode.parentNode.getElementsByTagName("select")[0];
            sl = s.length;
            h = this.parentNode.previousSibling;

            for (i = 0; i < sl; i++) {
                if (s.options[i].innerHTML == this.innerHTML) {
                    s.selectedIndex = i;
                    h.innerHTML = this.innerHTML;
                    y = this.parentNode.getElementsByClassName("same-as-selected");
                    yl = y.length;

                    for (k = 0; k < yl; k++) {
                        y[k].removeAttribute("class");
                    }

                    this.setAttribute("class", "same-as-selected");
                    break;
                }
            }
            h.click();
        });

        b.appendChild(c);
    }

    x[i].appendChild(b);

    a.addEventListener("click", function (e) {
        /* When the select box is clicked, close any other select boxes,
        and open/close the current select box: */
        e.stopPropagation();
        closeAllSelect(this);
        this.nextSibling.classList.toggle("dropdown-hide");
        this.classList.toggle("select-arrow-active");
    });
}

function closeAllSelect(elmnt) {
    /* A function that will close all select boxes in the document,
    except the current select box: */
    var x, y, i, xl, yl, arrNo = [];
    x = document.getElementsByClassName("dropdown-items");
    y = document.getElementsByClassName("dropdown-selected");
    xl = x.length;
    yl = y.length;
    for (i = 0; i < yl; i++) {
        if (elmnt == y[i]) {
            arrNo.push(i)
        } else {
            y[i].classList.remove("select-arrow-active");
        }
    }
    for (i = 0; i < xl; i++) {
        if (arrNo.indexOf(i)) {
            x[i].classList.add("dropdown-hide");
        }
    }
}
/* If the user clicks anywhere outside the select box,
then close all select boxes: */
document.addEventListener("click", closeAllSelect);



// ********** Notification **********
function closeNotification(notification) {
    notification.css("display", "none");
}



// ********** Modal **********
function showModal(selectedModal) {
    $(document.body).css("overflow", "hidden");
    selectedModal.css("display", "flex");
    $("#dlgReport").css("animation", "slideIn .3s ease-out forwards");
    $("#dlgSuggestion").css("animation", "slideIn .3s ease-out forwards");
}

function closeModal(selectedModal) {
    $(document.body).css("overflow", "auto");
    selectedModal.css("display", "none");
}

function confirmDelete(event, controlID) {
    if (confirm("This not only removes the entry from the dictionary, but also from the translator's dataset. This action cannot be undone. Are you sure?")) {
        __doPostBack(controlID, "");
    } else {
        event.preventDefault();
    }
}