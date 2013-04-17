<%@ Page Title="By Account" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="AccountSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.AccountSummaryReport" %>
    
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessUnit.ascx" TagPrefix="uc"
    TagName="ByBusinessUnit" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByProject.ascx" TagPrefix="uc" TagName="ByProject" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessDevelopment.ascx" TagPrefix="uc"
    TagName="ByBusinessDevelopment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ExpandOrCollapse.min.js", this) %>" type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet" type="text/css" />
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblAccountSummaryByBusinessReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });


        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblAccountSummaryByBusinessReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td class="vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td>
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Account:&nbsp;
                    </td>
                    <td class="textLeft">
                        <asp:DropDownList ID="ddlAccount" runat="server" AutoPostBack="true" CssClass="Width220Px"
                            OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels vTop lineHeight30Px">
                        Business Unit:&nbsp;
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            OnSelectedIndexChanged="cblProjectGroup_OnSelectedIndexChanged" NoItemsType="All"
                            onclick="scrollingDropdown_onclick('cblProjectGroup','Business Unit')" AutoPostBack="true"
                            DropDownListType="Business Unit" CellPadding="3" CssClass="AccountSummaryBusinessUnitsDiv" />
                        <ext:ScrollableDropdownExtender ID="sdeProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                            UseAdvanceFeature="true" Width="218px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td class="ReportFilterLabels">
                        Range:&nbsp;
                    </td>
                    <td class="textLeft">
                        <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                            CssClass="Width220Px">
                            <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                            <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                            <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
                            <asp:ListItem Text="This Week" Value="7"></asp:ListItem>
                            <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                            <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                            <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                            <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                            <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                            <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td>
                    </td>
                    <td class="textLeft">
                        <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                        <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                        <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                        <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="2">
                        &nbsp;
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="2">
                    </td>
                    <td>
                    </td>
                </tr>
                <tr class="ReportBorderBottomByAccount">
                    <td colspan="3">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass CustomDatesPopUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldCssClass="Width70Px" />
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
            <br />
            <div id="divWholePage" runat="server">
                <table class="PaddingTenPx AccountSummaryReportHeader">
                    <tr>
                        <td class="FirstTrTd1">
                            <table>
                                <tr>
                                    <td class="FirstTrTd2">
                                        <asp:Literal ID="ltAccount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTrTd3">
                                        <asp:Literal ID="ltHeaderCount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FirstTrTd4">
                                        <asp:Literal ID="ltRange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="FirstTrTd5">
                            <table class="ReportHeaderTotals">
                                <tr>
                                    <td class="Width27Percent">
                                        <table class="WholeWidth ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd no-wrap">
                                                    Total Project Hours
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlTotalProjectHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width27Percent">
                                        <table class="WholeWidth ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd">
                                                    BD
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlBDHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width27Percent vBottom">
                                        <table class="ReportHeaderBillAndNonBillTable">
                                            <tr>
                                                <td>
                                                    BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="billingHours">
                                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    NON-BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                                            <td class="billingGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                                            <td class="nonBillingGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width2Percent">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CommonCustomTabStyle AccountSummaryReportCustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellBusinessUnit" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnBusinessUnit" runat="server" Text="Business Unit" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Business Unit"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellProject" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnProject" runat="server" Text="Project" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="1" ToolTip="Project"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellBusinessDevelopment" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnBusinessDevelopment" runat="server" Text="Business Development"
                                    CausesValidation="false" OnCommand="btnView_Command" CommandArgument="2" ToolTip="Business Development"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvAccountReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwBusinessUnitReport" runat="server">
                        <asp:Panel ID="pnlBusinessUnitReport" runat="server" CssClass="WholeWidth">
                            <uc:ByBusinessUnit ID="tpByBusinessUnit" runat="server"></uc:ByBusinessUnit>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <uc:ByProject ID="tpByProject" runat="server"></uc:ByProject>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwBusinessDevelopmentReport" runat="server">
                        <asp:Panel ID="pnlBusinessDevelopmentReport" runat="server" CssClass="WholeWidth">
                            <uc:ByBusinessDevelopment ID="tpByBusinessDevelopment" runat="server"></uc:ByBusinessDevelopment>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="tpByProject$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByBusinessUnit$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByBusinessDevelopment$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

