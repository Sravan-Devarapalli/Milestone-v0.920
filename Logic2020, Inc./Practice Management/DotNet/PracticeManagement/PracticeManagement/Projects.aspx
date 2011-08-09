<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="PraticeManagement.Projects"
    Title="Practice Management - Projects" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="Controls/PracticeFilter.ascx" TagName="PracticeFilter" TagPrefix="uc1" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Projects</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Projected Project Profit & Loss
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="Scripts/ScrollinDropDown.js"></script>
    <script type="text/javascript">
        var IsExportAllDisplayed = false;

        function imgArrow_click() {
            IsExportAllDisplayed = true;
            var menu = document.getElementById('popupmenu');
            menu.style.display = 'block';
        }
        function imgArrow_mouseOver() {
            IsExportAllDisplayed = true;
        }

        function imgArrow_mouseOut() {
            IsExportAllDisplayed = false;
            setTimeout(function () {
                if (!IsExportAllDisplayed) {
                    var menu = document.getElementById('popupmenu');
                    menu.style.display = 'none';
                }

            }, 500);
        }

        function Exportall_click_mouseOver() {
            IsExportAllDisplayed = true;
            var menu = document.getElementById('popupmenu');
            menu.style.display = 'block';
        }


        function excludeDualSelection(target) {
            var targetElement = $get(target);

            if (targetElement)
                if (targetElement.checked)
                    targetElement.checked = false;
        }
        function RemoveExtraCharAtEnd(url) {
            if (url.lastIndexOf('#') == url.length - 1) {
                return url.substring(0, url.length - 1);
            }
            else {
                return url;
            }
        }
        function correctMonthMiniReportPosition(reportPanelId, headerId, scrollPanelId) {
            var reportPanel = $get(reportPanelId);
            var header = $get(headerId);
            var scrollPanel = $get(scrollPanelId);

            var reportPanelPos = getPosition(reportPanel);
            var headerPosition = getPosition(header);

            reportPanel.style.left = headerPosition.left - scrollPanel.scrollLeft - 25;
        }

        function getPosition(obj) {
            var topValue = 0, leftValue = 0;
            while (obj) {
                leftValue += obj.offsetLeft;
                topValue += obj.offsetTop;
                obj = obj.offsetParent;
            }

            function point() {
                this.left = 0;
                this.top = 0;
            }

            var result = new point();
            result.left = leftValue;
            result.top = topValue;

            return result;
        }

        function EnableOrDisableGroup() {
            var cbl = document.getElementById("<%= cblClient.ClientID %>");
            var arrayOfCheckBoxes = cbl.getElementsByTagName("input");
            var cblList = $("div[id^='sdeCblProjectGroup']");
            var anySingleItemChecked = "false";
            for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
                if (arrayOfCheckBoxes[i].checked) {
                    anySingleItemChecked = "true";
                }
            }

            if (anySingleItemChecked == "true") {
                cblList[0].disabled = "";
            }
            else {
                cblList[0].disabled = "disabled";
            }
            custom_ScrollingDropdown_onclick('cblProjectGroup', 'Group');
        }

        function resetFiltersTab() {
            GetDefaultcblList();
            GetDefault(document.getElementById("<%= cblClient.ClientID %>"));
            GetDefault(document.getElementById("<%= cblSalesperson.ClientID %>"));
            GetDefault(document.getElementById("<%= cblPractice.ClientID %>"));
            GetDefault(document.getElementById("<%= cblProjectOwner.ClientID %>"));
            custom_ScrollingDropdown_onclick('cblProjectGroup', 'Group');
        }

        function GetDefaultcblList() {
            var div = document.getElementById("<%= divProjectFilter.ClientID %>");
            var arrayOfCheckBoxes = div.getElementsByTagName('input');
            for (var i = 0; i < arrayOfCheckBoxes.length; i++) {
                arrayOfCheckBoxes[i].checked = true;
            }
        }

        function GetDefault(control) {
            control.fireEvent('onclick');
        }

        function resetCalculationsTab() {
            document.getElementById("<%= chbActive.ClientID %>").checked = true;
            document.getElementById("<%= chbInternal.ClientID %>").checked = true;
            document.getElementById("<%= chbProjected.ClientID %>").checked = true;
            document.getElementById("<%= chbCompleted.ClientID %>").checked = true;
            document.getElementById("<%= chbExperimental.ClientID %>").checked = false;
            document.getElementById("<%= chbInactive.ClientID %>").checked = false;

            document.getElementById("<%= ddlCalculateRange.ClientID %>").selectedIndex = 0;
        }

        function custom_ScrollingDropdown_onclick(control, type) {
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
                        if (arrayOfCheckBoxes[i].parentNode.parentNode.style.display != "none") {
                            temp++;
                            text = arrayOfCheckBoxes[i].parentNode.childNodes[1].innerHTML;
                        }
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

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            addListenersToParent('<%= cblProjectGroup.ClientID %>', '<%= cblClient.ClientID %>');
        }

        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            txtStartDate.value = hdnStartDate.value;
            txtEndDate.value = hdnEndDate.value;
        }
        function CheckIfDatesValid() {
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            var startDate = new Date(txtStartDate.value);
            var EtartDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= EtartDate) {
                var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
                btnCustDatesClose.click();
            }
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            if (ddlPeriod.value == '0') {
                imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
                lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
                if (imgCalender.fireEvent) {
                    imgCalender.attributes["class"].value = "";
                    lblCustomDateRange.attributes["class"].value = "";
                    imgCalender.click();
                }
                if (document.createEvent) {
                    imgCalender.attributes["class"].value = "";
                    lblCustomDateRange.attributes["class"].value = "";
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('click', true, true);
                    imgCalender.dispatchEvent(event);
                }
            }
            else {
                var btnddlPeriodChanged = document.getElementById('<%= btnddlPeriodChanged.ClientID %>');
                btnddlPeriodChanged.click();
            }
        }

        function ChangeDefaultFocus(e) {
            if (window.event) {
                e = window.event;
            }
            if (e.keyCode == 13) {
                var btn = document.getElementById('<%= btnSearchAll.ClientID %>');
                btn.click();
            }               
            
        }
    </script>
    <style>
        ul, li
        {
            margin: 0;
            padding: 0;
        }
        ul.pmenu
        {
            position: absolute;
            margin: 0;
            padding: 1px;
            list-style: none;
            width: 88px; /* Width of Menu Items */
            border: 1px solid #ccc;
            background: white;
            display: none;
            z-index: 10;
        }
        ul.pmenu li
        {
            position: relative;
        }
        ul.pmenu li ul
        {
            position: absolute;
            left: 100px; /* Set 1px less than menu width */
            top: 0;
            display: none;
            z-index: 10;
        }
        /* Styles for Menu Items */ul.pmenu li a
        {
            display: block;
            text-decoration: none;
            color: black;
            padding: 2px 5px 2px 20px;
        }
        ul.pmenu li a:hover
        {
            background: #335EA8;
            color: Black;
            font-weight: bold;
        }
        
        /* IE \*/* html ul.pmenu li
        {
            float: left;
            height: 1%;
        }
        * html ul.pmenu li a
        {
            height: 1%;
        }
        * html ul.pmenu li ul
        {
            left: 100px;
        }
        /* End */ul.pmenu li:hover ul, ul.pmenu li.over ul
        {
            display: block;
        }
        /* The magic */ul.pmenu li ul
        {
            left: 100px;
        }
        
        .xScrollProjects
        {
            overflow-x: auto;
        }
        .displayNone
        {
            display: none;
        }
    </style>
    <asp:UpdatePanel ID="flrPanel" runat="server">
        <ContentTemplate>
            <div class="filters">
                <div class="buttons-block">
                    <table style="border: none; padding-left: 10px;" class="WholeWidth">
                        <tr>
                            <td style="width: 3%">
                                <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg"
                                    CollapseControlID="btnExpandCollapseFilter" ExpandControlID="btnExpandCollapseFilter"
                                    Collapsed="True" TextLabelID="lblFilter" />
                                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                    ToolTip="Search, Filter and Calculation Options" />
                            </td>
                            <td style="width: 8%;">
                                &nbsp;Show&nbsp;Projects&nbsp;for&nbsp;
                            </td>
                            <td style="width: 8%;">
                                <asp:DropDownList ID="ddlPeriod" runat="server" Onchange=" return CheckAndShowCustomDatesPoup(this);">
                                    <asp:ListItem Text="Next 3 months" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="Next 6 months" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="Next 12 months" Value="12"></asp:ListItem>
                                    <asp:ListItem Text="Last 3 months" Value="-3"></asp:ListItem>
                                    <asp:ListItem Text="Last 6 months" Value="-6"></asp:ListItem>
                                    <asp:ListItem Text="Last 12 months" Value="-12"></asp:ListItem>
                                    <asp:ListItem Text="Previous FY" Value="-13"></asp:ListItem>
                                    <asp:ListItem Text="Current FY" Value="13"></asp:ListItem>
                                    <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="btnddlPeriodChanged" runat="server" OnClick="ddlPeriod_SelectedIndexChanged"
                                    Style="display: none" />
                                <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                    CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                                    PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                                    OnCancelScript="ReAssignStartDateEndDates();" />
                                <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
                            </td>
                            <td style="width: 48%; padding-left: 5px;" align="left">
                                <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                                <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                            </td>
                            <td style="width: 22%;" align="right">
                                <asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                                    <asp:ListItem Text="View 10" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="View 25" Value="25"></asp:ListItem>
                                    <asp:ListItem Text="View 50" Value="50"></asp:ListItem>
                                    <asp:ListItem Text="View ALL" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                                <asp:ShadowedHyperlink runat="server" Text="Add Project" ID="lnkAddProject" CssClass="add-btn"
                                    NavigateUrl="~/ProjectDetail.aspx?returnTo=Projects.aspx" />
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:Panel ID="pnlFilters" runat="server">
                    <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                        CssClass="CustomTabStyle">
                        <ajaxToolkit:TabPanel ID="tpSearch" runat="server">
                            <HeaderTemplate>
                                <span class="bg"><a href="#"><span>Search</span></a> </span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:Panel ID="pnlSearch" runat="server" Style="height: 80px;" CssClass="project-filter"
                                    DefaultButton="btnSearch">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td style="padding-right: 8px; padding-left: 4px;">
                                                <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" onkeypress="ChangeDefaultFocus(event);"  EnableViewState="False" />
                                            </td>
                                            <td style="width: 10px;">
                                                <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                                    ErrorMessage="Please type text to be searched." ToolTip="Please type text to be searched."
                                                    Text="*" SetFocusOnError="true" ValidationGroup="Search" CssClass="searchError"
                                                    Display="Static" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search View" ValidationGroup="Search"
                                                    OnClick="btnSearch_Clicked" Width="100px" EnableViewState="False" />
                                                &nbsp;
                                                <asp:Button ID="btnSearchAll" runat="server" Text="Search All" ValidationGroup="Search"
                                                    PostBackUrl="~/ProjectSearch.aspx" Width="100px" EnableViewState="False" />
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:ValidationSummary ID="valsSearch" runat="server" ValidationGroup="Search" EnableClientScript="true"
                                                    ShowMessageBox="false" CssClass="searchError" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="tpAdvancedFilters" runat="server">
                            <HeaderTemplate>
                                <span class="bg"><a href="#"><span>Filters</span></a> </span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div id="divProjectFilter" runat="server" class="project-filter">
                                    <table class="WholeWidth" style="height: 80px;">
                                        <tr class="tb-header">
                                            <td style="border-bottom: 1px solid black; width: 190px; text-align: center">
                                                Client / Group
                                            </td>
                                            <td style="padding: 5px; width: 10px;">
                                            </td>
                                            <td style="border-bottom: 1px solid black; width: 190px; text-align: center;">
                                                Sales Team
                                            </td>
                                            <td style="padding: 5px; width: 10px;">
                                            </td>
                                            <td style="border-bottom: 1px solid black; width: 190px; text-align: center;">
                                                Practice Area
                                            </td>
                                            <td rowspan="3" style="text-align: right;">
                                                <table style="text-align: right; width: 100%;">
                                                    <tr>
                                                        <td style="padding-bottom: 5px;">
                                                            <asp:Button ID="btnUpdateFilters" runat="server" Text="Update" Width="100px" OnClick="btnUpdateView_Click"
                                                                ValidationGroup="Filter" EnableViewState="False" CssClass="pm-button" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px;">
                                                            <asp:Button ID="btnResetFilters" runat="server" Text="Reset" Width="100px" CausesValidation="False"
                                                                OnClientClick="resetFiltersTab(); return false;" EnableViewState="False" CssClass="pm-button" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 5px;">
                                                <uc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup"
                                                    SetDirty="false" Width="240" Height="240px" onclick="scrollingDropdown_onclick('cblClient','Client');EnableOrDisableGroup();"
                                                    DropDownListType="Client" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <uc:ScrollingDropDown ID="cblSalesperson" runat="server" SetDirty="false" Width="240"
                                                    Height="240px" onclick="scrollingDropdown_onclick('cblSalesperson','Salesperson')"
                                                    DropDownListType="Salesperson" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblSalesperson" runat="server" TargetControlID="cblSalesperson"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <uc:ScrollingDropDown ID="cblPractice" runat="server" SetDirty="false" Width="240"
                                                    Height="240px" onclick="scrollingDropdown_onclick('cblPractice','Practice Area')"
                                                    DropDownListType="Practice Area" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblPractice" runat="server" TargetControlID="cblPractice"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 5px;">
                                                <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" Width="240"
                                                    Height="240px" onclick="custom_ScrollingDropdown_onclick('cblProjectGroup','Group')"
                                                    DropDownListType="Group" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <uc:ScrollingDropDown ID="cblProjectOwner" runat="server" SetDirty="false" Width="240"
                                                    Height="240px" onclick="scrollingDropdown_onclick('cblProjectOwner','Owner')"
                                                    DropDownListType="Owner" CellPadding="3" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblProjectOwner" runat="server" TargetControlID="cblProjectOwner"
                                                    UseAdvanceFeature="true" EditImageUrl="Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                            <HeaderTemplate>
                                <span class="bg"><a href="#"><span>Calculations</span></a> </span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <div class="project-filter">
                                    <table class="WholeWidth" cellpadding="5" style="height: 80px;">
                                        <tr class="tb-header">
                                            <td colspan="2" style="border-bottom: 1px solid black; text-align: center; width: 200px">
                                                Project Types Included
                                            </td>
                                            <td style="width: 20px;" rowspan="4">
                                            </td>
                                            <td style="border-bottom: 1px solid black; text-align: center; width: 180px">
                                                Calculated Grand Total
                                            </td>
                                            <td rowspan="4" style="text-align: right;">
                                                <table style="text-align: right; width: 100%;">
                                                    <tr>
                                                        <td style="padding-bottom: 5px;">
                                                            <asp:Button ID="btnUpdateCalculations" runat="server" Text="Update" Width="100px"
                                                                OnClick="btnUpdateView_Click" ValidationGroup="Filter" EnableViewState="False"
                                                                CssClass="pm-button" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px;">
                                                            <asp:Button ID="btnResetCalculations" runat="server" Text="Reset" Width="100px" CausesValidation="False"
                                                                OnClientClick="resetCalculationsTab(); return false;" EnableViewState="False"
                                                                CssClass="pm-button" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; width: 100px; padding-top: 1px;">
                                                <asp:CheckBox ID="chbActive" runat="server" Text="Active" Checked="True" EnableViewState="False" />
                                            </td>
                                            <td style="padding-left: 10px; width: 100px; padding-top: 1px;">
                                                <asp:CheckBox ID="chbInternal" runat="server" Text="Internal" Checked="True" EnableViewState="False" />
                                            </td>
                                            <td rowspan="2" style="text-align: center; padding-top: 5px;" valign="top">
                                                <asp:DropDownList ID="ddlCalculateRange" runat="server" Style="font-family: Arial !Important;
                                                    color: #333 !Important; font-size: 12px !Important;" AutoPostBack="false" Width="170px">
                                                    <asp:ListItem Text="Project Value in Range" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Total Project Value" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Current Fiscal Year" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px;">
                                                <asp:CheckBox ID="chbProjected" runat="server" Text="Projected" Checked="True" EnableViewState="False" />
                                            </td>
                                            <td style="padding-left: 10px;">
                                                <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive" EnableViewState="False" />
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px;">
                                                <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed" Checked="True" EnableViewState="False" />
                                            </td>
                                            <td style="padding-left: 10px;">
                                                <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental" EnableViewState="False" />
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </AjaxControlToolkit:TabContainer>
                </asp:Panel>
                <asp:ValidationSummary ID="valsPerformance" runat="server" Width="100%" ValidationGroup="Filter"
                    CssClass="searchError" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
            <asp:PostBackTrigger ControlID="btnExportAllToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <cc1:StyledUpdatePanel ID="StyledUpdatePanel" runat="server" CssClass="container"
        UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel class="this value set OnPageLoad" runat="server" ID="horisontalScrollDiv"
                CssClass="xScrollProjects">
                <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id" OnItemDataBound="lvProjects_ItemDataBound"
                    OnDataBinding="lvProjects_DataBinding" OnSorted="lvProjects_Sorted" OnDataBound="lvProjects_OnDataBound"
                    OnSorting="lvProjects_Sorting" OnPagePropertiesChanging="lvProjects_PagePropertiesChanging">
                    <LayoutTemplate>
                        <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                            <tr runat="server" id="lvHeader" class="CompPerfHeader">
                                <td class="CompPerfProjectState">
                                    <div class="ie-bg">
                                    </div>
                                </td>
                                <td class="CompPerfProjectNumber">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProject" CommandArgument="1" CommandName="Sort" runat="server"
                                            CssClass="arrow">Project #</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfClient">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortClient" CommandArgument="2" CommandName="Sort" runat="server"
                                            CssClass="arrow">Client</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfProject">
                                    <div class="ie-bg">
                                        <asp:LinkButton ID="btnSortProjectName" CommandArgument="3" CommandName="Sort" runat="server"
                                            CssClass="arrow">Project</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg alignCenter">
                                        <asp:LinkButton ID="btnSortStartDate" CommandArgument="4" CommandName="Sort" runat="server"
                                            CssClass="arrow">Start Date</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfPeriod">
                                    <div class="ie-bg  alignCenter">
                                        <asp:LinkButton ID="btnSortEndDate" CommandArgument="5" CommandName="Sort" runat="server"
                                            CssClass="arrow">End Date</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="CompPerfTotalSummary">
                                    <div class="ie-bg alignCenter">
                                        Total
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="lvSummary" class="summary">
                                <td colspan="6">
                                    <div class="cell-pad">
                                        Financial Summary</div>
                                </td>
                            </tr>
                            <tbody>
                                <tr runat="server" id="itemPlaceholder" />
                                <tr>
                                    <td colspan="7">
                                        <asp:DataPager ID="dpProjects" runat="server" PagedControlID="lvProjects">
                                            <Fields>
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton ID="LinkButton1" runat="server" Text="Previous" PostBackUrl="#" CommandName="<%# PagerPrevCommand %>"
                                                            Visible="<%# IsNeedToShowPrevButton() %>"></asp:LinkButton>
                                                    </PagerTemplate>
                                                </asp:TemplatePagerField>
                                                <asp:NumericPagerField ButtonCount="10" NextPageText="..." PreviousPageText="..." />
                                                <asp:TemplatePagerField OnPagerCommand="Pager_PagerCommand">
                                                    <PagerTemplate>
                                                        <asp:LinkButton ID="LinkButton2" runat="server" Text="Next" PostBackUrl="#" CommandName="<%# PagerNextCommand %>"
                                                            Visible="<%# IsNeedToShowNextButton() %>"></asp:LinkButton>
                                                    </PagerTemplate>
                                                </asp:TemplatePagerField>
                                            </Fields>
                                        </asp:DataPager>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr runat="server" id="boundingRow" style="background-color: White">
                            <td class="CompPerfProjectState">
                            </td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject">
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblStartDate" runat="server" />
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblEndDate" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr runat="server" id="boundingRow" class="rowEven">
                            <td class="CompPerfProjectState">
                            </td>
                            <td class="CompPerfProjectNumber">
                                <asp:Label ID="lblProjectNumber" runat="server" />
                            </td>
                            <td class="CompPerfClient">
                                <asp:HyperLink ID="btnClientName" runat="server" />
                            </td>
                            <td class="CompPerfProject">
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblStartDate" runat="server" />
                            </td>
                            <td class="CompPerfPeriod">
                                <asp:Label ID="lblEndDate" runat="server" />
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <tr runat="server" id="EmptyDataRow">
                            <td>
                                There is nothing to be displayed here.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
                <uc3:LoadingProgress ID="LoadingProgress1" runat="server" DisplayText="Refreshing Data..." />
            </asp:Panel>
            <div style="padding: 5px 10px 5px 5px;">
                <table class="WholeWidth">
                    <tr>
                        <td style="padding-left: 40%;">
                            <div style="font-weight: bold; width: 100%; padding-top: 5px; color: Black;">
                                Viewing&nbsp;
                                <asp:Label ID="lblCurrentPageCount" runat="server" Text='<%# GetCurrentPageCount() %>'></asp:Label>
                                &nbsp;of&nbsp;
                                <asp:Label ID="lblTotalCount" runat="server" Text='<%# GetTotalCount() %>'></asp:Label>&nbsp;
                                Projects
                            </div>
                        </td>
                        <td align="right">
                            <table cellpadding="0px" cellspacing="0px">
                                <tr>
                                    <td style="padding: 0px;">
                                        <asp:Button ID="btnExportToExcel" runat="server" Style="margin: 0px;" OnClick="btnExportToExcel_Click"
                                            Text="Export" Width="100%" />
                                    </td>
                                    <td style="padding: 0px;">
                                        <asp:Image ID="imgExportAllToExcel" runat="server" Style="margin: 0px;" ImageUrl="~/Images/Dropdown_Arrow_22.png"
                                            onclick="imgArrow_click();" onmouseover="imgArrow_mouseOver();" onmouseout="imgArrow_mouseOut();" />
                                    </td>
                                </tr>
                                <tr onmouseover="Exportall_click_mouseOver();" onmouseout="imgArrow_mouseOut();">
                                    <td colspan="2">
                                        <ul id="popupmenu" class="pmenu">
                                            <li>
                                                <asp:LinkButton ID="btnExportAllToExcel" runat="server" OnClick="btnExportAllToExcel_Click"
                                                    Text="Export All" Style="background-color: #CCCCCC; text-align: left!important;"
                                                    CssClass="pm-button" OnClientClick="this.parentNode.parentNode.style.display='none';return true;" />
                                            </li>
                                        </ul>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
                <table class="WholeWidth">
                    <tr>
                        <td colspan="2" align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldWidth="70" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 10px 0px 10px 0px;">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid();"
                                Text="OK" OnClick="btnUpdateView_Click" />
                            <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" runat="server" OnClientClick="ReAssignStartDateEndDates();"
                                Text="Cancel" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:ValidationSummary ID="valSum" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </cc1:StyledUpdatePanel>
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <div class="version">
        Version.
        <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></div>
</asp:Content>

