<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom:5px !important;" id="tbHeader" runat="server">
    <tr>
        <td style="font-size: 16px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; padding-bottom:10px;">
                        <asp:Literal ID="ltPersonCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; vertical-align: bottom;">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 610px; padding-bottom:10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 7%;">
                        <table>
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Total Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 7%;">
                        <table>
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Avg Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlAvgHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 7%;">
                        <table>
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Avg Utilization
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlAvgUtilization" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 7%; vertical-align: bottom;">
                        <table>
                            <tr>
                                <td style="text-align: center;">
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px; text-align: center;">
                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center;">
                                    NON-BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center;">
                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 2%; padding: 0px !important;">
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
                    <td style="vertical-align: bottom; width: 2%; padding: 0px;">
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
    <asp:Repeater ID="repResource" runat="server">
        <HeaderTemplate>
            <table id="tblTimePeriodSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth">
                <thead>
                    <tr>
                        <th style="width: 210px; text-align: left;" class="padLeft5">
                            Resource
                        </th>
                        <th style="width: 130px;">
                            Seniority
                        </th>
                        <th style="width: 110px;">
                            Pay Type
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
                            Internal
                        </th>
                        <th style="width: 100px;">
                            Time-Off
                        </th>
                        <th style="width: 100px;">
                            Total
                        </th>
                        <th style="width: 295px;">
                            Utilization Percent this Period
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5" style="text-align: left;">
                    <%# Eval("Person.PersonLastFirstName")%>
                </td>
                <td sorttable_customkey='<%# Eval("Person.Seniority.Name") %> <%#Eval("Person.PersonLastFirstName")%>'>
                    <%# Eval("Person.Seniority.Name")%>
                </td>
                <td sorttable_customkey='<%# GetPayTypeSortValue((string)Eval("Person.CurrentPay.TimescaleName"),(string)Eval("Person.PersonLastFirstName"))%>'>
                    <%# Eval("Person.CurrentPay.TimescaleName")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BusinessDevelopmentHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("InternalHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("AdminstrativeHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td sorttable_customkey='<%# Eval("Person.UtlizationPercent")%>'>
                    <table class="TdLevelNoBorder" width="100%">
                        <tr>
                            <td style="width: 5%">
                            </td>
                            <td style="text-align: right; width: 70%">
                                <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                    <tr>
                                        <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("Person.UtlizationPercent")%>%">
                                        </td>
                                        <td style="background-color: White; height: 18px;" width="<%# 100 - ((double)Eval("Person.UtlizationPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 20%; text-align: left; padding-left: 10px;">
                                <%# Eval("Person.UtlizationPercent")%>%
                            </td>
                            <td style="width: 5%;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Time Entries by any Employee for the selected range.
    </div>
</div>

