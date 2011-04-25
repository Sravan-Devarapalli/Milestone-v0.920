<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonChooser.ascx.cs" Inherits="PraticeManagement.Controls.Persons.PersonChooser" %>
<div style="white-space:nowrap; margin-left:10px;">
    <asp:Label ID="lblTip" runat="server" Text="Select a Person"/>&nbsp;
    <asp:DropDownList ID="ddlPersons" runat="server" AutoPostBack="True"
        onselectedindexchanged="ddlPersons_SelectedIndexChanged">
    </asp:DropDownList>
</div>
