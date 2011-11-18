<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="OpportunityDetail.aspx.cs" Inherits="PraticeManagement.OpportunityDetail"
    Title="Opportunity Details | Practice Management" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Opportunities.ViewProjectExtender" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Opportunities/PrevNextOpportunity.ascx" TagPrefix="uc"
    TagName="PrevNextOpportunity" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .transitions
        {
            height: 100px;
        }
        .position
        {
            position: absolute;
        }
        .watermark-Text
        {
            font-style: italic;
            color: Gray;
        }
        .noteWidthHeight
        {
            overflow-y: auto;
            width: 100%;
            height: 100px;
        }
        .black
        {
            color: Black !important;
        }
        
        .selectLink, .selectLink:hover
        {
            text-decoration: none;
            color: Black;
        }
        .FixedHeight
        {
            height: 15px;
            vertical-align: middle;
        }
        .NoteWidth
        {
            padding-right: 3px;
        }
        .InActive
        {
            background-color: InactiveBorder;
        }
        .WordWrap
        {
            word-wrap: break-word !important; /* Internet Explorer 5.5+ */
            word-break: break-all;
            white-space: normal;
        }
        .NoWrap
        {
            white-space: nowrap;
        }
        .lblNoNotes
        {
            background-color: White;
            text-align: left;
            width: 100%;
        }
        .RevenueTextWidth
        {
            width: 98%;
        }
        .LblNoteWidth
        {
            width: 200px;
        }
        
        image:hover:after
        {
            content: attr(title);
        }
        .strawman tr td
        {
            padding: 4px 0px 4px 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/date.js" type="text/javascript"></script>
    <script src="Scripts/datepicker.js" type="text/javascript"></script>
    <link href="Css/datepicker.css" rel="stylesheet" type="text/css" />
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
            var ddlQuantity = document.createElement("select");
            ddlQuantityOriginal = tblTeamStructure.rows[0].cells[1].children[0];
            ddlQuantity.style.width = ddlQuantityOriginal.style.width;
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
            txtNeedBy.className = "date-pick";
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
                // Each Strawman info is in the format "PersonId:PersonType|Quantity?NeedBy"
                ddlPerson.value = personString.substring(0, personString.indexOf(":", 0));
                ddlQuantity.value = personString.substring(personString.indexOf("|", 0) + 1, personString.indexOf("?", 0));
                var needByDate = new Date(personString.substring(personString.indexOf("?", 0) + 1, personString.length));
                txtNeedBy.value = needByDate.format('MM/dd/yyyy');

            }
            $find("behaviorIdTeamStructure").show();
            return false;
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

        function SetEndDateRequired(ddlPriority) {
            var lbEndDate = document.getElementById('<%= lbEndDate.ClientID %>');

            var optionList = ddlPriority.getElementsByTagName('option');
            var selectedText = "";

            for (var i = 0; i < optionList.length; ++i) {
                if (optionList[i].value == ddlPriority.value) {
                    selectedText = optionList[i].innerHTML.toLowerCase();
                    break;
                }
            }

            if (lbEndDate != null) {
                lbEndDate.style.fontWeight = (selectedText == 'a' || selectedText == 'b') ? 'bold' : '';
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
            <td style="padding-left: 5px;" valign="top">
                <asp:UpdatePanel ID="upOpportunityDetail" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="hdnHasProjectIdOrPermission" runat="server" />
                        <asp:Label ID="lblReadOnlyWarning" runat="server" ForeColor="Red" Visible="false">Since you are not the designated owner of this opportunity, you will not be able to make any changes.</asp:Label>
                        <table style="padding-left: 5px;" class="PaddingClass WholeWidth">
                            <tr style="height: 30px;">
                                <td style="width: 12%;">
                                    Opportunity Number
                                </td>
                                <td style="width: 43%">
                                    <asp:Label ID="lblOpportunityNumber" runat="server" />
                                    &nbsp;(last updated:
                                    <asp:Label ID="lblLastUpdate" runat="server" />) &nbsp;&nbsp;
                                    <asp:HyperLink ID="hpProject" runat="server"></asp:HyperLink>
                                </td>
                                <td colspan="2" style="white-space: nowrap; width: 45%;">
                                    <table cellpadding="4px;">
                                        <tr>
                                            <td style="padding-right: 4px;">
                                                <b>Start Date</b>
                                            </td>
                                            <td class="DatePickerPadding" style="padding-left: 4px; padding-right: 4px;">
                                                <uc1:DatePicker ID="dpStartDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                                    ErrorMessage="The Projected Start date is required" ToolTip="The Projected Start date is required."
                                                    ValidationGroup="Opportunity" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cmpStartDateDataTypeCheck" runat="server" ControlToValidate="dpStartDate"
                                                    ValidationGroup="Opportunity" Type="Date" Operator="DataTypeCheck" Text="*" Display="Dynamic"
                                                    ErrorMessage="The Projected Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                    ToolTip="The Projected Start Date has an incorrect format. It must be 'MM/dd/yyyy'."></asp:CompareValidator>
                                                <asp:CustomValidator ID="cvOpportunityStrawmanStartDateCheck" runat="server" OnServerValidate="cvOpportunityStrawmanStartDateCheck_ServerValidate"
                                                    ErrorMessage="Some exsisting Strawman Need By date is less than New Opportunity StartDate."
                                                    ToolTip="Some exsisting Strawman Need By date is less than New Opportunity StartDate."
                                                    EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="Opportunity"/>
                                            </td>
                                            <td style="padding-left: 4px; padding-right: 8px;">
                                                <asp:Label ID="lbEndDate" runat="server" Text="End Date"></asp:Label>
                                            </td>
                                            <td class="DatePickerPadding">
                                                <uc1:DatePicker ID="dpEndDate" ValidationGroup="Opportunity" AutoPostBack="false"
                                                    OnClientChange="EnableSaveButton();setDirty();" TextBoxWidth="62px" runat="server" />
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpEndDate"
                                                    ErrorMessage="End date is required to add Proposed Resources to project." ToolTip="End date is required to add Proposed Resources to project."
                                                    ValidationGroup="HasPersons" Display="Dynamic" Text="*" EnableClientScript="false"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cmpEndDateDataTypeCheck" runat="server" ControlToValidate="dpEndDate"
                                                    ValidationGroup="Opportunity" Type="Date" Operator="DataTypeCheck" Text="*" Display="Dynamic"
                                                    ErrorMessage="The Projected End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                    ToolTip="The Projected End Date has an incorrect format. It must be 'MM/dd/yyyy'."></asp:CompareValidator>
                                                <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                                                    ControlToCompare="dpStartDate" ErrorMessage="Opportunity End Date must greater or Equal to Opportunity Start Date."
                                                    ToolTip="Opportunity End Date must greater or Equal to Opportunity Start Date."
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    Operator="GreaterThanEqual" Type="Date" ValidationGroup="Opportunity"></asp:CompareValidator>
                                                <asp:CustomValidator ID="cvOpportunityStrawmanEndDateCheck" runat="server" OnServerValidate="cvOpportunityStrawmanEndDateCheck_ServerValidate"
                                                    ErrorMessage="Some exsisting Strawman Need By date is Greater than New Opportunity EndDate."
                                                    ToolTip="Some exsisting Strawman Need By date is Greater than New Opportunity EndDate."
                                                    EnableClientScript="false" Display="Dynamic" Text="*" ValidationGroup="Opportunity" />
                                                <asp:CustomValidator ID="cvEndDateRequired" runat="server" OnServerValidate="cvEndDateRequired_ServerValidate"
                                                    ErrorMessage="End date is required before opportunity can be saved with A, or B priority."
                                                    ToolTip="End date is required before opportunity can be saved with A, or B priority."
                                                    ValidationGroup="Opportunity" Display="Dynamic" Text="*" EnableClientScript="false"></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; width: 12%">
                                    Name
                                </td>
                                <td style="width: 38%">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:TextBox ID="txtOpportunityName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    MaxLength="50" Width="98%"></asp:TextBox>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqOpportunityName" runat="server" ControlToValidate="txtOpportunityName"
                                                    ToolTip="The Opportunity Name is required." Text="*" EnableClientScript="false"
                                                    SetFocusOnError="true" Display="Dynamic" Width="100%" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 12%; font-weight: bold;">
                                    Status
                                </td>
                                <td style="width: 38% !important;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlStatus" Width="100%" runat="server" onchange="EnableSaveButton();EnableOrDisableConvertOrAttachToProj();setDirty();"
                                                    CssClass="WholeWidth">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqStatus" runat="server" ControlToValidate="ddlStatus"
                                                    Width="50%" ToolTip="The Status is required." Text="*" EnableClientScript="false"
                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="custWonConvert" runat="server" Text="*" Width="50%" ErrorMessage="Cannot convert an opportunity with the status Won to project."
                                                    ValidationGroup="WonConvert" OnServerValidate="custWonConvert_OnServerValidate" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td style="font-weight: bold; width: 12%;">
                                    Client
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlClient" Width="100%" runat="server" AutoPostBack="true"
                                                    onchange="EnableSaveButton();setDirty();" OnSelectedIndexChanged="ddlClient_SelectedIndexChanged"
                                                    CssClass="WholeWidth" />
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqClient" runat="server" ControlToValidate="ddlClient"
                                                    Width="100%" ToolTip="The Client is required." Text="*" EnableClientScript="false"
                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="font-weight: bold; width: 12%">
                                    Priority
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
                                                                            <td align="center" valign="middle" style="text-align: center; padding: 0px; color: Black;
                                                                                font-size: 12px;">
                                                                                <asp:Label ID="lblPriority" Width="15px" runat="server" Text='<%# Eval("Priority") %>'></asp:Label>
                                                                            </td>
                                                                            <td align="center" valign="middle" style="text-align: center; padding: 0px; padding-left: 2px;
                                                                                padding-right: 2px; color: Black; font-size: 12px;">
                                                                                -
                                                                            </td>
                                                                            <td style="padding: 0px;">
                                                                                <asp:Label ID="lblDescription" runat="server" Width="180px" Style="white-space: normal;
                                                                                    color: Black; font-size: 12px;" Text='<%# Eval("Description") %>'></asp:Label>
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
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlPriority" runat="server" Width="100%" CssClass="WholeWidth"
                                                    onchange="EnableSaveButton();setDirty();SetEndDateRequired(this);">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqPriority" runat="server" ControlToValidate="ddlPriority"
                                                    Width="100%" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                    Text="*" ToolTip="The Priority is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvPriority" runat="server" ControlToValidate="ddlPriority"
                                                    ToolTip="You must add a Team Make-Up to this opportunity before it can be saved
                                    with a PO, A, or B priority." Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    OnServerValidate="cvPriority_ServerValidate" ValidationGroup="Opportunity" />
                                                <asp:CustomValidator ID="cvLinkedToProject" runat="server" ControlToValidate="ddlPriority"
                                                    ErrorMessage="To save an Opportunity as “PO” priority, the Opportunity must first be linked to a Project."
                                                    ToolTip="To save an Opportunity as “PO” priority, the Opportunity must first be linked to a Project."
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    OnServerValidate="cvLinkedToProject_ServerValidate" ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 12%;">
                                    Group
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 100%">
                                                <asp:DropDownList ID="ddlClientGroup" Width="97%" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    DataTextField="Name" DataValueField="Id" CssClass="WholeWidth">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 12%; padding-right: 0px !important;">
                                    <table width="100%">
                                        <tr style="width: 100%;">
                                            <td style="font-weight: bold; padding-left: 1px; white-space: nowrap;">
                                                Est. Revenue
                                            </td>
                                            <td style="text-align: right; white-space: nowrap;">
                                                $
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 38%; white-space: nowrap;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:TextBox ID="txtEstRevenue" CssClass="alignRight" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    Width="98%"></asp:TextBox>
                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkEstRevenue" runat="server"
                                                    TargetControlID="txtEstRevenue" WatermarkText="Ex: 15000, minimum 1000" EnableViewState="false"
                                                    WatermarkCssClass="watermarkedtext" />
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="The Est. Revenue is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="custEstimatedRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                                                    EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custEstimatedRevenue_ServerValidate"
                                                    Display="Dynamic" ValidationGroup="Opportunity"></asp:CustomValidator>
                                                <asp:CustomValidator ID="custEstRevenue" runat="server" ControlToValidate="txtEstRevenue"
                                                    ToolTip="Est. Revenue minimum value should be 1000." Text="*" EnableClientScript="false"
                                                    SetFocusOnError="true" Display="Dynamic" OnServerValidate="custEstRevenue_ServerValidate"
                                                    ValidationGroup="Opportunity" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr style="height: 30px;">
                                <td style="font-weight: bold; width: 12%;">
                                    Salesperson
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlSalesperson" Width="100%" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    CssClass="WholeWidth">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqSalesperson" runat="server" ControlToValidate="ddlSalesperson"
                                                    Width="100%" Display="Dynamic" EnableClientScript="false" SetFocusOnError="true"
                                                    Text="*" ToolTip="The Salesperson is required." ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="font-weight: bold; width: 12%;">
                                    Owner
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlOpportunityOwner" runat="server" CssClass="WholeWidth" onchange="EnableSaveButton();setDirty();" />
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqOpportunityOwner" runat="server" ControlToValidate="ddlOpportunityOwner"
                                                    EnableClientScript="false" ValidationGroup="Opportunity" ErrorMessage="The Owner is required."
                                                    SetFocusOnError="true" Text="*" ToolTip="The Owner is required."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; width: 12%;">
                                    Buyer Last Name
                                </td>
                                <td style="width: 38%">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:TextBox ID="txtBuyerName" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    MaxLength="100" Width="98%"></asp:TextBox>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ToolTip="The Buyer Name is required." Text="*" SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                    ValidationGroup="Opportunity" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="font-weight: bold; width: 12%;">
                                    Practice
                                </td>
                                <td style="width: 38%;">
                                    <table width="100%">
                                        <tr>
                                            <td style="width: 97%">
                                                <asp:DropDownList ID="ddlPractice" runat="server" onchange="EnableSaveButton();setDirty();"
                                                    Width="100%" CssClass="WholeWidth">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 3%">
                                                <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                                    ToolTip="The Practice is required." Width="100%" Text="*" EnableClientScript="false"
                                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="Opportunity"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <ajax:AnimationExtender ID="animHide" TargetControlID="btnClosePriority" runat="server">
                        </ajax:AnimationExtender>
                        <ajax:AnimationExtender ID="animShow" TargetControlID="imgPriorityHint" runat="server">
                        </ajax:AnimationExtender>
                        <asp:HiddenField ID="hdnCanShowPopup" Value="false" runat="server" />
                        <AjaxControlToolkit:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="hdnCanShowPopup"
                            CancelControlID="btnClose" BehaviorID="mpePriorityPopup" BackgroundCssClass="modalBackground"
                            PopupControlID="pnlPopup" DropShadow="false" />
                        <asp:Panel ID="pnlPopup" runat="server" BackColor="White" BorderColor="Black" CssClass="ConfirmBoxClassError"
                            Style="display: none" BorderWidth="2px">
                            <table width="100%">
                                <tr>
                                    <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                                        valign="bottom">
                                        <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                                        <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close" ToolTip="Cancel"
                                            Style="float: right;" Text="X"></asp:Button>
                                    </th>
                                </tr>
                                <tr>
                                    <td style="padding: 10px;" colspan="2">
                                        <table>
                                            <tr>
                                                <td>
                                                    <p>
                                                        You must add a Team Make-Up to this opportunity before it can be saved with a PO,
                                                        A, or B priority.
                                                    </p>
                                                    <br />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnAttach" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                    </Triggers>
                </asp:UpdatePanel>
                <br style="height: 1px;" />
                <ajax:TabContainer ID="tcOpportunityDetails" runat="server" CssClass="CustomTabStyle"
                    ActiveTabIndex="0">
                    <ajax:TabPanel ID="tpDescription" runat="server">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>Description</span></a></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div style="width: 99%; padding-left: 4px; padding-right: 4px; overflow-y: auto;">
                                <asp:UpdatePanel ID="upDescription" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <table class="WholeWidth">
                                            <tr>
                                                <td style="text-align: right">
                                                    <asp:CustomValidator ID="custOppDesciption" runat="server" ControlToValidate="txtDescription"
                                                        Display="Dynamic" OnServerValidate="custOppDescription_ServerValidation" SetFocusOnError="True"
                                                        ToolTip="The opportunity description cannot be more than 2000 symbols" ValidationGroup="Opportunity">*</asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding: 0px 5px 0px 4px;">
                                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" CssClass="WholeWidth"
                                                        Height="80px" onchange="EnableSaveButton();setDirty();" MaxLength="2000" Style="overflow-y: auto;
                                                        resize: none; font-size: 12px; font-family: Arial, Helvetica, sans-serif;"></asp:TextBox>
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
                                        <asp:AsyncPostBackTrigger ControlID="btnCancel" />
                                        <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                        <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
                                        <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
                                        <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </ContentTemplate>
                    </ajax:TabPanel>
                    <ajax:TabPanel ID="tpHistory" runat="server" Visible="true">
                        <HeaderTemplate>
                            <span class="bg"><a href="#"><span>History</span></a></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="upActivityLog" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div style="width: 99%; height: 310px; padding-left: 4px; padding-right: 4px; overflow-y: auto;">
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
                    </ajax:TabPanel>
                </ajax:TabContainer>
                <table class="WholeWidth" style="background: #e2ebff;">
                    <tr>
                        <td align="center" style="padding-left: 8px; padding-right: 8px;">
                            <asp:UpdatePanel ID="upTeamMakeUp" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <table style="width: 100%;">
                                        <tr>
                                            <td align="center" style="padding-bottom: 5px;">
                                                <b style="font-size: 16px;">Team Make-Up</b>
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 100%;" width="100%">
                                        <tr>
                                            <td style="width: 15%;">
                                            </td>
                                            <td style="width: 70%;">
                                                <table style="width: 100%;" width="100%">
                                                    <tr style="padding-top: 2px;">
                                                        <td align="right" style="width: 5%; padding-right: 15px;">
                                                            <asp:Image ID="imgProposed" ToolTip="Select Team Resources" Style="cursor: pointer;"
                                                                onclick="ShowPotentialResourcesModal(this);" ImageUrl="~/Images/People_Icon_Large.png"
                                                                runat="server" />
                                                        </td>
                                                        <td style="width: 90%;">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td align="center">
                                                                        <b>Team Resources</b>
                                                                    </td>
                                                                    <td align="center">
                                                                        <b>Team Structure</b>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 50%; padding-right: 2px;">
                                                                        <div align="left" style="height: 175px; width: 100%; overflow-y: auto; border: 1px solid black;
                                                                            background: white; padding-left: 3px; line-height: 19px; padding-top: 3px; padding-bottom: 3px;">
                                                                            <asp:DataList ID="dtlProposedPersons" runat="server" Style="white-space: normal;
                                                                                width: 100%;">
                                                                                <ItemTemplate>
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                </ItemTemplate>
                                                                                <AlternatingItemTemplate>
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                </AlternatingItemTemplate>
                                                                                <AlternatingItemStyle BackColor="#f9faff" />
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
                                                                    <td style="width: 50%; padding-left: 2px;">
                                                                        <div align="left" style="height: 175px; width: 100%; overflow-y: auto; line-height: 19px;
                                                                            border: 1px solid black; background: white; padding-left: 3px; padding-top: 3px;
                                                                            padding-bottom: 3px;">
                                                                            <asp:DataList ID="dtlTeamStructure" runat="server" Style="white-space: normal; width: 100%;">
                                                                                <ItemTemplate>
                                                                                    <div style="width: 100%;">
                                                                                        <span style="float: left; padding-left: 10px;">
                                                                                            <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>(<%# Eval("Quantity") %>)
                                                                                        </span><span style="float: right; padding-right: 10px;">
                                                                                            <%# ((DateTime)Eval("NeedBy")).ToString("MM/dd/yyyy")%></span>
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <AlternatingItemTemplate>
                                                                                    <div style="width: 100%;">
                                                                                        <span style="float: left; padding-left: 10px;">
                                                                                            <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>(<%# Eval("Quantity") %>)
                                                                                        </span><span style="float: right; padding-right: 10px;">
                                                                                            <%# ((DateTime)Eval("NeedBy")).ToString("MM/dd/yyyy")%></span>
                                                                                    </div>
                                                                                </AlternatingItemTemplate>
                                                                                <AlternatingItemStyle BackColor="#f9faff" />
                                                                            </asp:DataList>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left" style="width: 5%; padding-left: 15px;">
                                                            <asp:Image ID="imgStrawMan" ToolTip="Select Team Structure" Style="cursor: pointer;"
                                                                ImageUrl="~/Images/StrawMan_Large.png" onclick="ShowTeamStructureModal(this);"
                                                                runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td style="width: 15%;">
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
                                                            OnDataBound="cblPotentialResources_OnDataBound" CellPadding="3">
                                                        </uc:MultipleSelectionCheckBoxList>
                                                    </div>
                                                    <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
                                                        <input type="button" value="Clear All" onclick="javascript:ClearProposedResources();" />
                                                    </div>
                                                    <%-- <table style="width: 100%">
                                                        <tr>
                                                            <td style="width: 93% !important;">
                                                                <asp:TextBox ID="txtOutSideResources" runat="server" Width="100%" Height="16px" Style="padding-bottom: 4px;
                                                                    margin-bottom: 4px;" MaxLength="4000"></asp:TextBox>
                                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmOutSideResources" runat="server"
                                                                    TargetControlID="txtOutSideResources" WatermarkText="Enter Other Names(s) (optional) separated by semi-colons."
                                                                    EnableViewState="false" WatermarkCssClass="watermarkedtext" BehaviorID="wmBhOutSideResources" />
                                                            </td>
                                                            <td align="right">
                                                                <img id="imgtrash" src="Images/trash-icon-Large.png" onclick="clearOutSideResources();"
                                                                    style="cursor: pointer; padding-bottom: 5px;" />
                                                            </td>
                                                        </tr>
                                                    </table>--%>
                                                    <br />
                                                    <table width="356px;">
                                                        <tr>
                                                            <td align="right">
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
                                                    <div  style="height: 250px; width: 400px; overflow-y: scroll;
                                                        border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                                                        <table width="100%" id="tblTeamStructure" class="strawman">
                                                            <tr>
                                                                <td style="width: 220px;">
                                                                    <asp:DropDownList ID="ddlStrawmen" runat="server" DataTextField="Name" DataValueField="Id"
                                                                        Width="220px">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="width: 76px;" align="center">
                                                                    <asp:DropDownList ID="ddlQuantity" onchange="this.style.backgroundColor = '';" runat="server"
                                                                        DataTextField="Name" DataValueField="Id" Style="width: 50px;">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td align="center">
                                                                    <asp:TextBox ID="txtNeedBy" runat="server" Style="width: 80px;float:left;" CssClass="date-pick"></asp:TextBox>
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
                                                    </div>
                                                    <div style="text-align: right; width: 404px; padding: 8px 0px 10px 0px">
                                                        <input type="button" value="Clear All" onclick="javascript:ClearTeamStructure();" />
                                                    </div>
                                                    <table width="404px;">
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Button ID="btnSaveTeamStructure" runat="server" Text="Add/Update" ToolTip="Add/Update"
                                                                    OnClientClick="return saveTeamStructure();" OnClick="btnSaveTeamStructure_OnClick" />
                                                                &nbsp;
                                                                <asp:Button ID="btnTeamCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div id="divNeedbyError" style="text-align: left; display: none; color: Red; width: 404px;
                                                        padding: 8px 0px 10px 0px">
                                                        Need by Date must be greater than or equals to Opportunity start date and less than
                                                        or equals to Opportunity end date.
                                                    </div>
                                                    <div id="divQuantityError" style="text-align: left; display: none; color: Red; width: 404px;
                                                        padding: 8px 0px 10px 0px">
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
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="hdnValueChanged" Value="false" runat="server" />
                        <table class="WholeWidth" style="background: #e2ebff;">
                            <tr>
                                <td align="center" style="padding: 10px 0px 0px 4px; width: 100%;">
                                    <asp:Button ID="btnConvertToProject" runat="server" Text="Convert This Opportunity To Project"
                                        OnClick="btnConvertToProject_Click" OnClientClick="if (!confirmSaveDirty(true)) return false;" />&nbsp;
                                    <asp:Image ID="hintConvertProject" runat="server" ImageUrl="~/Images/hint.png" ToolTip="When this button is clicked, Practice Management will attempt to create a new Project with the basic information already contained in this Opportunity. If any Proposed Resources have been selected, they will be attached to the new Project as well. " />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" style="padding: 6px 0px 0px 0px; width: 100%;">
                                    <asp:Button ID="btnAttachToProject" runat="server" Text="Attach This Opportunity to Existing Project"
                                        OnClick="btnAttachToProject_Click" ToolTip="Attach This Opportunity to Existing Project" />
                                    <asp:HiddenField ID="hdnField" runat="server" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachToProject" runat="server" TargetControlID="hdnField"
                                        CancelControlID="btnCancel" BackgroundCssClass="modalBackground" PopupControlID="pnlAttachToProject"
                                        DropShadow="false" />
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnOpportunityId" runat="server" />
                        <div style="background-color: #e2ebff; padding: 2px;">
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
                        <table style="width: 100%; background: #e2ebff;">
                            <tr>
                                <td style="padding: 4px; height: 35px; width: 64%;">
                                    <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                    <asp:ValidationSummary ID="vsumOpportunity" runat="server" ValidationGroup="Opportunity"
                                        EnableClientScript="false" HeaderText="Error occurred while saving an opportunity." />
                                    <asp:ValidationSummary ID="vsumOpportunityTransition" runat="server" ValidationGroup="OpportunityTransition"
                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to proceed with opportunity transition due to the following errors:" />
                                    <asp:ValidationSummary ID="vsumWonConvert" runat="server" ValidationGroup="WonConvert"
                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to convert opportunity due to the following errors:" />
                                    <asp:ValidationSummary ID="vsumHasPersons" runat="server" ValidationGroup="HasPersons"
                                        DisplayMode="BulletList" EnableClientScript="false" HeaderText="Unable to convert opportunity due to the following errors:" />
                                </td>
                                <td style="padding: 4px; height: 35px; width: 13%;">
                                    <asp:HiddenField ID="hdnOpportunityDelete" runat="server"></asp:HiddenField>
                                    <asp:Button ID="btnDelete" runat="server" Visible="false" Enabled="false" Text="Delete Opportunity"
                                        OnClientClick="ConfirmToDeleteOpportunity();" OnClick="btnDelete_Click" />
                                </td>
                                <td style="padding: 4px; height: 35px; width: 13%;">
                                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClick="btnSave_Click" />
                                </td>
                                <td style="padding: 4px; height: 35px; width: 13%;">
                                    <asp:Button ID="btnCancelChanges" runat="server" Text="Cancel Changes" OnClientClick="if(getDirty()){return true;}else{return false;}"
                                        OnClick="btnCancelChanges_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding: 4px; width: 100%;">
                                </td>
                            </tr>
                        </table>
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
                <table width="100%">
                    <tr style="background-color: Gray; height: 27px;">
                        <td align="center" style="white-space: nowrap; font-size: 14px; width: 100%">
                            Attach This Opportunity to Existing Project
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 6px 6px 2px 2px;">
                            <asp:DropDownList ID="ddlProjects" runat="server" AppendDataBoundItems="true" onchange="setDirty();"
                                AutoPostBack="false" Style="width: 350px">
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
                            <asp:Button ID="btnAttach" runat="server" Text="Attach" OnClick="btnSave_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
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
            <asp:AsyncPostBackTrigger ControlID="btnCancel" />
            <asp:AsyncPostBackTrigger ControlID="btnSave" />
            <asp:AsyncPostBackTrigger ControlID="btnCancelChanges" />
            <asp:AsyncPostBackTrigger ControlID="btnConvertToProject" />
            <asp:AsyncPostBackTrigger ControlID="btnAttachToProject" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="lpOpportunityDetails" runat="server" />
</asp:Content>

