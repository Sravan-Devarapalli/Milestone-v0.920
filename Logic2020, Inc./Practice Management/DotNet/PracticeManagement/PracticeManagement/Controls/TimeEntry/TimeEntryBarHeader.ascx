<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryBarHeader.ascx.cs" Inherits="PraticeManagement.Controls.TimeEntry.TimeEntryBarHeader" %>
<th>
    <td>Project - Milestone</td>
    <td>Time Type</td>
    <asp:Repeater ID="repEntries" runat="server">
        <ItemTemplate>
            <td>
                <%# Eval("DayOfWeek") %>
            </td>
        </ItemTemplate>
    </asp:Repeater>    
</th>

