// TODO - Web Browser Debug
document.onkeydown = checkKey;

function checkKey(e) {

    e = e || window.event;

    if (e.keyCode == '38') {
        // up arrow
        decrement();
    }
    else if (e.keyCode == '40') {
        // down arrow
        increment();
    }
    else if (e.keyCode == '37') {
        // left arrow
        selectPrevious();
    }
    else if (e.keyCode == '39') {
        // right arrow
        selectNext();
    }
    else if (e.keyCode == '69') {
        // e
        incrementTab();
    }
    else if (e.keyCode == '81') {
        // q
        decrementTab();
    }
}
// TODO - End Web Browser Debug