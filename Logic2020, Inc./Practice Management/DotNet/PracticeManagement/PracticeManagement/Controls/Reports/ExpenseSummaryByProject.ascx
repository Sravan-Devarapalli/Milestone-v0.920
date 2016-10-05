<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpenseSummaryByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ExpenseSummaryByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ExpenseDetailsByMonth.ascx" TagName="ExpenseDetailsByProject"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ExpenseSummaryDetails.ascx" TagName="ExpenseSummaryDetails"
    TagPrefix="uc" %>
<table class="PaddingTenPx TimePeriodSummaryReportHeader">
    <tr>
        <td class="font16Px fontBold">
            <table>
                <tr>
                    <td class="vtop PaddingBottom10Imp">
                        <asp:Literal ID="ltProjectCount" runat="server"></asp:Literal>
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
        <asp:Panel ID="pnlExpenseSummaryReport" runat="server" CssClass="tab-pane">
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
            <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
                <HeaderTemplate>
                    <div style="max-height: 400px; overflow: auto;">
                        <table id="tblExpenseSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth ">
                            <thead>
                                <tr runat="server" id="lvHeader">
                                    <th class="ProjectColoum no-wrap">
                                        Project
                                    </th>
                                    <th class="Width110Px no-wrap">
                                        Division
                                    </th>
                                    <th class="Width110Px no-wrap">
                                        Practice Area
                                    </th>
                                    <th class="Width170PxImp">
                                        Executive Incharge
                                    </th>
                                    <th class="Width170PxImp">
                                        Project Manager
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="ReportItemTemplate" runat="server" id="lvItem">
                        <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.ProjectNumber")%>'>
                            <table class="TdLevelNoBorder PeronSummaryReport">
                                <tr>
                                    <td class="FirstTd">
                                        <%# Eval("Project.Client.HtmlEncodedName")%>
                                        >
                                        <%# Eval("Project.Group.HtmlEncodedName")%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SecondTd">
                                        <asp:LinkButton ID="lnkProject" AccountId='<%# Eval("Project.Client.Id")%>' GroupId='<%# Eval("Project.Group.Id")%>'
                                            ClientName=' <%# Eval("Project.Client.HtmlEncodedName")%>' GroupName=' <%# Eval("Project.Group.HtmlEncodedName")%>'
                                            ProjectId='<%# Eval("Project.Id") %>' ProjectNumber='<%# Eval("Project.ProjectNumber")%>'
                                            runat="server" ToolTip='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'
                                            OnClick="lnkProject_OnClick" Text='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="no-wrap">
                            <%# Eval("Project.Division.Name")%>
                        </td>
                        <td>
                            <%# Eval("Project.Practice.Name")%>
                        </td>
                        <td>
                            <%# Eval("Project.ExecutiveInChargeName")%>
                        </td>
                        <td>
                            <%# Eval("Project.ProjectManagerNames")%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="ReportItemTemplate bgGroupByProjectHeader" runat="server" id="lvItem">
                        <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.ProjectNumber")%>'>
                            <table class="TdLevelNoBorder PeronSummaryReport">
                                <tr>
                                    <td class="FirstTd">
                                        <%# Eval("Project.Client.HtmlEncodedName")%>
                                        >
                                        <%# Eval("Project.Group.HtmlEncodedName")%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SecondTd">
                                        <asp:LinkButton ID="lnkProject" AccountId='<%# Eval("Project.Client.Id")%>' GroupId='<%# Eval("Project.Group.Id")%>'
                                            ClientName=' <%# Eval("Project.Client.HtmlEncodedName")%>' GroupName=' <%# Eval("Project.Group.HtmlEncodedName")%>'
                                            ProjectId='<%# Eval("Project.Id") %>' ProjectNumber='<%# Eval("Project.ProjectNumber")%>'
                                            runat="server" ToolTip='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'
                                            OnClick="lnkProject_OnClick" Text='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="no-wrap">
                            <%# Eval("Project.Division.Name")%>
                        </td>
                        <td>
                            <%# Eval("Project.Practice.Name")%>
                        </td>
                        <td>
                            <%# Eval("Project.ExecutiveInChargeName")%>
                        </td>
                        <td>
                            <%# Eval("Project.ProjectManagerNames")%>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody></table></div>
                </FooterTemplate>
            </asp:Repeater>
            <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
                There are no Expenses for the selected Filters.
            </div>
        </asp:Panel>
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
        <asp:Panel ID="pnlProjectSummaryDetailsTab" runat="server" CssClass="tab-pane">
            <uc:ExpenseSummaryDetails ID="ucExpenseSummaryDetails" runat="server" IsByProject="true" />
        </asp:Panel>
    </asp:View>
</asp:MultiView>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeProjectDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelProjectDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlProjectDetailReport"
    DropShadow="false" />
<asp:Panel ID="pnlProjectDetailReport" class="" Style="display: none; max-width: 75%;"
    runat="server" CssClass="TimePeriodByProject_ProjectDetailReport">
    <table class="WholeWidth Padding5">
        <tr>
            <td class="WholeWidth">
                <table class="WholeWidthWithHeight">
                    <tr class="bgColor_F5FAFF">
                        <td class="TimePeriodByProject_ProjectName">
                            <asp:HyperLink ID="lnkProjectName" runat="server" CssClass="Blink" />
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <div class="TimePeriodByProject_ProjectDetailsDiv">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="Width97Percent">
                                            <uc:ExpenseDetailsByProject ID="ucExpenseDetailsByProject" runat="server" Visible="false"
                                                IsExpensetypeDetails="false" IsProjectExpenseDetails="true" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr class="bgColor_F5FAFF">
                        <td class="textRight Padding3PX">
                            <asp:Button ID="btnCancelProjectDetailReport" Text="Close" ToolTip="Close" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

