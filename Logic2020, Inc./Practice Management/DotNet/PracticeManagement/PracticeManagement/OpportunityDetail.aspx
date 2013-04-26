<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="OpportunityDetail.aspx.cs" Inherits="PraticeManagement.OpportunityDetail"
    Title="Opportunity Details | Practice Management" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Opportunities/PrevNextOpportunity.ascx" TagPrefix="uc"
    TagName="PrevNextOpportunity" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/date.min.js", this) %>" type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/datepicker.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/datepicker.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function checkDirty(arg) {
            __doPostBack('__Page', arg);
            return true;
        }

        function ConfirmToDeleteOpportunity() {
            var hdnProject = document.getElementById('<%= hdnOpportunityDelete.ClientID %>');
            var result = confirm("Do you really want to delete the opportunity?");
            hdnProject.value = result ? 1 : 0;
        }
    </script>
    <script type="text/javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(getEvents);
        function getEvents() {
            var divPriority = $("#<%= pnlPriority.ClientID %>");
            var imgPriorityHint = $("#<%= imgPriorityHint.ClientID %>");
            setHintPosition(imgPriorityHint, divPriority);

            imgPriorityHint.click(function () {
                setHintPosition(imgPriorityHint, divPriority);
            });
        }

        function selectDefaultoption() {
            var ddlProjects = document.getElementById('<%= ddlProjects.ClientID %>');
            var optionList = ddlProjects.getElementsByTagName('option');
            ddlProjects.value = optionList[0].value;
        }

        function ShowDescriptionSelections(ddlProjects) {

            var divDescription = document.getElementById('<%= divDescription.ClientID %>');

            if (ddlProjects.value != "-1" && ddlProjects.value != "") {

                var rbtnOpportunityDescription = document.getElementById('<%= rbtnOpportunityDescription.ClientID %>');
                var rbtnProjectDescription = document.getElementById('<%= rbtnProjectDescription.ClientID %>');
                var lblOpportunityDescription = document.getElementById('<%= lblOpportunityDescription.ClientID %>');
                var lblProjectDescription = document.getElementById('<%= lblProjectDescription.ClientID %>');
                var txtDescription = document.getElementById('<%= txtDescription.ClientID %>');


                lblOpportunityDescription.innerHTML = "";
                lblProjectDescription.innerHTML = "";
                divDescription.setAttribute("class", "Padding15Px displayNone");
                var optionList = ddlProjects.getElementsByTagName('option');

                var selectedProjectDescription = "";

                for (var i = 0; i < optionList.length; ++i) {
                    if (optionList[i].value == ddlProjects.value) {
                        selectedProjectDescription = optionList[i].attributes["Description"].value;
                        break;
                    }
                }

                lblOpportunityDescription.title = txtDescription.value;
                lblProjectDescription.title = selectedProjectDescription;

                if (txtDescription.value.length > 100) {
                    lblOpportunityDescription.innerHTML = EncodeString(txtDescription.value.substring(0, 100) + "..");
                }
                else {
                    lblOpportunityDescription.innerHTML = EncodeString(txtDescription.value);
                }

                if (selectedProjectDescription.length > 100) {
                    lblProjectDescription.innerHTML = EncodeString(selectedProjectDescription.substring(0, 100) + "..");
                }
                else {
                    lblProjectDescription.innerHTML = EncodeString(selectedProjectDescription);
                }

                divDescription.setAttribute("class", "Padding15Px");
            }
            else {
                divDescription.setAttribute("class", "Padding15Px displayNone");
            }
        }


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
            ddlStrawMan.className = "Width220PxImp";


            var hdnStrawmanListInDropdown = document.getElementById('<%= hdnStrawmanListInDropdown.ClientID %>');
            var hdnColoumSpliter = document.getElementById('<%= hdnColoumSpliter.ClientID %>');
            var hdnRowSpliter = document.getElementById('<%= hdnRowSpliter.ClientID %>');
            var strawmans = hdnStrawmanListInDropdown.value.split(hdnRowSpliter.value);
            for (var i = 0; i < strawmans.length; i++) {
                var strawmanDetails = strawmans[i].split(hdnColoumSpliter.value);
                AddOption(strawmanDetails[0], strawmanDetails[1], ddlStrawMan);
            }

            cell1.appendChild(ddlStrawMan);

            var cell2 = row.insertCell(1);
            var ddlQuantity = document.createElement("select");
            ddlQuantityOriginal = tblTeamStructure.rows[0].cells[1].children[0];
            ddlQuantity.style.width = ddlQuantityOriginal.style.width;
            ddlQuantity.className = "Width50PxImp";
            ddlQuantity.setAttribute("onchange", "this.style.backgroundColor = '';");
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
            cell2.align = "center";
            cell2.appendChild(ddlQuantity);

            var cell3 = row.insertCell(2);
            var txtNeedBy = document.createElement("input");
            txtNeedBy.type = "text";
            txtNeedBy.style.width = tblTeamStructure.rows[0].cells[2].children[0].style.width;
            txtNeedBy.style.cssText = txtNeedBy.style.cssText + ";float:left;";
            txtNeedBy.className = "date-pick Width80Px FloatLeft";
            cell3.appendChild(txtNeedBy);
            OptyStartDate = document.getElementById('<%= (dpStartDate.FindControl("txtDate") as TextBox).ClientID %>').value;
            if (OptyStartDate != '')
                txtNeedBy.value = (new Date(OptyStartDate)).format('MM/dd/yyyy');
            txtNeedBy.readOnly = true;


            setcalendar();
            return false;
        }


        function ClearProposedResources() {
            var chkboxList = document.getElementById('<%=cblPotentialResources.ClientID %>');
            var chkboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
            for (var i = 0; i < chkboxes.length; i++) {
                chkboxes[i].checked = false;
                chkboxes[i].disabled = false;
            }
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

                var OpportunityStartDate = document.getElementById('<%= (dpStartDate.FindControl("txtDate") as TextBox).ClientID %>').value;
                if (OpportunityStartDate != '')
                    txtNeedBy.value = new Date(OpportunityStartDate).format('MM/dd/yyyy');
                else
                    txtNeedBy.value = '';
            }
        }

        function ShowPotentialResourcesModal(image) {
            var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
            var attachedResourcesIndexes = document.getElementById('<%=hdnProposedPersonsIndexes.ClientID %>').value.split(",");
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
            $find("behaviorIdPotentialResources").show();
            return false;
        }

        function ShowTeamStructureModal(image) {
            clearErrorMessages();
            var attachedTeam = document.getElementById('<%=hdnTeamStructure.ClientID %>').value.split(",");
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
                    OptyStartDate = document.getElementById('<%= (dpStartDate.FindControl("txtDate") as TextBox).ClientID %>').value;
                    if (OptyStartDate != '')
                        txtNeedBy.value = new Date(OptyStartDate).format('MM/dd/yyyy'); ;
                    txtNeedBy.className = "date-pick Width80Px FloatLeft";
                    txtNeedBy.readOnly = true;
                    setcalendar();
                }
                else {
                    tblTeamStructure.deleteRow(i);
                }
            }
            var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
            var hdnUsedInactiveStrawmanList = document.getElementById('<%=hdnUsedInactiveStrawmanList.ClientID %>');
            var hdnColoumSpliter = document.getElementById('<%= hdnColoumSpliter.ClientID %>');
            var hdnRowSpliter = document.getElementById('<%= hdnRowSpliter.ClientID %>');
            var strawmans = hdnUsedInactiveStrawmanList.value.split(hdnRowSpliter.value);

            for (var i = 0; i < attachedTeam.length - 1; i++) {
                if (i > 0) {
                    AddStrawmanRow();
                }
                var ddlPerson = tblTeamStructure.rows[i].cells[0].children[0];
                var ddlQuantity = tblTeamStructure.rows[i].cells[1].children[0];
                var txtNeedBy = tblTeamStructure.rows[i].cells[2].children[0];
                var personString = attachedTeam[i];
                //Strawmans' info is separated  by ","s.
                // Each Strawman info is in the format "PersonId:PersonType|Quantity?NeedBy"
                var selectedValue = personString.substring(0, personString.indexOf(":", 0))
                ddlPerson.value = selectedValue;
                if (ddlPerson.value == '' || ddlPerson.value == null || ddlPerson.value != selectedValue) {

                    var strawmanname = '';
                    for (var j = 0; j < strawmans.length; j++) {
                        var strawmanDetails = strawmans[j].split(hdnColoumSpliter.value);
                        if (selectedValue == strawmanDetails[1]) {
                            strawmanname = strawmanDetails[0];
                        }
                    }
                    AddOption(strawmanname, selectedValue, ddlPerson);
                    ddlPerson.value = selectedValue;
                }
                ddlQuantity.value = personString.substring(personString.indexOf("|", 0) + 1, personString.indexOf("?", 0));
                var needByDate = new Date(personString.substring(personString.indexOf("?", 0) + 1, personString.length));
                txtNeedBy.value = needByDate.format('MM/dd/yyyy');

            }
            $find("behaviorIdTeamStructure").show();
            return false;
        }

        function AddOption(optionText, optionValue, ddl) {
            var option = document.createElement("option");
            option.text = optionText;
            option.value = optionValue;
            try {
                ddl.add(option, null); //Standard    
            } catch (error) {
                ddl.add(option); // IE only    
            }
        }

        function GetProposedPersonIdsListWithPersonType() {
            var cblPotentialResources = document.getElementById("<%= cblPotentialResources.ClientID%>");
            var potentialCheckboxes = $('#<%=cblPotentialResources.ClientID %> tr td :input');
            var hdnProposedPersonIdsList = document.getElementById("<%= hdnProposedResourceIdsWithTypes.ClientID%>");
            var hdnProposedPersonsIndexes = document.getElementById('<%=hdnProposedPersonsIndexes.ClientID %>');

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
            hdnProposedPersonIdsList.value = PersonIdList;

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
            hdnTeamStructure.value = PersonIdList;
        }

        function saveProposedResources() {
            setDirty(); EnableSaveButton();
            GetProposedPersonIdsListWithPersonType();
        }

        function validateNeedByDates() {
            var result = true;
            var OpportunityStartDate = document.getElementById('<%= (dpStartDate.FindControl("txtDate") as TextBox).ClientID %>').value;
            var OpportunityEndDate = document.getElementById('<%= (dpEndDate.FindControl("txtDate") as TextBox).ClientID %>').value;

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

        function validateAll() {
            var result1 = true;
            var result2 = true;

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


            return result1 && result2;
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


            if (validateAll()) {
                setDirty();
                EnableSaveButton();
                UpdateTeamStructureForHiddenfields();
                return true;
            }
            else {
                return false;
            }
        }

        function showQuantityErrorMessage() {
            document.getElementById('divQuantityError').style.display = "block";
        }

        function showNeedbyErrorMessage() {
            document.getElementById('divNeedbyError').style.display = "block";
        }

        function clearErrorMessages() {
            document.getElementById('divNeedbyError').style.display = "none";
            document.getElementById('divQuantityError').style.display = "none";
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

        function setHintPosition(image, displayPanel) {
            var iptop = image.offset().top;
            var ipleft = image.offset().left;
            iptop = iptop + 10;
            ipleft = ipleft - 10;

            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function checkDirty(arg) {
            __doPostBack('__Page', arg);
            return true;
        }

        function EnableSaveButton() {
            var button = document.getElementById("<%= btnSave.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnValueChanged.ClientID%>");

            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
        }

        function EnableOrDisableConvertOrAttachToProj() {
            var ddlStatus = document.getElementById("<%= ddlStatus.ClientID%>");
            var btnAttachToProject = document.getElementById("<%= btnAttachToProject.ClientID%>");
            var btnConvertToProject = document.getElementById("<%= btnConvertToProject.ClientID%>");
            var hdnHasProjectIdOrPermission = document.getElementById("<%= hdnHasProjectIdOrPermission.ClientID%>");

            if (hdnHasProjectIdOrPermission.value != "true" && ddlStatus.value == "1") { //1 active
                btnAttachToProject.disabled = false;
                btnConvertToProject.disabled = false;
            } else {
                btnAttachToProject.disabled = true;
                btnConvertToProject.disabled = true;
            }

        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

        function SetWrapText(str) {
            for (var i = 30; i < str.length; i = i + 10) {
                str = str.slice(0, i) + "<wbr/>" + str.slice(i, str.length);
            }
            return str;
        }

        function GetWrappedText(childObj) {
            if (childObj != null) {

                for (var i = 0; i < childObj.children.length; i++) {
                    if (childObj.children[i] != null) {
                        if (childObj.children[i].children.length == 0) {
                            if (childObj.children[i].innerHTML != null && childObj.children[i].innerHTML != "undefined" && childObj.children[i].innerHTML.length > 70) {
                                childObj.children[i].innerHTML = SetWrapText(childObj.children[i].innerHTML);
                            }
                        }
                    }

                }
            }
        }

        function ModifyInnerTextToWrapText() {
            if (navigator.userAgent.indexOf(" Firefox/") > -1) {
                var tbl = $("table[id*='gvActivities']");
                if (tbl != null && tbl.length > 0) {
                    var gvActivitiesclientId = tbl[0].id;
                    var lastTds = $('#' + gvActivitiesclientId + ' tr td:nth-child(3)');

                    for (var i = 0; i < lastTds.length; i++) {
                        GetWrappedText(lastTds[i]);
                    }
                }
            }
        }

        /* Start: Attach to project logic*/
        function ddlPriority_OnSelectedIndexChanged(ddlPriority) {
            EnableSaveButton();
            setDirty();
            var optionList = ddlPriority.getElementsByTagName('option');
            var selectedText = "";

            if (ddlPriority.value != "5") {
                ddlPriority.setAttribute("selectedPriorityText", selectedText);
            }
            else {
                var btnAttachToProject = document.getElementById("<%= btnAttachToProject.ClientID%>");

                //Raise btnAttachToProject_Click event
                btnAttachToProject.click();
            }
            return false;
        }

        function btnCancel_click() {

            var ddlPriority = document.getElementById("<%= ddlPriority.ClientID%>");
            selectDefaultoption();

            if (ddlPriority.getAttribute("selectedPriorityText") != null) {
                var selectedPriorityText = ddlPriority.getAttribute("selectedPriorityText").toLowerCase();
                var optionList = ddlPriority.getElementsByTagName('option');
                var selectedText = "";
                var selectedPriorityId = "";

                //PervTextValue
                for (var i = 0; i < optionList.length; ++i) {
                    if (optionList[i].value == ddlPriority.value) {
                        selectedText = optionList[i].innerHTML.toLowerCase();
                        break;
                    }
                }
                if (ddlPriority.value == "5") {
                    for (var i = 0; i < optionList.length; ++i) {
                        if (optionList[i].innerHTML.toLowerCase() == selectedPriorityText) {
                            PervValue = optionList[i].value;
                            break;
                        }
                    }
                    ddlPriority.value = PervValue;
                }
            }

            return false;
        }

        function ddlProjects_change(ddlProjects) {
            ShowDescriptionSelections(ddlProjects);
            var btnAttach = document.getElementById('<%= btnAttach.ClientID %>');
            if (ddlProjects.value == "") {
                btnAttach.setAttribute('disabled', 'disabled');
            }
            else {
                btnAttach.removeAttribute('disabled');
            }
        }

        /* End: Attach to project logic*/

        function ResetStartDate() {
            var hdnOpportunityProjectedStartDate = document.getElementById('<%= hdnOpportunityProjectedStartDate.ClientID %>');
            var dpStartDate = document.getElementById('<%= (dpStartDate.FindControl("txtDate") as TextBox).ClientID %>');
            var dpStartDateExtender = $find('dpStartDateBehaviourId');

            if (hdnOpportunityProjectedStartDate != null && hdnOpportunityProjectedStartDate.value != '' && dpStartDate != null && dpStartDateExtender != null) {
                var previousStartDate = new Date(hdnOpportunityProjectedStartDate.value).format('MM/dd/yyyy');

                dpStartDate.value = previousStartDate;
                dpStartDateExtender.set_selectedDate(previousStartDate);
            }
        }

    </script>
    <table class="CompPerfTable WholeWidth">
        <tr>
            <td>
                <uc:PrevNextOpportunity ID="prevNext" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="padLeft5 vTop">
                <asp:UpdatePanel ID="upOpportunityDetail" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="hdnHasProjectIdOrPermission" runat="server" />
                        <asp:Label ID="lblReadOnlyWarning" runat="server" ForeColor="Red" Visible="false">Since you are not the designated owner of this opportunity, you will not be able to make any changes.</asp:Label>
                        <table class="PaddingClass WholeWidth padLeft5">
                            <tr class="height30P">
                                <td class="Width12Per">
                                    Opportunity Number
                                </td>
                                <td class="Width43Percent" style="width: 35%">
                                    <asp:Label ID="lblOpportunityNumber" runat="server" />
                                    &nbsp;(last updated:
                                    <asp:Label ID="lblLastUpdate" runat="server" />) &nbsp;&nbsp;
                                    <asp:HyperLink ID="hpProject" runat="server"></asp:HyperLink>
                                </td>
                                <td colspan="2" class="Width45Percent no-wrap">
                                    <table cellpadding="4px;">
                                        <tr>
                                            <td class="LblEndDate">
                                                <asp:Label ID="lblCloseDate" CssClass="fontBold" runat="server" Text="Close Date"></asp:Label>
                                            </td>
                                            <td>
                                                <uc1:DatePicker ID="dpCloseDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqCloseDate" runat="server" ControlToValidate="dpCloseDate"
                                                    ErrorMessage="The Close Date is required" ToolTip="The Close Date is required."
                                                    ValidationGroup="Opportunity" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cmpCloseDateFormatCheck" runat="server" ControlToValidate="dpCloseDate"
                                                    ValidationGroup="Opportunity" Type="Date" Operator="DataTypeCheck" Text="*" Display="Dynamic"
                                                    ErrorMessage="The Close Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                    ToolTip="The Close Date has an incorrect format. It must be 'MM/dd/yyyy'."></asp:CompareValidator>
                                            </td>
                                            <td class="PaddingRight4PxImp">
                                                <b>Project Start Date</b>
                                            </td>
                                            <td class="UcDatePickerPadding">
                                                <uc1:DatePicker ID="dpStartDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                    BehaviorID="dpStartDateBehaviourId" OnClientChange="EnableSaveButton();setDirty();"
                                                    TextBoxWidth="62px" runat="server" />
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                                    ErrorMessage="The Project Start Date is required" ToolTip="The Project Start Date is required."
                                                    ValidationGroup="Opportunity" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cmpStartDateDataTypeCheck" runat="server" ControlToValidate="dpStartDate"
                                                    ValidationGroup="Opportunity" Type="Date" Operator="DataTypeCheck" Text="*" Display="Dynamic"
                                                    ErrorMessage="The Project Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                    ToolTip="The Project Start Date has an incorrect format. It must be 'MM/dd/yyyy'."></asp:CompareValidator>
                                            </td>
                                            <td class="LblEndDate">
                                                <asp:Label ID="lbEndDate" CssClass="fontBold" runat="server" Text="Project End Date"></asp:Label>
                                            </td>
                                            <td>
                                                <uc1:DatePicker ID="dpEndDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                            </td>
                                            <td class="padRight7Imp">
                                                <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpEndDate"
                                                    ErrorMessage="The Project End Date is required" ToolTip="The Project End Date is required."
                                                    ValidationGroup="Opportunity" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cmpEndDateDataTypeCheck" runat="server" ControlToValidate="dpEndDate"
                                                    ValidationGroup="Opportunity" Type="Date" Operator="DataTypeCheck" Text="*" Display="Dynamic"
                                                    ErrorMessage="The Project End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                    ToolTip="The Project End Date has an incorrect format. It must be 'MM/dd/yyyy'."></asp:CompareValidator>
                                                <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                                                    ControlToCompare="dpStartDate" ErrorMessage="Project End Date must be greater or equal to Project Start Date."
                                                    ToolTip="Project End Date must be greater or equal to Project Start Date." Text="*"
                                                    EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="GreaterThanEqual"
                                                    Type="Date" ValidationGroup="Opportunity"></asp:CompareValidator>
                                                <asp:CustomValidator ID="cvOpportunityStrawmanEndDateCheck" runat="server" OnServerValidate="cvOpportunityStrawmanEndDateCheck_ServerValidate"
                                                    ErrorMessage="Some exsisting Strawman Need By date is Greater than New Opportunity EndDate."
                                                    ToolTip="Some exsisting Strawman Need By date is Greater than New Opportunity EndDate."
                                                    EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="fontBold Width12Per">
                                    Name
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:TextBox ID="txtOpportunityName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    MaxLength="50" CssClass="Width99Percent"></asp:TextBox>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqOpportunityName" runat="server" ControlToValidate="txtOpportunityName"
                                                    ToolTip="The Opportunity Name is required." Text="*" EnableClientScript="false"
                                                    ErrorMessage="The Opportunity Name is required." SetFocusOnError="true" Display="Dynamic"
                                                    CssClass="Width100Per" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="fontBold Width12Per">
                                    Status
                                </td>
                                <td class="Width38PerImp">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlStatus" runat="server" onchange="EnableSaveButton();EnableOrDisableConvertOrAttachToProj();setDirty();"
                                                    CssClass="WholeWidth Width100Per">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqStatus" runat="server" ControlToValidate="ddlStatus"
                                                    CssClass="Width50Percent" ToolTip="The Status is required." Text="*" EnableClientScript="false"
                                                    ErrorMessage="The Status is required." SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="custWonConvert" runat="server" Text="*" CssClass="Width50Percent"
                                                    ErrorMessage="Cannot convert an opportunity with the status Won to project."
                                                    ValidationGroup="WonConvert" OnServerValidate="custWonConvert_OnServerValidate" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="height30P">
                                <td class="fontBold Width12Per">
                                    Account
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlClient" runat="server" AutoPostBack="true" onchange="EnableSaveButton();setDirty();"
                                                    OnSelectedIndexChanged="ddlClient_SelectedIndexChanged" CssClass="WholeWidth Width100Per" />
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="ddlClient"
                                                    ErrorMessage="The Account is required." ToolTip="The Account is required." Text="*"
                                                    EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"
                                                    CssClass="Width100Per" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="fontBold Width12Per">
                                    Sales Stage
                                    <asp:Image ID="imgPriorityHint" runat="server" ImageUrl="~/Images/hint.png" />
                                    <asp:Panel ID="pnlPriority" Style="display: none;" CssClass="MiniReport SummaryMiniReport"
                                        runat="server">
                                        <table>
                                            <tr>
                                                <th class="textRightImp">
                                                    <asp:Button ID="btnClosePriority" OnClientClick="return false;" runat="server" CssClass="mini-report-close"
                                                        Text="x" />
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:ListView ID="lvOpportunityPriorities" runat="server">
                                                        <LayoutTemplate>
                                                            <div class="lvOpportunityPriorities">
                                                                <table id="itemPlaceHolderContainer" runat="server" class="WholeWidth BackGroundColorWhite">
                                                                    <tr runat="server" id="itemPlaceHolder">
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td class="LabelPriority">
                                                                    <asp:Label ID="lblPriority" CssClass="Width100Px DisplayInline" runat="server" Text='<%# Eval("HtmlEncodedDisplayName") %>'></asp:Label>
                                                                </td>
                                                                <td class="LabelPriority">
                                                                    -
                                                                </td>
                                                                <td class="LabelPriority">
                                                                    <asp:Label ID="lblDescription" runat="server" CssClass="WhiteSpaceNormal DisplayInline"
                                                                        Text='<%# HttpUtility.HtmlEncode((string)Eval("Description")) %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <EmptyDataTemplate>
                                                            <tr>
                                                                <td class="PaddingLeft2px vMiddle">
                                                                    <asp:Label ID="lblNoPriorities" runat="server" Text="No Priorities."></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </EmptyDataTemplate>
                                                    </asp:ListView>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Percent">
                                                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="WholeWidth Width100Per"
                                                    onchange="return ddlPriority_OnSelectedIndexChanged(this);  ">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Percent">
                                                <asp:RequiredFieldValidator ID="reqPriority" runat="server" ControlToValidate="ddlPriority"
                                                    class="Width100Per" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                    ErrorMessage="The Sales Stage is required." Text="*" ToolTip="The Sales Stage is required."
                                                    ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvPriority" runat="server" ControlToValidate="ddlPriority"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    OnServerValidate="cvPriority_ServerValidate" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="Width12Percent">
                                    <b>Business Unit</b>
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width50Percent">
                                                <asp:DropDownList ID="ddlClientGroup" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    OnSelectedIndexChanged="ddlClientGroup_SelectedIndexChanged" DataTextField="Name"
                                                    DataValueField="Id" CssClass="WholeWidth Width97PercentImp" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width50Percent">
                                                <b>Business Group</b>
                                                <asp:Label ID="lblBusinessGroup" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width12Percent PaddingRight0Px">
                                    <table class="Width100Per">
                                        <tr class="Width100Per">
                                            <td class="fontBold PaddingLeft1Px no-wrap">
                                                Est. Revenue
                                            </td>
                                            <td class="no-wrap textRightImp">
                                                $
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width38Per no-wrap">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Percent">
                                                <asp:TextBox ID="txtEstRevenue" CssClass="alignRight" Style="width: 99%" runat="server"
                                                    onchange="EnableSaveButton();setDirty();"></asp:TextBox>
                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkEstRevenue" runat="server"
                                                    TargetControlID="txtEstRevenue" WatermarkText="Ex: 15000, minimum 1000" EnableViewState="false"
                                                    WatermarkCssClass="watermarkedtext Width98Percent" />
                                                <AjaxControlToolkit:FilteredTextBoxExtender ID="fteEstRevenue" TargetControlID="txtEstRevenue"
                                                    FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars="." />
                                            </td>
                                            <td class="Width3Percent">
                                                <asp:RequiredFieldValidator ID="reqEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="The Est. Revenue is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    ErrorMessage="The Est. Revenue is required." Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="custEstimatedRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for the Est. Revenue."
                                                    EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custEstimatedRevenue_ServerValidate"
                                                    Display="Dynamic" ValidationGroup="Opportunity"></asp:CustomValidator>
                                                <asp:CustomValidator ID="custEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="Est. Revenue minimum value should be 1000." Text="*" EnableClientScript="false"
                                                    ErrorMessage="Est. Revenue minimum value should be 1000." SetFocusOnError="true"
                                                    Display="Dynamic" OnServerValidate="custEstRevenue_ServerValidate" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="height30P">
                                <td class="fontBold Width12Per">
                                    Salesperson
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlSalesperson" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    CssClass="WholeWidth Width100Per">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqSalesperson" runat="server" ControlToValidate="ddlSalesperson"
                                                    CssClass="Width100Per" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                    ErrorMessage="The Salesperson is required." Text="*" ToolTip="The Salesperson is required."
                                                    ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="fontBold Width12Per">
                                    Owner
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlOpportunityOwner" runat="server" CssClass="WholeWidth" onchange="EnableSaveButton();setDirty();" />
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqOpportunityOwner" runat="server" ControlToValidate="ddlOpportunityOwner"
                                                    EnableClientScript="false" ValidationGroup="Opportunity" ErrorMessage="The Owner is required."
                                                    SetFocusOnError="true" Text="*" ToolTip="The Owner is required."></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvOwner" runat="server" EnableClientScript="false" ValidationGroup="Opportunity"
                                                    ErrorMessage="The selected owner has been terminated or made inactive.  Please select another owner."
                                                    ValidateEmptyText="true" OnServerValidate="cvOwner_OnServerValidate" SetFocusOnError="true"
                                                    Text="*" ToolTip="The selected owner has been terminated or made inactive.  Please select another owner."></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="fontBold Width12Per">
                                    Buyer Last Name
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:TextBox ID="txtBuyerName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    MaxLength="100" CssClass="Width99Percent"></asp:TextBox>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ToolTip="The Buyer Name is required." Text="*" SetFocusOnError="true" Display="Dynamic"
                                                    ErrorMessage="The Buyer Name is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                    ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                    ValidationGroup="Opportunity" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="fontBold Width12Per">
                                    Practice
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlPractice" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    CssClass="WholeWidth Width100Per">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                                    ToolTip="The Practice is required." Width="100%" Text="*" EnableClientScript="false"
                                                    ErrorMessage="The Practice is required." SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Opportunity" CssClass="Width100Per"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="fontBold Width12Per">
                                    Pricing List
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per WhiteSpaceNoWrap">
                                                <asp:DropDownList ID="ddlPricingList" runat="server" CssClass="WholeWidth" onchange="EnableSaveButton();setDirty();">
                                                    <asp:ListItem Text="--Select Pricing List--" Value="" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Per">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="fontBold Width12Per">
                                    New/Extension
                                </td>
                                <td class="Width38Per">
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width97Per">
                                                <asp:DropDownList ID="ddlBusinessOptions" CssClass="Width100Per" runat="server" onchange="EnableSaveButton();setDirty();">
                                                    <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td class="Width3Per">
                                                <asp:RequiredFieldValidator ID="reqBusinessTypes" runat="server" ControlToValidate="ddlBusinessOptions"
                                                    ToolTip="The New/Extension is required." Width="100%" Text="*" EnableClientScript="false"
                                                    ErrorMessage="The New/Extension is required." SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Opportunity" CssClass="Width100Per"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <AjaxControlToolkit:AnimationExtender ID="animHide" TargetControlID="btnClosePriority"
                            runat="server">
                        </AjaxControlToolkit:AnimationExtender>
                        <AjaxControlToolkit:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint"
                            runat="server">
                        </AjaxControlToolkit:AnimationExtender>
                        <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                            TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                            CancelControlID="btnCancelErrorPanel" DropShadow="false" />
                        <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                            <table class="Width100Per">
                                <tr>
                                    <th class="bgcolorGray TextAlignCenterImp vBottom">
                                        <b class="BtnClose">Attention!</b>
                                        <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                            ToolTip="Cancel" Text="X"></asp:Button>
                                    </th>
                                </tr>
                                <tr>
                                    <td class="Padding10Px">
                                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                        <asp:ValidationSummary ID="vsumOpportunity" runat="server" DisplayMode="BulletList"
                                            CssClass="ApplyStyleForDashBoardLists" ValidationGroup="Opportunity" ShowMessageBox="false"
                                            ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving an opportunity." />
                                        <asp:ValidationSummary ID="vsumWonConvert" runat="server" ValidationGroup="WonConvert"
                                            CssClass="ApplyStyleForDashBoardLists" DisplayMode="BulletList" EnableClientScript="false"
                                            HeaderText="Unable to convert opportunity due to the following errors:" />
                                        <asp:ValidationSummary ID="vsumHasPersons" runat="server" ValidationGroup="HasPersons"
                                            CssClass="ApplyStyleForDashBoardLists" DisplayMode="BulletList" EnableClientScript="false"
                                            HeaderText="Unable to convert opportunity due to the following errors:" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="Padding10Px TextAlignCenterImp">
                                        <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:HiddenField ID="hdnNewStrawmansList" Value="" runat="server" />
                        <AjaxControlToolkit:ModalPopupExtender ID="mpeStrawmansImpactedWithOpportunityStartDate"
                            runat="server" TargetControlID="hdnNewStrawmansList" CancelControlID="btnStrawmansImpactCancel"
                            BehaviorID="mpeBehaviourStrawmansImpactedWithOpportunityStartDate" BackgroundCssClass="modalBackground"
                            PopupControlID="pnlStrawmansImpactedWithOpportunityStartDatePopUp" DropShadow="false"
                            OnCancelScript="ResetStartDate(); return false;" />
                        <asp:Panel ID="pnlStrawmansImpactedWithOpportunityStartDatePopUp" BackColor="White"
                            BorderColor="Black" BorderWidth="2px" runat="server" CssClass="ConfirmBoxErrorClass"
                            Style="display: none;">
                            <table class="Width100Per">
                                <tr>
                                    <td class="AttentionPopUpHeaderStyle vBottom">
                                        Attention!
                                        <asp:Button ID="btnPopupClose" runat="server" CssClass="mini-report-close floatright"
                                            ToolTip="Cancel" OnClientClick="$find('mpeBehaviourStrawmansImpactedWithOpportunityStartDate').hide(); return false;"
                                            Text="X"></asp:Button>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="Padding6">
                                        Following Strawman(s) Need by date will be Adjusted to Opportunity's New Start Date
                                        <asp:Label ID="lblNewOpportunityStartDate" Font-Bold="true" runat="server"></asp:Label>.
                                        <br />
                                        <asp:Label ID="lblStrawmansImpacted" runat="server"></asp:Label>
                                        <p>
                                            <br />
                                            Click Ok to update the Strawman(s) 'Needed By' dates to reflect new Opportunity's
                                            Start Date.
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="Padding6 TextAlignCenterImp">
                                        <asp:Button ID="btnStrawmansImpactOkSave" runat="server" Text="Ok" OnClick="btnStrawmansImpactOkSave_Click" />
                                        <asp:Button ID="btnStrawmansImpactCancel" runat="server" Text="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                    </Triggers>
                </asp:UpdatePanel>
                <br class="height1Px" />
                <AjaxControlToolkit:TabContainer ID="tcOpportunityDetails" runat="server" CssClass="CustomTabStyle"
                    ActiveTabIndex="0">
                    <AjaxControlToolkit:TabPanel ID="tpDescription" runat="server">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Description</span></a></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div class="DivDescription">
                                <asp:UpdatePanel ID="upDescription" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <table class="WholeWidth">
                                            <tr>
                                                <td class="textRightImp">
                                                    <asp:CustomValidator ID="custOppDesciption" runat="server" ControlToValidate="txtDescription"
                                                        Display="Dynamic" OnServerValidate="custOppDescription_ServerValidation" SetFocusOnError="True"
                                                        ErrorMessage="The opportunity description cannot be more than 2000 symbols" ToolTip="The opportunity description cannot be more than 2000 symbols"
                                                        ValidationGroup="Opportunity">*</asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="TxtDescriptionPadding">
                                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="TextBoxProjectNotes WholeWidthImp"
                                                        onchange="EnableSaveButton();setDirty();" MaxLength="2000"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <br />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                    <AjaxControlToolkit:TabPanel ID="tpHistory" runat="server" Visible="true">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>History</span></a></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upActivityLog" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="DivActivityLogControl">
                                        <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Opportunity"
                                            DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false" />
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                    <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                    <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                    <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </AjaxControlToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
                <table class="WholeWidth Backgrounde2ebff">
                    <tr>
                        <td class="TextAlignCenter PaddingLeft8Px padRight8">
                            <asp:UpdatePanel ID="upTeamMakeUp" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="PaddingBottom5 TextAlignCenterImp">
                                                <b class="FontSize16Px">Team Make-Up</b>
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="Width100Per">
                                        <tr>
                                            <td class="Width15Per">
                                            </td>
                                            <td class="Width70Per">
                                                <table class="Width100Per">
                                                    <tr class="PaddingTop2Px">
                                                        <td class="padRight15 textRightImp Width5Percent">
                                                            <asp:Image ID="imgProposed" ToolTip="Select Team Resources" onclick="ShowPotentialResourcesModal(this);"
                                                                ImageUrl="~/Images/People_Icon_Large.png" runat="server" CssClass="CursorPointer" />
                                                        </td>
                                                        <td class="Width90Percent">
                                                            <table class="Width100Per">
                                                                <tr>
                                                                    <td class="TextAlignCenterImp">
                                                                        <b>Team Resources</b>
                                                                    </td>
                                                                    <td class="TextAlignCenterImp">
                                                                        <b>Team Structure</b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="Width50Percent padRight2">
                                                                        <div class="DivProposedPersons textLeft">
                                                                            <asp:DataList ID="dtlProposedPersons" runat="server" CssClass="WhiteSpaceNormal WholeWidthImp">
                                                                                <ItemTemplate>
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                </ItemTemplate>
                                                                                <AlternatingItemTemplate>
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                </AlternatingItemTemplate>
                                                                                <AlternatingItemStyle CssClass="alterrow" />
                                                                            </asp:DataList>
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Literal ID="ltrlOutSideResources" runat="server"></asp:Literal>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                    </td>
                                                                    <td class="Width50Percent padRight2">
                                                                        <div class="DivProposedPersons textLeft">
                                                                            <asp:DataList ID="dtlTeamStructure" runat="server" CssClass="WhiteSpaceNormal WholeWidthImp">
                                                                                <ItemTemplate>
                                                                                    <div class="WholeWidthImp">
                                                                                        <span class="FloatLeft">
                                                                                            <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>(<%# Eval("Quantity") %>)
                                                                                        </span><span class="padRight10 floatright">
                                                                                            <%# ((DateTime)Eval("NeedBy")).ToString("MM/dd/yyyy")%></span>
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <AlternatingItemTemplate>
                                                                                    <div class="WholeWidthImp">
                                                                                        <span class="FloatLeft">
                                                                                            <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>(<%# Eval("Quantity") %>)
                                                                                        </span><span class="padRight10 floatright">
                                                                                            <%# ((DateTime)Eval("NeedBy")).ToString("MM/dd/yyyy")%></span>
                                                                                    </div>
                                                                                </AlternatingItemTemplate>
                                                                                <AlternatingItemStyle CssClass="alterrow" />
                                                                            </asp:DataList>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td class="textLeft Width5Percent paddingLeft15Imp">
                                                            <asp:Image ID="imgStrawMan" ToolTip="Select Team Structure" ImageUrl="~/Images/StrawMan_Large.png"
                                                                onclick="ShowTeamStructureModal(this);" runat="server" CssClass="CursorPointer" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td class="Width15Percent">
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnProposedResourceIdsWithTypes" runat="server" />
                                    <asp:HiddenField ID="hdnProposedPersonsIndexes" runat="server" />
                                    <%--<asp:HiddenField ID="hdnProposedOutSideResources" runat="server" />--%>
                                    <asp:HiddenField ID="hdnmpePotentialResources" runat="server" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpePotentialResources" runat="server"
                                        BehaviorID="behaviorIdPotentialResources" TargetControlID="hdnmpePotentialResources"
                                        EnableViewState="false" BackgroundCssClass="modalBackground" PopupControlID="pnlPotentialResources"
                                        CancelControlID="btnCancelProposedResources" DropShadow="false" />
                                    <asp:Panel ID="pnlPotentialResources" runat="server" BorderColor="Black" BackColor="#d4dff8"
                                        Style="display: none;" Width="372px" BorderWidth="1px">
                                        <table class="Width100Per">
                                            <tr>
                                                <td class="TeamResourcesPadding">
                                                    <center>
                                                        <b>Team Resources</b>
                                                    </center>
                                                    <asp:TextBox ID="txtSearchBox" runat="server" CssClass="TextSearchBoxResources Width353Height16"
                                                        MaxLength="4000" onkeyup="filterPotentialResources(this);"></asp:TextBox>
                                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmSearch" runat="server" TargetControlID="txtSearchBox"
                                                        WatermarkText="Begin typing here to filter the list of resources below." EnableViewState="false"
                                                        WatermarkCssClass="watermarkedtext TextSearchBoxResources Width353Height16" BehaviorID="wmbhSearchBox" />
                                                    <table>
                                                        <tr>
                                                            <td class="Width304Px">
                                                            </td>
                                                            <td class="PaddingRight2Px">
                                                                <asp:Image ID="imgCheck" runat="server" ImageUrl="~/Images/right_icon.png" />
                                                            </td>
                                                            <td class="PaddingLeft2px">
                                                                <asp:Image ID="imgCross" runat="server" ImageUrl="~/Images/cross_icon.png" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div class="cbfloatRight CblPotentialResources">
                                                        <pmc:MultipleSelectionCheckBoxList ID="cblPotentialResources" runat="server" Width="100%"
                                                            BackColor="White" AutoPostBack="false" DataTextField="Name" DataValueField="id"
                                                            OnDataBound="cblPotentialResources_OnDataBound" CellPadding="3">
                                                        </pmc:MultipleSelectionCheckBoxList>
                                                    </div>
                                                    <div class="BtnClearAll">
                                                        <input type="button" value="Clear All" onclick="javascript:ClearProposedResources();" />
                                                    </div>
                                                    <br />
                                                    <table class="Width356Px">
                                                        <tr>
                                                            <td class="TextAlignRight">
                                                                <asp:Button ID="btnAddProposedResources" OnClientClick="saveProposedResources();"
                                                                    OnClick="btnAddProposedResources_Click" runat="server" Text="Add/Update" ToolTip="Add/Update" />
                                                                &nbsp;
                                                                <asp:Button ID="btnCancelProposedResources" runat="server" Text="Cancel" ToolTip="Cancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:HiddenField ID="hdnmpeTeamStructure" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnRowSpliter" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnColoumSpliter" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnStrawmanListInDropdown" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnUsedInactiveStrawmanList" runat="server" Value="" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeTeamStructure" runat="server" BehaviorID="behaviorIdTeamStructure"
                                        TargetControlID="hdnmpeTeamStructure" EnableViewState="false" BackgroundCssClass="modalBackground"
                                        PopupControlID="pnlTeamStructure" CancelControlID="btnTeamCancel" DropShadow="false" />
                                    <asp:Panel ID="pnlTeamStructure" runat="server" BorderColor="Black" BackColor="#d4dff8"
                                        Width="416px" BorderWidth="1px" Style="display: none;">
                                        <table class="Width100Per">
                                            <tr>
                                                <td class="TeamResourcesPadding">
                                                    <center>
                                                        <b>Team Structure</b>
                                                    </center>
                                                    <br />
                                                    <div class="cbfloatRight Width400Px">
                                                        <table class="Width100Per">
                                                            <tr>
                                                                <td class="TextAlignCenterImp Width220Px">
                                                                    <b>Role / Skill</b>
                                                                </td>
                                                                <td class="TextAlignCenterImp Width70Px">
                                                                    <b>QTY</b>
                                                                </td>
                                                                <td class="TextAlignCenterImp">
                                                                    <b>Needed By</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <div class="DivTeamStructure">
                                                        <table id="tblTeamStructure" class="strawman Width100Per strawmanOpportunityDetails">
                                                            <tr>
                                                                <td class="Width220PxImp">
                                                                    <asp:DropDownList ID="ddlStrawmen" runat="server" DataTextField="Name" DataValueField="Id"
                                                                        CssClass="Width220PxImp">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td class="Width76Px TextAlignCenter">
                                                                    <asp:DropDownList ID="ddlQuantity" onchange="this.style.backgroundColor = '';" runat="server"
                                                                        DataTextField="Name" DataValueField="Id" CssClass="Width50PxImp">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td class="TextAlignCenter">
                                                                    <asp:TextBox ID="txtNeedBy" runat="server" CssClass="date-pick Width80Px FloatLeft"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="3" class="textLeft">
                                                                    <asp:ImageButton ID="imgAddStrawman" runat="server" ImageUrl="~/Images/add_16.png"
                                                                        AlternateText="Add Strawman" CssClass="FloatLeft" OnClientClick=" return AddStrawmanRow();" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    </div>
                                                    <div class="DivClearAll">
                                                        <input type="button" value="Clear All" onclick="javascript:ClearTeamStructure();" />
                                                    </div>
                                                    <table class="Width404Px">
                                                        <tr>
                                                            <td class="textRightImp">
                                                                <asp:Button ID="btnSaveTeamStructure" runat="server" Text="Add/Update" ToolTip="Add/Update"
                                                                    OnClientClick="return saveTeamStructure();" OnClick="btnSaveTeamStructure_OnClick" />
                                                                &nbsp;
                                                                <asp:Button ID="btnTeamCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div id="divNeedbyError" class="DivNeedbyError displayNone">
                                                        Need by Date must be greater than or equals to Opportunity start date and less than
                                                        or equals to Opportunity end date.
                                                    </div>
                                                    <div id="divQuantityError" class="DivNeedbyError displayNone">
                                                        Quantity for a Straw Man must be less than or equals to 10 for same Need By date.
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:HiddenField ID="hdnTeamStructure" runat="server" Value="" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                                    <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                    <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                    <asp:AsyncPostBackTrigger ControlID="btnStrawmansImpactOkSave" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="hdnValueChanged" Value="false" runat="server" />
                        <table class="WholeWidth Backgrounde2ebff">
                            <tr>
                                <td class="TdConvertToProject">
                                    <asp:Button ID="btnConvertToProject" runat="server" Text="Convert This Opportunity To Project"
                                        OnClick="btnConvertToProject_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />&nbsp;
                                    <asp:Image ID="hintConvertProject" runat="server" ImageUrl="~/Images/hint.png" ToolTip="When this button is clicked, Practice Management will attempt to create a new Project with the basic information already contained in this Opportunity. If any Proposed Resources have been selected, they will be attached to the new Project as well. " />
                                </td>
                            </tr>
                            <tr>
                                <td class="TdAttachToProject">
                                    <asp:Button ID="btnAttachToProject" runat="server" Text="Attach This Opportunity to Existing Project"
                                        OnClick="btnAttachToProject_Click" ToolTip="Attach This Opportunity to Existing Project" />
                                    <asp:HiddenField ID="hdnField" runat="server" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachToProject" runat="server" TargetControlID="hdnField"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlAttachToProject" CancelControlID="btnCancel"
                                        DropShadow="false" />
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnOpportunityId" runat="server" />
                        <div class="Backgrounde2ebff Padding2Px">
                            <asp:CustomValidator ID="custOpportunityNotSaved" runat="server" ErrorMessage="The opportunity must be saved at first."
                                ToolTip="The opportunity must be saved at first." EnableClientScript="false"
                                EnableViewState="false"></asp:CustomValidator>
                            <asp:Literal ID="ltrWonConvertInvalid" runat="server" EnableViewState="false" Visible="false"
                                Mode="PassThrough">
                                        <script type="text/javascript">
                                            alert('{0}');
                                        </script>
                            </asp:Literal>
                        </div>
                        <table class="Backgrounde2ebff Width100Per">
                            <tr>
                                <td class="Padding4Px Width64Per Height35Px">
                                </td>
                                <td class="Width36Per">
                                    <table class="floatright">
                                        <tr>
                                            <td class="Padding8">
                                                <asp:HiddenField ID="hdnOpportunityDelete" runat="server"></asp:HiddenField>
                                                <asp:Button ID="btnDelete" runat="server" Visible="false" Enabled="false" Text="Delete Opportunity"
                                                    OnClientClick="ConfirmToDeleteOpportunity();" OnClick="btnDelete_Click" />
                                            </td>
                                            <td class="Padding8">
                                                <asp:UpdatePanel ID="upSave" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                            <td class="Padding8">
                                                <asp:UpdatePanel ID="upCancelChanges" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Button ID="btnCancelChanges" runat="server" Text="Cancel" OnClientClick="if(getDirty()){return true;}else{return false;}"
                                                            OnClick="btnCancelChanges_Click" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="Padding4Px Width100Per">
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnOpportunityProjectedStartDate" runat="server" Value="" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="upAttachToProject" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlAttachToProject" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                <table class="Width100Per">
                    <tr class="bgcolorGray Height27Px">
                        <td class="TdAddAttachmentText WholeWidthImp TextAlignCenterImp">
                            Attach This Opportunity to Existing Project
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <table>
                                <tr>
                                    <td>
                                        <p>
                                            Select a Project and Click Attach to Link
                                            <asp:Label ID="lblOpportunityName" runat="server" Font-Bold="true"></asp:Label>
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
                        <td class="OpportunityPrioritiesPopUpTd">
                            <pmc:CustomDropDown ID="ddlProjects" runat="server" AppendDataBoundItems="true" onchange="setDirty();ddlProjects_change(this);"
                                AutoPostBack="false" CssClass="Width350Px">
                            </pmc:CustomDropDown>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="divDescription" runat="server" class="Padding15Px displayNone">
                                <table class="Width100Per">
                                    <tr>
                                        <td class="Width3Percent">
                                        </td>
                                        <td class="Width94Per">
                                            <table class="DescriptionError Width100Per">
                                                <tr>
                                                    <td class="Width100Per Padding4Px">
                                                        <b>Please choose any one of the project/opportunity description.</b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Width100Per Padding4Px vTop">
                                                        <asp:RadioButton ID="rbtnOpportunityDescription" runat="server" GroupName="Description"
                                                            Checked="true" /><b>Opportunity :</b>
                                                        <asp:Label ID="lblOpportunityDescription" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="Width100Per Padding4Px vTop">
                                                        <asp:RadioButton ID="rbtnProjectDescription" runat="server" GroupName="Description"
                                                            Checked="false" /><b>Project &nbsp;:</b>
                                                        <asp:Label ID="lblProjectDescription" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width3Percent">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="OpportunityPrioritiesPopUpTd no-wrap">
                            <asp:Button ID="btnAttach" runat="server" Text="Attach" OnClick="btnSave_Click" disabled="disabled" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return btnCancel_click()" />
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
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAttach" />
            <asp:AsyncPostBackTrigger ControlID="btnSave" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
            <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
            <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="lpOpportunityDetails" runat="server" DisplayText="Saving..." />
    <uc:LoadingProgress ID="lpSave" runat="server" DisplayText="Saving..." AssociatedUpdatePanelID="upSave" />
    <uc:LoadingProgress ID="lpCancel" runat="server" DisplayText="Please wait..." AssociatedUpdatePanelID="upCancelChanges" />
</asp:Content>

