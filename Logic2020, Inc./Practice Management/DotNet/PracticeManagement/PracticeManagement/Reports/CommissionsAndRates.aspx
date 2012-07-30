<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAndRates.aspx.cs"
    Inherits="PraticeManagement.CommissionsAndRates" MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/CommissionsAndRates.ascx" TagPrefix="uc" TagName="CommissionsAndRates" %>
<asp:Content ID="ctrlhead" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='<%# Generic.GetClientUrl("~/Scripts/Cookie.min.js", this) %>'></script>
</asp:Content>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Commissions and Rates | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Commissions and Rates
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:CommissionsAndRates ID="repComRates" runat="server" />
</asp:Content>

