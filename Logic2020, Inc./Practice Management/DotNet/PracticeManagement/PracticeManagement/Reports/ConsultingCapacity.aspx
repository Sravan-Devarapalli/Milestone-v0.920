<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="ConsultingCapacity.aspx.cs" Inherits="PraticeManagement.Reporting.ConsultingCapacity" %>

<%@ Register Src="~/Controls/Reports/ConsultantsWeeklyReport.ascx" TagPrefix="uc"
    TagName="ConsultantsWeeklyReport" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Capacity | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Consulting Capacity
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
<script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <uc:ConsultantsWeeklyReport ID="repWeekly" runat="server" IsCapacityMode="true"/>
</asp:Content>
