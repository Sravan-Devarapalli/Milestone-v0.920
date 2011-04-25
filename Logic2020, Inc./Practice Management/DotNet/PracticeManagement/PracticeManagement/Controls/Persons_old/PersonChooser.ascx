<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonChooser.ascx.cs" Inherits="PraticeManagement.Controls.Persons.PersonChooser" %>
<asp:Label ID="lblTip" runat="server" Text="Select person: "></asp:Label>
<asp:DropDownList ID="ddlPersons" runat="server" AutoPostBack="True"
    onselectedindexchanged="ddlPersons_SelectedIndexChanged">
</asp:DropDownList>
