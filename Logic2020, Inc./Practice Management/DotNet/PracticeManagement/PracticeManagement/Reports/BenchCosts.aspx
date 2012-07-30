<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BenchCosts.aspx.cs" Inherits="PraticeManagement.BenchCosts"
    MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/BenchCosts.ascx" TagPrefix="uc" TagName="BenchCosts" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Bench Costs | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Bench Costs
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:BenchCosts ID="repBenchCosts" runat="server" />
</asp:Content>

