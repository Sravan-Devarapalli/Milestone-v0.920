<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="LoggedOut.aspx.cs" Inherits="PraticeManagement.LoggedOut" %>

<%@ OutputCache Duration="1" Location="None" NoStore="true" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Logged Out</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    logged off due to inactivity
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    You have been logged off due to inactivity for security reasons. Please click <a
        href="Login.aspx">here</a> to log back in.
</asp:Content>

