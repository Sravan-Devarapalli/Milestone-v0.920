<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Persons.aspx.cs" Inherits="PraticeManagement.Config.Persons" %>

<%@ PreviousPageType VirtualPath="~/DashBoard.aspx" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Persons/PersonSummary.ascx" TagName="PersonSummary" TagPrefix="PS" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Persons | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Persons
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<PS:PersonSummary ID="projectSummary" runat="server" />
</asp:Content>

