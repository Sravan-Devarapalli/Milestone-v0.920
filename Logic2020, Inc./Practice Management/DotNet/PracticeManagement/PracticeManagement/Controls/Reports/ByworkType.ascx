<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByworkType.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByworkType" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
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
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
            </td>
            <td class="Width10Percent padRight5">
                <table class="WholeWidth">
                    <tr>
                        <td>
                            Export:
                        </td>
                        <td>
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                        <td>
                            <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                                Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repWorkType" runat="server" OnItemDataBound="repWorkType_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblProjectSummaryByWorkType" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th class="padLeft5Imp Width460Px textLeft">
                                WorkType
                            </th>
                            <th class="Width150px">
                                <asp:Label ID="lblBillable" runat="server" Text="Billable"></asp:Label>
                            </th>
                            <th class="Width150px">
                                <asp:Label ID="lblNonBillable" runat="server" Text="Non-Billable"></asp:Label>
                            </th>
                            <th class="Width140px">
                                <asp:Label ID="lblActualHours" runat="server" Text="Actual Hours"></asp:Label>
                            </th>
                            <th class="Width160px">
                            </th>
                            <th class="Width325Px">
                                Percent of Total Actual Hours
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5Imp textLeft">
                    <%# Eval("WorkType.Name")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td>
                </td>
                <td sorttable_customkey='<%# Eval("WorkTypeTotalHoursPercent")%>'>
                    <table class="TdLevelNoBorder WholeWidth">
                        <tr>
                            <td class="Width1Percent">
                            </td>
                            <td class="textRight Width80Percent">
                                <table class="ByWorkTypeGraph">
                                    <tr>
                                        <td class="FirstTd" width="<%# Eval("WorkTypeTotalHoursPercent")%>%">
                                        </td>
                                        <td class="SecondTd" width="<%# 100 - ((int)Eval("WorkTypeTotalHoursPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width17Percent textLeft padLeft10Imp">
                                <%# Eval("WorkTypeTotalHoursPercent")%>%
                            </td>
                            <td class="Width2Percent">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
        There are no Time Entries by any Employee for the selected range.
    </div>
</div>
<asp:Panel ID="pnlTotalBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Billable:
    </label>
    <asp:Label ID="lblTotalBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalNonBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Non-Billable:
    </label>
    <asp:Label ID="lblTotalNonBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalActualHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <table>
        <tr>
            <td class="fontBold">
                Total Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total Non-Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalNonBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold padRight15">
                Total Actual Hours:
            </td>
            <td>
                <asp:Label ID="lblTotalActualHours" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>

