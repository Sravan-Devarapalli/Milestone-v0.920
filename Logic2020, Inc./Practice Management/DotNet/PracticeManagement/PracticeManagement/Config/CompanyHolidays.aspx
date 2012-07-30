<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="CompanyHolidays.aspx.cs" Inherits="PraticeManagement.Config.CompanyHolidays" %>
    <%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Calendar.ascx" TagName="PMCalendar" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Company Holidays | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Company Holidays
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:PMCalendar ID="calendar" runat="server"  />
</asp:Content>

