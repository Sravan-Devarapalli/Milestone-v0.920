function scrollingDropdown_onclick(control, type,pluralform,pluralformType) {
    var temp = 0;
    var text = "";
    if (pluralform == undefined || pluralform == null || pluralform == '') {
        pluralform = "s";
    }
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
                if (pluralformType == undefined || pluralformType == null || pluralformType == '') {
                    pluralformType = type.toString() + pluralform;
                }
                text = "Multiple " + pluralformType + " selected";

            }
            if (arrayOfCheckBoxes[0].checked) {
                text = arrayOfCheckBoxes[0].parentNode.childNodes[1].innerHTML;
            }
            if (temp === 0) {
                text = "Please Choose " + type.toString() + "("+pluralform +")";
            }
        }
        text = DecodeString(text);
        if (text.length > 33) {
            text = text.substr(0, 31) + "..";
        }
        scrollingDropdownList.parentNode.children[1].children[0].firstChild.nodeValue = text;
    }
}

