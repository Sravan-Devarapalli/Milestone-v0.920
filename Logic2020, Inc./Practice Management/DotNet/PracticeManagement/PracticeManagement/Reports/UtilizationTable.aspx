<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UtilizationTable.aspx.cs"
    Inherits="PraticeManagement.Reporting.UtilizationTable" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/ConsultantsReport.ascx" TagPrefix="uc" TagName="ConsultantsReport" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports - Utilization Table</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Utilization Table
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:ConsultantsReport ID="repConsultantsReport" runat="server" />
</asp:Content>

