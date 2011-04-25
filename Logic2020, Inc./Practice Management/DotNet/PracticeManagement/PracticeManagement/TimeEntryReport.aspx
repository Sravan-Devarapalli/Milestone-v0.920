<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryReport.aspx.cs" Inherits="PraticeManagement.TimeEntryReport" MasterPageFile="~/PracticeManagement.Master"
    Title="Practice Management - Time Entry Report" %>

<%@ Register Src="~/Controls/TimeEntry/TimeEntryManagement.ascx" TagPrefix="uc" TagName="TeManage" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Time Entry Report</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Time Entry Report
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:TeManage ID="teManage" runat="server" />
</asp:Content>
