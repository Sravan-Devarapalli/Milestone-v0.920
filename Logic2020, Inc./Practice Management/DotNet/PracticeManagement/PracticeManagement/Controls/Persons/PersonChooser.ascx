<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonChooser.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonChooser" %>
<div style="white-space: nowrap;">
    <asp:Label ID="lblTip" runat="server" Text="" style="font-weight:bold; font-size:14px;" />
    <asp:DropDownList ID="ddlPersons" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPersons_SelectedIndexChanged">
    </asp:DropDownList>
</div>

