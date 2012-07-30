<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtilizationTimeline.aspx.cs"
    Inherits="PraticeManagement.Reporting.UtilizationTimeline" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/ConsultantsWeeklyReport.ascx" TagPrefix="uc"
    TagName="ConsultantsWeeklyReport" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Utilization | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Consulting Utilization
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:ConsultantsWeeklyReport ID="repWeekly" runat="server" />
</asp:Content>

