<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CommissionsAndRates.aspx.cs" 
    Inherits="PraticeManagement.CommissionsAndRates"  MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/CommissionsAndRates.ascx" TagPrefix="uc"
    TagName="CommissionsAndRates" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports - Commissions and Rates</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Commissions and Rates
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:CommissionsAndRates ID="repComRates" runat="server" />
</asp:Content>
