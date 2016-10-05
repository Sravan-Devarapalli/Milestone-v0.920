<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpenseSummaryDetails.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ExpenseSummaryDetails" %>
<div>
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="3" class="Width90Percent">
                <input id="btnExpand" runat="server" type="button" value="Expand All" class="expandOrCollapseAll" />
            </td>
            <td class=" Width10Percent padRight5">
                <table class="WholeWidth">
                    <tr>
                        <td class="textRight">
                            Export:
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repExpense" runat="server" OnItemDataBound="repExpense_ItemDataBound">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="WholeWidthWithHeight">
                <tr class="textLeft">
                    <td colspan="4" class="ProjectAccountName Width95Percent no-wrap">
                        <%# Eval("HtmlEncodedName")%>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlExpenseDetails" runat="server">
                <asp:Repeater ID="repMonth" runat="server" OnItemDataBound="repMonth_ItemDataBound">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table class="WholeWidthWithHeight">
                            <tr class="textLeft bgColorD4D0C9">
                                <td class="Width80Percent padLeft20Imp">
                                    <asp:Image ID="imgDate" runat="server" TargetControlID="pnlDetails" CssClass="detailsChevron chevronImage"
                                        ImageUrl="~/Images/expand.jpg" ToolTip="Expand Date Details" />
                                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                    <asp:Label ID="lblMonthName" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:Panel ID="pnlDetails" runat="server" CssClass="Pad-Left1P displayNone">
                            <asp:Repeater ID="repMilestoneExpenses" runat="server" OnItemDataBound="repMilestoneExpenses_ItemDataBound">
                                <HeaderTemplate>
                                    <div>
                                        <table id="tblExpenseSummaryByMilestone" class="CompPerfTable BackGroundColorWhite">
                                            <thead>
                                                <tr class="ie-bg NoBorder">
                                                    <th>
                                                        <asp:Label ID="lblProjectNameHeader" runat="server"></asp:Label>
                                                    </th>
                                                    <th class="Width14Percent">
                                                        Expense Type
                                                    </th>
                                                    <th class="Width8Per">
                                                        Start Date
                                                    </th>
                                                    <th class="Width8Per">
                                                        End Date
                                                    </th>
                                                    <th class="Width10Per hideEstCol" id="thEstExpense" runat="server">
                                                        Estimated Expense($)
                                                    </th>
                                                    <th class="Width10Per hideActCol" id="thActExpense" runat="server">
                                                        Actual Expense($)
                                                    </th>
                                                    <th class="hideDiffCol" id="thDiffExpense" runat="server">
                                                        Difference($)
                                                    </th>
                                                    <th class="Width10Per">
                                                        Reimbursement %
                                                    </th>
                                                    <th class="Width10Per">
                                                        Reimburse Amount($)
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class="BackGroundColorWhite">
                                        <td class="textLeft LeftPadding10px">
                                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Type.Name")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                                        </td>
                                        <td class="textCenter hideEstCol" id="tdEstExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideActCol" id="tdActExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideDiffCol" id="tdDiffExpense" runat="server">
                                            <%#  (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Reimbursement")%>
                                            %
                                        </td>
                                        <td class="textCenter">
                                            <asp:Label ID="lblReimburse" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="alterrow">
                                        <td class="textLeft LeftPadding10px">
                                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Type.Name")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                                        </td>
                                        <td class="textCenter hideEstCol" id="tdEstExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideActCol" id="tdActExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideDiffCol" id="tdDiffExpense" runat="server">
                                            <%#  (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Reimbursement")%>
                                            %
                                        </td>
                                        <td class="textCenter">
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
                    <AlternatingItemTemplate>
                        <table class="WholeWidthWithHeight">
                            <tr class="textLeft bgcolor_ECE9D9">
                                <td class="Width80Percent padLeft20Imp">
                                    <asp:Image ID="imgDate" runat="server" TargetControlID="pnlDateDetails" CssClass="detailsChevron chevronImage"
                                        ImageUrl="~/Images/expand.jpg" ToolTip="Expand Date Details" />
                                    <asp:Label ID="lbDate" Style="display: none;" runat="server"></asp:Label>
                                    <asp:Label ID="lblMonthName" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:Panel ID="pnlDateDetails" runat="server" CssClass="Pad-Left1P displayNone">
                            <asp:Repeater ID="repMilestoneExpenses" runat="server" OnItemDataBound="repMilestoneExpenses_ItemDataBound">
                                <HeaderTemplate>
                                    <div>
                                        <table id="tblExpenseSummaryByMilestone" class="CompPerfTable BackGroundColorWhite">
                                            <thead>
                                                <tr class="ie-bg NoBorder">
                                                    <th>
                                                        <asp:Label ID="lblProjectNameHeader" runat="server"></asp:Label>
                                                    </th>
                                                    <th class="Width14Percent">
                                                        Expense Type
                                                    </th>
                                                    <th class="Width8Per">
                                                        Start Date
                                                    </th>
                                                    <th class="Width8Per">
                                                        End Date
                                                    </th>
                                                    <th class="Width10Per hideEstCol" id="thEstExpense" runat="server">
                                                        Estimated Expense($)
                                                    </th>
                                                    <th class="Width10Per hideActCol" id="thActExpense" runat="server">
                                                        Actual Expense($)
                                                    </th>
                                                    <th class="hideDiffCol" id="thDiffExpense" runat="server">
                                                        Difference($)
                                                    </th>
                                                    <th class="Width10Per">
                                                        Reimbursement %
                                                    </th>
                                                    <th class="Width10Per">
                                                        Reimburse Amount($)
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class="BackGroundColorWhite">
                                        <td class="textLeft LeftPadding10px">
                                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Type.Name")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                                        </td>
                                        <td class="textCenter hideEstCol" id="tdEstExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideActCol" id="tdActExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideDiffCol" id="tdDiffExpense" runat="server">
                                            <%#  (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Reimbursement")%>
                                            %
                                        </td>
                                        <td class="textCenter">
                                            <asp:Label ID="lblReimburse" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="alterrow">
                                        <td class="textLeft LeftPadding10px">
                                            <asp:Label ID="lblExpenseName" runat="server"></asp:Label>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Type.Name")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.StartDate"))%>
                                        </td>
                                        <td class="textCenter">
                                            <%# GetDateFormat((DateTime?)Eval("Expense.EndDate"))%>
                                        </td>
                                        <td class="textCenter hideEstCol" id="tdEstExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.ExpectedAmount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideActCol" id="tdActExpense" runat="server">
                                            <%# (((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Amount).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter hideDiffCol" id="tdDiffExpense" runat="server">
                                            <%#(((DataTransferObjects.Reports.ExpenseSummary)Container.DataItem).Expense.Difference).ToString("###,###,###,###,###,##0.##")%>
                                        </td>
                                        <td class="textCenter">
                                            <%# Eval("Expense.Reimbursement")%>
                                            %
                                        </td>
                                        <td class="textCenter">
                                            <asp:Label ID="lblReimburse" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate>
                                    </tbody></table></div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </asp:Panel>
                    </AlternatingItemTemplate>
                </asp:Repeater>
            </asp:Panel>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
    There are no Expenses for the selected Filters.
</div>

