function btnOk_Click(okButton, hdnSelectedIndexes, cbl) {
    var str = "";
    var cblList = document.getElementById(cbl).getElementsByTagName('input');

    for (var i = 0; i < cblList.length; i++) {
        if (cblList[i].checked) {
            str += i.toString() + "_";
        }
    }

    document.getElementById(hdnSelectedIndexes).value = str;
    document.getElementById(okButton).click();
}

function btnCancelButton_Click(filterdiv) {
    var divObject = document.getElementById(filterdiv);
    divObject.style.display = 'none';
    return false;
}

function uncheckAllCheckBoxes(cblList) {
    for (var i = 0; i < cblList.length; i++) {
        cblList[i].checked = false;
    }

}

function Filter_Click(filterdiv, selectedIndexes, cbl, txtSearchBox) {

    var divObject = document.getElementById(filterdiv);
    divObject.style.display = '';
    var watermark = $find(txtSearchBox);
    watermark.set_Text("");
    filterTableRows(txtSearchBox, cbl, true);
    var indexesArray = selectedIndexes.split('_');
    var cblList = document.getElementById(cbl).getElementsByTagName('input');

    uncheckAllCheckBoxes(cblList);

    for (var i = 0; i < indexesArray.length; i++) {
        if (indexesArray[i] != '')
            cblList[indexesArray[i]].checked = true;
    }
}
