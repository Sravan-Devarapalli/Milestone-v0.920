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
    </style>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
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
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
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

        function clearStrawman(image) {
            ddlQuantity = image.parentNode.parentNode.getElementsByTagName('SELECT')[0];
            ddlQuantity.selectedIndex = 0;
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

            var ddlQtys = $('#tblTeamStructure tr td :input[type = "select-one"]');
            for (var i = 0; i < ddlQtys.length; i++) {
                ddlQtys[i].selectedIndex = 0;
            }
        }





        //        function clearOutSideResources() {
        //            $find("wmBhOutSideResources").set_Text('');
        //        }

        function ShowPotentialResourcesModal(image) {
            var trPotentialResources = document.getElementById('<%=cblPotentialResources.ClientID %>').getElementsByTagName('tr');
            var attachedResourcesIndexes = document.getElementById('<%=hdnProposedPersonsIndexes.ClientID %>').value.split(",");
            //            $find("wmBhOutSideResources").set_Text(document.getElementById('hdnProposedOutSideResources.ClientID %>').value);
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
            var attachedTeam = document.getElementById('<%=hdnTeamStructure.ClientID %>').value.split(",");
            var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
            for (var i = 0; i < trTeamStructure.length; i++) {
                var hdnPersonId = trTeamStructure[i].children[0].getElementsByTagName('input')[0];
                var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
                //                var chkEnabled = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
                ddlQuantity.selectedIndex = 0;
                //                chkEnabled.checked = false;
                for (var j = 0; j < attachedTeam.length; j++) {
                    var personString = attachedTeam[j];
                    var personId = personString.substring(0, personString.indexOf(":", 0));
                    if (personId == hdnPersonId.value) {
                        var personType = personString.substring(personString.indexOf(":", 0) + 1, personString.indexOf("|", 0));
                        var Quantity = personString.substring(personString.indexOf("|", 0) + 1, personString.length);
                        ddlQuantity.value = Quantity;
                        //                        if (personType != "1") {
                        //                            chkEnabled.checked = true;
                        //                        }
                        break;
                    }
                }

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

            var hdnTeamStructureWithIndexes = document.getElementById("<%= hdnTeamStructureWithIndexes.ClientID%>");
            var hdnTeamStructure = document.getElementById("<%= hdnTeamStructure.ClientID%>");

            var trTeamStructure = document.getElementById('tblTeamStructure').getElementsByTagName('tr');
            var PersonIdList = '';
            var personIndexesList = '';
            var personType;
            for (var i = 0; i < trTeamStructure.length; i++) {

                var hdnPersonId = trTeamStructure[i].children[0].getElementsByTagName('input')[0];
                var ddlQuantity = trTeamStructure[i].children[1].getElementsByTagName('SELECT')[0];
                //                var chkEnabled = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
                var hdnIndex = trTeamStructure[i].children[2].getElementsByTagName('input')[0];
                if (ddlQuantity.selectedIndex > 0) {
                    personType = '1';
                    //                    if (!chkEnabled.checked) {
                    //                        personType = '1';
                    //                    }
                    //                    else {
                    //                        personType = '2';
                    //                    }
                    PersonIdList = PersonIdList + hdnPersonId.value + ':' + personType + '|' + ddlQuantity.value + ',';
                    personIndexesList += hdnIndex.value + ':' + personType + '|' + ddlQuantity.value + ',';
                }
            }

            hdnTeamStructure.value = PersonIdList;
            hdnTeamStructureWithIndexes.value = personIndexesList;
        }

        function saveProposedResources() {
            setDirty(); EnableSaveButton();
            //            var hdnProposedOutSideResources = document.getElementById('=hdnProposedOutSideResources.ClientID %>');
            //            hdnProposedOutSideResources.value = $find("wmBhOutSideResources").get_Text();
            GetProposedPersonIdsListWithPersonType();
        }

        function saveTeamStructure() {
            setDirty();
            EnableSaveButton();
            UpdateTeamStructureForHiddenfields();
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
                                            </td>
                                            <td style="padding-left: 4px; padding-right: 8px;">
                                                End Date
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
                                                    ControlToCompare="dpStartDate" ErrorMessage="The Projected End must be greater or equal to the Projected Start."
                                                    ToolTip="The Projected End must be greate or equals to the Projected Start."
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    Operator="GreaterThanEqual" Type="Date" ValidationGroup="Opportunity"></asp:CompareValidator>
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
                                                    onchange="EnableSaveButton();setDirty();">
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
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                    (<%# Eval("Quantity") %>)
                                                                                </ItemTemplate>
                                                                                <AlternatingItemTemplate>
                                                                                    <%# GetFormattedPersonName((string)Eval("Name"), (int)Eval("PersonType"))%>
                                                                                    (<%# Eval("Quantity") %>)
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
                                        Width="372px" BorderWidth="1px" Style="display: none;">
                                        <table width="100%">
                                            <tr>
                                                <td style="padding-left: 5px; padding-top: 5px; padding-bottom: 5px; padding-right: 2px;">
                                                    <center>
                                                        <b>Team Structure</b>
                                                    </center>
                                                    <asp:TextBox ID="txtTeamSearchBox" runat="server" Width="353px" Height="16px" Style="padding-bottom: 4px;
                                                        margin-bottom: 4px;" MaxLength="4000" onkeyup="filterTeamStructure(this);"></asp:TextBox>
                                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmTeamSearchTeam" runat="server"
                                                        TargetControlID="txtTeamSearchBox" WatermarkText="Begin typing here to filter the list of resources below."
                                                        EnableViewState="false" WatermarkCssClass="watermarkedtext" BehaviorID="wmbhSearchBox" />
                                                    <table>
                                                        <tr>
                                                            <td style="width: 275px;">
                                                            </td>
                                                            <td style="text-align: left; width: 45px;">
                                                                QTY
                                                            </td>
                                                            <td style="padding-left: 2px;">
                                                                <%--<asp:Image ID="imgTeamCross" runat="server" ImageUrl="~/Images/cross_icon.png" />--%>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div class="cbfloatRight" style="height: 250px; width: 350px; overflow-y: scroll;
                                                        border: 1px solid black; background: white; padding-left: 3px; text-align: left !important;">
                                                        <table width="100%" id="tblTeamStructure">
                                                            <asp:Repeater ID="rpTeamStructure" runat="server" OnItemDataBound="rpTeamStructure_OnItemDataBound">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td style="padding-top: 4px; width: 275px;">
                                                                            <asp:Label ID="lblStrawman" FirstName='<%# Eval("FirstName") %>' LastName='<%# Eval("LastName") %>'
                                                                                runat="server" Text='<%# Eval("Name") %>'>
                                                                            </asp:Label>
                                                                            <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("Id") %>' />
                                                                        </td>
                                                                        <td style="padding-top: 4px; padding-right: 5px; width: 40px;">
                                                                            <asp:DropDownList ID="ddlQuantity" runat="server" DataTextField="Name" DataValueField="Id">
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>
                                                                            <%--<asp:CheckBox ID="chkEnabled" runat="server" />--%>
                                                                            <asp:Image ID="imgTeamCross" Style="cursor: pointer;" onclick="clearStrawman(this);"
                                                                                runat="server" ImageUrl="~/Images/cross_icon.png" />
                                                                            <asp:HiddenField ID="hdnIndex" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <AlternatingItemTemplate>
                                                                    <tr style="background-color: #f9faff;">
                                                                        <td style="padding-top: 4px; width: 275px;">
                                                                            <asp:Label ID="lblStrawman" FirstName='<%# Eval("FirstName") %>' LastName='<%# Eval("LastName") %>'
                                                                                runat="server" Text='<%# Eval("Name") %>'>
                                                                            </asp:Label>
                                                                            <asp:HiddenField ID="hdnPersonId" runat="server" Value='<%# Eval("Id") %>' />
                                                                        </td>
                                                                        <td style="padding-top: 4px; padding-right: 5px; width: 40px;">
                                                                            <asp:DropDownList ID="ddlQuantity" runat="server" DataTextField="Name" DataValueField="Id">
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>
                                                                            <%--<asp:CheckBox ID="chkEnabled" runat="server" />--%>
                                                                            <asp:Image ID="imgTeamCross" Style="cursor: pointer;" onclick="clearStrawman(this);"
                                                                                runat="server" ImageUrl="~/Images/cross_icon.png" />
                                                                            <asp:HiddenField ID="hdnIndex" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                </AlternatingItemTemplate>
                                                            </asp:Repeater>
                                                        </table>
                                                    </div>
                                                    <div style="text-align: right; width: 356px; padding: 8px 0px 8px 0px">
                                                        <input type="button" value="Clear All" onclick="javascript:ClearTeamStructure();" />
                                                    </div>
                                                    <br />
                                                    <table width="356px;">
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Button ID="btnSaveTeamStructure" runat="server" Text="Add/Update" ToolTip="Add/Update"
                                                                    OnClientClick="javascript:saveTeamStructure();" OnClick="btnSaveTeamStructure_OnClick" />
                                                                &nbsp;
                                                                <asp:Button ID="btnTeamCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:HiddenField ID="hdnTeamStructure" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnTeamStructureWithIndexes" runat="server" Value="" />
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

