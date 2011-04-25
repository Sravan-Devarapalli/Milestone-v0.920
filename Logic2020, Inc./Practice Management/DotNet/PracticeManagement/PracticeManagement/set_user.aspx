<%@ Page MasterPageFile="~/PracticeManagementMain.Master" Language="C#" AutoEventWireup="true" CodeBehind="set_user.aspx.cs" Inherits="PraticeManagement.set_user" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    The page is for development purpouse only
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
        <asp:DropDownList ID="ddlUsers" runat="server">
        </asp:DropDownList>
        <asp:Button ID="btnSetUser" runat="server" Text="Become" OnClick="btnSetUser_Click" /> 
        <asp:Label ID="lblBecameUser" runat="server" ForeColor="Green"></asp:Label>        
        <p>
        Go to:
        <asp:Menu ID="menu" runat="server" DataSourceID="odsSiteMap" StaticDisplayLevels="3" />
        </p>
        <asp:SiteMapDataSource ID="odsSiteMap" runat="server" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="footer">
    <div class="version">
        Version.
        <asp:Label ID="lblCurrentVersion" runat="server"></asp:Label></div>
</asp:Content>
