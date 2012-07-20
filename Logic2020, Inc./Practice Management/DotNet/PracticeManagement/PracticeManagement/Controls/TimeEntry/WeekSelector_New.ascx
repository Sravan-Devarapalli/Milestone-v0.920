<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekSelector_New.ascx.cs" Inherits="PraticeManagement.Controls.TimeEntry.WeekSelector_New" %>
<script type="text/javascript">

    function confirmSaveDirtyTimeEntry() {
        var result = true;
        if (getDirty()) {
            result = confirm("Some data isn't saved. Click Ok to Save the data or Cancel to continue without saving.");
            if (!result) {
                clearDirty();
            }
        }
    }
</script>
<asp:Panel ID="pnlWeekContainer" runat="server">
    <table cellpadding="3" cellspacing="3">
        <tr>
            <td>
                <asp:ImageButton ID="imgbtnPrevWeek" runat="server" ImageUrl="~/Images/previous.gif" AlternateText="Previous" ToolTip="Previous"
                 OnClick="imgbtnPrevWeek_OnClick" OnClientClick="return checkDirtyWithRedirectInTimeEntry();" />
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <span class="WeekSelectorLable">Week of</span>
                            <asp:TextBox ID="txtDate" runat="server" AutoPostBack="true" class="WeekSelectorHiddenTextBox" onchange="confirmSaveDirtyTimeEntry()" OnTextChanged="txtDate_TextChanged"></asp:TextBox>
                            <asp:HyperLink ID="lnkCalendar" runat="server" ImageUrl="~/Images/calendar.gif" NavigateUrl="#"></asp:HyperLink>
                            <AjaxControlToolkit:CalendarExtender ID="txtDate_CalendarExtender" runat="server" TargetControlID="txtDate"
                                PopupButtonID="lnkCalendar">
                            </AjaxControlToolkit:CalendarExtender>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="lblWeek" runat="server" EnableViewState="False" CssClass="WeekSelectorDateSpan" />
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:ImageButton ID="imgbtnNextWeek" runat="server" ImageUrl="~/Images/next.gif" AlternateText="Next" ToolTip="Next"
                 OnClick="imgbtnNextWeek_OnClick" OnClientClick="return checkDirtyWithRedirectInTimeEntry();" />
                <asp:HiddenField ID="hdPreviousWeekSelectedDay" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlPopupCalendar" runat="server" class="calendarWrapper" style="display:none;">
    <asp:Calendar ID="calendar" runat="server" SelectionMode="Day" OnSelectionChanged="calendar_SelectionChanged"
        OnVisibleMonthChanged="calendar_OnVisibleMonthChanged">
        <DayHeaderStyle CssClass="calendarDayHeader" />
        <DayStyle CssClass="calendarDay" />
        <NextPrevStyle CssClass="calendarNextPrev" />
        <OtherMonthDayStyle CssClass="calendarOtherMonth" />
        <SelectedDayStyle CssClass="calendarSelectedDay" />
        <SelectorStyle CssClass="calendarSelector" />
        <TitleStyle CssClass="calendarTitle" />
        <TodayDayStyle CssClass="calendarTodayDay" />
        <WeekendDayStyle CssClass="calendarWeekendDay" />
    </asp:Calendar>
</asp:Panel>
<AjaxControlToolkit:PopupControlExtender ID="popupExcalendar" runat="server" TargetControlID="lblWeek"
    PopupControlID="pnlPopupCalendar" Position="Bottom" OffsetX="0" OffsetY="0" />

