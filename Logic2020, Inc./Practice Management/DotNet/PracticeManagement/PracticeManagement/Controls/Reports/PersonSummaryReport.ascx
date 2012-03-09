<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonSummaryReport" %>
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
                        Total Percent of Total Hours this Period
                    </div>
                </th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <%# Eval("Client.Name") %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <%# Eval("BillableHours")%>
            </td>
            <td>
                <%# Eval("BillableValue") %>
            </td>
            <td>
                <%# Eval("NonBillableHours")%>
            </td>
            <td>
                <%# Eval("TotalHours")%>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <table style="border: 1px solid black; width: 240px; height: 20px;">
                                <tr>
                                    <td style="background-color: Green; height: 20px;" width="<%# Eval("BillablePercent")%>%">
                                    </td>
                                    <td style="background-color: White; height: 20px;" width="<%# Eval("NonBillablePercent")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <%# Eval("BillablePercent")%>
                            %
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>

