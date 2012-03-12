<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonSummaryReport" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" style="width: 85%;">
        </td>
        <td style="text-align: right; width: 15%; padding-right: 5px;">
            <table width="100%" style="text-align: right;">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExcel" runat="server" Text="Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnPDF" runat="server" Text="PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repSummary" runat="server">
    <HeaderTemplate>
        <table class="CompPerfTable WholeWidth">
            <tr class="CompPerfHeader">
                <th>
                    <div class="ie-bg">
                        Project Name
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Billable
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Value
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Non-Billable
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Total
                    </div>
                </th>
                <th>
                    <div class="ie-bg">
                        Billable Percent of Total Hours this Period
                    </div>
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr style="background-color: #f9faff;">
            <td>
                <table>
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px;">
                            <%# Eval("Client.Name") %>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px;">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("BillableValue"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td style="text-align: center;">
                <table width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right; width:67%">
                            <table style="border: 1px solid black; width: 240px; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("BillablePercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# Eval("NonBillablePercent")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="padding-left: 5px; text-align: right;width:13%">
                            <%# Eval("BillablePercent")%>
                            %
                        </td>
                        <td style="width: 10%">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr style="background-color: White; padding-bottom: 2px;">
            <td>
                <table>
                    <tr>
                        <td style="color: Gray; padding-bottom: 3px; padding-left: 2px;">
                            <%# Eval("Client.Name") %>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px;">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("BillableValue"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td style="text-align: center;">
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td style="text-align: center;">
                <table width="100%">
                    <tr>
                        <td style="width: 10%">
                        </td>
                        <td style="text-align: right;width:67%;">
                            <table style="border: 1px solid black; width: 240px; height: 18px; padding-left: 5px;">
                                <tr>
                                    <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("BillablePercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 18px;" width="<%# Eval("NonBillablePercent")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="padding-left: 5px; text-align: right;width:13%;">
                            <%# Eval("BillablePercent")%>
                            %
                        </td>
                        <td style="width: 10%">
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

