<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bench.aspx.cs" Inherits="PraticeManagement.Reporting.Bench"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/BenchReport.ascx" TagPrefix="uc" TagName="BenchReport" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports - Bench Report</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Bench Report
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <uc:BenchReport ID="repBenchReport" runat="server" />
</asp:Content>

