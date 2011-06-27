<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UTilTimelineFilter.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.UTilTimeLineFilter" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/MonthPicker.ascx" TagPrefix="uc" TagName="MonthPicker" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<script type="text/javascript">
    function ChangeSortByRadioButtons(sender) {
        var sortby = document.getElementById("<%= ddlSortBy.ClientID%>");
        if (sortby.selectedIndex > 1) {
            sender.checked = false;
        }
    }
    function EnableResetButton() {
        var button = document.getElementById("<%= btnResetFilter.ClientID%>");
        var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
        if (button != null) {
            button.disabled = false;
            hiddenField.value = "true";
        }
    }
    function EnableDisableRadioButtons() {

        var sortby = document.getElementById("<%= ddlSortBy.ClientID%>");
        var rbAsc = document.getElementById("<%= rbSortbyAsc.ClientID%>");
        var rbDesc = document.getElementById("<%= rbSortbyDesc.ClientID%>");
        if (sortby.selectedIndex == 0) {
            rbAsc.checked = false;
            rbDesc.checked = true;
        }
        else if (sortby.selectedIndex == 1) {
            rbAsc.checked = true;
            rbDesc.checked = false;
        }
        else if (sortby.selectedIndex == 2) {
            rbAsc.checked = false;
            rbDesc.checked = false;
        }
        else {
            rbAsc.checked = false;
            rbDesc.checked = false;
        }
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
            var startYear = parseInt(startDate.format('yyyy'));
            var endYear = parseInt(endDate.format('yyyy'));
            var startMonth = 0;
            var endMonth = 0;
            if (startDate.format('MM')[0] == '0') {
                startMonth = parseInt(startDate.format('MM')[1]);
            }
            else {
                startMonth = parseInt(startDate.format('MM'));
            }
            if (endDate.format('MM')[0] == '0') {
                endMonth = parseInt(endDate.format('MM')[1]);
            }
            else {
                endMonth = parseInt(endDate.format('MM'));
            }
            if ((startYear == endYear && ((endMonth - startMonth + 1) <= 3))
            || (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 3)
            || ((endDate - startDate) / (1000 * 60 * 60 * 24)) < 90
            ) {
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
        }
    }

    function ValidatePeriod(sender, args) {
        hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
        hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
        txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
        txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
        ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
        var startDate = new Date(txtStartDate.value);
        var endDate = new Date(txtEndDate.value);
        if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate && ddlPeriod.value == '0') {
            var startYear = parseInt(startDate.format('yyyy'));
            var endYear = parseInt(endDate.format('yyyy'));
            var startMonth = 0;
            var endMonth = 0;
            if (startDate.format('MM')[0] == '0') {
                startMonth = parseInt(startDate.format('MM')[1]);
            }
            else {
                startMonth = parseInt(startDate.format('MM'));
            }
            if (endDate.format('MM')[0] == '0') {
                endMonth = parseInt(endDate.format('MM')[1]);
            }
            else {
                endMonth = parseInt(endDate.format('MM'));
            }
            if (startYear == endYear) {
                args.IsValid = ((endMonth - startMonth + 1) <= 3);
            }
            else {
                args.IsValid = (((((endYear - startYear) * 12 + endMonth) - startMonth + 1)) <= 3);
            }
            if (((endDate - startDate) / (1000 * 60 * 60 * 24)) < 90) {
                args.IsValid = true;
            }
        }
        else {
            args.IsValid = true;
        }
    }

    function CheckAndShowCustomDatesPoup(ddlPeriod) {
        imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
        lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
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
    function ChangeStartEndDates() {
        ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
        ddlDetalization = document.getElementById('<%=  ddlDetalization.ClientID %>');
        if (ddlPeriod.value == '0' && ddlDetalization.value == '30') {

            hdnStartDateTxtBoxId = document.getElementById('<%= hdnStartDateTxtBoxId.ClientID %>');
            hdnEndDateTxtBoxId = document.getElementById('<%= hdnEndDateTxtBoxId.ClientID %>');
            hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            txtStartDate = document.getElementById(hdnStartDateTxtBoxId.value);
            txtEndDate = document.getElementById(hdnEndDateTxtBoxId.value);
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
                var startYear = parseInt(startDate.format('yyyy'));
                var endYear = parseInt(endDate.format('yyyy'));
                var startMonth = 0;
                var endMonth = 0;
                if (startDate.format('MM')[0] == '0') {
                    startMonth = parseInt(startDate.format('MM')[1]);
                }
                else {
                    startMonth = parseInt(startDate.format('MM'));
                }
                if (endDate.format('MM')[0] == '0') {
                    endMonth = parseInt(endDate.format('MM')[1]);
                }
                else {
                    endMonth = parseInt(endDate.format('MM'));
                }

                startDate = new Date(startMonth.toString() + '/01/' + startYear.toString());
                if (endMonth == 12) {
                    endYear = endYear + 1;
                    endMonth = 1;
                    endDate = new Date('01/01/' + endYear.toString());
                }
                else {
                    endMonth = endMonth + 1;
                    endDate = new Date(endMonth.toString() + '/01/' + endYear.toString());
                }
                endDate = new Date((endDate - (1000 * 60 * 60 * 24)));
                if ((endYear - startYear) * 12 + endMonth - startMonth > 3) {
                    endMonth = (startMonth + 2) % 12;
                    if (startMonth > endMonth) {
                        endYear = startYear + 1;
                    }
                    else {
                        endYear = startYear;
                    }
                    endDate = new Date((endMonth + 1).toString() + '/01/' + endYear.toString());
                    endDate = new Date((endDate - (1000 * 60 * 60 * 24)));
                }
                var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
                var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
                if (startDateCalExtender != null) {
                    startDateCalExtender.set_selectedDate(new Date(startDate.format("MM/dd/yyyy")));
                }
                if (endDateCalExtender != null) {
                    endDateCalExtender.set_selectedDate(new Date(endDate.format("MM/dd/yyyy")));
                }
                hdnStartDate.value = startDate.format("MM/dd/yyyy");
                hdnEndDate.value = endDate.format("MM/dd/yyyy");
                lblCustomDateRange.innerHTML = '(' + hdnStartDate.value + '&nbsp;-&nbsp;' + hdnEndDate.value + ')';
            }
        }
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
<style type="text/css">
    .displayNone
    {
        display: none;
    }
</style>
<script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.4.1.js"></script>
<div class="filters" style="margin-bottom: 10px;">
    <div class="buttons-block">
        <table class="WholeWidth">
            <tr>
                <td align="left" style="width: 98%;">
                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                        ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                        ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                        ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                    <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                        ToolTip="Expand Filters and Sort Options" />
                    &nbsp;
                    <asp:Label ID="lblUtilizationFrom" runat="server" Text="Show Utilization for"></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" Onchange=" EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                        <asp:ListItem Text="Next 3 months" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Last 3 months" Value="-3"></asp:ListItem>
                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                    </asp:DropDownList>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
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
                    <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    &nbsp;
                    <asp:Label ID="lblBy" runat="server" Text="by "></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlDetalization" runat="server" AutoPostBack="false" onchange="EnableResetButton();ChangeStartEndDates();">
                        <asp:ListItem Value="1">1 Day</asp:ListItem>
                        <asp:ListItem Selected="True" Value="7">1 Week</asp:ListItem>
                        <asp:ListItem Value="30">1 Month</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Label ID="lblUtilization" runat="server" Text="  where U% is "></asp:Label>
                    &nbsp;
                    <asp:DropDownList ID="ddlAvgUtil" runat="server" AutoPostBack="false" onchange="EnableResetButton();">
                        <asp:ListItem Value="2147483647">0 - 106+</asp:ListItem>
                        <asp:ListItem Value="106">&lt; 106+</asp:ListItem>
                        <asp:ListItem Value="90">&lt; 90</asp:ListItem>
                        <asp:ListItem Value="50">&lt; 50</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="90px" OnClick="btnUpdateView_OnClick"
                                    EnableViewState="False" />
                            </td>
                            <td>
                                <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="90px" OnClick="btnResetFilter_OnClick" />
                            </td>
                            <td>
                                <asp:Button ID="btnSaveReport" runat="server" Text="Save Report" Width="90px" OnClick="btnSaveReport_OnClick"
                                    EnableViewState="False" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
            CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
            <table class="WholeWidth">
                <tr>
                    <td align="center">
                        <table>
                            <tr>
                                <td>
                                    <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                        FromToDateFieldWidth="70" />
                                </td>
                                <td>
                                    <asp:CustomValidator ID="cstvalPeriodRange" runat="server" ClientValidationFunction="ValidatePeriod"
                                        Text="*" EnableClientScript="true" ValidationGroup="<%# ClientID %>" ToolTip="Period should not be more than three months"
                                        ErrorMessage="Period should not be more than three months."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 10px 0px 10px 0px;">
                        <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid();"
                            Text="OK" Style="float: none !Important;" CausesValidation="true" />
                        <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                            OnClientClick="return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnCustDatesCancel" runat="server" Text="Cancel" Style="float: none !Important;" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:ValidationSummary ID="valSum" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <asp:Panel ID="pnlFilters" runat="server">
        <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
            CssClass="CustomTabStyle">
            <AjaxControlToolkit:TabPanel runat="server" ID="tpFilters">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Filters</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table class="WholeWidth">
                        <tr align="center">
                            <td style="width: 125px; border-bottom: 1px solid black;" valign="top">
                                Person Status
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 150px; border-bottom: 1px solid black;" valign="top">
                                Pay Type
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 320px; border-bottom: 1px solid black;" colspan="2" valign="top">
                                Project Type
                            </td>
                            <td style="width: 30px;">
                            </td>
                            <td style="width: 250px; border-bottom: 1px solid black;">
                                Practice Area
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chbActivePersons" runat="server" Text="Active" ToolTip="Include active persons into report"
                                    AutoPostBack="false" Checked="True" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td rowspan="2" class="floatRight">
                                <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')" BackColor="White"
                                    CellPadding="3" NoItemsType="All" SetDirty="False" Width="200px" DropDownListType="Pay Type"
                                    Height="100px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                    UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="200px">
                                </ext:ScrollableDropdownExtender>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbActiveProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Active" ToolTip="Include active projects into report" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbInternalProjects" runat="server" AutoPostBack="false" Checked="True"
                                    onclick="EnableResetButton();" Text="Internal" ToolTip="Include internal projects into report" />
                            </td>
                            <td>
                            </td>
                            <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                                    CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                                    Width="260px" BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                    UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chbProjectedPersons" runat="server" Text="Projected" ToolTip="Include projected persons into report"
                                    AutoPostBack="false" Checked="false" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chbProjectedProjects" runat="server" AutoPostBack="false" Checked="True"
                                    Text="Projected" ToolTip="Include projected projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chbExperimentalProjects" runat="server" AutoPostBack="false" Text="Experimental"
                                    ToolTip="Include experimental projects into report" onclick="EnableResetButton();" />
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkExcludeInternalPractices" runat="server" Text="Exclude Internal Practice Areas"
                                    onclick="EnableResetButton();" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
            <AjaxControlToolkit:TabPanel runat="server" ID="tpGranularity">
                <HeaderTemplate>
                    <span class="bg"><a href="#"><span>Sort Options</span></a> </span>
                </HeaderTemplate>
                <ContentTemplate>
                    <table class="opportunity-description" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                Sort by &nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlSortBy" runat="server" Style="width: 300px;" onchange="EnableResetButton(); EnableDisableRadioButtons();">
                                    <asp:ListItem Value="0">Average Utilization by Period</asp:ListItem>
                                    <asp:ListItem Value="1">Alphabetical by User</asp:ListItem>
                                    <asp:ListItem Value="2">Pay Type</asp:ListItem>
                                    <asp:ListItem Value="3">Practice Area</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="width: 40px;">
                            </td>
                            <td>
                                <asp:RadioButton ID="rbSortbyAsc" runat="server" Text="Ascending" onclick="EnableResetButton();ChangeSortByRadioButtons(this);"
                                    GroupName="Sortby" AutoPostBack="false" />
                            </td>
                            <td>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="rbSortbyDesc" runat="server" Text="Descending" onclick="EnableResetButton();ChangeSortByRadioButtons(this);"
                                    GroupName="Sortby" AutoPostBack="false" Checked="true" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </ContentTemplate>
            </AjaxControlToolkit:TabPanel>
        </AjaxControlToolkit:TabContainer>
    </asp:Panel>
    <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
</div>
<asp:HiddenField ID="hdnIsSampleReport" runat="server" />

