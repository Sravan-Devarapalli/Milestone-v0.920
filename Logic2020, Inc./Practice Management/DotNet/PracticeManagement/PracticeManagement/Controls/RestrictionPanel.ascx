<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RestrictionPanel.ascx.cs" Inherits="PraticeManagement.Controls.RestrictionPanel" %>
<asp:Panel ID="pnlRestrictionPanel" runat="server">
    <table cellpadding="5" class="border-collapse">
        <tr>
        <td><strong>Accounts</strong></td>
        <td><strong>Business Units</strong></td>
        <td><strong>Salespersons</strong></td>
        <td><strong>Project Access</strong></td>
        <td><strong>Practice Areas</strong></td>
        </tr>
        <tr>
            <td>
                <pmc:CascadingMsdd ID="msddClients" runat="server" TargetControlId="msddGroups" CssClass="PersonDetailPermissionsScrollingDropDown"/>
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddGroups" runat="server"  CssClass="PersonDetailPermissionsScrollingDropDown"/>
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddSalespersons" runat="server"  CssClass="PersonDetailPermissionsScrollingDropDown"/>
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddPracticeManagers" runat="server" CssClass="PersonDetailPermissionsScrollingDropDown"/>
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddPractices" runat="server" CssClass="PersonDetailPermissionsScrollingDropDown"/>
            </td>
        </tr>
    </table>
</asp:Panel>



