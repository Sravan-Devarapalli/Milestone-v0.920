function scrollingDropdown_onclick(control, type) {
    var temp = 0;
    var text = "";
    var scrollingDropdownList = document.getElementById(control.toString());
    var arrayOfCheckBoxes = scrollingDropdownList.getElementsByTagName("input");
    if (arrayOfCheckBoxes.length == 1 && arrayOfCheckBoxes[0].disabled) {
        text = "No " + type.toString() + "s to select.";
    }
    else {
        for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
            if (arrayOfCheckBoxes[i].checked) {
                temp++;
                text = arrayOfCheckBoxes[i].parentNode.childNodes[1].innerHTML;
            }
            if (temp > 1) {
                text = "Multiple " + type.toString() + "s selected";

            }
            if (arrayOfCheckBoxes[0].checked) {
                text = arrayOfCheckBoxes[0].parentNode.childNodes[1].innerHTML;
            }
            if (temp === 0) {
                text = "Please Choose " + type.toString() + "(s)";
            }
        }
        if (text.length > 32) {
            text = text.substr(0, 30) + "..";
        }
        scrollingDropdownList.parentNode.children[1].children[0].firstChild.nodeValue = text;
    }
}
