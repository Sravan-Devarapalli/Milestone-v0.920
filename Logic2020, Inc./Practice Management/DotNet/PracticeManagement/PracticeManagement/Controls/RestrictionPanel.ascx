<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RestrictionPanel.ascx.cs" Inherits="PraticeManagement.Controls.RestrictionPanel" %>
<style type="text/css">
    .style1
    {
        border-collapse: collapse;
    }
    .tab-visible{
        display: block;
    }
	
    .tab-invisible{
        display: none;
    }
</style>
<asp:Panel ID="pnlRestrictionPanel" runat="server">
    <table cellpadding="5" class="style1">
        <tr>
        <td><strong>Clients</strong></td>
        <td><strong>Groups</strong></td>
        <td><strong>Salespersons</strong></td>
        <td><strong>Project owners</strong></td>
        <td><strong>Practice Areas</strong></td>
        </tr>
        <tr>
            <td>
                <pmc:CascadingMsdd ID="msddClients" runat="server" TargetControlId="msddGroups" 
                    Height="150" BorderColor="#eeeeee" Width="180" />
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddGroups" runat="server"  
                    Height="150" BorderColor="#eeeeee" Width="180" />
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddSalespersons" runat="server"  
                    Height="150" BorderColor="#eeeeee" Width="180" />
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddPracticeManagers" runat="server" 
                    Height="150" BorderColor="#eeeeee" Width="180" />
            </td>
            <td>
                <pmc:ScrollingDropDown ID="msddPractices" runat="server" 
                    Height="150" BorderColor="#eeeeee" Width="180" />
            </td>
        </tr>
    </table>
</asp:Panel>



