<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditByPerson.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.AuditByPerson" %>
<asp:Repeater ID="repPersons" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="WholeWidthWithHeight">
            <tr style="text-align: left;">
                <td colspan="4" class="ProjectAccountName" style="width: 95%; white-space: nowrap;
                    font-weight: bold;">
                    <%# Eval("Person.PersonLastFirstName")%>
                    (<%# Eval("Person.Status.Name")%>,<%# Eval("Person.CurrentPay.TimescaleName")%>)
                </td>
                <td style="width: 5%; font-weight: bolder; font-size: 15px; text-align: right; padding-right: 10px;">
                    <%# GetDoubleFormat((double)Eval("NetChange"))%>
                </td>
            </tr>
        </table>
        <asp:Repeater ID="repPersonTimeEntriesHistory" DataSource='<%# Eval("TimeEntryRecords") %>'
            runat="server">
            <HeaderTemplate>
                <table class="WholeWidthWithHeight">
                    <tr>
                        <td>
                            Affected Date
                        </td>
                        <td>
                            Modified Date
                        </td>
                        <td>
                            Project-Project Name
                        </td>
                        <td>
                            Work Type
                        </td>
                        <td>
                            Original Hours
                        </td>
                        <td>
                            New Hours
                        </td>
                        <td>
                            Net Change
                        </td>
                        <td>
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr style="text-align: left; background-color: #D4D0C9;">
                    <td>
                        <%# Eval("MilestoneDate")%>
                    </td>
                    <td>
                        <%# Eval("ModifiedDate")%>
                    </td>
                    <td>
                        <%# Eval("ChargeCode.Project.ProjectNumber")%>
                        -
                        <%# Eval("ChargeCode.Project.Name")%>
                    </td>
                    <td>
                        <%# Eval("ChargeCode.TimeType.Name")%>
                    </td>
                    <td>
                        <%# Eval("ActualHours")%>
                    </td>
                    <td>
                        <%# Eval("OldHours")%>
                    </td>
                    <td>
                        <%# Eval("NetChange")%>
                    </td>
                    <td>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr style="text-align: left; background-color: #ECE9D9;">
                    <td>
                        <%# Eval("MilestoneDate")%>
                    </td>
                    <td>
                        <%# Eval("ModifiedDate")%>
                    </td>
                    <td>
                        <%# Eval("ChargeCode.Project.ProjectNumber")%>
                        -
                        <%# Eval("ChargeCode.Project.Name")%>
                    </td>
                    <td>
                        <%# Eval("ChargeCode.TimeType.Name")%>
                    </td>
                    <td>
                        <%# Eval("ActualHours")%>
                    </td>
                    <td>
                        <%# Eval("OldHours")%>
                    </td>
                    <td>
                        <%# Eval("NetChange")%>
                    </td>
                    <td>
                    </td>
                </tr>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries by any Employee for the selected range.
</div>

