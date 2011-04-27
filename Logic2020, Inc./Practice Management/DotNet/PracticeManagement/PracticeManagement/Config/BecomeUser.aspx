<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="BecomeUser.aspx.cs" Inherits="PraticeManagement.Config.BecomeUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Become User</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Become User
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <div id="dvBecomeUser" runat="server" visible="false" style="display: block">
        <asp:DropDownList ID="ddlBecomeUserList" runat="server" Visible="false" />
        <asp:LinkButton ID="lbBecomeUserOk" runat="server" OnClick="lbBecomeUserOk_OnClick"
            Visible="false">Ok</asp:LinkButton>
    </div>
</asp:Content>

