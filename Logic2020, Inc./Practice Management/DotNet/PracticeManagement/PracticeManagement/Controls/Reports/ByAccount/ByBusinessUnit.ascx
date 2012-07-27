<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByBusinessUnit.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.ByBusinessUnit" %>
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
    <asp:Panel ID="pnlFilterBusinessUnit" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repBusinessUnit" runat="server" OnItemDataBound="repBusinessUnit_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th class="ResourceColum">
                                Business Unit
                                <img alt="Filter" title="Filter" src="../../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgBusinessUnitFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnitFilter" runat="server"
                                    TargetControlID="imgBusinessUnitFilter" BehaviorID="pceBusinessUnitFilter" PopupControlID="pnlFilterBusinessUnit"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width130px">
                                # of Projects
                            </th>
                            <th class="Width100Px">
                                Billable
                            </th>
                            <th class="Width100Px">
                                Non-Billable
                            </th>
                            <th class="Width100Px">
                                BD
                            </th>
                            <th class="Width100Px">
                                Total
                            </th>
                            <th class="Width295Px">
                                Percent of Total Hours
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5 textLeft">
                    <%# Eval("BusinessUnit.HtmlEncodedName")%>
                    (<%# ((Boolean)Eval("BusinessUnit.IsActive")) ? "Active" : "Inactive"%>)
                </td>
                <td>
                    <%# Eval("ProjectsCount")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BusinessDevelopmentHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td sorttable_customkey='<%# Eval("BusinessUnitTotalHoursPercent")%>'>
                    <table class="TdLevelNoBorder UtlizationGraph">
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="GraphTd">
                                <table>
                                    <tr>
                                        <td class="FirstTd" width="<%# Eval("BusinessUnitTotalHoursPercent")%>%">
                                        </td>
                                        <td class="SecondTd" width="<%# 100 - ((int)Eval("BusinessUnitTotalHoursPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="GraphValueTd">
                                <%# Eval("BusinessUnitTotalHoursPercent")%>%
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
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>

