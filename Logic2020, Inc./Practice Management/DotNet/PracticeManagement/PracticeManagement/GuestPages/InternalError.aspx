<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="InternalError.aspx.cs" Inherits="PraticeManagement.InternalError" Title="Error" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Error</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	Internal site error occurs
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<h2>Error occurred. Please contact your administrator.</h2>
</asp:Content>

