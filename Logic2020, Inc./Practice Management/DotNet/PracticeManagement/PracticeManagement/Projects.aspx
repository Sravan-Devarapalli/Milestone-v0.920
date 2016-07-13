<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="PraticeManagement.Projects"
    Title="Projects Summary | Practice Management" MasterPageFile="~/PracticeManagementMain.Master"
    EnableEventValidation="false" EnableViewState="true" %>

<%@ Register Src="~/Controls/Projects/ProjectSummary.ascx" TagName="ProjectSummary"
    TagPrefix="PS" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Summary | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Summary
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <PS:ProjectSummary ID="projectSummary" runat="server" />
</asp:Content>

