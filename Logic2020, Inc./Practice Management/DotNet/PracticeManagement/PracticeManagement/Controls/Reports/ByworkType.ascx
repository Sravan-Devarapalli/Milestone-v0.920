﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByworkType.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByworkType" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;
    height: 115px;" id="tbHeader" runat="server">
    <tr>
        <td style="font-size: 14px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; color: Gray; padding-bottom: 5px;">
                        <asp:Literal ID="ltrlAccount" runat="server"></asp:Literal>
                        >
                        <asp:Literal ID="ltrlBusinessUnit" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectNumber" runat="server"></asp:Literal>-
                        <asp:Literal ID="ltrlProjectName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectStatusAndBillingType" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 470px; padding-bottom: 10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 27%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Projected Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlProjectedHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Total Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%; vertical-align: bottom; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td>
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px;">
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
                    <td style="vertical-align: bottom; width: 8%; padding: 0px !important;">
                        <table width="100%">
                            <tr>
                                <td style="padding: 0px !important;">
                                    <table width="100%" style="table-layout: fixed;">
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                            <td style="background-color: #7FD13B; border: 1px solid Gray;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 8%; padding: 0px;">
                        <table width="100%">
                            <tr>
                                <td style="padding: 0px !important;">
                                    <table width="100%" style="table-layout: fixed;">
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                            <td style="background-color: #BEBEBE; border: 1px solid Gray;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 2%;">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
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
    <asp:Panel ID="pnlFilterCategory" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblCategory" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repWorkType" runat="server" OnItemDataBound="repWorkType_ItemDataBound">
        <HeaderTemplate>
            <div style="min-height: 250px;">
                <table id="tblProjectSummaryByWorkType" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th style="width: 210px; text-align: left;" class="padLeft5">
                                WorkType
                            </th>
                            <th style="width: 130px;">
                                Category
                                <img alt="Filter" src="../../Images/search_filter.png" runat="server" id="imgCategoryFilter"
                                    style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceCategory" runat="server" TargetControlID="imgCategoryFilter"
                                    PopupControlID="pnlFilterCategory" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 100px">
                                Billable
                            </th>
                            <th style="width: 100px;">
                                Non-Billable
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
                    <%# Eval("WorkType.Name")%>
                </td>
                <td class="t-center padLeft5">
                    <%# Eval("WorkType.Category")%>
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
                <td sorttable_customkey='<%# Eval("WorkTypeTotalHoursPercent")%>'>
                    <table class="TdLevelNoBorder" width="100%">
                        <tr>
                            <td style="width: 5%">
                            </td>
                            <td style="text-align: right; width: 70%">
                                <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                    <tr>
                                        <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("WorkTypeTotalHoursPercent")%>%">
                                        </td>
                                        <td style="background-color: White; height: 18px;" width="<%# 100 - ((int)Eval("WorkTypeTotalHoursPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 20%; text-align: left; padding-left: 10px;">
                                <%# Eval("WorkTypeTotalHoursPercent")%>%
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
        There are no Time Entries by any Employee for the selected range.
    </div>
</div>

