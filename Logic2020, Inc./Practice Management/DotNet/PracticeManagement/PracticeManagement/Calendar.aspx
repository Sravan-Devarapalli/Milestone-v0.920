<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="PraticeManagement.Calendar" 
MasterPageFile="~/PracticeManagementMain.Master" Title="Company Calendar" %>

<%@ Register Src="Controls/Calendar.ascx" TagName="Calendar" TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<%@ Register src="Controls/MonthCalendar.ascx" tagname="MonthCalendar" tagprefix="uc1" %>
<%@ Register src="Controls/CalendarLegend.ascx" tagname="CalendarLegend" tagprefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Practice Management - Calendar</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	<asp:UpdatePanel ID="pnlHeader" runat="server">
		<ContentTemplate>
			Calendar&nbsp;-&nbsp;<asp:Label ID="lblCalendarOwnerName" runat="server" Text="Whole Company" EnableViewState="false"></asp:Label>
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:Calendar ID="calendar" runat="server" CompanyHolidays="false" />
</asp:Content>

