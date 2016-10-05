<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpenseSummaryByType.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ExpenseSummaryByType" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ExpenseDetailsByMonth.ascx" TagName="ExpenseDetailsByExpenseType"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ExpenseSummaryDetails.ascx" TagName="ExpenseSummaryDetails"
    TagPrefix="uc" %>
<table class="PaddingTenPx TimePeriodSummaryReportHeader">
    <tr>
        <td class="font16Px fontBold">
            <table>
                <tr>
                    <td class="vtop PaddingBottom10Imp">
                        <asp:Literal ID="ltTypeCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="PaddingTop10Imp vBottom">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Table ID="tblViewSwitch" runat="server" CssClass="CommonCustomTabStyle ProjectSummaryReportPageCustomTabStyle">
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
<asp:MultiView ID="mvReport" runat="server" ActiveViewIndex="0">
    <asp:View ID="vwExpenseSummaryReport" runat="server">
        <div class="tab-pane">
            <table class="WholeWidthWithHeight">
                <tr>
                    <td colspan="3" class="Width90Percent">
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
            <asp:Repeater ID="repExpenseType" runat="server" OnItemDataBound="repExpenseType_ItemDataBound">
                <HeaderTemplate>
                    <div style="max-height: 400px; overflow: auto;">
                        <table id="tblExpenseSummaryByType" class="tablesorter TimePeriodByproject WholeWidth">
                            <thead>
                                <tr runat="server" id="lvHeader">
                                    <th class="ProjectColoum no-wrap">
                                        Expense Type
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="ReportItemTemplate no-wrap" runat="server" id="lvItem">
                        <td class="t-left padLeft5 " sorttable_customkey='<%# Eval("Type.Name")%>'>
                            <asp:LinkButton ID="lnkExpenseType" ExpenseTypeId='<%# Eval("Type.Id") %>' runat="server"
                                CssClass="fontBold HyplinkConsultantDetailReport Blink" ToolTip='<%#  Eval("Type.Name")%>'
                                OnClick="lnkExpenseType_OnClick" Text='<%#  Eval("Type.Name")%>'></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="ReportItemTemplate bgGroupByProjectHeader no-wrap" runat="server" id="lvItem">
                        <td class="t-left padLeft5 " sorttable_customkey='<%# Eval("Type.Name")%>'>
                            <asp:LinkButton ID="lnkExpenseType" ExpenseTypeId='<%# Eval("Type.Id") %>' runat="server"
                                CssClass="fontBold HyplinkConsultantDetailReport Blink" ToolTip='<%#  Eval("Type.Name")%>'
                                OnClick="lnkExpenseType_OnClick" Text='<%#  Eval("Type.Name")%>'></asp:LinkButton>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
            <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
                There are no Expenses the selected Filters.
            </div>
        </div>
        <br />
        <div class="buttons-block BenchCostFooter SupFont">
            <table>
                <tr>
                    <td>
                        <b>Legend</b>
                    </td>
                </tr>
                <tr>
                    <td>
                        1 - Amount in <span style="color: #0000FF">Blue</span> represents Actual Expenses.
                    </td>
                </tr>
                <tr>
                    <td>
                        2 - Amount in <span style="color: #696969">Grey</span> represents Estimated Expenses.
                    </td>
                </tr>
            </table>
        </div>
    </asp:View>
    <asp:View ID="vwProjectExpenseDetailReport" runat="server">
        <asp:Panel ID="pnlExpenseTypeSummaryDetailsTab" runat="server" CssClass="tab-pane">
            <uc:ExpenseSummaryDetails ID="ucExpenseSummaryDetails" runat="server" IsByProject="false" />
        </asp:Panel>
    </asp:View>
</asp:MultiView>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeExpenseTypeDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelExpenseDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlExpenseTypeDetailReport"
    DropShadow="false" />
<asp:Panel ID="pnlExpenseTypeDetailReport" class="" Style="display: none; max-width: 75%;"
    runat="server" CssClass="TimePeriodByProject_ProjectDetailReport">
    <table class="WholeWidth Padding5">
        <tr>
            <td class="WholeWidth">
                <table class="WholeWidthWithHeight">
                    <tr class="bgColor_F5FAFF">
                        <td class="TimePeriodByProject_ProjectName">
                            <asp:Literal ID="ltrlExpenseType" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <div class="TimePeriodByProject_ProjectDetailsDiv">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="Width97Percent">
                                            <uc:ExpenseDetailsByExpenseType ID="ucExpenseDetailsByExpenseType" runat="server"
                                                Visible="false" IsExpensetypeDetails="true" IsProjectExpenseDetails="false" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr class="bgColor_F5FAFF">
                        <td class="textRight Padding3PX">
                            <asp:Button ID="btnCancelExpenseDetailReport" Text="Close" ToolTip="Close" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

