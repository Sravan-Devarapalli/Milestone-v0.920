<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BecomeUser.ascx.cs"
    Inherits="PraticeManagement.Controls.Generic.BecomeUser" %>
<div id="dvBecomeUser" runat="server" visible="false" style="display: block">
    <asp:DropDownList ID="ddlBecomeUserList" runat="server" Visible="false" />
    <asp:LinkButton ID="lbBecomeUserOk" runat="server" OnClick="lbBecomeUserOk_OnClick"
        Visible="false">Ok</asp:LinkButton>
</div>

