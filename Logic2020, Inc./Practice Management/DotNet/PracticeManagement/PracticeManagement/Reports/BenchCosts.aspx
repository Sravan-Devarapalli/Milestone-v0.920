<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BenchCosts.aspx.cs" 
    Inherits="PraticeManagement.BenchCosts"   MasterPageFile="~/PracticeManagementMain.Master" %>

<%@ Register Src="~/Controls/Reports/BenchCosts.ascx" TagPrefix="uc"
    TagName="BenchCosts" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports - BenchC osts</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Bench Costs
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <uc:BenchCosts ID="repBenchCosts" runat="server" />
</asp:Content>
