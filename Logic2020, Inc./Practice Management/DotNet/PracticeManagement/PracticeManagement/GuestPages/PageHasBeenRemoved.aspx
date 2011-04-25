<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageHasBeenRemoved.aspx.cs" 
    Inherits="PraticeManagement.GuestPages.PageHasBeenRemoved" MasterPageFile="~/PracticeManagementMain.Master" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Error</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	Internal site error occurs
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<h2>Page does not exist or has been removed.</h2>
	<p>Please contact your administrator.</p>
</asp:Content>
