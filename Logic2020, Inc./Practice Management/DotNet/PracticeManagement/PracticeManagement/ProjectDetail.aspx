<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="True"
    CodeBehind="ProjectDetail.aspx.cs" Inherits="PraticeManagement.ProjectDetail"
    Title="Project Details | Practice Management" EnableEventValidation="false" ValidateRequest="False" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register TagPrefix="extDisable" Namespace="PraticeManagement.Controls.Generic.ElementDisabler"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectMilestonesFinancials.ascx"
    TagName="ProjectMilestonesFinancials" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectTimeTypes.ascx" TagName="ProjectTimeTypes" %>
<%@ Register Src="Controls/ProjectExpenses/ProjectExpensesControl.ascx" TagName="ProjectExpenses"
    TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectFinancials.ascx" TagName="ProjectFinancials" %>
<%--<%@ Register TagPrefix="uc" Src="~/Controls/Projects/ProjectPersons.ascx" TagName="ProjectPersons" %>--%>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/ScrollinDropDown.js" type="text/javascript"></script>
    <script src="Scripts/FilterTable.js" type="text/javascript"></script>
    <script src="Scripts/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        function checkDirty(target, entityId) {
            if (showDialod()) {
                __doPostBack('__Page', target + ':' + entityId);
                return true;
            }

            return false;
        }

        function ConfirmUnlink() {

            if (confirm("Are you sure you want to unlink the existing Opportunity?")) {
                return true;
            }

            return false;
        }

        function RedirectToOpportunity() {
            var imgRedirectLink = document.getElementById('<%= imgNavigateToOpp.ClientID %>');
            var url = imgRedirectLink.getAttribute("NavigateUrl");

            window.open(url);
        }

        function cvProjectAttachment_ClientValidationFunction(obj, args) {
            var fuControl = document.getElementById('<%= fuProjectAttachment.ClientID %>');
            var FileUploadPath = fuControl.value;
            var Extension = FileUploadPath.substring(FileUploadPath.lastIndexOf('.') + 1).toLowerCase();
            if (Extension == "pdf" || Extension == "doc" || Extension == "docx") {
                args.IsValid = true; // Valid file type
            }
            else {
                args.IsValid = false; // Not valid file type
            }
        }
        function cvAttachment_ClientValidationFunction(obj, args) {
            var fuControl = document.getElementById('<%= fuProjectAttachment.ClientID %>');
            var fileUploadPath = fuControl.value;
            if (fileUploadPath != null && fileUploadPath != undefined) {
                args.IsValid = true; // Valid file type
            }
            else {
                args.IsValid = false; // Not valid file type
            }
        }

        function cvAttachmentCategory_ClientValidationFunction(obj, args) {
            var ddlAttachmentCategory = document.getElementById('<%= ddlAttachmentCategory.ClientID %>');
            var categoryValue = ddlAttachmentCategory.value;
            if (categoryValue != "0") {
                args.IsValid = true; // Valid 
            }
            else {
                args.IsValid = false; // Not valid 
            }
        }

        function EnableUploadButton() {
            var cvProjectAttachment = document.getElementById('<%= cvProjectAttachment.ClientID %>');
            var cvAttachmentCategory = document.getElementById('<%= cvAttachmentCategory.ClientID %>');
            var cvAttachment = document.getElementById('<%= cvAttachment.ClientID %>');
            var UploadButton = document.getElementById('<%= btnUpload.ClientID %>');
            if (cvProjectAttachment.isvalid && cvAttachmentCategory.isvalid && cvAttachment.isvalid) {
                UploadButton.disabled = "";
            }
            else {
                UploadButton.disabled = "disabled";
            }
        }

        function CanShowPrompt() {
            return true;
        }

        function ConfirmToDeleteProject() {
            var hdnProject = document.getElementById('<%= hdnProjectDelete.ClientID %>');
            var result = confirm("Do you really want to delete the project?");
            hdnProject.value = result ? 1 : 0;
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

        function CheckIfDatesValid() {
            txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
            txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
                var btnCustDatesClose = document.getElementById('<%= activityLog.ClientID %>_btnCustDatesClose');
                hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
                hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
                lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                var startDate = new Date(txtStartDate.value);
                var endDate = new Date(txtEndDate.value);
                var startDateStr = startDate.format("MM/dd/yyyy");
                var endDateStr = endDate.format("MM/dd/yyyy");
                hdnStartDate.value = startDateStr;
                hdnEndDate.value = endDateStr;
                lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
                btnCustDatesClose.click();

            }
            return false;
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
            lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "";
                    lblCustomDateRange.style.display = "";
                    imgCalender.click();
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('click', true, true);
                    imgCalender.dispatchEvent(event);
                }
            }
            else {
                imgCalender.attributes["class"].value = "displayNone";
                lblCustomDateRange.attributes["class"].value = "displayNone";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }
        }

        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
            hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
            txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
            txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnStartDateCalExtenderBehaviourId');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnEndDateCalExtenderBehaviourId');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }
            CheckIfDatesValid();
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            SetTooltipsForallDropDowns();
            $("#tblProjectAttachments").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
            var activityLog = document.getElementById('<%= activityLog.ClientID%>');
            if (activityLog != null) {
                imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
                lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                ddlPeriod = document.getElementById('<%=  activityLog.ClientID %>_ddlPeriod');
                if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }

        }

        //region project time types script

        function btnClose_OnClientClick() {
            $find("mpeTimetypeAlertMessage").hide();
            return false;
        }

        function DeleteWorkType(timetypeid) {

            if (confirm("Are you sure you want to delete this Work Type?")) {
                var btnDeleteWorkType = document.getElementById('<%= btnDeleteWorkType.ClientID%>');
                var hdnWorkTypeId = document.getElementById('<%= hdnWorkTypeId.ClientID%>');
                hdnWorkTypeId.value = timetypeid;
                btnDeleteWorkType.click();
            }

            return false;
        }

        // End Region projecttimetypes script

        $(document).ready(function () {
            SetTooltipsForallDropDowns();
            $("#tblProjectAttachments").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        });
    </script>
    <style type="text/css">
        /* --------- Tabs for person and project details pages ------ */
        
        .projectTimeTypes tr
        {
            height: 20px !important;
        }
        
        .projectTimeTypes td
        {
            float: none !important;
            padding: 0px !important;
            height: 20px !important;
        }
        
        .CustomTabStyle tr
        {
            height: 30px;
        }
        
        .CustomTabStyle td
        {
            float: left;
            padding: 8px 0px 6px 0px;
            position: relative;
        }
        
        .CustomTabStyle td a
        {
            text-decoration: none;
        }
        
        .CustomTabStyle td span a
        {
            border-bottom: 1px dashed #0898e6;
        }
        
        .CustomTabStyle td span a:hover
        {
            border-bottom: 1px dashed #006699;
        }
        
        .CustomTabStyle td a.collapse
        {
            display: none;
            position: absolute;
        }
        
        .CustomTabStyle .SelectedSwitch a.collapse
        {
            display: block;
            right: 5px;
            top: 10px;
        }
        
        .CustomTabStyle td span.bg
        {
            padding: 8px 20px 7px 10px;
        }
        
        .CustomTabStyle .SelectedSwitch span.bg
        {
            background-color: #e2ebff;
        }
        
        .tab-pane
        {
            background-color: #e2ebff;
            padding: 5px;
        }
        
        /* ------------------------ */
        
        table.ProjectDetail-ProjectInfo-Table td
        {
            padding-left: 4px;
        }
        
        .ProjectAttachmentNameWrap
        {
            display: inline-block;
            white-space: normal !important;
            word-wrap: break-word;
        }
    </style>
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td style="width: 2%">
                    </td>
                    <td>
                        <table class="WholeWidth">
                            <tr>
                                <td style="width: 66%; padding: 3px 0px 3px 0px; padding-left: 0px;">
                                    <asp:TextBox ID="txtProjectNameFirstTime" runat="server" Visible="false" Width="500"></asp:TextBox>
                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="txtweProjectNameFirstTime" runat="server"
                                        TargetControlID="txtProjectNameFirstTime" WatermarkText="Enter a Project Name here..."
                                        EnableViewState="false" WatermarkCssClass="watermarkedtext" />
                                    <asp:Label ID="lblProjectNumber" runat="server" Style="font-size: 15px; font-weight: bold;"></asp:Label>
                                    <asp:Label ID="lblProjectName" runat="server" Style="font-size: 15px; font-weight: bold;"></asp:Label>
                                    <asp:Label ID="lblProjectRange" runat="server" Style="font-size: 15px; font-weight: bold;"></asp:Label>
                                    <asp:Image ID="imgEditProjectName" ToolTip="Edit Project Name" ImageUrl="~/Images/icon-edit.png"
                                        runat="server" />
                                    <asp:CustomValidator ID="cvProjectName" runat="server" ErrorMessage="The Project Name is required."
                                        ToolTip="The Project Name is required." ValidationGroup="Project" Text="*" EnableClientScript="false"
                                        SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvProjectName_ServerValidate"></asp:CustomValidator>
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeEditProjectName" runat="server" TargetControlID="imgEditProjectName"
                                        CancelControlID="btncloseEditProjectName" BehaviorID="mpeEditProjectName" BackgroundCssClass="modalBackground"
                                        PopupControlID="pnlProjectName" DropShadow="false" />
                                </td>
                                <td style="width: 2%;">
                                </td>
                                <td style="width: 32%;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="font-weight: bold; font-size: 15px; width: 33%;">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            Project Status
                                                        </td>
                                                        <td>
                                                            <div id="divStatus" style="padding-left: 3px;" runat="server">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td style="vertical-align: middle; line-height: 1px; width: 65%;">
                                                <asp:DropDownList ID="ddlProjectStatus" runat="server" onchange="setDirty();" AutoPostBack="True"
                                                    CssClass="Width95Per" OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="reqProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                    ErrorMessage="The Status is required." ToolTip="The Status is required." ValidationGroup="Project"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="custProjectStatus" runat="server" ControlToValidate="ddlProjectStatus"
                                                    ErrorMessage="Only administrators can make projects Active or Completed." ToolTip="Only administrators can make projects Active or Completed."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" OnServerValidate="custProjectStatus_ServerValidate"></asp:CustomValidator>
                                                <asp:CustomValidator ID="cvIsInternal" runat="server" EnableClientScript="false"
                                                    ErrorMessage="Can not change project status as some work types are already in use."
                                                    ValidateEmptyText="true" Text="*" ToolTip="Can not change project status as some timetypes are already in use."></asp:CustomValidator>
                                            </td>
                                            <td style="width: 2%;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 2%">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 2%">
                    </td>
                    <td>
                        <table class="WholeWidth">
                            <tr>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                Account
                                            </td>
                                            <td style="width: 65%;">
                                                <asp:DropDownList ID="ddlClientName" runat="server" OnSelectedIndexChanged="ddlClientName_SelectedIndexChanged"
                                                    CssClass="Width95Per" AutoPostBack="True" onchange="setDirty();">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:RequiredFieldValidator ID="reqClientName" runat="server" ControlToValidate="ddlClientName"
                                                    ErrorMessage="The Account Name is required." ToolTip="The Account Name is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvClient" runat="server" ErrorMessage="Project's account cannot be modified as some time entered towards this Account-BusinessUnit-Project."
                                                    ToolTip="Project's account cannot be modified as some time entered towards this Account-BusinessUnit-Project."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" OnServerValidate="cvClient_ServerValidate"></asp:CustomValidator>
                                                <asp:CustomValidator ID="cvClientOpportunityLinked" runat="server" ErrorMessage="Project's account cannot be modified as this project is linked to an Opportunity, Please unlink the Opportunity before account changed."
                                                    ToolTip="Project's account cannot be modified as this project is linked to an Opportunity, Please unlink the Opportunity before account changed."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" OnServerValidate="cvClientOpportunityLinked_ServerValidate"></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                Salesperson
                                            </td>
                                            <td style="width: 65%; white-space: nowrap;">
                                                &nbsp;&nbsp;
                                                <asp:DropDownList ID="ddlSalesperson" runat="server" AutoPostBack="True" CssClass="Width95Per"
                                                    Enabled="false" onchange="setDirty();" OnSelectedIndexChanged="ddlSalesperson_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:HiddenField ID="hidSalesCommissionId" runat="server" />
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:RequiredFieldValidator ID="reqSalesperson" runat="server" ControlToValidate="ddlSalesperson"
                                                    EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Sales person is required."
                                                    SetFocusOnError="true" Text="*" ToolTip="The Sales person is required."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 33%;">
                                                Client Director
                                            </td>
                                            <td style="width: 65%;">
                                                <asp:DropDownList ID="ddlDirector" runat="server" Enabled="false" CssClass="Width95Per"
                                                    onchange="setDirty();">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 2%;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                Business Unit
                                            </td>
                                            <td style="width: 65%;">
                                                <asp:DropDownList ID="ddlProjectGroup" runat="server" Enabled="false" CssClass="Width95Per"
                                                    OnSelectedIndexChanged="ddlProjectGroup_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:CustomValidator ID="cvGroup" runat="server" ErrorMessage="Project's business unit cannot be modified as some time entered towards this Account-BusinessUnit-Project."
                                                    ToolTip="Project's business unit cannot be modified as some time entered towards this Account-BusinessUnit-Project."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" OnServerValidate="cvGroup_ServerValidate"></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                Buyer Name
                                            </td>
                                            <td style="width: 65%; white-space: nowrap;">
                                                &nbsp;&nbsp;
                                                <asp:TextBox ID="txtBuyerName" runat="server" onchange="setDirty();" CssClass="Width92Per"
                                                    MaxLength="100"></asp:TextBox>
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:RequiredFieldValidator ID="reqBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ErrorMessage="The Buyer Name is required." ToolTip="The Buyer Name is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="valregBuyerName" runat="server" ControlToValidate="txtBuyerName"
                                                    ErrorMessage="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                    ToolTip="Buyer Name should be limited to 2-30 characters in length containing only letters and/or an apostrophe or hyphen."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" ValidationExpression="^[a-zA-Z'\-]{2,30}$"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 33%;">
                                                Project Owner
                                            </td>
                                            <td style="width: 65%;">
                                                <asp:DropDownList ID="ddlProjectOwner" runat="server" onchange="setDirty();" CssClass="Width95Per">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 2%; padding-left: 2px;">
                                                <asp:RequiredFieldValidator ID="reqProjectOwner" runat="server" ControlToValidate="ddlProjectOwner"
                                                    EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Project Owner is required."
                                                    SetFocusOnError="true" Text="*" ToolTip="The Project Owner is required."></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvProjectOwner" runat="server" EnableClientScript="false"
                                                    ValidationGroup="Project" ErrorMessage="The selected owner has been terminated or made inactive.  Please select another owner."
                                                    ValidateEmptyText="true" OnServerValidate="cvProjectOwner_OnServerValidate" SetFocusOnError="true"
                                                    Text="*" ToolTip="The selected owner has been terminated or made inactive.  Please select another owner."></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                Practice Area
                                            </td>
                                            <td style="width: 65%;">
                                                <asp:DropDownList ID="ddlPractice" runat="server" onchange="setDirty();" CssClass="Width95Per"
                                                    AutoPostBack="True" OnSelectedIndexChanged="DropDown_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:RequiredFieldValidator ID="reqPractice" runat="server" ControlToValidate="ddlPractice"
                                                    EnableClientScript="false" ValidationGroup="Project" ErrorMessage="The Practice Area is required."
                                                    SetFocusOnError="true" Text="*" ToolTip="The Practice Area is required."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 30%;">
                                                SOW Budget
                                            </td>
                                            <td style="width: 65%; white-space: nowrap;">
                                                $
                                                <asp:TextBox ID="txtSowBudget" runat="server" onchange="setDirty();" CssClass="Width92Per"
                                                    Style="width: 92% !important" MaxLength="100"></asp:TextBox>
                                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarkSowBudget" runat="server"
                                                    TargetControlID="txtSowBudget" WatermarkText="Ex: 15000" EnableViewState="false"
                                                    WatermarkCssClass="watermarkedtext" />
                                                <AjaxControlToolkit:FilteredTextBoxExtender ID="fteSowBudget" TargetControlID="txtSowBudget"
                                                    FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                                            </td>
                                            <td style="width: 5%;">
                                                <asp:CustomValidator ID="custSowBudget" runat="server" ControlToValidate="txtSowBudget"
                                                    ToolTip="A number with 2 decimal digits is allowed for the Est. Revenue." Text="*"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for the SOW Budget."
                                                    EnableClientScript="false" SetFocusOnError="true" OnServerValidate="custSowBudget_ServerValidate"
                                                    Display="Dynamic" ValidationGroup="Project"></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 2%">
                                </td>
                                <td style="width: 32%; padding: 3px 0px 3px 0px;">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="width: 33%; vertical-align: top; line-height: 20px;">
                                                Project Manager(s)
                                            </td>
                                            <td style="width: 65%;" class="ScrollingDropDownWholeWidth">
                                                <cc2:ScrollingDropDown ID="cblProjectManagers" runat="server" SetDirty="true" CssClass="Width92Per"
                                                    AllSelectedReturnType="AllItems" Height="240px" onclick="scrollingDropdown_onclick('cblProjectManagers','Project Manager');"
                                                    DropDownListType="Manager" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeProjectManagers" runat="server" TargetControlID="cblProjectManagers"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png">
                                                </ext:ScrollableDropdownExtender>
                                                <asp:HiddenField ID="hidPracticeManagementCommissionId" runat="server" />
                                            </td>
                                            <td style="width: 2%; padding-left: 2px;">
                                                <asp:CustomValidator ID="cvProjectManager" runat="server" EnableClientScript="false"
                                                    ValidationGroup="Project" ErrorMessage="The Project Manager(s) is required."
                                                    ValidateEmptyText="true" OnServerValidate="cvProjectManager_OnServerValidate"
                                                    SetFocusOnError="true" Text="*" ToolTip="The Project Manager(s) is required."></asp:CustomValidator>
                                                <asp:CustomValidator ID="cvProjectManagerStatus" runat="server" EnableClientScript="false"
                                                    ValidationGroup="Project" ErrorMessage="The selected Project Manager(s) has been terminated or made inactive.  Please select another Project Manager(s)."
                                                    ValidateEmptyText="true" OnServerValidate="cvProjectManagerStatus_OnServerValidate"
                                                    SetFocusOnError="true" Text="*" ToolTip="The selected Project Manager(s) has been terminated or made inactive.  Please select another Project Manager(s)."></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 2%">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 2%">
                    </td>
                    <td>
                        <table class="WholeWidth">
                            <tr>
                                <td style="width: 55%; height: 120px;">
                                    <table class="WholeWidth" height="100%">
                                        <tr>
                                            <td style="height: 20px; font-size: 15px; font-style: italic; vertical-align: bottom;
                                                padding-bottom: 5px;">
                                                <u>Project Notes</u>
                                                <asp:CustomValidator ID="custProjectDesciption" runat="server" ControlToValidate="txtDescription"
                                                    Display="Dynamic" OnServerValidate="custProjectDesciption_ServerValidation" SetFocusOnError="True"
                                                    ErrorMessage="The project description cannot be more than 2000 symbols" ToolTip="The project description cannot be more than 2000 symbols"
                                                    ValidationGroup="Project">*</asp:CustomValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" Width="98%"
                                                    Height="80px" onchange="setDirty();" Style="overflow-y: auto; resize: none; font-size: 12px;
                                                    font-family: Arial, Helvetica, sans-serif;"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 45%; padding-left: 10px; height: 120px;">
                                    <table class="WholeWidth" height="100%">
                                        <tr>
                                            <td style="height: 20px; font-size: 15px; font-style: italic; vertical-align: bottom;
                                                padding-bottom: 5px;">
                                                <u>Opportunity Linking</u>
                                                <asp:ImageButton ID="imgLink" runat="server" AlternateText="Link Opportunity" ToolTip="Link Opportunity"
                                                    OnClick="imgLink_Click" Visible="true" ImageUrl="~/Images/link.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 30px; padding-left: 10px; vertical-align: top;">
                                                <asp:Label ID="lbOpportunity" runat="server" Style="line-height: 20px; vertical-align: middle;"></asp:Label>
                                                <asp:ImageButton ID="imgNavigateToOpp" runat="server" AlternateText="Navigate to Opportunity"
                                                    OnClientClick="RedirectToOpportunity(); return false;" Visible="false" ToolTip="Navigate to Opportunity"
                                                    ImageUrl="~/Images/arrow_16x16.png" />
                                                <asp:ImageButton ID="imgUnlink" runat="server" AlternateText="Unlink Opportunity"
                                                    OnClientClick="if(!ConfirmUnlink()) return false;" OnClick="imgUnlink_Click"
                                                    ToolTip="Unlink Opportunity" Visible="false" ImageUrl="~/Images/close_16.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 30px; font-size: 15px; font-style: italic; vertical-align: bottom;">
                                                <u>Time Entry</u>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 20px; padding-left: 10px; vertical-align: bottom; padding-bottom: 3px;">
                                                Notes for this project are
                                                <asp:DropDownList ID="ddlNotes" runat="server" Width="200">
                                                    <asp:ListItem Selected="True" Text="Required" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Optional" Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 2%">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Table ID="tblProjectDetailTabViewSwitch" runat="server" CssClass="CustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellMilestones" runat="server" CssClass="SelectedSwitch">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnMilestones" runat="server" Text="Milestones" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="CellAttachments" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnAttachments" runat="server" Text="Attachments" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellFinancials" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnFinancials" runat="server" Text="Financial Summary" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellCommissions" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnCommissions" runat="server" Text="Commissions" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellExpenses" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnExpenses" runat="server" Text="Expenses" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <%--<asp:TableCell ID="cellPersons" runat="server">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnPersons" runat="server" Text="Persons" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>--%>
                                <asp:TableCell ID="TableCellHistoryg" runat="server" Visible="false">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnHstory" runat="server" Text="History" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellProjectTools" runat="server" Visible="false">
                                    <span class="bg"><span>
                                        <asp:LinkButton ID="btnProjectTools" runat="server" Text="Tools" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <asp:MultiView ID="mvProjectDetailTab" runat="server" ActiveViewIndex="0">
                            <asp:View ID="vwMilestones" runat="server">
                                <asp:Panel ID="pnlRevenueMilestones" runat="server" CssClass="tab-pane">
                                    <div style="padding-bottom: 35px;">
                                        <asp:ShadowedTextButton ID="btnAddMilistone" runat="server" CausesValidation="false"
                                            OnClick="btnAddMilistone_Click" CssClass="add-btn" OnClientClick="if (!confirmSaveDirty()) return false;"
                                            Text="Add Milestone" />
                                    </div>
                                    <uc:ProjectMilestonesFinancials ID="milestones" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vmAttachments" runat="server">
                                <asp:Panel ID="pnlAttachments" runat="server" CssClass="tab-pane">
                                    <div style="padding-bottom: 35px;">
                                        <asp:ShadowedTextButton ID="stbAttachSOW" runat="server" CausesValidation="false"
                                            CssClass="add-btn" OnClientClick="return false;" Text="Add Attachment" />
                                    </div>
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAttachSOW" runat="server" TargetControlID="stbAttachSOW"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlAttachSOW" DropShadow="false" />
                                    <asp:Repeater ID="repProjectAttachments" runat="server">
                                        <HeaderTemplate>
                                            <table class="CompPerfTable tablesorter" width="100%" align="center" id="tblProjectAttachments">
                                                <thead>
                                                    <tr class="CompPerfHeader">
                                                        <th style="width: 43%;">
                                                            <div class="ie-bg NoBorder">
                                                                Attachment Name
                                                            </div>
                                                        </th>
                                                        <th style="width: 13%;">
                                                            <div class="ie-bg NoBorder">
                                                                Category
                                                            </div>
                                                        </th>
                                                        <th style="width: 11%;">
                                                            <div class="ie-bg NoBorder">
                                                                Size
                                                            </div>
                                                        </th>
                                                        <th style="width: 13%;">
                                                            <div class="ie-bg NoBorder">
                                                                Uploaded Date
                                                            </div>
                                                        </th>
                                                        <th style="width: 15%;">
                                                            <div class="ie-bg NoBorder">
                                                                Uploader
                                                            </div>
                                                        </th>
                                                        <th style="width: 5%;">
                                                            <div class="ie-bg NoBorder">
                                                                &nbsp;
                                                            </div>
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="textLeft padLeft20">
                                                    <% if (Project != null && Project.Id.HasValue)
                                                       { %>
                                                    <asp:HyperLink ID="hlnkProjectAttachment1" CssClass="ProjectAttachmentNameWrap" runat="server"
                                                        Text='<%# GetWrappedText( (string)Eval("AttachmentFileName")) %>' NavigateUrl='<%# GetNavigateUrl((string)Eval("AttachmentFileName"), (int)Eval("AttachmentId")) %>'></asp:HyperLink>
                                                    <% }
                                                       else
                                                       { %>
                                                    <asp:LinkButton ID="lnkProjectAttachment1" runat="server" CssClass="ProjectAttachmentNameWrap"
                                                        Visible="<%# IsProjectCreated() %>" CommandName='<%# Eval("AttachmentId") %>'
                                                        Text='<%# GetWrappedText((string)Eval("AttachmentFileName")) %>' OnClick="lnkProjectAttachment_OnClick" />
                                                    <% } %>
                                                </td>
                                                <td class="textCenter">
                                                    <asp:Label ID="lblAttachmentCategory" runat="server" Text='<%# Eval("Category")%>'></asp:Label>
                                                </td>
                                                <td class="textCenter" sorttable_customkey='<%# ((int)Eval("AttachmentSize")/1024) %>'>
                                                    <asp:Label ID="lblAttachmentSize" runat="server" Text='<%# string.Format("{0}Kb", (int)Eval("AttachmentSize")/1024)  %>'></asp:Label>
                                                </td>
                                                <td class="textCenter">
                                                    <asp:Label ID="lblUploadedDate" runat="server" Text='<%# ((DateTime?)Eval("UploadedDate")).HasValue ? string.Format("{0}", ((DateTime?)Eval("UploadedDate")).Value.ToString("yyyy/MM/dd")) : string.Empty %>'></asp:Label>
                                                </td>
                                                <td class="textCenter">
                                                    <asp:Label ID="lblUploader" runat="server" Text='<%# Eval("Uploader") %>'></asp:Label>
                                                </td>
                                                <td class="textCenter">
                                                    <asp:ImageButton ID="imgbtnDeleteAttachment1" OnClick="imgbtnDeleteAttachment_Click"
                                                        AttachmentId='<%# Eval("AttachmentId") %>' OnClientClick="if(confirm('Do you really want to delete the project attachment?')){ return true;}return false;"
                                                        Visible="true" runat="server" ImageUrl="~/Images/trash-icon-Large.png" ToolTip="Delete Attachment" />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody> </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div id="divEmptyMessage" style="display: none; background-color: White;" runat="server">
                                        No attachments have been uploaded for this project.
                                    </div>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwFinancials" runat="server">
                                <uc:ProjectFinancials ID="financials" runat="server" />
                            </asp:View>
                            <asp:View ID="vwCommissions" runat="server">
                                <asp:Panel ID="pnlCommissions" runat="server" CssClass="tab-pane">
                                    <table>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="chbReceivesSalesCommission" runat="server" Text="Receives sales commission of"
                                                    AutoPostBack="True" OnCheckedChanged="chbReceivesSalesCommission_CheckedChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSalesCommission" runat="server" Width="40px" onchange="setDirty();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="reqSalesCommission" runat="server" ControlToValidate="txtSalesCommission"
                                                    ErrorMessage="The Sales Commission is required." ToolTip="The Sales Commission is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cstSalesCommission" runat="server" ControlToValidate="txtSalesCommission"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Project" OnServerValidate="cstSalesCommission_ServerValidate"></asp:CustomValidator>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap" rowspan="2">
                                                <asp:CheckBox ID="chbReceiveManagementCommission" runat="server" Text="receives practice mgmt commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbReceiveManagementCommission_CheckedChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtManagementCommission" runat="server" Width="40px" onchange="setDirty();"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="reqManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="The Management Commission is required." ToolTip="The Management Commission is required."
                                                    ValidationGroup="Project" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cstManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                    ValidationGroup="Project" OnServerValidate="cstManagementCommission_ServerValidate"></asp:CustomValidator>
                                            </td>
                                            <td rowspan="2">
                                                based&nbsp;on&nbsp;
                                            </td>
                                            <td rowspan="2">
                                                <asp:RadioButtonList ID="rlstManagementCommission" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="rlstManagementCommission_SelectedIndexChanged">
                                                    <asp:ListItem Text="Own margin" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Sub-ordinate person margin" Value="2" Selected="True"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwExpenses" runat="server">
                                <div class="tab-pane">
                                    <uc2:ProjectExpenses runat="server" ID="projectExpenses" />
                                </div>
                            </asp:View>
                            <%--<asp:View ID="vwPersons" runat="server">
                                <uc:ProjectPersons ID="persons" runat="server" />
                            </asp:View>--%>
                            <asp:View ID="vwHistory" runat="server">
                                <asp:Panel ID="plnTabHistory" runat="server" CssClass="tab-pane">
                                    <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Project"
                                        ShowProjectDropDown="false" LabelTextBeforeDropDown="Show Project changes over "
                                        DateFilterValue="Year" ShowDisplayDropDown="false" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwProjectTools" runat="server">
                                <asp:Panel ID="pnlTools" runat="server" CssClass="tab-pane">
                                    <table style="background-color: #F9FAFF">
                                        <tr>
                                            <td>
                                                <ul style="list-style: none;">
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneMilestones" runat="server" Checked="true" Text="Clone milestones and milestone person details" /></li>
                                                    <li>
                                                        <asp:CheckBox ID="chbCloneCommissions" runat="server" Checked="true" Text="Clone commissions" /></li>
                                                    <li>Clone status
                                                        <asp:DropDownList ID="ddlCloneProjectStatus" runat="server" DataSourceID="odsProjectStatus"
                                                            DataTextField="Name" DataValueField="Id" />
                                                    </li>
                                                </ul>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="lnkClone" runat="server" Text="Clone *" ToolTip="Clone *" OnClick="lnkClone_OnClick" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span style="color: Gray">* You will be redirected to the cloned project after you click
                                                    the button.</span>
                                                <extDisable:ElementDisablerExtender ID="edeCloneButton" runat="server" TargetControlID="lnkClone"
                                                    ControlToDisableID="lnkClone" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:ObjectDataSource ID="odsProjectStatus" runat="server" SelectMethod="GetProjectStatuses"
                                        TypeName="PraticeManagement.ProjectStatusService.ProjectStatusServiceClient">
                                    </asp:ObjectDataSource>
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td style="padding-bottom: 6px; width: 100%" colspan="3">
                        <br style="height: 1px;" />
                        <ajax:TabContainer ID="tcProjectDetails" runat="server" CssClass="CustomTabStyle"
                            ActiveTabIndex="0">
                            <ajax:TabPanel ID="tpDescription" runat="server">
                                <HeaderTemplate>
                                    <span class="bg"><a href="#"><span>Project Time Types</span></a></span>
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div style="padding-bottom: 6px;">
                                        <uc:ProjectTimeTypes ID="ucProjectTimeTypes" runat="server" />
                                        <asp:Button ID="btnDeleteWorkType" OnClick="btnDeleteWorkType_OnClick" runat="server"
                                            Style='display: none;' Text="" /><asp:HiddenField ID="hdnWorkTypeId" runat="server" />
                                        <asp:CustomValidator ID="cvWorkTypesAssigned" runat="server" EnableClientScript="false"
                                            ErrorMessage="" ValidateEmptyText="true" Text=""></asp:CustomValidator>
                                    </div>
                                </ContentTemplate>
                            </ajax:TabPanel>
                        </ajax:TabContainer>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <asp:HiddenField ID="hdnProjectId" runat="server" />
                        <asp:HiddenField ID="hdnProjectDelete" runat="server" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete Project" OnClick="btnDelete_Click" ToolTip="Delete Project"
                            OnClientClick="ConfirmToDeleteProject();" Enabled="false" Visible="false" />&nbsp;
                        <asp:Button ID="btnSave" runat="server" Text="Save" ToolTip="Save" OnClick="btnSave_Click" ValidationGroup="Project" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <asp:HiddenField ID="hdnLinkPopup" runat="server" Value="" />
            <asp:HiddenField ID="hdnCanShowPopup" runat="server" />
            <asp:Panel ID="pnlProjectName" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none" BorderWidth="2px" Width="480px">
                <table class="WholeWidth">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray; white-space: nowrap;
                            font-weight: lighter; font-size: 14px; padding: 6px;" colspan="3" valign="middle">
                            Edit Project Name
                        </th>
                    </tr>
                    <tr>
                        <td colspan="3">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 20%; padding-bottom: 5px; padding-right: 5px;">
                            Project Name
                        </td>
                        <td style="padding-bottom: 5px; width: 70%;">
                            <asp:TextBox ID="txtProjectName" runat="server" Style="width: 95%"></asp:TextBox>
                        </td>
                        <td style="padding-bottom: 5px; width: 10%;">
                            <asp:RequiredFieldValidator ID="reqProjectName" runat="server" ControlToValidate="txtProjectName"
                                ErrorMessage="The Project Name is required." ToolTip="The Project Name is required."
                                ValidationGroup="ProjectName" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="text-align: center; padding-bottom: 5px;">
                            <asp:Button ID="btnUpdateProjectName" runat="server" Text="Update" ToolTip="Update" OnClick="btnUpdateProjectName_OnClick"
                                onchange="setDirty();" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btncloseEditProjectName" runat="server" ToolTip="Cancel" Text="Cancel">
                            </asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="padding-left: 5px; padding-bottom: 5px;">
                            <asp:ValidationSummary ID="VsumProjectName" runat="server" EnableClientScript="false"
                                ValidationGroup="ProjectName" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlAttachSOW" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none" BorderWidth="2px" Width="465px">
                <table class="WholeWidth" style="padding: 5px;">
                    <tr style="background-color: Gray; height: 27px;">
                        <td align="center" style="white-space: nowrap; font-size: 14px;" colspan="2">
                            Add Attachment
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px;" colspan="2">
                            <asp:FileUpload ID="fuProjectAttachment" onchange="EnableUploadButton();" BackColor="White"
                                runat="server" Width="435px" Size="68" />
                            <asp:CustomValidator ID="cvAttachment" runat="server" ControlToValidate="fuProjectAttachment"
                                EnableClientScript="true" ClientValidationFunction="cvAttachment_ClientValidationFunction"
                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvAttachment_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*" ToolTip="File is Required." ErrorMessage="File is Required."></asp:CustomValidator>
                            <asp:CustomValidator ID="cvProjectAttachment" runat="server" ControlToValidate="fuProjectAttachment"
                                EnableClientScript="true" ClientValidationFunction="cvProjectAttachment_ClientValidationFunction"
                                SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvProjectAttachment_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*" ToolTip="File Format must be PDF/DOC/DOCX."
                                ErrorMessage="File Format must be PDF/DOC/DOCX."></asp:CustomValidator>
                            <asp:CustomValidator ID="cvalidatorProjectAttachment" runat="server" ControlToValidate="fuProjectAttachment"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvalidatorProjectAttachment_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px;" colspan="2">
                            <asp:Label ID="lblAttachmentMessage" ForeColor="Gray" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="white-space: nowrap; padding-left: 10px; padding-right: 20px; padding-top: 10px;">
                            <asp:DropDownList ID="ddlAttachmentCategory" runat="server" onchange="EnableUploadButton();">
                            </asp:DropDownList>
                            <asp:CustomValidator ID="cvAttachmentCategory" runat="server" ControlToValidate="ddlAttachmentCategory"
                                EnableClientScript="true" SetFocusOnError="true" Display="Dynamic" OnServerValidate="cvAttachmentCategory_OnServerValidate"
                                ValidationGroup="ProjectAttachment" Text="*" ToolTip="Category is required."
                                ClientValidationFunction="cvAttachmentCategory_ClientValidationFunction" ErrorMessage="Category is required."></asp:CustomValidator>
                        </td>
                        <td align="right" style="white-space: nowrap; padding-left: 10px; padding-right: 20px;
                            padding-top: 10px;">
                            <asp:Button ID="btnUpload" Enabled="false" ValidationGroup="ProjectAttachment" runat="server"
                                Text="Upload" ToolTip="Upload" OnClick="btnUpload_Click" />
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" OnClick="btnCancel_OnClick" runat="server" Text="Cancel" ToolTip="Cancel"  />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                OkControlID="btnOKErrorPanel" CancelControlID="btnOKErrorPanel" DropShadow="false" />
            <asp:Panel ID="pnlErrorPanel" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none; max-height: 400px; max-width: 550px; min-height: 100px;
                min-width: 400px" BorderWidth="2px">
                <table width="100%">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                            valign="bottom">
                            <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                        </th>
                    </tr>
                    <tr>
                        <td style="padding: 10px;">
                            <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                            <asp:ValidationSummary ID="vsumProject" runat="server" EnableClientScript="false"
                                ValidationGroup="Project" />
                            <asp:ValidationSummary ID="vsumProjectAttachment" runat="server" EnableClientScript="false"
                                ValidationGroup="ProjectAttachment" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px; text-align: center;">
                            <asp:Button ID="btnOKErrorPanel" runat="server" ToolTip="OK" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeLinkOpportunityPopup" runat="server"
                TargetControlID="hdnLinkPopup" CancelControlID="btnLinkOpportunityCancel" BehaviorID="mpeLinkOpportunityPopup"
                BackgroundCssClass="modalBackground" PopupControlID="pnlLinkOpportunity" DropShadow="false"
                OkControlID="btnLinkOpportunityCancel" />
            <asp:Panel ID="pnlLinkOpportunity" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none;" BorderWidth="2px" Width="465px">
                <table class="WholeWidth">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray; white-space: nowrap;
                            font-weight: lighter; font-size: 14px; padding: 6px;" colspan="2" valign="middle">
                            Link This Project to Existing Opportunity
                        </th>
                    </tr>
                    <tr>
                        <td style="padding: 10px;" colspan="2">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="padding-bottom: 15px;">
                                        Select an Opportunity and Click Link Button to Link
                                        <asp:Label ID="lblProjectNameLinkPopUp" runat="server" Font-Bold="true"></asp:Label>
                                        Project.
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlOpportunities" runat="server" Width="400">
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="cvOpportunityRequired" runat="server" OnServerValidate="cvOpportunityRequired_Validate"
                                            ValidationGroup="LinkOpportunity" Display="Dynamic" SetFocusOnError="true" Text="*"
                                            ErrorMessage="Opportunity is required." ToolTip="Opportunity is required."></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 5px; padding-bottom: 5px;">
                                        <asp:ValidationSummary ID="valSumLinkOpportunity" runat="server" ValidationGroup="LinkOpportunity" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center;">
                                        <asp:Button ID="btnLinkOpportunity" runat="server" Text="Link" ToolTip="Link" OnClick="btnLinkOpportunity_Click" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btnLinkOpportunityCancel" runat="server" Text="Cancel" ToolTip="Cancel"  />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeTimeEntriesRelatedToitPopup" runat="server"
                TargetControlID="hdnCanShowPopup" CancelControlID="btnClose" BehaviorID="mpeTEsRelatedToItPopup"
                BackgroundCssClass="modalBackground" PopupControlID="pnlPopup" DropShadow="false"
                OkControlID="btnOk" />
            <asp:Panel ID="pnlPopup" runat="server" BackColor="White" BorderColor="Black" CssClass="ConfirmBoxClassError"
                Style="display: none" BorderWidth="2px">
                <table class="WholeWidth">
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
                                            You cannot delete this Work type.Because, there are some time entries related to
                                            it.
                                        </p>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: center;">
                                        <asp:Button ID="btnOk" runat="server" Text="OK" ToolTip="OK" OnClientClick="return false;" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
            <asp:PostBackTrigger ControlID="btnUpload" />
            <asp:PostBackTrigger ControlID="btnCancel" />
            <asp:PostBackTrigger ControlID="repProjectAttachments" />
            <asp:PostBackTrigger ControlID="btnOKErrorPanel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

