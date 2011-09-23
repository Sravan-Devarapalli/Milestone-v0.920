<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekSelector.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.WeekSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
                <asp:HyperLink ID="hprlnkPreviousWeek" runat="server" Width="30" onclick="return checkDirtyWithRedirect(this.href)">
        <img alt="Previous" src="Images/previous.gif" />
                </asp:HyperLink>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <span style="font-size: xx-small; color: navy;">Week of</span>
                            <asp:TextBox ID="txtDate" runat="server" AutoPostBack="true" Width="102px" Style="visibility: hidden;
                                margin-left: -40px;" onchange="confirmSaveDirtyTimeEntry()" OnTextChanged="txtDate_TextChanged"></asp:TextBox>
                            <asp:HyperLink ID="lnkCalendar" runat="server" ImageUrl="~/Images/calendar.gif" NavigateUrl="#"></asp:HyperLink>
                            <ajaxToolkit:CalendarExtender ID="txtDate_CalendarExtender" runat="server" TargetControlID="txtDate"
                                PopupButtonID="lnkCalendar">
                            </ajaxToolkit:CalendarExtender>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="lblWeek" runat="server" EnableViewState="False" Font-Size="Large"
                                Font-Bold="True" ForeColor="Navy" />
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:HyperLink ID="hprlnkNextWeek" runat="server" Width="30" onclick="return checkDirtyWithRedirect(this.href)">
        <img alt="Next" src="Images/next.gif" />
                </asp:HyperLink>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlPopupCalendar" runat="server" class="calendarWrapper">
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

