<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryByResource" %>
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <table class="PersonSummaryReport WholeWidth">
            <thead>
                <tr>
                    <th>
                        Resource
                    </th>
                    <th>
                        Project Role
                    </th>
                    <th>
                        Billable
                    </th>
                    <th>
                        Non-Billable
                    </th>
                    <th>
                        Total
                    </th>
                    <th>
                        Value
                    </th>
                    <th>
                        Person Variance Percentage
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="padLeft5">
                <%# Eval("Person.PersonLastFirstName")%>
            </td>
            <td>
                <%# Eval("Person.ProjectRoleName")%>
            </td>
            <td>
                <%# Eval("BillabileHours")%>
            </td>
            <td>
                <%# Eval("NonBillableHours")%>
            </td>
            <td>
                <%# Eval("TotalHours")%>
            </td>
            <td>
                <%# Eval("BillableValue")%>
            </td>
            <td>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="ReportAlternateItemTemplate">
            <td class="padLeft5">
                <%# Eval("Person.PersonLastFirstName")%>
            </td>
            <td>
                <%# Eval("Person.ProjectRoleName")%>
            </td>
            <td>
                <%# Eval("BillabileHours")%>
            </td>
            <td>
                <%# Eval("NonBillableHours")%>
            </td>
            <td>
                <%# Eval("TotalHours")%>
            </td>
            <td>
                <%# Eval("BillableValue")%>
            </td>
            <td>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>

