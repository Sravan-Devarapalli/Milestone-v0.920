<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="CompanyHolidays.aspx.cs" Inherits="PraticeManagement.Config.CompanyHolidays" %>

<%@ Register Src="~/Controls/Calendar.ascx" TagName="PMCalendar" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Company Holidays</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Company Holidays
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:PMCalendar ID="calendar" runat="server" CompanyHolidays="true" />
</asp:Content>

