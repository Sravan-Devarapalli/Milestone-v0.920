<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityListControl.ascx.cs"
    Inherits="PraticeManagement.Controls.Opportunities.OpportunityListControl" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc2" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/Opportunities/ProposedResources.ascx" TagName="ProposedResources"
    TagPrefix="uc" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="Scripts/date.js" type="text/javascript"></script>
<script src="Scripts/datepicker.js" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    var isStrawmansExistForCurrentOpportunity = "false";
    var isProposedPersonsExistForCurrentOpportunity = "false";
    var image_icon1 = null;
    var image_icon2 = null;
    var CurrentOpportunityPriorityName;
    var currenthdnProposedPersonsIndexesId = "";
    var currenthdnTeamStructurePersonsId = "";
    var refreshOpportunityIdsFromLastRefresh = new Array();
    var CurrentOptyStartDate = "";
    var CurrentOptyEndDate = "";

    function setcalendar() {
        $('.date-pick').datePicker({ autoFocusNextInput: true });
    }

    function AddStrawmanRow() {

        var tblTeamStructure = document.getElementById('tblTeamStructure');
        var rowCount = tblTeamStructure.rows.length;
        var row = tblTeamStructure.insertRow(rowCount - 1);
        var cell1 = row.insertCell(0);
        var ddlStrawMan = document.createElement("select");

        ddlStrawManOriginal = tblTeamStructure.rows[0].cells[0].children[0];
        ddlStrawMan.style.width = ddlStrawManOriginal.style.width;
        for (var i = 0; i < ddlStrawManOriginal.children.length; i++) {
            var option = document.createElement("option");
            option.text = ddlStrawManOriginal.children[i].text;
            option.value = ddlStrawManOriginal.children[i].value;
            try {
                ddlStrawMan.add(option, null); //Standard    
            } catch (error) {
                ddlStrawMan.add(option); // IE only    
            }
        }

        cell1.appendChild(ddlStrawMan);

        var cell2 = row.insertCell(1);
        cell2.align = "center";
        var ddlQuantity = document.createElement("select");
        ddlQuantityOriginal = tblTeamStructure.rows[0].cells[1].children[0];
        ddlQuantity.setAttribute("onchange", "this.style.backgroundColor = '';");
        ddlQuantity.style.width = ddlQuantityOriginal.style.width;
        for (var i = 0; i < ddlQuantityOriginal.children.length; i++) {
            var option = document.createElement("option");
            option.text = ddlQuantityOriginal.children[i].text;
            option.value = ddlQuantityOriginal.children[i].value;
            try {
                ddlQuantity.add(option, null); //Standard    
            } catch (error) {
                ddlQuantity.add(option); // IE only    
            }
        }

        cell2.appendChild(ddlQuantity);

        var cell3 = row.insertCell(2);
        var txtNeedBy = document.createElement("input");
        txtNeedBy.type = "text";
        txtNeedBy.style.width = tblTeamStructure.rows[0].cells[2].children[0].style.width;
        txtNeedBy.style.cssText = txtNeedBy.style.cssText + ";float:left;";
        txtNeedBy.className = "date-pick";
        txtNeedBy.readOnly = true;
        cell3.appendChild(txtNeedBy);
        if (CurrentOptyStartDate != '')
            txtNeedBy.value = new Date(CurrentOptyStartDate).format('MM/dd/yyyy');

        setcalendar();
        return false;
    }

    function ShowTeamStructureModal(image) {

        clearErrorMessages();
        image_icon1 = image;

        var anothorImage = document.getElementById(image.attributes['anothorImageId'].value);
        image_icon2 = anothorImage;

        var oppId = image.attributes['opportunityid'].value;

        isProposedPersonsExistForCurrentOpportunity = image.attributes['hasproposedpersons'].value;
        isStrawmansExistForCurrentOpportunity = image.attributes['hasstrawmans'].value;
        var ddlPriority = document.getElementById(image.attributes['ddlPriorityId'].value);

        var optionList = ddlPriority.getElementsByTagName('option');

        var selectedText = "";

        for (var i = 0; i < optionList.length; ++i) {
            if (optionList[i].value == ddlPriority.value) {
                selectedText = optionList[i].innerHTML.toLowerCase();
                break;
            }
        }

        CurrentOpportunityPriorityName = selectedText;

        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var hdnClickedRowIndex = document.getElementById('<%=hdnClickedRowIndex.ClientID %>');
        hdnClickedRowIndex.value = image.attributes['RowIndex'].value;

        var attachedTeam = image.parentNode.children[1].value.split(",");
        CurrentOptyStartDate = image.parentNode.children[2].value;
        CurrentOptyEndDate = image.parentNode.children[3].value;
        currenthdnTeamStructurePersonsId = image.parentNode.children[1].id;
        var refreshLableParentNode = $(image.parentNode.parentNode);
        var lblRefreshMessage = refreshLableParentNode.find("span[id$='lblRefreshMessage']")[0];
        lblRefreshMessage.style.display = 'block';
        Array.add(refreshOpportunityIdsFromLastRefresh, oppId);
        var tblTeamStructure = document.getElementById('tblTeamStructure');

        for (var i = tblTeamStructure.rows.length - 2; i >= 0; i--) {
            if (i == 0) {
                var ddlPerson = tblTeamStructure.rows[i].cells[0].children[0];
                var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
                var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];

                txtNeedBy.style.backgroundColor = "";
                ddlQuantity.style.backgroundColor = "";

                ddlPerson.value = 0;
                ddlQuantity.value = 0;
                if (CurrentOptyStartDate != '')
                    txtNeedBy.value = new Date(CurrentOptyStartDate).format('MM/dd/yyyy');
                txtNeedBy.className = "date-pick";
                txtNeedBy.readOnly = true;
                setcalendar();
            }
            else {
                tblTeamStructure.deleteRow(i);
            }
        }
        var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        for (var i = 0; i < attachedTeam.length - 1; i++) {
            if (i > 0) {
                AddStrawmanRow();
            }
            var ddlPerson = tblTeamStructure.rows[i].cells[0].children[0];
            var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
            var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];
            var personString = attachedTeam[i];

            //Strawmans' info is separated  by ","s.
            // Each Strawman info is in the format PersonId:PersonType|Quantity?NeedBy

            ddlPerson.value = personString.substring(0, personString.indexOf(":", 0));
            ddlQuantity.value = personString.substring(personString.indexOf("|", 0) + 1, personString.indexOf("?", 0));
            var needByDate = new Date(personString.substring(personString.indexOf("?", 0) + 1, personString.length));
            txtNeedBy.value = needByDate.format('MM/dd/yyyy');

        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        $find("behaviorIdTeamStructure").show();
        return false;
    }


    function ShowPotentialResourcesModal(image) {
        clearErrorMessagesForTeamResources();
        var oppId = image.attributes['opportunityid'].value;
        image_icon1 = image;

        var anothorImage = document.getElementById(image.attributes['anothorImageId'].value);
        image_icon2 = anothorImage;

        isProposedPersonsExistForCurrentOpportunity = image.attributes['hasproposedpersons'].value;
        isStrawmansExistForCurrentOpportunity = image.attributes['hasstrawmans'].value;
        var ddlPriority = document.getElementById(image.attributes['ddlPriorityId'].value);

        var optionList = ddlPriority.getElementsByTagName('option');

        var selectedText = "";

        for (var i = 0; i < optionList.length; ++i) {
            if (optionList[i].value == ddlPriority.value) {
                selectedText = optionList[i].innerHTML.toLowerCase();
                break;
            }
        }

        CurrentOpportunityPriorityName = selectedText;


        var hdnCurrentOpportunityId = document.getElementById('<%=hdnCurrentOpportunityId.ClientID %>');
        var hdnClickedRowIndex = document.getElementById('<%=hdnClickedRowIndex.ClientID %>');
        hdnClickedRowIndex.value = image.attributes['RowIndex'].value;

        var attachedResourcesIndexes = image.parentNode.children[1].value.split(",");
        currenthdnProposedPersonsIndexesId = image.parentNode.children[1].id;

        var refreshLableParentNode = $(image.parentNode.parentNode);
        var lblRefreshMessage = refreshLableParentNode.find("span[id$='lblRefreshMessage']")[0];
        lblRefreshMessage.style.display = 'block';
        Array.add(refreshOpportunityIdsFromLastRefresh, oppId);
        var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');

        //        $find("wmBhOutSideResources").set_Text(image.parentNode.children[2].value);
        $find("wmbhSearchBox").set_Text('');

        for (var i = 0; i < trPotentialResources.length; i++) {
            var checkBox = trPotentialResources[i].children[0].getElementsByTagName('input')[0];
            var strikeCheckBox = trPotentialResources[i].children[1].getElementsByTagName('input')[0];
            checkBox.checked = checkBox.disabled = strikeCheckBox.checked = strikeCheckBox.disabled = false;
            trPotentialResources[i].style.display = "";
            for (var j = 0; j < attachedResourcesIndexes.length; j++) {
                var indexString = attachedResourcesIndexes[j];
                var index = indexString.substring(0, indexString.indexOf(":", 0));
                var checkBoxType = indexString.substring(indexString.indexOf(":", 0) + 1, indexString.length);
                if (i == index && index != '') {
                    if (checkBoxType == 1) {
                        checkBox.checked = true;
                        strikeCheckBox.disabled = true;
                    }
                    else {
                        strikeCheckBox.checked = true;
                        checkBox.disabled = true;
                    }
                    break;
                }
            }
        }
        hdnCurrentOpportunityId.value = image.attributes["OpportunityId"].value;
        $find("behaviorIdPotentialResources").show();
        return false;
    }

    function GetProposedPersonIdsListWithPersonType() {
        var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        var hdnProposedPersonIdsList = document.getElementById("<%= hdnProposedResourceIdsWithTypes.ClientID%>");
        var PersonIdList = '';
        var personIndexesList = '';
        if (cblPotentialResources != null) {
            for (var i = 0; i < potentialCheckboxes.length; ++i) {
                if (potentialCheckboxes[i].checked) {
                    PersonIdList += potentialCheckboxes[i].parentNode.attributes['personid'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                    personIndexesList += potentialCheckboxes[i].parentNode.attributes['itemIndex'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                }
            }
        }

        if (PersonIdList == "") {
            isProposedPersonsExistForCurrentOpportunity = "false";
            image_icon1.attributes['hasproposedpersons'].value = "false";
            image_icon2.attributes['hasproposedpersons'].value = "false";
        }
        else {
            isProposedPersonsExistForCurrentOpportunity = "true";
            image_icon1.attributes['hasproposedpersons'].value = "true";
            image_icon2.attributes['hasproposedpersons'].value = "true";
        }

        hdnProposedPersonIdsList.value = PersonIdList;
        var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
        hdnProposedPersonsIndexes.value = personIndexesList;
    }

    function UpdateTeamStructureForHiddenfields() {
        var hdnTeamStructure = document.getElementById("<%= hdnTeamStructure.ClientID%>");
        var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        var PersonIdList = '';
        var personType;
        var array = new Array();
        for (var i = 0; i < trTeamStructure.length - 1; i++) {
            var ddlPerson = trTeamStructure[i].children[0].getElementsByTagName('SELECT')[0];
            var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
            var txtNeedBy = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
            var obj = null;
            if (ddlPerson.value == "0" || ddlQuantity.value == "0" || txtNeedBy.value == '')
                continue;
            for (var j = 0; j < array.length; j++) {
                if (array[j].personId == ddlPerson.value && array[j].needBy == (new Date(txtNeedBy.value)).format('MM/dd/yyyy')) {
                    obj = array[j];
                    break;
                }
            }
            if (obj == null) {
                obj = {
                    "personId": ddlPerson.value,
                    "needBy": txtNeedBy.value,
                    "quantity": ddlQuantity.value
                }
                Array.add(array, obj);
            }
            else {
                obj.quantity = parseInt(obj.quantity) + parseInt(ddlQuantity.value);
            }
        }
        for (var i = 0; i < array.length; i++) {
            personType = '1';
            PersonIdList = PersonIdList + array[i].personId + ':' + personType + '|' + array[i].quantity + '?' + array[i].needBy + ',';
        }

        if (PersonIdList == "") {
            isStrawmansExistForCurrentOpportunity = "false";
            image_icon1.attributes['hasstrawmans'].value = "false";
            image_icon2.attributes['hasstrawmans'].value = "false";
        }
        else {
            isStrawmansExistForCurrentOpportunity = "true";
            image_icon1.attributes['hasstrawmans'].value = "true";
            image_icon2.attributes['hasstrawmans'].value = "true";
        }

        var currenthdnTeamStructurePersons = document.getElementById(currenthdnTeamStructurePersonsId);
        hdnTeamStructure.value = currenthdnTeamStructurePersons.value = PersonIdList;
    }


    function SetisProposedPersonsExistForCurrentOpportunity() {

        var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
        var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        var PersonIdList = '';

        if (cblPotentialResources != null) {
            for (var i = 0; i < potentialCheckboxes.length; ++i) {
                if (potentialCheckboxes[i].checked) {
                    PersonIdList += potentialCheckboxes[i].parentNode.attributes['personid'].value + ':' + potentialCheckboxes[i].parentNode.attributes['persontype'].value + ',';
                }
            }
        }

        if (PersonIdList == "") {
            isProposedPersonsExistForCurrentOpportunity = "false";
        }
        else {
            isProposedPersonsExistForCurrentOpportunity = "true";
        }
    }

    function SetisStrawmansExistForCurrentOpportunity() {
        var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        var PersonIdList = '';
        var personType;
        var array = new Array();
        for (var i = 0; i < trTeamStructure.length - 1; i++) {
            var ddlPerson = trTeamStructure[i].children[0].getElementsByTagName('SELECT')[0];
            var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
            var txtNeedBy = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
            var obj = null;
            if (ddlPerson.value == "0" || ddlQuantity.value == "0" || txtNeedBy.value == '')
                continue;
            for (var j = 0; j < array.length; j++) {
                if (array[j].personId == ddlPerson.value && array[j].needBy == (new Date(txtNeedBy.value)).format('MM/dd/yyyy')) {
                    obj = array[j];
                    break;
                }
            }
            if (obj == null) {
                obj = {
                    "personId": ddlPerson.value,
                    "needBy": txtNeedBy.value,
                    "quantity": ddlQuantity.value
                }
                Array.add(array, obj);
            }
            else {
                obj.quantity = parseInt(obj.quantity) + parseInt(ddlQuantity.value);
            }
        }
        for (var i = 0; i < array.length; i++) {
            personType = '1';
            PersonIdList = PersonIdList + array[i].personId + ':' + personType + '|' + array[i].quantity + '?' + array[i].needBy + ',';
        }

        if (PersonIdList == "") {
            isStrawmansExistForCurrentOpportunity = "false";
        }
        else {
            isStrawmansExistForCurrentOpportunity = "true";
        }
    }

    function validateOpportunity() {
        if (CurrentOpportunityPriorityName == "po" || CurrentOpportunityPriorityName == "a" || CurrentOpportunityPriorityName == "b") {
            if (isProposedPersonsExistForCurrentOpportunity == "false" && isStrawmansExistForCurrentOpportunity == "false") {
                return false;
            }

        }

        return true;
    }

    function saveProposedResources() {
        SetisProposedPersonsExistForCurrentOpportunity();
        if (validateOpportunity()) {
            var buttonSave = document.getElementById('<%=btnSaveProposedResourcesHidden.ClientID %>');
            var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
            var hdnProposedPersonsIndexes = document.getElementById(currenthdnProposedPersonsIndexesId);
            GetProposedPersonIdsListWithPersonType();
            buttonSave.click();
            return true;
        }
        else {
            showOpportunityErrorMessageForTeamResources();
            return false;
        }
    }

    function saveTeamStructure() {
        clearErrorMessages();
        var tblTeamStructure = document.getElementById('tblTeamStructure');
        for (var i = 0; i < tblTeamStructure.rows.length - 1; i++) {
            var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
            var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];
            txtNeedBy.style.backgroundColor = "";
            ddlQuantity.style.backgroundColor = "";
        }
        SetisStrawmansExistForCurrentOpportunity();
        if (validateAll()) {
            var buttonSave = document.getElementById('<%=btnSaveTeamStructureHidden.ClientID %>');
            var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
            UpdateTeamStructureForHiddenfields();
            buttonSave.click();
            return true;
        }
        else {
            return false;
        }
    }

    function validateAll() {
        var result1 = true;
        var result2 = true;
        var result3 = true;

        // Validate NeedByDate
        result1 = validateNeedByDates();

        if (!result1) {

            showNeedbyErrorMessage();
        }

        // Validate Quantity
        result2 = validateQuantity();

        if (!result2) {
            showQuantityErrorMessage();

        }

        result3 = validateOpportunity();

        if (!result3) {
            showOpportunityErrorMessage();

        }

        return result1 && result2 && result3;
    }

    function validateNeedByDates() {
        var result = true;
        var OpportunityStartDate = CurrentOptyStartDate;
        var OpportunityEndDate = CurrentOptyEndDate;

        var tblTeamStructure = document.getElementById('tblTeamStructure');

        for (var i = 0; i < tblTeamStructure.rows.length - 1; i++) {

            var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
            var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];

            if (ddlQuantity.value != 0 && OpportunityStartDate != '') {
                var dateval = new Date(txtNeedBy.value);

                if (!(dateval >= new Date(OpportunityStartDate) && ((OpportunityEndDate == '') || (dateval <= new Date(OpportunityEndDate))))) {
                    txtNeedBy.style.backgroundColor = "Red";
                    result = false;
                }
                else {
                    txtNeedBy.style.backgroundColor = "";
                }

            }
            else {
                txtNeedBy.style.backgroundColor = "";
            }
        }
        return result;
    }

    function validateQuantity() {
        var result = true;

        var tblTeamStructure = document.getElementById('tblTeamStructure');

        for (var i = 0; i < tblTeamStructure.rows.length - 1; i++) {

            var ddlPerson = tblTeamStructure.rows[i].cells[0].children[0];
            var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
            var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];

            if (ddlPerson.value != 0 && ddlQuantity.value != 0 && txtNeedBy.value != '') {
                var quantityCount = parseInt(ddlQuantity.value);

                var ddlQuantityObjectList = new Array();


                for (var j = 0; j < tblTeamStructure.rows.length - 1; j++) {
                    if (i != j) {
                        var ddlPersonObject = tblTeamStructure.rows[j].cells[0].children[0];
                        var ddlQuantityObject = tblTeamStructure.rows[j].cells[1].children[0];
                        var txtNeedByObject = tblTeamStructure.rows[j].cells[2].children[0];

                        if (ddlPerson.value == ddlPersonObject.value && txtNeedBy.value == txtNeedByObject.value) {
                            quantityCount += parseInt(ddlQuantityObject.value);
                            Array.add(ddlQuantityObjectList, ddlQuantityObject);
                        }

                    }

                    if (quantityCount > 10) {

                        for (var k = 0; k < ddlQuantityObjectList.length; k++) {

                            ddlQuantityObjectList[k].style.backgroundColor = "red";
                        }

                        result = false;
                    }

                }

            }

        }

        return result;
    }

    function showOpportunityErrorMessageForTeamResources() {
        document.getElementById('divTeamResources').style.display = "block";
    }

    function showOpportunityErrorMessage() {
        document.getElementById('divOpportunityError').style.display = "block";
    }

    function showQuantityErrorMessage() {
        document.getElementById('divQuantityError').style.display = "block";
    }

    function showNeedbyErrorMessage() {
        var errormsg = "Need by Date must be greater than or equals to Opportunity start date (" + CurrentOptyStartDate + ") and less than or equals to Opportunity end date (" + CurrentOptyEndDate + ") .";

        var divNeedBy = document.getElementById('divNeedbyError');
        divNeedBy.innerHTML = errormsg;
        divNeedBy.style.display = "";
    }

    function clearErrorMessages() {
        document.getElementById('divNeedbyError').style.display = "none";
        document.getElementById('divQuantityError').style.display = "none";
        document.getElementById('divOpportunityError').style.display = "none";
    }

    function clearErrorMessagesForTeamResources() {
        document.getElementById('divTeamResources').style.display = "none";
    }


    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

    function endRequestHandle(sender, Args) {
        var lblRefreshMessageList = $("span[id$='lblRefreshMessage']");
        for (var j = 0; j < lblRefreshMessageList.length; j++) {
            for (var i = 0; i < refreshOpportunityIdsFromLastRefresh.length; i++) {
                if (lblRefreshMessageList[j].attributes['opportunityid'].value == refreshOpportunityIdsFromLastRefresh[i]) {
                    lblRefreshMessageList[j].style.display = 'block';
                }
            }
        }

    }
    function filterPotentialResources(searchtextBox) {
        var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
        var searchText = searchtextBox.value.toLowerCase();
        for (var i = 0; i < trPotentialResources.length; i++) {
            var checkBox = trPotentialResources[i].children[0].getElementsByTagName('input')[0];
            var checkboxText = checkBox.parentNode.children[1].innerHTML.toLowerCase();

            if (checkboxText.length >= searchText.length && checkboxText.substr(0, searchText.length) == searchText) {

                trPotentialResources[i].style.display = "";
            }
            else {

                trPotentialResources[i].style.display = "none";
            }
        }
        changeAlternateitemsForProposedResources('<%=cblPotentialResources.ClientID %>');
    }

    function filterTeamStructure(searchtextBox) {
        var trTeamMembers = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
        var searchText = searchtextBox.value.toLowerCase();
        for (var i = 0; i < trTeamMembers.length; i++) {
            var label = trTeamMembers[i].children[0].getElementsByTagName('span')[0];
            var labelText = label.innerHTML.toLowerCase();

            if (labelText.length >= searchText.length && labelText.substr(0, searchText.length) == searchText) {

                trTeamMembers[i].style.display = "";
            }
            else {

                trTeamMembers[i].style.display = "none";
            }
        }
        changeAlternateitemsForProposedResources('tblTeamStructure');
    }

    function ClearProposedResources() {
        clearErrorMessagesForTeamResources();
        var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
        for (var i = 0; i < chkboxes.length; i++) {
            chkboxes[i].checked = false;
            chkboxes[i].disabled = false;
        }
    }

    function ClearTeamStructure() {
        clearErrorMessages();

        var tblTeamStructure = document.getElementById('tblTeamStructure');
        for (var i = 0; i < tblTeamStructure.rows.length - 1; i++) {

            var ddlPerson = tblTeamStructure.rows[i].cells[0].children[0];
            var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
            var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];

            txtNeedBy.style.backgroundColor = "";
            ddlQuantity.style.backgroundColor = "";
            ddlPerson.value = 0;
            ddlQuantity.value = 0;

            if (CurrentOptyStartDate != '')
                txtNeedBy.value = new Date(CurrentOptyStartDate).format('MM/dd/yyyy');
            else
                txtNeedBy.value = '';

        }
    }


    function setHintPosition(img, displayPnl) {
        var image = $("#" + img);
        var displayPanel = $("#" + displayPnl);
        iptop = image.offset().top;
        ipleft = image.offset().left;
        iptop = iptop + 10;
        ipleft = ipleft - 10;
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
    }

    function setPosition(item, ytop, xleft) {
        item.offset({ top: ytop, left: xleft });
    }

    /* Start: Attach to project logic*/

    function addOptionsToDropDownList(ddlProjects, projectsJsonstring) {

        var projectsJson = jQuery.parseJSON(projectsJsonstring);
        $.each(projectsJson, function (key, value) {
            var ddlOption = document.createElement("option");
            ddlOption.text = value;
            ddlOption.value = key;
            ddlProjects.add(ddlOption);
        });

        $find('mpePopupAttachToProject').show();
    }

    function getvalueinDropDownForGivenText(dropdown, dropdowntext) {
        var dropdownvalue = "";

        var optionList = dropdown.getElementsByTagName('option');

        for (var i = 0; i < optionList.length; ++i) {
            if (optionList[i].innerHTML.toLowerCase() == dropdowntext) {
                dropdownvalue = optionList[i].value;
                break;
            }
        }
        return dropdownvalue;
    }

    function ddlProjects_change(ddlProjects) {
        var btnAttach = document.getElementById('<%= btnAttach.ClientID %>');
        if (ddlProjects.value == -1) {
            btnAttach.setAttribute('disabled', 'disabled');
        }
        else {
            btnAttach.removeAttribute('disabled');
        }
    }

    function btnAttach_onclick() {

        var ddlProjects = document.getElementById('<%= ddlProjects.ClientID %>');
        var projectId = ddlProjects.value;

        if (projectId == -1) {
            return false;
        }

        var ddlPriorityClientId = ddlProjects.attributes["ddlPriorityClientId"].value;
        var ddlPriority = document.getElementById(ddlPriorityClientId);
        var opportunityID = ddlPriority.attributes["OpportunityID"].value;

        var priorityId = getvalueinDropDownForGivenText(ddlPriority, 'po');
      
        if (opportunityID != null && opportunityID != 'undefined' && projectId != null && projectId != 'undefined' && priorityId != null && priorityId != 'undefined') {
            var urlVal = "AttachProjectToOpportunityHandler.ashx?opportunityID=" + opportunityID + "&projectId=" + projectId + "&priorityId=" + priorityId;
            $.post(urlVal, function (dat) {

                ddlPriority.value = priorityId;
                ddlPriority.attributes["isLinkedToProject"].value = "1";

                //To Show "Please refresh message" for that opportunity
                var lblRefreshMessageClientId = ddlPriority.attributes["lblRefreshMessageClientId"].value;
                if (lblRefreshMessageClientId != "") {
                    var lblRefreshMessage = document.getElementById(lblRefreshMessageClientId);
                    lblRefreshMessage.style.display = 'block';
                    Array.add(refreshOpportunityIdsFromLastRefresh, opportunityID);
                }
            });
        }
        $find('mpePopupAttachToProject').hide();
        return false;
    }



    function btnCancleAttachToProject_OnClientClick() {
        //To reset the ddlPriority value
        var ddlProjects = document.getElementById('<%= ddlProjects.ClientID %>');
        var ddlPriorityClientId = ddlProjects.attributes["ddlPriorityClientId"].value;
        var ddlPriority = document.getElementById(ddlPriorityClientId);
        ddlPriority.value = ddlPriority.attributes["selectedPriorityId"].value;

        ddlProjects.value = "-1";
        //To disable the attach button
        ddlProjects_change(ddlProjects);

        $find('mpePopupAttachToProject').hide();
        return false;
    }

    function btnCancelTeamstructueMessagePopup_OnClientClick() {
        $find('mpePopup').hide();
        return false;
    }

    function ddlPriorityList_onchange(ddlPriority) {
        var optionList = ddlPriority.getElementsByTagName('option');

        var selectedText = "";

        for (var i = 0; i < optionList.length; ++i) {
            if (optionList[i].value == ddlPriority.value) {
                selectedText = optionList[i].innerHTML.toLowerCase();
                break;
            }
        }

        var opportunityID = ddlPriority.attributes["OpportunityID"].value;

        if (!(selectedText == "po" || selectedText == "a" || selectedText == "b")) {
            changeOpportunityStatus(opportunityID, ddlPriority);
            return true;
        }

        var havingTeamStructure = false;
        var isProjectLinked = false;

        if (ddlPriority.attributes["isTeamstructueAvalilable"].value.toLowerCase() == "true") {
            havingTeamStructure = true;
        }

        if (ddlPriority.attributes["isLinkedToProject"].value == "1") {
            isProjectLinked = true;
        }

        if (!havingTeamStructure) {

            // Show error message saying that team structure not defined.
            var lblOpportunityName = document.getElementById('<%= lblOpportunityName.ClientID %>');
            var lblOpportunityName1 = document.getElementById('<%= lblOpportunityName1.ClientID %>');
            lblOpportunityName1.innerHTML = lblOpportunityName.innerHTML = ddlPriority.attributes["OpportunityName"].value;

            ddlPriority.value = ddlPriority.attributes["selectedPriorityId"].value;
            var hdnRedirectOpportunityId = document.getElementById('<%= hdnRedirectOpportunityId.ClientID %>');
            hdnRedirectOpportunityId.value = opportunityID;
            $find('mpePopup').show();

            return false;
        }

        if (selectedText != "po") {
            changeOpportunityStatus(opportunityID, ddlPriority);
            return true;
        }

        if (isProjectLinked) {
            changeOpportunityStatus(opportunityID, ddlPriority);
            return true;
        }

        // now we need to show attach to project window.
        showAttachToProjectWindow(ddlPriority);
    }

    function GetClientProjects(clientId, ddlProjects) {
        var urlVal = "AttachProjectToOpportunityHandler.ashx?clientId=" + clientId + "&getClientProjects=true";
        $.post(urlVal, function (data) {
            addOptionsToDropDownList(ddlProjects, data);
        });
    }

    function showAttachToProjectWindow(ddlPriority) {

        var ddlProjects = document.getElementById('<%=ddlProjects.ClientID %>');

        ddlProjects.setAttribute("ddlPriorityClientId", ddlPriority.getAttribute("id"));
        var clientId = ddlPriority.attributes["ClientId"].value;

        //To show OpportunityName in the popup window
        var lblOpportunityName2 = document.getElementById('<%= lblOpportunityName2.ClientID %>');
        lblOpportunityName2.innerHTML = ddlPriority.attributes["OpportunityName"].value;

        GetClientProjects(clientId, ddlProjects);
    }

    function changeOpportunityStatus(OpportunityID, ddlPriority) {
        var urlVal = "OpportunityPriorityHandler.ashx?OpportunityID=" + OpportunityID + "&PriorityID=" + ddlPriority.value;
        $.post(urlVal, function (dat) {
        });
        ddlPriority.attributes["selectedPriorityId"].value = ddlPriority.value;

    }

    /* End: Attach to project logic*/

    function SetTooltipText(descriptionText, hlinkObj) {
        var hlinkObjct = $('#' + hlinkObj.id);
        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        iptop = hlinkObjct.offset().top - hlinkObjct[0].offsetHeight;
        ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 10;
        iptop = iptop;
        ipleft = ipleft;
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();
        setPosition(displayPanel, iptop, ipleft);
        displayPanel.show();

        var lbloppNameTooltipContent = document.getElementById('<%= lbloppNameTooltipContent.ClientID %>');
        lbloppNameTooltipContent.innerHTML = descriptionText.toString();

        if (navigator.userAgent.indexOf(' Chrome/') > -1) {
            if (descriptionText.toString().length > 50) {
                lbloppNameTooltipContent.style.width = "330px";
            }
            else {
                lbloppNameTooltipContent.style.width = "100%";
            }
        }
    }

    function pageLoad() {
        var lbloppNameTooltipContent = document.getElementById('<%= lbloppNameTooltipContent.ClientID %>');

        if (navigator.userAgent.indexOf(' Chrome/') > -1) {
            lbloppNameTooltipContent.style.width = "330px";
        }
    }

    function HidePanel() {

        var displayPanel = $('#<%= oppNameToolTipHolder.ClientID %>');
        displayPanel.hide();
    }


