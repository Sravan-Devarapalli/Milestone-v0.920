<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByProject" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;"
    id="tbHeader" runat="server">
    <tr>
        <td style="font-size: 16px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; padding-bottom: 10px;">
                        <asp:Literal ID="ltProjectCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; vertical-align: bottom;">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 470px; padding-bottom: 10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 27%;">
                        <table width="100%">
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
                    <td style="width: 27%;">
                        <table width="100%">
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
                    <td style="width: 27%; vertical-align: bottom;">
                        <table width="100%">
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
    <asp:Repeater ID="repProject" runat="server">
        <HeaderTemplate>
            <table id="tblProjectSummaryByProject" class="tablesorter PersonSummaryReport WholeWidth">
                <thead>
                    <tr>
                        <th class="t-left padLeft5" style="width: 500px;height:30px;">
                            Project
                        </th>
                        <th style="width: 110px;height:30px;">
                            Status
                        </th>
                        <th style="width: 110px;height:30px;">
                            Billing
                        </th>
                        <th style="width: 100px;height:30px;">
                            Billable
                        </th>
                        <th style="width: 100px;height:30px;">
                            Non-Billable
                        </th>
                        <th style="width: 100px;height:30px;">
                            Total
                        </th>
                        <th style="width: 325px;height:30px;">
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
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>
    <br />
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>

