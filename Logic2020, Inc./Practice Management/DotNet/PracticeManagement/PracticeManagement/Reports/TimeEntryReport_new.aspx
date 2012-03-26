<%@ Page Title="Reports" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="TimeEntryReport_new.aspx.cs" Inherits="PraticeManagement.Reporting.TimeEntryReport_new" %>
<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc" TagName="TimeEntryReportsHeader" %>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
 <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
</asp:Content>