</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="oppNameToolTipHolder" Style="display: none; position: absolute; z-index: 2000;"
            runat="server" CssClass="ToolTip">
            <table cellpadding="0" cellspacing="0">
                <tr class="top">
                    <td class="lt">
                        <div class="tail">
                        </div>
                    </td>
                    <td class="tbor">
                    </td>
                    <td class="rt">
                    </td>
                </tr>
                <tr class="middle">
                    <td class="lbor">
                    </td>
                    <td class="content">
                        <pre>
<asp:Label ID="lbloppNameTooltipContent" CssClass="WordWrap" Width="100%" runat="server"></asp:Label>
</pre>
                    </td>
                    <td class="rbor">
                    </td>
                </tr>
                <tr class="bottom">
                    <td class="lb">
                    </td>
                    <td class="bbor">
                    </td>
                    <td class="rb">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table cellpadding="0" cellspacing="0" align="center" style="width: 100%; padding-bottom: 10px;
            margin-bottom: 10px;">
            <tr>
                <td align="center">
                    <asp:Label ID="lblOpportunitiesCount" runat="server" Font-Bold="true" Font-Size="Medium"
                        Text="{0} Opportunities"></asp:Label>
                </td>
            </tr>
        </table>
        <div>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td>
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpeSummary" runat="Server" TargetControlID="pnlSummary"
                                ImageControlID="btnExpandCollapseSummary" CollapsedImage="~/Images/expand.jpg"
                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseSummary"
                                ExpandControlID="btnExpandCollapseSummary" Collapsed="True" TextLabelID="lblSummary" />
                            <asp:Image ID="btnExpandCollapseSummary" runat="server" ImageUrl="~/Images/collapse.jpg"
                                ToolTip="Expand Summary Details" />&nbsp;
                            <asp:Label ID="lblSummary" runat="server" Text="Summary"></asp:Label>
                        </td>
                        <td>
                            <asp:ShadowedHyperlink runat="server" Text="Add Opportunity" ID="lnkAddOpportunity"
                                CssClass="add-btn" NavigateUrl="~/OpportunityDetail.aspx?returnTo=DiscussionReview2.aspx" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel CssClass="summary" Style="white-space: nowrap; overflow-x: auto;" ID="pnlSummary"
                runat="server">
            </asp:Panel>
        </div>
        <div id="opportunity-list">
            <asp:ListView ID="lvOpportunities" runat="server" DataKeyNames="Id" EnableViewState="true"
                OnSorting="lvOpportunities_Sorting" OnItemDataBound="lvOpportunities_OnItemDataBound">
                <LayoutTemplate>
                    <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                        <tr runat="server" id="lvHeader" class="CompPerfHeader">
                            <td width="1%">
                                <div class="ie-bg no-wrap">
                                </div>
                            </td>
                            <td width="4%" align="center">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnPrioritySort" runat="server" Text="Priority" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Priority" Style="padding-left: 10px !important;" />
                                    <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                                    <asp:Panel ID="pnlPriority" Style="display: none;" CssClass="MiniReport" runat="server">
                                        <table>
                                            <tr>
                                                <th align="right">
                                                    <asp:Button ID="btnClosePriority" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                        Text="x" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                                        <LayoutTemplate>
                                                            <div style="max-height: 150px; overflow-y: auto; overflow-x: hidden;">
                                                                <table id="itemPlaceHolderContainer" runat="server" style="background-color: White;"
                                                                    class="WholeWidth">
                                                                    <tr runat="server" id="itemPlaceHolder">
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td style="width: 100%; padding-left: 2px;">
                                                                    <table class="WholeWidth">
                                                                        <tr>
                                                                            <td align="center" valign="middle" style="text-align: center; color: Black; font-size: 12px;
                                                                                padding: 0px;">
                                                                                <asp:Label ID="lblPriority" Width="15px" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>
                                                                            </td>
                                                                            <td align="center" valign="middle" style="text-align: center; color: Black; font-size: 12px;
                                                                                padding: 0px; padding-left: 2px; padding-right: 2px;">
                                                                                -
                                                                            </td>
                                                                            <td style="padding: 0px; color: Black; font-size: 12px;">
                                                                                <asp:Label ID="lblDescription" runat="server" Width="180px" Style="white-space: normal;"
                                                                                    Text='<%# Eval("Description") %>'></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <EmptyDataTemplate>
                                                            <tr>
                                                                <td valign="middle" style="padding-left: 2px;">
                                                                    <asp:Label ID="lblNoPriorities" runat="server" Text="No Priorities."></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </EmptyDataTemplate>
                                                    </asp:ListView>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnClosePriority"
                                        runat="server">
                                    </AjaxControlToolkit:AnimationExtender>
                                    <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint"
                                        runat="server">
                                    </AjaxControlToolkit:AnimationExtender>
                                </div>
                            </td>
                            <td width="4%" align="center">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnStartDateSort" runat="server" Text="Start" CommandName="Sort"
                                        CssClass="arrow" Style="padding-left: 10px !important;" CommandArgument="StartDate" />
                                </div>
                            </td>
                            <td width="13%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnClientNameSort" runat="server" Text="Client - Group" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="ClientName" />
                                </div>
                            </td>
                            <td width="9%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnBuyerNameSort" runat="server" Text="Buyer Name" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="BuyerName" />
                                </div>
                            </td>
                            <td width="23%">
                                <div class="ie-bg no-wrap" style="white-space: nowrap">
                                    <asp:LinkButton ID="btnOpportunityNameSort" runat="server" Text="Opportunity Name"
                                        CommandName="Sort" CssClass="arrow" CommandArgument="OpportunityName" />
                                </div>
                            </td>
                            <td width="7%">
                                <div class="ie-bg no-wrap">
                                    <asp:LinkButton ID="btnSalespersonSort" runat="server" Text="Sales Team" CommandName="Sort"
                                        CssClass="arrow" CommandArgument="Salesperson" />
                                </div>
                            </td>
                            <td align="center" width="10%">
                                <div class="ie-bg no-wrap" style="text-align: center;">
                                    <asp:LinkButton ID="btnEstimatedRevenue" runat="server" Text="Est. Revenue" CommandName="Sort"
                                        CssClass="arrow" Style="padding-left: 10px !important;" CommandArgument="EstimatedRevenue" />
                                </div>
                            </td>
                            <td align="center" width="29%">
                                <div class="ie-bg no-wrap" style="color: Black;">
                                    Team Make-Up
                                </div>
                            </td>
                        </tr>
                        <tr runat="server" id="itemPlaceholder" class="CompPerfHeader" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trOpportunity" runat="server">
                        <td>
                            <div class="cell-pad">
                                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                                    ButtonProjectNameToolTip='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem) %>'
                                    ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                                    ButtonProjectNameHref="<%# GetProjectDetailUrl((Opportunity) Container.DataItem) %>"
                                    Target="_blank" />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:DropDownList ID="ddlPriorityList" onchange="return ddlPriorityList_onchange(this);"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("ProjectedStartDate") == null ? string.Empty : string.Format("{0:MMM} '{0:yy}", ((DateTime)Eval("ProjectedStartDate")))%>
                            </div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblClientName" runat="server" Text='<%# GetWrappedText( ((Opportunity) Container.DataItem).ClientAndGroup, 17) %>' /></div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblBuyerName" runat="server" Text='<%# GetWrappedText( HttpUtility.HtmlEncode((string)Eval("BuyerName")), 15) %>' /></div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:HyperLink ID="hlName" Description='<%# GetWrappedText((string)((Opportunity) Container.DataItem).Description) %>'
                                    onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                                    runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# GetWrappedText(HttpUtility.HtmlEncode((string)Eval("Name")), 25)%>
                                </asp:HyperLink>
                            </div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblSalesTeam" runat="server" Text='<%# GetSalesTeam((((Opportunity)Container.DataItem).Salesperson),(((Opportunity)Container.DataItem).Owner))%>' /></div>
                        </td>
                        <td align="right" style="padding-right: 10px;">
                            <div class="cell-pad">
                                <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' />
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <table width="100%" style="padding: 2px;">
                                    <tr>
                                        <td style="width: 96%;">
                                            <asp:DataList ID="dtlProposedPersons" runat="server" ItemStyle-CssClass="OpportunitySummaryWordWrap">
                                                <ItemTemplate>
                                                    <%# GetWrappedText( GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType ), 20) %>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <asp:DataList ID="dtlTeamStructure" runat="server" ItemStyle-CssClass="OpportunitySummaryWordWrap">
                                                <ItemTemplate>
                                                    <%# GetWrappedText( GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType ), 20)%>
                                                    (<%#((DataTransferObjects.OpportunityPerson)Container.DataItem).Quantity.ToString() %>)
                                                    <%-- By
                                                     ((DataTransferObjects.OpportunityPerson)Container.DataItem).NeedBy.Value.ToString("MM/dd/yyyy") %>--%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <asp:Label ID="lblRefreshMessage" opportunityid='<%# Eval("Id") %>' runat="server"
                                                Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png"
                                                ToolTip="Select Team Resources" onclick="ShowPotentialResourcesModal(this);"
                                                Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                        </td>
                                        <td style="padding-left: 4px; width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgTeamStructure" runat="server" ImageUrl="~/Images/Strawman.png"
                                                ToolTip="Select Team Structure" onclick="ShowTeamStructureModal(this);" Style="cursor: pointer;"
                                                opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnTeamStructure" runat="server" />
                                            <asp:HiddenField ID="hdnStartDate" runat="server" />
                                            <asp:HiddenField ID="hdnEndDate" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr id="trOpportunity" runat="server" class="rowEven">
                        <td>
                            <div class="cell-pad">
                                <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                                    ButtonProjectNameToolTip='<%# PraticeManagement.Utils.OpportunitiesHelper.GetToolTip((Opportunity) Container.DataItem) %>'
                                    ButtonCssClass='<%# PraticeManagement.Utils.OpportunitiesHelper.GetIndicatorClass((Opportunity) Container.DataItem)%>'
                                    ButtonProjectNameHref="<%# GetProjectDetailUrl((Opportunity) Container.DataItem) %>"
                                    Target="_blank" />
                            </div>
                        </td>
                        <td align="center">
                            <div class="cell-pad">
                                <asp:DropDownList ID="ddlPriorityList" onchange="return ddlPriorityList_onchange(this);"
                                    runat="server">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <%# Eval("ProjectedStartDate") == null ? string.Empty : string.Format("{0:MMM} '{0:yy}", ((DateTime)Eval("ProjectedStartDate")))%>
                            </div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblClientName" runat="server" Text='<%# GetWrappedText( ((Opportunity) Container.DataItem).ClientAndGroup, 17) %>' /></div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblBuyerName" runat="server" Text='<%# GetWrappedText( HttpUtility.HtmlEncode((string)Eval("BuyerName")), 15)%>' /></div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:HyperLink ID="hlName" Description='<%# GetWrappedText((string)((Opportunity) Container.DataItem).Description) %>'
                                    onmouseout="HidePanel();" onmouseover="SetTooltipText(this.attributes['Description'].value,this);"
                                    runat="server" NavigateUrl='<%# GetOpportunityDetailsLink((int) Eval("Id"), Container.DisplayIndex) %>'>
                            <%# GetWrappedText(HttpUtility.HtmlEncode((string)Eval("Name")), 25)%>
                                </asp:HyperLink>
                            </div>
                        </td>
                        <td class="WordWrap">
                            <div class="cell-pad">
                                <asp:Label ID="lblSalesTeam" runat="server" Text='<%# GetSalesTeam((((Opportunity)Container.DataItem).Salesperson),(((Opportunity)Container.DataItem).Owner))%>' /></div>
                        </td>
                        <td align="right" style="padding-right: 10px;">
                            <div class="cell-pad">
                                <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# GetFormattedEstimatedRevenue((Decimal?)Eval("EstimatedRevenue")) %>' />
                            </div>
                        </td>
                        <td align="left">
                            <div class="cell-pad">
                                <table width="100%" style="padding: 2px;">
                                    <tr>
                                        <td style="width: 96%;">
                                            <asp:DataList ID="dtlProposedPersons" runat="server" ItemStyle-CssClass="OpportunitySummaryWordWrap">
                                                <ItemTemplate>
                                                    <%# GetWrappedText( GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType ), 20)%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <%--<div style="white-space: normal;">
                                                <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                            </div>--%>
                                            <asp:DataList ID="dtlTeamStructure" runat="server" ItemStyle-CssClass="OpportunitySummaryWordWrap">
                                                <ItemTemplate>
                                                    <%# GetWrappedText( GetFormattedPersonName(((DataTransferObjects.OpportunityPerson)Container.DataItem).Person.PersonLastFirstName, 
                                                    ((DataTransferObjects.OpportunityPerson)Container.DataItem).PersonType ), 20)%>
                                                    (<%#((DataTransferObjects.OpportunityPerson)Container.DataItem).Quantity.ToString() %>)
                                                    <%--By  #((DataTransferObjects.OpportunityPerson)Container.DataItem).NeedBy.Value.ToString("MM/dd/yyyy") %>--%>
                                                </ItemTemplate>
                                            </asp:DataList>
                                            <asp:Label ID="lblRefreshMessage" opportunityid='<%# Eval("Id") %>' runat="server"
                                                Text="Please &lt;a href='javascript:location.reload(true)'&gt;refresh&lt;/a&gt; to see new changes."
                                                Style="display: none; font-style: italic;"></asp:Label>
                                        </td>
                                        <td style="width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgPeople_icon" runat="server" ImageUrl="~/Images/People_icon.png"
                                                ToolTip="Select Team Resources" onclick="ShowPotentialResourcesModal(this);"
                                                Style="cursor: pointer;" opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                        </td>
                                        <td style="padding-left: 4px; width: 4%; white-space: normal;" align="right">
                                            <asp:Image ID="imgTeamStructure" runat="server" ImageUrl="~/Images/Strawman.png"
                                                ToolTip="Select Team Structure" onclick="ShowTeamStructureModal(this);" Style="cursor: pointer;"
                                                opportunityid='<%# Eval("Id") %>' />
                                            <asp:HiddenField ID="hdnTeamStructure" runat="server" />
                                            <asp:HiddenField ID="hdnStartDate" runat="server" />
                                            <asp:HiddenField ID="hdnEndDate" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <EmptyDataTemplate>
                    <tr runat="server" id="EmptyDataRow">
                        <td>
                            No opportunities found.
                        </td>
                    </tr>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <asp:HiddenField ID="hdnRedirectOpportunityId" runat="server" />
        <asp:HiddenField ID="hdnClickedRowIndex" runat="server" />
        <asp:HiddenField ID="hdnCanShowPopup" Value="false" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="hdnCanShowPopup"
            CancelControlID="btnCancelTeamstructueMessagePopup" BehaviorID="mpePopup" BackgroundCssClass="modalBackground"
            PopupControlID="pnlTeamStructueMessagePopup" DropShadow="false" />
        <asp:Panel ID="pnlTeamStructueMessagePopup" runat="server" BackColor="White" BorderColor="Black"
            CssClass="ConfirmBoxClassError" Style="display: none" BorderWidth="2px">
            <table width="100%">
                <tr>
                    <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                        valign="bottom">
                        <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                        <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close" ToolTip="Cancel"
                            OnClientClick="$find('mpePopup').hide(); return false;" Style="float: right;"
                            Text="X"></asp:Button>
                    </th>
                </tr>
                <tr>
                    <td style="padding: 10px;" colspan="2">
                        <table>
                            <tr>
                                <td>
                                    <p>
                                        You must add a Team Make-Up to
                                        <asp:Label ID="lblOpportunityName" runat="server" Font-Bold="true"></asp:Label>
                                        opportunity before it can be saved with a PO, A, or B priority.
                                    </p>
                                    <br />
                                    <p>
                                        Click OK to edit
                                        <asp:Label ID="lblOpportunityName1" runat="server" Font-Bold="true"></asp:Label>
                                        opportunity and make the necessary changes. Clicking Cancel will result in no changes
                                        to the opportunity.</p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="padding: 6px 6px 15px 6px;">
                        <table>
                            <tr>
                                <td style="padding-right: 3px;">
                                    <asp:Button ID="btnRedirectToOpportunityDetail" runat="server" Text="OK" ToolTip="OK"
                                        OpportunityID="" OnClick="btnRedirectToOpportunityDetail_OnClick" />
                                </td>
                                <td style="padding-left: 3px;">
                                    <asp:Button ID="btnCancelTeamstructueMessagePopup" runat="server" Text="Cancel" ToolTip="Cancel"
                                        OnClientClick="return btnCancelTeamstructueMessagePopup_OnClientClick();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdMpePopupAttachToProject" Value="false" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpePopupAttachToProject" runat="server"
            TargetControlID="hdMpePopupAttachToProject" CancelControlID="btnCancleAttachToProject"
            BehaviorID="mpePopupAttachToProject" BackgroundCssClass="modalBackground" PopupControlID="pnlAttachToProject"
            DropShadow="false" />
        <asp:Panel ID="pnlAttachToProject" runat="server" BackColor="White" BorderColor="Black"
            CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
            <table width="100%">
                <tr style="background-color: Gray; height: 27px;">
                    <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                        Attach This Opportunity to Existing Project
                    </td>
                </tr>
                <tr>
                    <td style="padding: 10px;">
                        <table>
                            <tr>
                                <td>
                                    <p>
                                        Select a Project and Click Attach to Link
                                        <asp:Label ID="lblOpportunityName2" runat="server" Font-Bold="true"></asp:Label>
                                        opportunity.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 6px 6px 2px 2px;">
                        <asp:DropDownList ID="ddlProjects" runat="server" AppendDataBoundItems="true" onchange="setDirty(); ddlProjects_change(this);"
                            OpportunityID="" AutoPostBack="false" Style="width: 350px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 6px 6px 2px 2px; white-space: nowrap;">
                        <asp:Button ID="btnAttach" runat="server" Text="Attach" OnClientClick="return btnAttach_onclick();"
                            po disabled="disabled" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnCancleAttachToProject" runat="server" Text="Cancel" OnClientClick="return btnCancleAttachToProject_OnClientClick();" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hdnmpePotentialResources" runat="server" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpePotentialResources" runat="server"
            BehaviorID="behaviorIdPotentialResources" TargetControlID="hdnmpePotentialResources"
            EnableViewState="false" BackgroundCssClass="modalBackground" PopupControlID="pnlPotentialResources"
            CancelControlID="btnCancel" DropShadow="false" />
        <asp:Panel ID="pnlPotentialResources" runat="server" BorderColor="Black" BackColor="#d4dff8"
            Style="display: none;" Width="372px" BorderWidth="1px">
            <table width="100%">
                <tr>
                    <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                        <center>
                            <b>Team Resources</b>
                        </center>
                        <asp:TextBox ID="txtSearchBox" runat="server" Width="353px" Height="16px" Style="padding-bottom: 4px;
                            margin-bottom: 4px;" MaxLength="4000" onkeyup="filterPotentialResources(this);"></asp:TextBox>
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                            WatermarkText="Begin typing here to filter the list of resources below." EnableViewState="false"
                            WatermarkCssClass="watermarkedtext" BehaviorID="wmbhSearchBox" />
                        <table>
                            <tr>
                                <td style="width: 304px;">
                                </td>
                                <td style="padding-right: 2px;">
                                    <asp:Image ID="imgCheck" runat="server" ImageUrl="~/Images/right_icon.png" />
                                </td>
                                <td style="padding-left: 2px;">
                                    <asp:Image ID="imgCross" runat="server" ImageUrl="~/Images/cross_icon.png" />
                                </td>
                            </tr>
                        </table>
                        <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                            border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                            <uc:MultipleSelectionCheckBoxList ID="cblPotentialResources" runat="server" Width="100%"
                                BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                                CellPadding="3">
                            </uc:MultipleSelectionCheckBoxList>
                        </div>
                        <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
                            <input type="button" value="Clear All" onclick="javascript:ClearProposedResources();" />
                        </div>
                        <br />
                        <table width="356px;">
                            <tr>
                                <td align="right">
                                    <input type="button" id="btnSaveProposedResources" value="Save" onclick="javascript:saveProposedResources();" />
                                    &nbsp;
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                        <div id="divTeamResources" style="text-align: left; display: none; color: Red; width: 356px;
                            padding: 8px 0px 10px 0px">
                            An Opportunity with a PO, A, or B priority must have a Team Make-Up.
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdnCurrentOpportunityId" runat="server" Value="" />
        <asp:HiddenField ID="hdnProposedResourceIdsWithTypes" runat="server" Value="" />
        <%--<asp:HiddenField ID="hdnProposedOutSideResources" runat="server" Value="" />--%>
        <asp:Button ID="btnSaveProposedResourcesHidden" runat="server" OnClick="btnSaveProposedResources_OnClick"
            Style="display: none;" />
        <asp:HiddenField ID="hdnmpeTeamStructure" runat="server" Value="" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeTeamStructure" runat="server" BehaviorID="behaviorIdTeamStructure"
            TargetControlID="hdnmpeTeamStructure" EnableViewState="false" BackgroundCssClass="modalBackground"
            PopupControlID="pnlTeamStructure" CancelControlID="btnTeamCancel" DropShadow="false" />
        <asp:Panel ID="pnlTeamStructure" runat="server" BorderColor="Black" BackColor="#d4dff8"
            Width="416px" BorderWidth="1px" Style="display: none;">
            <table width="100%">
                <tr>
                    <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                        <center>
                            <b>Team Structure</b>
                        </center>
                        <br />
                        <div class="cbfloatRight" style="width: 400px;">
                            <table width="100%">
                                <tr>
                                    <td style="width: 220px;" align="center">
                                        <b>Role / Skill</b>
                                    </td>
                                    <td style="text-align: center; width: 70px;">
                                        <b>QTY</b>
                                    </td>
                                    <td align="center">
                                        <b>Needed By</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="cbfloatRight" style="height: 250px; width: 400px; overflow-y: scroll;
                            border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                            <table width="100%" id="tblTeamStructure" class="strawman">
                                <tr>
                                    <td style="width: 220px;">
                                        <asp:DropDownList ID="ddlStrawmen" runat="server" DataTextField="Name" DataValueField="Id"
                                            Width="220px">
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width: 70px;" align="center">
                                        <asp:DropDownList ID="ddlQuantity" runat="server" onchange="this.style.backgroundColor = '';"
                                            DataTextField="Name" DataValueField="Id" Style="width: 50px;">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtNeedBy" runat="server" Style="width: 80px; float: left;" CssClass="date-pick"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" align="left">
                                        <asp:ImageButton ID="imgAddStrawman" runat="server" ImageUrl="~/Images/add_16.png"
                                            AlternateText="Add Strawman" Style="float: left;" OnClientClick=" return AddStrawmanRow();" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="text-align: right; width: 404px; padding: 8px 0px 10px 0px">
                            <input type="button" value="Clear All" onclick="javascript:ClearTeamStructure();" />
                        </div>
                        <table width="404px;" style="padding-right: 6px; padding-bottom: 4px;">
                            <tr>
                                <td align="right">
                                    <input type="button" id="btnSaveTeamStructure" value="Save" onclick="javascript:saveTeamStructure();" />
                                    &nbsp;
                                    <asp:Button ID="btnTeamCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                        <div id="divNeedbyError" style="text-align: left; display: none; color: Red; width: 404px;
                            padding: 8px 0px 10px 0px">
                        </div>
                        <div id="divQuantityError" style="text-align: left; display: none; color: Red; width: 404px;
                            padding: 8px 0px 10px 0px">
                            Quantity for a Straw Man must be less than or equals to 10 for same Need By date.
                        </div>
                        <div id="divOpportunityError" style="text-align: left; display: none; color: Red;
                            width: 404px; padding: 8px 0px 10px 0px">
                            An Opportunity with a PO, A, or B priority must have a Team Make-Up.
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdnTeamStructure" runat="server" Value="" />
        <asp:Button ID="btnSaveTeamStructureHidden" runat="server" OnClick="btnSaveTeamStructureHidden_OnClick"
            Style="display: none;" />
    </ContentTemplate>
</asp:UpdatePanel>
<uc1:LoadingProgress ID="LoadingProgress1" AssociatedUpdatePanelID="UpdatePanel1"
    DisplayText="Please Wait ..." runat="server" />
<uc1:LoadingProgress ID="LoadingProgress2" AssociatedUpdatePanelID="UpdatePanel2"
    DisplayText="Saving ..." runat="server" />
<asp:ValidationSummary ID="valsum" ValidationGroup="Notes" runat="server" />

