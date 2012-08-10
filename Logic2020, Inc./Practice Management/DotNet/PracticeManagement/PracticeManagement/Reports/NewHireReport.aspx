<%@ Page Title="New Hire Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="NewHireReport.aspx.cs" Inherits="PraticeManagement.Reporting.NewHireReport " %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/HumanCapitalReportsHeader.ascx"
    TagPrefix="uc" TagName="HumanCapitalReportsHeader" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/NewHireReportSummaryView.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/NewHireReportGraphView.ascx" TagPrefix="uc"
    TagName="GraphView" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">

        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
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
            var hdnStartDate = document.getElementById('<%= hdnStartDate.ClientID %>');
            var hdnEndDate = document.getElementById('<%= hdnEndDate.ClientID %>');
            var hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= hdnStartDateCalExtenderBehaviourId.ClientID %>');
            var hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= hdnEndDateCalExtenderBehaviourId.ClientID %>');

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
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <uc:HumanCapitalReportsHeader ID="humanCapitalReportsHeader" runat="server"></uc:HumanCapitalReportsHeader>
            <br />
            <table style="width: 100%;">
                <tr>
                    <td id="tdFirst" runat="server" class="Width10Percent">
                    </td>
                    <td id="tdSecond" runat="server" class="Width80Percent">
                        <div class="filters Margin-Bottom10Px">
                            <div class="buttons-block">
                                <table class="WholeWidth">
                                    <tr>
                                        <td align="left" class="Width98Percent">
                                            <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                                ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                                ToolTip="Expand Filters and Sort Options" />
                                            &nbsp; <b>Show Report for</b> &nbsp;
                                            <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="false" onchange="EnableResetButton(); CheckAndShowCustomDatesPoup(this);">
                                                <asp:ListItem Text="This Month" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Last Month" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Q1" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Q2" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Q3" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Q4" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Year To Date" Value="7" Selected="True"></asp:ListItem>
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
                                            <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                            <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                        </td>
                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width90PxImp"
                                                            OnClick="btnUpdateView_OnClick" EnableViewState="False" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width90PxImp"
                                                            OnClick="btnResetFilter_OnClick" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp"
                                Style="display: none;">
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
                                            <asp:Button ID="btnCustDatesOK" runat="server" OnClientClick="CheckIfDatesValid();"
                                                Text="OK" CausesValidation="true" />
                                            <asp:Button ID="btnCustDatesClose" runat="server" Style="display: none;" CausesValidation="true"
                                                OnClientClick="return false;" />
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
                                <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                                    CssClass="CustomTabStyle">
                                    <AjaxControlToolkit:TabPanel runat="server" ID="tpFilters">
                                        <HeaderTemplate>
                                            <span class="bg"><a href="#"><span>Filters</span></a> </span>
                                        </HeaderTemplate>
                                        <ContentTemplate>
                                            <table class="WholeWidth" style="width: 1000px;">
                                                <tr align="center">
                                                    <td class="BorderBottom1px vTop" style="width: 30%;">
                                                        Person Status
                                                    </td>
                                                    <td class="" style="width: 5%;">
                                                    </td>
                                                    <td class="BorderBottom1px vTop" style="width: 25%;">
                                                        Pay Type
                                                    </td>
                                                    <td class="" style="width: 5%;">
                                                    </td>
                                                    <td class="BorderBottom1px" style="width: 35%;">
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
                                                        <pmc:ScrollingDropDown ID="cblTimeScales" runat="server" AllSelectedReturnType="AllItems"
                                                            onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')" CellPadding="3"
                                                            NoItemsType="All" SetDirty="False" DropDownListType="Pay Type" CssClass="NewHireReportCblTimeScales" />
                                                        <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                                            BehaviorID="sdeTimeScales" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                                            Width="200px">
                                                        </ext:ScrollableDropdownExtender>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td rowspan="2" class="floatRight PaddingTop5 padLeft3">
                                                        <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="AllItems"
                                                            onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" CellPadding="3"
                                                            NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" CssClass="NewHireReportCblPractices" />
                                                        <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                                            UseAdvanceFeature="true" Width="300px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                                        </ext:ScrollableDropdownExtender>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="chbProjectedPersons" runat="server" AutoPostBack="false" Checked="True"
                                                            onclick="EnableResetButton();" Text="Projected" ToolTip="Include projected persons into report" /><br />
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="chbTerminatedPersons" runat="server" Text="Terminated" ToolTip="Include terminated persons into report"
                                                            AutoPostBack="false" Checked="True" onclick="EnableResetButton();" />
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td class="PaddingTop5 padLeft3">
                                                        <asp:CheckBox ID="chbInternalProjects" runat="server" AutoPostBack="false" Checked="false"
                                                            onclick="EnableResetButton();" Text="Exclude Internal Practice Areas" ToolTip="Exclude Internal Practice Areas" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </AjaxControlToolkit:TabPanel>
                                </AjaxControlToolkit:TabContainer>
                            </asp:Panel>
                            <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
                        </div>
                    </td>
                    <td id="tdThird" runat="server" class="Width10Percent">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="ReportBorderBottom">
                    </td>
            </table>
            <div id="divWholePage" runat="server" >
                <table class="PaddingTenPx TimePeriodSummaryReportHeader">
                    <tr>
                        <td class="font16Px fontBold">
                            <table>
                                <tr>
                                    <td class="vtop PaddingBottom10Imp">
                                        <asp:Literal ID="ltPersonCount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PaddingTop10Imp vBottom">
                                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="TimePeriodTotals">
                            <table class="tableFixed WholeWidth">
                                <tr>
                                    <td class="Width25Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd">
                                                    Total Employees
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlTotalEmployees" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width25Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd">
                                                    Total Contractors
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlTotalContractors" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width7Percent">
                                    </td>
                                    <td class="ReportHeaderBandNBGraph Width12Percent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlW2SalaryCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trW2Salary" runat="server" title="W2-Salary">
                                                            <td class="W2SalaryGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                W2-Salary
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph Width12Percent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlW2HourlyCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trW2Hourly" runat="server" title="W2-Hourly">
                                                            <td class="W2HourlyGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                W2-Hourly
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph Width12Percent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlContractorsCount" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trContrator" runat="server" title="Contractors">
                                                            <td class="ContractorsGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                Contractors
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width7Percent">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:MultiView ID="mvNewHireReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwSummary" runat="server">
                        <asp:Panel ID="pnlSummary" runat="server" CssClass="WholeWidth">
                            <uc:SummaryView ID="tpSummary" runat="server"></uc:SummaryView>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwGraph" runat="server">
                        <asp:Panel ID="pnlGraph" runat="server" CssClass="WholeWidth">
                            <uc:GraphView ID="tpGraph" runat="server"></uc:GraphView>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

