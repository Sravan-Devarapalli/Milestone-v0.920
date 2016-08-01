<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="PraticeManagement.Projects"
    Title="Projects Summary | Practice Management" MasterPageFile="~/PracticeManagementMain.Master"
    EnableEventValidation="false" EnableViewState="true" %>

<%@ Register Src="~/Controls/Projects/ProjectSummary.ascx" TagName="ProjectSummary"
    TagPrefix="PS" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Summary | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Summary
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <PS:ProjectSummary ID="projectSummary" runat="server" />
</asp:Content>

