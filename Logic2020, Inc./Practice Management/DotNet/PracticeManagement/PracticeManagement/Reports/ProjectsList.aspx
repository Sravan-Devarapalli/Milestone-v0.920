<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectsList.aspx.cs" Inherits="PraticeManagement.Reports.ProjectsList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="title" ContentPlaceHolderID="title" runat="server">
    <title>Projects List | Practice Management</title>
</asp:Content>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Projects Report
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#tblAllProjectsReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });

        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            addListenersToParent('<%= cblProjectGroup.ClientID %>', '<%= cblClient.ClientID %>');
            $("#tblAllProjectsReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }

        function CheckIfDatesValid() {
            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
                var btnCustDatesClose = document.getElementById('<%= btnCustDatesClose.ClientID %>');
                hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
                hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
                lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
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
            custom_ScrollingDropdown_onclick('cblProjectGroup', 'Business Unit');
        }

        function custom_ScrollingDropdown_onclick(control, type, pluralform, pluralformType, maxNoOfCharacters) {
            var temp = 0;
            var text = "";
            if (pluralform == undefined || pluralform == null || pluralform == '') {
                pluralform = "s";
            }
            if (maxNoOfCharacters == undefined || maxNoOfCharacters == null || maxNoOfCharacters == '' || isNaN(maxNoOfCharacters)) {
                maxNoOfCharacters = 33;
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
                        text = "-- Select " + type.toString() + "(" + pluralform + ") --";
                    }
                }
                var fulltext = text;
                text = DecodeString(text);
                var isLengthExceded = (text.length > maxNoOfCharacters);
                scrollingDropdownList.parentNode.children[1].children[0].firstChild.nodeValue = isLengthExceded ? text.substr(0, maxNoOfCharacters - 2) + ".." : text;
                scrollingDropdownList.parentNode.children[1].children[0].firstChild.parentNode.attributes['title'].nodeValue = isLengthExceded ? fulltext : '';
            }
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "fontBold";
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
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }
            btnCustDatesOK = document.getElementById('<%= btnCustDatesOK.ClientID %>');
            btnCustDatesOK.click();
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
        }
    </script>
    <div class="filters Margin-Bottom10Px">
        <div class="filter-section-color">
            <table class="WholeWidth">
                <tr>
                    <td align="left" class="width30P">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                            ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                            ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Expand Filters and Sort Options" />
                        &nbsp;
                        <asp:Label ID="lblProjectsText" runat="server" Text="Show Projects for"></asp:Label>
                        &nbsp;
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" onchange="EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                            <asp:ListItem Text="Next 3 months" Value="3" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Next 6 months" Value="6"></asp:ListItem>
                            <asp:ListItem Text="Next 12 months" Value="12"></asp:ListItem>
                            <asp:ListItem Text="Last 3 months" Value="-3"></asp:ListItem>
                            <asp:ListItem Text="Last 6 months" Value="-6"></asp:ListItem>
                            <asp:ListItem Text="Last 12 months" Value="-12"></asp:ListItem>
                            <asp:ListItem Text="Previous FY" Value="-13"></asp:ListItem>
                            <asp:ListItem Text="Current FY" Value="13"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        <ajaxToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                            CancelControlID="btnCustDatesCancel" OkControlID="btnCustDatesClose" BackgroundCssClass="modalBackground"
                            PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates" DropShadow="false"
                            OnCancelScript="ReAssignStartDateEndDates();" OnOkScript="return false;" />
                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnStartDateTxtBoxId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDateTxtBoxId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnStartDateCalExtenderBehaviourId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDateCalExtenderBehaviourId" runat="server" Value="" />
                        &nbsp;
                        <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    </td>
                    <td>
                    </td>
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width100Px"
                                        OnClick="btnUpdateView_OnClick" EnableViewState="False" />
                                </td>
                                <td class="LeftPadding10px">
                                    <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width100Px" UseSubmitBehavior="false" OnClick="btnResetFilter_Click"
                                        />
                                       
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp hidden">
            <table class="WholeWidth">
                <tr>
                    <td align="center" class="no-wrap">
                        <table>
                            <tr>
                                <td>
                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                        FromToDateFieldCssClass="Width70Px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="custBtns">
                        <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="return CheckIfDatesValid();" ValidationGroup="<%# ClientID %>"
                            Text="OK" CausesValidation="true" />
                        <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                            OnClientClick="return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
                <tr>
                    <td class="textCenter">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlFilters" runat="server">
            <table class="WholeWidth CustomTabStyle">
                <tr>
                    <td>
                        <asp:Table ID="tblFilterViewSwitch" runat="server" CssClass="CommonCustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellSecurity" runat="server" CssClass="SelectedSwitch">
                                    <span class="bg">
                                        <a id="btnFilters" class="Width80Px">Filters</a>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <asp:MultiView ID="mvFilter" runat="server" ActiveViewIndex="0">
                            <asp:View ID="vwFilter" runat="server">
                                <div id="divProjectFilter" class="filter-section-color" runat="server">
                                    <table class="WholeWidth Height80Px">
                                        <tr class="tb-header ProjectSummaryAdvancedFiltersHeader">
                                            <th class="Width10PerImp">
                                                Account / Business Unit
                                            </th>
                                            <td class="Width10Px">
                                            </td>
                                            <th class="Width10PerImp"">
                                                Sales Team
                                            </th>
                                            <td class="Width10Px">
                                            </td>
                                            <th class="Width10PerImp">
                                                Practice Area
                                            </th>
                                            <td class="Width10Px">
                                            </td>
                                            <th class="Width20PerImp">
                                                Project Types Included
                                            </th>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Padding5">
                                                <pmc:CascadingMsdd ID="cblClient" runat="server" TargetControlId="cblProjectGroup"
                                                    SetDirty="false" CssClass="ProjectSummaryScrollingDropDown Width245Px" onclick="scrollingDropdown_onclick('cblClient','Account');EnableOrDisableGroup();"
                                                    DropDownListType="Account" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblClient" runat="server" TargetControlID="cblClient"
                                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <pmc:ScrollingDropDown ID="cblSalesperson" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                    onclick="scrollingDropdown_onclick('cblSalesperson','Salesperson')" DropDownListType="Salesperson" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblSalesperson" runat="server" TargetControlID="cblSalesperson"
                                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <pmc:ScrollingDropDown ID="cblPractice" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                    onclick="scrollingDropdown_onclick('cblPractice','Practice Area')" DropDownListType="Practice Area" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblPractice" runat="server" TargetControlID="cblPractice"
                                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td rowspan="2">
                                                <table>
                                                    <tr class="PadLeft10Td">
                                                        <td class="ProjectSummaryCheckboxTd">
                                                            <asp:CheckBox ID="chbActive" runat="server" Text="Active" onclick="EnableResetButton();"
                                                                Checked="True" EnableViewState="False" />
                                                        </td>
                                                        <td class="ProjectSummaryCheckboxTd">
                                                            <asp:CheckBox ID="chbInternal" runat="server" Text="Internal" onclick="EnableResetButton();"
                                                                Checked="True" EnableViewState="False" />
                                                        </td>
                                                        <td class="ProjectSummaryCheckboxTd">
                                                            <asp:CheckBox ID="chbProposed" runat="server" Text="Proposed" onclick="EnableResetButton();"
                                                                Checked="True" EnableViewState="False" />
                                                        </td>
                                                    </tr>
                                                    <tr class="PadLeft10Td">
                                                        <td>
                                                            <asp:CheckBox ID="chbProjected" runat="server" Text="Projected" onclick="EnableResetButton();"
                                                                Checked="True" EnableViewState="False" />
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chbInactive" runat="server" Text="Inactive" onclick="EnableResetButton();"
                                                                EnableViewState="False" />
                                                        </td>
                                                    </tr>
                                                    <tr class="PadLeft10Td">
                                                        <td>
                                                            <asp:CheckBox ID="chbCompleted" runat="server" Text="Completed" Checked="True" onclick="EnableResetButton();"
                                                                EnableViewState="False" />
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chbExperimental" runat="server" Text="Experimental" onclick="EnableResetButton();"
                                                                EnableViewState="False" />
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                              <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Padding5">
                                                <pmc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown Width245Px"
                                                    onclick="custom_ScrollingDropdown_onclick('cblProjectGroup','Business Unit')"
                                                    DropDownListType="Business Unit" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <pmc:ScrollingDropDown ID="cblProjectOwner" runat="server" SetDirty="false" CssClass="ProjectSummaryScrollingDropDown"
                                                    onclick="scrollingDropdown_onclick('cblProjectOwner','Project Access','es','Project Accesses',33);"
                                                    DropDownListType="Project Access" DropDownListTypePluralForm="Project Accesses"
                                                    PluralForm="es" />
                                                <ext:ScrollableDropdownExtender ID="sdeCblProjectOwner" runat="server" TargetControlID="cblProjectOwner"
                                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="240px">
                                                </ext:ScrollableDropdownExtender>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </div>
    <div id="divWholePage" runat="server">
        <table id="tblRange" runat="server" class="WholeWidth">
            <tr>
                <td class="fontBold font16Px">
                    Projected Range:
                    <asp:Label ID="lblRange" runat="server"></asp:Label>
                </td>
                <td class="ProjectsListExport">
                    <label class="Width40P">
                        Export:</label>
                    <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                        Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel." />
                    <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                        Enabled="true" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="ManagementReportHeader">
                    Project List Report
                </td>
            </tr>
        </table>
        <br />
        <asp:Repeater ID="repProjectsList" runat="server" OnItemDataBound="repProjectsList_ItemDataBound">
            <HeaderTemplate>
                <div class="minheight250Px">
                    <table id="tblAllProjectsReport" class="tablesorter PersonSummaryReport zebra WholeWidthImp">
                        <thead class=" no-wrap">
                            <tr>
                                <th class="TextAlignLeftImp Padding5Imp">
                                    Project Number
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Account
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Business Group
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Business Unit
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Project Name
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Status
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Start Date
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    End Date
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Practice Area
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp Width20PerImp">
                                    Capabilities
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Project Manager
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Excutive in charge
                                </th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="ReportItemTemplate ">
                    <td class="padLeft5 textLeft">
                        <%# Eval("ProjectNumber")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <asp:HyperLink ID="HyperLink1" runat="server" Text=' <%# Eval("Client.HtmlEncodedName")%>'
                            Target="_blank" NavigateUrl='<%# GetAcountDetailsLink((int?)(Eval("Client.Id"))) %>'>
                        </asp:HyperLink>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("BusinessGroup.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Group.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%#  Eval("HtmlEncodedName")%>'
                            Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id"))) %>'>
                        </asp:HyperLink>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Status.Name")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# GetDateFormat((DateTime?)Eval("StartDate"))%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# GetDateFormat((DateTime?)Eval("EndDate"))%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Practice.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                       <asp:Label ID="lblCapabilities" runat="server"></asp:Label>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("ProjectManagerNames")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("ExecutiveInChargeName")%>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="ReportAlternateItemTemplate">
                    <td class="padLeft5 textLeft">
                        <%# Eval("ProjectNumber")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <asp:HyperLink ID="HyperLink1" runat="server" Text=' <%# Eval("Client.HtmlEncodedName")%>'
                            Target="_blank" NavigateUrl='<%# GetAcountDetailsLink((int?)(Eval("Client.Id"))) %>'>
                        </asp:HyperLink>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("BusinessGroup.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Group.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%#Eval("HtmlEncodedName")%>'
                            Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Id"))) %>'>
                        </asp:HyperLink>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Status.Name")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# GetDateFormat((DateTime?)Eval("StartDate"))%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# GetDateFormat((DateTime?)Eval("EndDate"))%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("Practice.HtmlEncodedName")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <asp:Label ID="lblCapabilities" runat="server"></asp:Label>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("ProjectManagerNames")%>
                    </td>
                    <td class="DayTotalHoursBorderLeft Padding5Imp">
                        <%# Eval("ExecutiveInChargeName")%>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </tbody></table></div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div id="divEmptyMessage" class="EmptyMessagediv" runat="server" visible="false">
        There are no resources for the selected filters.
    </div>
    <uc3:LoadingProgress ID="LoadingProgress1" runat="server" DisplayText="Refreshing Data..." />
</asp:Content>

