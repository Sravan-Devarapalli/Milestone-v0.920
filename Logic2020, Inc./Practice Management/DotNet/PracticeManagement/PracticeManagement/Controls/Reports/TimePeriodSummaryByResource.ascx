<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
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
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick" Enabled="false"
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
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
                    <th style="width:18%;text-align:left;" class="padLeft5">
                        Resource
                    </th>
                    <th style="width:13%">
                        Seniority
                    </th>
                     <th style="width: 10%;">
                        Billable
                    </th>
                    <th style="width: 13%;">
                        Non-Billable
                    </th>
                    <th style="width: 10%;">
                        Total
                    </th>
                    <th style="width: 10%;">
                        Value
                    </th>
                    <th style="width: 26%;">
                        Utilization Percent this Period
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="padLeft5" style="text-align:left;" >
                <%# Eval("Person.PersonLastFirstName")%>
            </td>
            <td sorttable_customkey='<%# Eval("Person.Seniority.Name") %> <%#Eval("Person.PersonLastFirstName")%>'>
                <%# Eval("Person.Seniority.Name")%>
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
            <td sorttable_customkey='<%# GetBillableSortValue((double)Eval("BillableValue"), (bool)Eval("IsPersonNotAssignedToFixedProject"))%>' >
               <%# GetBillableValue((double)Eval("BillableValue"), (bool)Eval("IsPersonNotAssignedToFixedProject"))%>
            </td>
            <td sorttable_customkey='<%# Eval("Person.UtlizationPercent")%>'>
             <table class="TdLevelNoBorder" width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right; width: 65%">
                            <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("Person.UtlizationPercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# 100 - ((double)Eval("Person.UtlizationPercent") )%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 15%; text-align: right; padding-left: 10px;">
                            <%# Eval("Person.UtlizationPercent")%>%
                        </td>
                        <td style="width: 10%;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="ReportAlternateItemTemplate">
            <td class="padLeft5" style="text-align:left;">
                <%# Eval("Person.PersonLastFirstName")%>
            </td>
           <td sorttable_customkey='<%# Eval("Person.Seniority.Name") %> <%#Eval("Person.PersonLastFirstName")%>'>
                <%# Eval("Person.Seniority.Name")%>
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
            <td sorttable_customkey='<%# GetBillableSortValue((double)Eval("BillableValue"), (bool)Eval("IsPersonNotAssignedToFixedProject"))%>'>
               <%# GetBillableValue((double)Eval("BillableValue"), (bool)Eval("IsPersonNotAssignedToFixedProject"))%>
            </td>
            <td sorttable_customkey='<%# Eval("Person.UtlizationPercent")%>'>
              <table class="TdLevelNoBorder" width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right; width: 65%">
                            <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("Person.UtlizationPercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# 100 - ((double)Eval("Person.UtlizationPercent") )%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 15%; text-align: right; padding-left: 10px;">
                            <%# Eval("Person.UtlizationPercent")%>%
                        </td>
                        <td style="width: 10%;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries towards this range selected.
</div>

