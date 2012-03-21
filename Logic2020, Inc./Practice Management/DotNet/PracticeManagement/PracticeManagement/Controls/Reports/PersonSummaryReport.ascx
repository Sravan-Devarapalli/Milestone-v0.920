<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonSummaryReport" %>
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
                            UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repSummary" runat="server">
    <HeaderTemplate>
        <table class="PersonSummaryReport" style="width: 100%;">
            <tr>
                <th style="text-align: left; width: 35%;">
                    Project Name
                </th>
                <th>
                    Billable
                </th>
                <th>
                    Value
                </th>
                <th style="width: 10%;">
                    Non-Billable
                </th>
                <th>
                    Total
                </th>
                <th style="width: 25%;">
                    Percent of Total Hours this Period
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr style="background-color: White;">
            <td style="text-align: left;">
                <table class="TdLevelNoBorder">
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px; text-align: left;">
                            <%# Eval("Client.Name") %>
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
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetCurrencyFormat((double)Eval("BillableValue"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td>
                <table class="TdLevelNoBorder" width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right; width: 65%">
                            <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("ProjectTotalHoursPercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# Eval("TotalHoursPercentExceptThisProject")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 15%; text-align: right; padding-left: 10px;">
                            <%# Eval("ProjectTotalHoursPercent")%>%
                        </td>
                        <td style="width: 10%;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr style="background-color: #f9faff;">
            <td style="text-align: left;">
                <table class="TdLevelNoBorder">
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px; text-align: left;">
                            <%# Eval("Client.Name") %>
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
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetCurrencyFormat((double)Eval("BillableValue"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td>
                <table class="TdLevelNoBorder" width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right; width: 65%">
                            <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("ProjectTotalHoursPercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# Eval("TotalHoursPercentExceptThisProject")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 15%; text-align: right; padding-left: 10px;">
                            <%# Eval("ProjectTotalHoursPercent")%>%
                        </td>
                        <td style="width: 10%;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    This person has not entered Time Entries for the selected period.
</div>

