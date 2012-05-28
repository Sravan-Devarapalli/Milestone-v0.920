<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByProject.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ByAccount.ByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" style="width: 90%;">
            </td>
            <td style="text-align: right; width: 10%; padding-right: 5px;">
                <table width="100%" style="text-align: right;">
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
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" Height="125px" />
    </asp:Panel>
    <asp:Panel ID="pnlBilling" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBilling" runat="server" Height="125px" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
        <HeaderTemplate>
            <div style="min-height: 250px;">
                <table id="tblAccountSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="t-left padLeft5" style="width: 500px; height: 20px;">
                                Project
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBusinessUnitFilter" style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnit" runat="server" TargetControlID="imgBusinessUnitFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 110px; height: 20px;">
                                Status
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 110px; height: 20px;">
                                Billing
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBilling" style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBilling" runat="server"
                                    TargetControlID="imgBilling" PopupControlID="pnlBilling"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Billable
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Non-Billable
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Total
                            </th>
                            <th style="width: 325px; height: 20px;">
                                Project Variance (in Hours)
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'>
                    <table class="TdLevelNoBorder">
                        <tr>
                            <td style="color: Gray; padding-bottom: 3px; padding-left: 2px; text-align: left;">
                                <%# Eval("Project.Client.Name")%>
                                >
                                <%# Eval("Project.Group.Name")%>
                            </td>
                        </tr>
                        <tr>
                            <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px; text-align: left;">
                                <%# Eval("Project.ProjectNumber")%>
                                -
                                <%# Eval("Project.Name")%>
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
                    <table class="WholeWidth  TdLevelNoBorder">
                        <tr>
                            <td style="width: 5%;">
                            </td>
                            <td style="width: 70%; text-align: right;">
                                <table class="WholeWidth">
                                    <tr style="border: 1px solid black;">
                                        <td style="width: 50%; border-right: 1px solid black;">
                                            <table width="100%">
                                                <tr>
                                                    <td style="<%# Eval("BillableFirstHalfHtmlStyle")%>">
                                                    </td>
                                                    <td style="<%# Eval("BillableSecondHalfHtmlStyle")%>">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 50%; border-left: 1px solid black;">
                                            <table width="100%">
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
                            <td style="width: 20%;">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="text-align: right; padding-right: 3px;">
                                            <%# Eval("Variance")%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 5%;">
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
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>

