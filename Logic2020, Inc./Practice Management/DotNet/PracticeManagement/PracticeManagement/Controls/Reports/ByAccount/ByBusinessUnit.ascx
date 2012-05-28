<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByBusinessUnit.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.ByBusinessUnit" %>
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
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" UseSubmitBehavior="false"
                                ToolTip="Export To Excel" />
                        </td>
                        <td>
                            <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" Enabled="false" UseSubmitBehavior="false"
                                ToolTip="Export To PDF" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlFilterBusinessUnit" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repBusinessUnit" runat="server" OnItemDataBound="repBusinessUnit_ItemDataBound">
        <HeaderTemplate>
            <div style="min-height: 250px;">
                <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th style="width: 210px; text-align: left;" class="padLeft5">
                                Business Unit
                                <img alt="Filter" title="Filter" src="../../../Images/search_filter.png" style="position: absolute;
                                    padding-left: 2px;" runat="server" id="imgBusinessUnitFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnitFilter" runat="server"
                                    TargetControlID="imgBusinessUnitFilter" BehaviorID="pceBusinessUnitFilter" PopupControlID="pnlFilterBusinessUnit"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 130px;">
                                # of Projects
                            </th>
                            <th style="width: 100px">
                                Billable
                            </th>
                            <th style="width: 100px;">
                                Non-Billable
                            </th>
                            <th style="width: 100px;">
                                BD
                            </th>
                            <th style="width: 100px;">
                                Total
                            </th>
                            <th style="width: 295px;">
                                Percent of Total Hours
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5" style="text-align: left;">
                    <%# Eval("BusinessUnit.Name") %>(<%# ((Boolean)Eval("BusinessUnit.IsActive")) ? "Active" : "Inactive"%>)
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
                    <table class="TdLevelNoBorder" width="100%">
                        <tr>
                            <td style="width: 5%">
                            </td>
                            <td style="text-align: right; width: 70%">
                                <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                    <tr>
                                        <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("BusinessUnitTotalHoursPercent")%>%">
                                        </td>
                                        <td style="background-color: White; height: 18px;" width="<%# 100 - ((int)Eval("BusinessUnitTotalHoursPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 20%; text-align: left; padding-left: 10px;">
                                <%# Eval("BusinessUnitTotalHoursPercent")%>%
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
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Business Units for the selected range.
    </div>
</div>

