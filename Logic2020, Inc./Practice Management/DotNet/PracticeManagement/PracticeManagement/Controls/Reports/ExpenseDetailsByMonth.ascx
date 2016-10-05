<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpenseDetailsByMonth.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ExpenseDetailsByMonth" %>
<%@ Import Namespace="DataTransferObjects.Reports" %>
<asp:HiddenField ID="hdncpeExtendersIds" runat="server" Value="" />
<asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
<table id="tblExportSection" runat="server" class="WholeWidthWithHeight">
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
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repMonths" runat="server" OnItemDataBound="repMonths_ItemDataBound">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="">
            <tr class="textLeft">
                <td colspan="4" class="ProjectAccountName Width80Percent no-wrap">
                    <asp:Label ID="lblMonthName" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlExpenseDetails" runat="server" CssClass="bg-white">
            <asp:Repeater ID="repMilestoneExpenses" runat="server" OnItemDataBound="repMilestoneExpenses_ItemDataBound">
                <HeaderTemplate>
                    <div>
                        <table id="tblExpenseSummaryByMilestone" class="CompPerfTable WholeWidth BackGroundColorWhite">
                            <thead>
                                <tr class="ie-bg NoBorder">
                                    <th class="no-wrap">
                                        <asp:Label ID="lblProjectNameHeader" runat="server"></asp:Label>
                                    </th>
                                    <th class="Width14Percent no-wrap">
                                        Expense Type
                                    </th>
                                    <th class="Width8Per no-wrap">
                                        Start Date
                                    </th>
                                    <th class="Width8Per no-wrap">
                                        End Date
                                    </th>
                                    <th class="Width10Per hideEstCol no-wrap">
                                        Estimated Expense($)
                                    </th>
                                    <th class="Width10Per hideActCol no-wrap">
                                        Actual Expense($)
                                    </th>
                                    <th class="hideDiffCol no-wrap">
                                        Difference($)
                                    </th>
                                    <th class="Width10Per no-wrap">
                                        Reimbursement %
                                    </th>
                                    <th class="Width10Per no-wrap">
                                        Reimburse Amount($)
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="BackGroundColorWhite">
                        <td class="textLeft no-wrap">
                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# Eval("Expense.Type.Name")%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                        </td>
                        <td class="textCenter hideEstCol no-wrap">
                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter hideActCol no-wrap">
                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter hideDiffCol no-wrap">
                            <%#  (((ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# Eval("Expense.Reimbursement")%>
                            %
                        </td>
                        <td class="textRight padRight35 no-wrap">
                            <asp:Label ID="lblReimburse" runat="server"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alterrow">
                        <td class="textLeft no-wrap">
                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# Eval("Expense.Type.Name")%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                        </td>
                        <td class="textCenter hideEstCol no-wrap">
                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter hideActCol no-wrap">
                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter hideDiffCol no-wrap">
                            <%#(((ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                        </td>
                        <td class="textCenter no-wrap">
                            <%# Eval("Expense.Reimbursement")%>
                            %
                        </td>
                        <td class="textRight padRight35 no-wrap">
                            <asp:Label ID="lblReimburse" runat="server"></asp:Label>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Expenses.
</div>

