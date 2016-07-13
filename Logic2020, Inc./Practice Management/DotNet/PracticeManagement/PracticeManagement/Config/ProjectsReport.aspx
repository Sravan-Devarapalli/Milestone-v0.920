<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="ProjectsReport.aspx.cs" Inherits="PraticeManagement.Config.ProjectsReport" %>

<%@ Register Src="~/Controls/Projects/ProjectSummary.ascx" TagName="ProjectSummary" TagPrefix="PS" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Projects Summary Report | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Summary Report
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<PS:ProjectSummary ID="projectSummary" runat="server" />
</asp:Content>

