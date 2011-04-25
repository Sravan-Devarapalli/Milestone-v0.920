<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonthPicker.ascx.cs" Inherits="PraticeManagement.Controls.MonthPicker" %>

<div style="white-space: nowrap">
<asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="true" 
		onselectedindexchanged="ddlMonth_SelectedIndexChanged">
	<asp:ListItem Text="Jan" Value="1"></asp:ListItem>
	<asp:ListItem Text="Feb" Value="2"></asp:ListItem>
	<asp:ListItem Text="Mar" Value="3"></asp:ListItem>
	<asp:ListItem Text="Apr" Value="4"></asp:ListItem>
	<asp:ListItem Text="May" Value="5"></asp:ListItem>
	<asp:ListItem Text="Jun" Value="6"></asp:ListItem>
	<asp:ListItem Text="Jul" Value="7"></asp:ListItem>
	<asp:ListItem Text="Aug" Value="8"></asp:ListItem>
	<asp:ListItem Text="Sep" Value="9"></asp:ListItem>
	<asp:ListItem Text="Oct" Value="10"></asp:ListItem>
	<asp:ListItem Text="Nov" Value="11"></asp:ListItem>
	<asp:ListItem Text="Dec" Value="12"></asp:ListItem>
</asp:DropDownList>

<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="true" 
		onselectedindexchanged="ddlYear_SelectedIndexChanged">
</asp:DropDownList>
</div>
