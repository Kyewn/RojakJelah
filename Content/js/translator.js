// ********** set date ************
const date = document.getElementById('date');
date.innerHTML = new Date().getFullYear();
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