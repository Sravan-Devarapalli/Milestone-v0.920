<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="PageNotFound.aspx.cs" Inherits="PraticeManagement.PageNotFound" Title="Page not found" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Page not found</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	Error 404: Page Not Found
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
	<h2>The requested resource was not found.</h2>
	Please verify an URL you have entered.
</asp:Content>

