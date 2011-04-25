<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WeekSelector.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.WeekSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>



<asp:Panel ID="pnlWeekContainer" runat="server">
<table cellpadding="3" cellspacing="3">
    <tr>
        <td>
        <asp:HyperLink ID="hprlnkPreviousWeek" runat="server" Width="30" onclick="return checkDirtyWithRedirect(this.href)" >
        <img alt="Previous" src="Images/previous.gif" />
        </asp:HyperLink>
        </td>
        <td>
            <table>
                <tr>
                    <td>
                        <span style="font-size: xx-small; color: navy">Week of</span>
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
        <asp:HyperLink ID="hprlnkNextWeek" runat="server" Width="30" onclick="return checkDirtyWithRedirect(this.href)" >
        <img alt="Next" src="Images/next.gif" />
        </asp:HyperLink>
        </td>
    </tr>
</table>
</asp:Panel>
<asp:Panel ID="pnlPopupCalendar" runat="server" class="calendarWrapper">
    <asp:Calendar ID="calendar" runat="server" 
        SelectionMode="Day" 
        onselectionchanged="calendar_SelectionChanged" 
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
<AjaxControlToolkit:PopupControlExtender ID="popupExcalendar" runat="server" 
        TargetControlID="lblWeek" 
        PopupControlID="pnlPopupCalendar" 
        Position="Bottom" OffsetX="0" OffsetY="0"/>
        
<asp:UpdateProgress ID="upTimeEntries" runat="server">
    <ProgressTemplate>
        <div class="please-wait-holder ToolTip" style="display: block;">
            <table>
			    <tr class="top">
				    <td class="lt"></td>
				    <td class="tbor"></td>
				    <td class="rt"></td>
			    </tr>
			    <tr class="middle">
				    <td class="lbor"></td>
				    <td class="content">
					    <div id="divWait">
                            <span style="color: Black; font-weight: bold;">
                                <nobr>Please Wait...</nobr>
                            </span>
                            <br /><br />
                            <asp:Image ID="img" runat="server" ImageUrl="~/Images/loading.gif" />
                        </div>
				    </td>
				    <td class="rbor"></td>
			    </tr>
			    <tr class="bottom">
				    <td class="lb"></td>
				    <td class="bbor"></td>
				    <td class="rb"></td>
			    </tr>
		    </table>
		</div>
    </ProgressTemplate>
</asp:UpdateProgress>

