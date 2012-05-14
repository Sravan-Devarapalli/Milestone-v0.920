<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="PraticeManagement.Calendar"
    MasterPageFile="~/PracticeManagementMain.Master" Title="Calendar | Practice Management" %>

<%@ Register Src="~/Controls/PersonCalendar.ascx" TagName="Calendar" TagPrefix="uc" %>
<%@ Register Src="Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Calendar | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:Calendar ID="calendar" runat="server" />
    <br />
    <div class="TimeEntry_New_Legend">
        <uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
    </div>
    <br />
</asp:Content>

