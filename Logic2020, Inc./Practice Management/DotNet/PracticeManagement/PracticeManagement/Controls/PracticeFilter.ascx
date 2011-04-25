<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PracticeFilter.ascx.cs" Inherits="PraticeManagement.Controls.PracticeFilter" %>
<tr>
    <td>Active only</td>
    <td>
	    <asp:CheckBox ID="chbShowActiveOnly" runat="server" AutoPostBack="true" TextAlign="Left" Checked="true" oncheckedchanged="chbShowActiveOnly_CheckedChanged" />
    </td>
</tr>
<tr>
    <td>Practice Area</td>
    <td><asp:DropDownList ID="ddlFilter" runat="server" AutoPostBack="true" onselectedindexchanged="ddlFilter_SelectedIndexChanged" /></td>
</tr>
	
	
