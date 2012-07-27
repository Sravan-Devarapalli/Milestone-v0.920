<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByProject.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ByAccount.ByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
            </td>
            <td class="textRight Width10Percent padRight5">
                <table class="textRight WholeWidth">
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
    <asp:Panel ID="pnlFilterResource" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlBilling" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBilling" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblAccountSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="ProjectColoum">
                                Project
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBusinessUnitFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnit" runat="server" TargetControlID="imgBusinessUnitFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width110Px">
                                Status
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width110Px">
                                Billing
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBilling" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBilling" runat="server" TargetControlID="imgBilling"
                                    PopupControlID="pnlBilling" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width100Px">
                                Billable
                            </th>
                            <th class="Width100Px">
                                Non-Billable
                            </th>
                            <th class="Width100Px">
                                Total
                            </th>
                            <th class="Width325Px">
                                Project Variance (in Hours)
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'>
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
                                <%# Eval("Project.ProjectNumber")%>
                                -
                                <%# Eval("Project.HtmlEncodedName")%>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="textCenter" sorttable_customkey='<%# Eval("Project.Status.Name") %><%#Eval("Project.ProjectNumber")%>'>
                    <%# Eval("Project.Status.Name")%>
                </td>
                <td>
                    <%# Eval("BillingType")%>
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
                <td sorttable_customkey='<%# GetVarianceSortValue((string)Eval("Variance"))%>'>
                    <table class="WholeWidth TdLevelNoBorder">
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width70Per textRight">
                                <table class="WholeWidth">
                                    <tr class="border1Px">
                                        <td class="Width50Percent borderRightImp">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td style="<%# Eval("BillableFirstHalfHtmlStyle")%>">
                                                    </td>
                                                    <td style="<%# Eval("BillableSecondHalfHtmlStyle")%>">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width50Percent borderLeft">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td style="<%# Eval("ForecastedFirstHalfHtmlStyle")%>">
                                                    </td>
                                                    <td style="<%# Eval("ForecastedSecondHalfHtmlStyle")%>">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width20Percent">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="TimePeriodByProjectVariance">
                                            <%# Eval("Variance")%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width5Percent">
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
    <br />
    <div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>

