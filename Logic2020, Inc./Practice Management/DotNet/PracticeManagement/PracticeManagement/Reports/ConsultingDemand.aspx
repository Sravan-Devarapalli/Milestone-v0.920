<%@ Page Title="Consulting Demand | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ConsultingDemand.aspx.cs" Inherits="PraticeManagement.Reporting.ConsultingDemand" %>
    
<%@ Register Src="~/Controls/Reports/ConsultingDemand.ascx" TagName="ConsultingDemand"
    TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Demand | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Consulting Demand
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:ConsultingDemand ID="cdDemand" runat="server" DefaultMonths="4" />
</asp:Content>

