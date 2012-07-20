<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonChooser.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonChooser" %>
<div class="no-wrap">
    <asp:Label ID="lblTip" runat="server" Text="" class="fontBold font14Px" />
    <asp:DropDownList ID="ddlPersons" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPersons_SelectedIndexChanged">
    </asp:DropDownList>
</div>

