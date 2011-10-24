<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtilizationTimeline.aspx.cs"
    Inherits="PraticeManagement.Reporting.UtilizationTimeline" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/ConsultantsWeeklyReport.ascx" TagPrefix="uc"
    TagName="ConsultantsWeeklyReport" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Utilization | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Consulting Utilization
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <uc:ConsultantsWeeklyReport ID="repWeekly" runat="server" />
</asp:Content>

