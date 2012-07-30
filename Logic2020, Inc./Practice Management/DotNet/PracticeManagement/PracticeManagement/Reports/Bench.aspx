<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bench.aspx.cs" Inherits="PraticeManagement.Reporting.Bench"
    MasterPageFile="~/PracticeManagementMain.Master" %>
    <%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/BenchReport.ascx" TagPrefix="uc" TagName="BenchReport" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Bench Report | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Bench Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:BenchReport ID="repBenchReport" runat="server" />
</asp:Content>

