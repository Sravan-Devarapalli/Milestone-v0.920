function filterTableRows(searchtextBoxId, tblControlId, isCheckBoxList) {

    var searchtextBox = null;
    var tblControl = null;

    if (typeof (searchtextBoxId) == "string") {
        searchtextBox = document.getElementById(searchtextBoxId);
    }
    else {
        searchtextBox = searchtextBoxId;
    }

    if (typeof (tblControlId) == "string") {
        tblControl = document.getElementById(tblControlId);
    }
    else {
        tblControl = tblControlId;
    }

    if (tblControl != null && searchtextBox != null) {
        var trControls = tblControl.getElementsByTagName('tr');
        var searchText = searchtextBox.value.toLowerCase();
        for (var i = 0; i < trControls.length; i++) {

            var rowText = '';
            if (isCheckBoxList) {
                var checkBox = trControls[i].children[0].getElementsByTagName('input')[0];
                rowText = checkBox.parentNode.children.length > 1 ? checkBox.parentNode.children[1].innerHTML.toLowerCase():'';
            }
            else {
                rowText = trControls[i].children[0].innerHTML.toLowerCase();
            }

            if (rowText.length >= searchText.length && rowText.substr(0, searchText.length) == searchText) {

                trControls[i].style.display = "";
            }
            else {

                trControls[i].style.display = "none";
            }
        }
        changeAlternateitemsForTable(trControls);
    }
}

function changeAlternateitemsForTable(trControls) {
    var index = 0;
    for (var i = 0; i < trControls.length; i++) {
        if (trControls[i].style.display != 'none') {
            index++;
            if ((index) % 2 == 0) {
                trControls[i].style.backgroundColor = '#f9faff';
            }
            else {
                trControls[i].style.backgroundColor = '';
            }
        }
    }

}
