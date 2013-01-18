<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryByResource" %>
<%@ Register Src="~/Controls/Reports/ProjectDetailTabByResource.ascx" TagPrefix="uc"
    TagName="ProjectDetailReport" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryTabByResource.ascx" TagPrefix="uc"
    TagName="ProjectSummaryReport" %>
<table class="PaddingTenPx ProjectSummaryReportHeader" id="tbHeader" runat="server">
    <tr>
        <td class="font14Px fontBold">
            <table class="projectSummaryHeaderDetails">
                <tr>
                    <td class="account">
                        <asp:Literal ID="ltrlAccount" runat="server"></asp:Literal>
                        >
                        <asp:Literal ID="ltrlBusinessUnit" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal ID="ltrlProjectNumber" runat="server"></asp:Literal>-
                        <asp:Literal ID="ltrlProjectName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal ID="ltrlProjectStatusAndBillingType" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="PaddingTop5Imp vBottom">
                        <asp:Literal ID="ltrlProjectRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td class="ProjectSummaryReportTotals">
            <table class="tableFixed WholeWidth">
                <tr>
                    <td class="Width27Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Projected Hours
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Literal ID="ltrlProjectedHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width27Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Total Actual Hours
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
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
<div class="WholeWidth">
    <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CommonCustomTabStyle ProjectSummaryReportPageCustomTabStyle">
        <asp:TableRow ID="rowSwitcher" runat="server">
            <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                <span class="bg"><span>
                    <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                        OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                </span>
            </asp:TableCell>
            <asp:TableCell ID="cellDetail" runat="server">
                <span class="bg"><span>
                    <asp:LinkButton ID="lnkbtnDetail" runat="server" Text="Detail" CausesValidation="false"
                        OnCommand="btnView_Command" CommandArgument="1" ToolTip="Detail"></asp:LinkButton></span>
                </span>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:MultiView ID="mvProjectReport" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwProjectSummaryReport" runat="server">
            <asp:Panel ID="pnlProjectSummaryReport" runat="server" CssClass="tab-pane">
                <uc:ProjectSummaryReport ID="ucProjectSummaryReport" runat="server" />
            </asp:Panel>
        </asp:View>
        <asp:View ID="vwProjectDetailReport" runat="server">
            <asp:Panel ID="pnlProjectDetailReport" runat="server" CssClass="tab-pane">
                <uc:ProjectDetailReport ID="ucProjectDetailReport" runat="server" />
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</div>

