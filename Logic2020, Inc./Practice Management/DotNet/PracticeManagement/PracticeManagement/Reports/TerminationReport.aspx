<%@ Page Title="Termination Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TerminationReport.aspx.cs" Inherits="PraticeManagement.Reporting.TerminationReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/HumanCapitalReportsHeader.ascx"
    TagPrefix="uc" TagName="HumanCapitalReportsHeader" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/TerminationReportSummaryView.ascx"
    TagPrefix="uc" TagName="SummaryView" %>
<%@ Register Src="~/Controls/Reports/HumanCapital/TerminationReportGraphView.ascx"
    TagPrefix="uc" TagName="GraphView" %>
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
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#tblTerminationSummaryReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                }
                );
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            $("#tblTerminationSummaryReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                }
                );
        }
        function ClickHiddenImg(imgId) {
            var img = document.getElementById(imgId);
            img.click();
        }

        function ShowPanel(object, displaypnl, position) {

            var obj = $("#" + object);
            var displayPanel = $("#" + displaypnl);
            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left - position;
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }

        function HidePanel(hiddenpnl) {

            var displayPanel = $("#" + hiddenpnl);
            displayPanel.hide();
        }
    </script>
    <uc:HumanCapitalReportsHeader ID="humanCapitalReportsHeader" runat="server"></uc:HumanCapitalReportsHeader>
    <br />
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td id="tdFirst" runat="server" class="Width10Percent">
                    </td>
                    <td id="tdSecond" runat="server" class="Width80Percent">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    Show Report for:&nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="252">
                                        <asp:ListItem Text="Last Month" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Last 3 months" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Last 6 months" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Last 9 months" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="Last 12 months" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Year to Date" Value="6" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Custom Date Range" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond" style="vertical-align: middle!important;">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                </td>
                                <td class="SecondTdNewHire">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                    <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                                        BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                                        DropShadow="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    Pay Type:&nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <pmc:ScrollingDropDown ID="cblTimeScales" runat="server" AllSelectedReturnType="Null"
                                        OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Pay Type"
                                        CssClass="NewHireReportCblTimeScales" />
                                    <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                        BehaviorID="sdeTimeScales" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                        Width="250px">
                                    </ext:ScrollableDropdownExtender>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    Seniority:&nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <pmc:ScrollingDropDown ID="cblSeniorities" runat="server" AllSelectedReturnType="Null"
                                        OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblSeniorities','Seniority')"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Seniority"
                                        CssClass="NewHireReportCblTimeScales Height160PxIMP" />
                                    <ext:ScrollableDropdownExtender ID="sdeSeniorities" runat="server" TargetControlID="cblSeniorities"
                                        BehaviorID="sdeSeniorities" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                        Width="250px">
                                    </ext:ScrollableDropdownExtender>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    Termination Reason:&nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <pmc:ScrollingDropDown ID="cblTerminationReasons" runat="server" AllSelectedReturnType="Null"
                                        OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblTerminationReasons','Termination Reason')"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Termination Reason"
                                        CssClass="NewHireReportCblTimeScales Height160PxIMP" />
                                    <ext:ScrollableDropdownExtender ID="sdeTerminationReasons" runat="server" TargetControlID="cblTerminationReasons"
                                        BehaviorID="sdeTerminationReasons" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png"
                                        Width="250px">
                                    </ext:ScrollableDropdownExtender>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    Practices:&nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <pmc:ScrollingDropDown ID="cblPractices" runat="server" AllSelectedReturnType="Null"
                                        OnSelectedIndexChanged="Filters_Changed" AutoPostBack="true" onclick="scrollingDropdown_onclick('cblPractices','Practice Area')"
                                        CellPadding="3" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                                        CssClass="NewHireReportCblPractices" />
                                    <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                        UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                    </ext:ScrollableDropdownExtender>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd70">
                                    &nbsp;
                                </td>
                                <td class="SecondTdNewHire">
                                    <asp:CheckBox ID="chbInternalProjects" runat="server" AutoPostBack="true" Checked="false"
                                        OnCheckedChanged="Filters_Changed" Text="Exclude Internal Practice Areas" ToolTip="Exclude Internal Practice Areas" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="ReportBorderBottom">
                    </td>
                </tr>
            </table>
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
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                OnClick="btnCustDatesCancel_OnClick" />
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
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
                    <td class="TimePeriodTotals TerminationReportHeaderTotals">
                        <table class="tableFixed WholeWidth">
                            <tr>
                                <td class="Width20Percent">
                                    <table class="ReportHeaderTotalsTable">
                                        <tr>
                                            <td class="FirstTd fontBold">
                                                Attrition*
                                                <asp:Image alt="Attrition Hint" ImageUrl="~/Images/hint1.png" runat="server" ID="imgAttritionHint" />                                               
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="SecondTd">
                                                <asp:Label ID="lblAttrition" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Panel ID="pnlAtrritionCalculation" CssClass="pnlAttritionCalculation" runat="server"
                                        Style="display: none;">
                                        <table>
                                            <tr class="vTop font15PxImp PaddingBottom5Imp">
                                                <th class="Width50Percent">
                                                    Variables
                                                </th>
                                                <th class="Width3Percent">
                                                </th>
                                                <th class="Width5Percent textCenter">
                                                    Calculation
                                                </th>
                                                <th class="Width2Percent">
                                                </th>
                                                <th class="Width18Percent">
                                                    Attrition&nbsp;%
                                                </th>
                                            </tr>
                                            <tr>
                                                <td class="Width50Percent TextAlignLeft PaddingBottom10Imp">
                                                    # of employee terminations during specified period:
                                                </td>
                                                <td class="Width5Percent TextAlignCenter vMiddle PaddingBottom10Imp">
                                                    <asp:Label ID="lblPopUPTerminations" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width25Percent TextAlignCenter font15PxImp vMiddle no-wrap" rowspan="3">
                                                    <asp:Label ID="lblPopUPTerminationsCount" runat="server"></asp:Label>
                                                    <hr class="hrArritionCalculation" />
                                                    <asp:Label ID="lblPopUPActivensCount" runat="server"></asp:Label>
                                                    &nbsp;+&nbsp;
                                                    <asp:Label ID="lblPopUPNewHiresCount" runat="server"></asp:Label>
                                                    &nbsp;-&nbsp;
                                                    <asp:Label ID="lblPopUPTerminationsCountDenominator" runat="server"></asp:Label>
                                                </td>
                                                <td class="Width2Percent TextAlignCenter vMiddle" rowspan="3">
                                                    =
                                                </td>
                                                <td class="Width18Percent TextAlignCenter font15PxImp vMiddle" rowspan="3">
                                                    <asp:Label ID="lblPopUpArrition" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="Width50Percent TextAlignLeft PaddingBottom10Imp">
                                                    # of active employees at beginning of specified period:
                                                </td>
                                                <td class="Width5Percent TextAlignCenter PaddingBottom10Imp vMiddle">
                                                    <asp:Label ID="lblPopUPActivens" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="Width50Percent TextAlignLeft PaddingBottom10Imp">
                                                    # of new hires during specified period:
                                                </td>
                                                <td class="Width5Percent TextAlignCenter vMiddle PaddingBottom10Imp">
                                                    <asp:Label ID="lblPopUPNewHires" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td class="Width20Percent">
                                    <table class="ReportHeaderTotalsTable">
                                        <tr>
                                            <td class="FirstTd fontBold">
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
                                <td class="Width20Percent">
                                    <table class="ReportHeaderTotalsTable">
                                        <tr>
                                            <td class="FirstTd fontBold">
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
                                                        <td class="FontSize10PX">
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
                                                        <td class="FontSize10PX">
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
                                                        <td class="FontSize10PX">
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
            <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CommonCustomTabStyle PersonDetailReportCustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDetail" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="lnkbtnGraph" runat="server" Text="Graphs" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="1" ToolTip="Graphs"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <div class="tab-pane">
                <asp:MultiView ID="mvTerminationReport" runat="server" ActiveViewIndex="0">
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
            <asp:PostBackTrigger ControlID="tpSummary$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpGraph$tpSummary$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <div class="PaddingTop5">
        *Attrition calculation does not include contractors
    </div>
    <asp:Panel ID="pnlAttrition" runat="server" CssClass="popUpAttrition" Style="display: none;">
        <table>
            <tr>
                <td class="Width20Percent">
                </td>
                <td class="Width2Percent">
                </td>
                <td class="Width78Percent textCenter">
                    # of employee terminations during specified time period
                </td>
            </tr>
            <tr>
                <td class="Width25Percent">
                    <label class="LabelProject">
                        Attrition percentage
                    </label>
                    =
                </td>
                <td class="Width2Percent">
                </td>
                <td class="Width73Percent textCenter">
                    <hr class="WholeWidth hrArrition" />
                </td>
            </tr>
            <tr>
                <td class="Width20Percent">
                </td>
                <td class="Width2Percent">
                </td>
                <td class="Width78Percent">
                    <span class="TextAlignLeft">(# of active employees at beginning of specified time period)
                        + (# of new employee hires during the specified time period) – (# of employee terminations
                        during specified time period) </span>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="font-size: 10px; padding-top: 12px;">
                    <span class="TextAlignLeft">Note: in this calculation, “employee” is defined as individuals
                        with a Pay Type of “W2-Salary” and “W2-Hourly” (i.e., contractors do not factor
                        into calculation) </span>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

