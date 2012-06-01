<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonMonthCalendar.ascx.cs"
    Inherits="PraticeManagement.Controls.PersonMonthCalendar" %>
<asp:UpdatePanel ID="pnlMonth" runat="server">
    <ContentTemplate>
        <asp:DataList ID="lstCalendar" runat="server" RepeatColumns="7" RepeatDirection="Horizontal">
            <HeaderTemplate>
                </td> </tr>
                <tr>
                    <th>
                        Sun
                    </th>
                    <th>
                        Mon
                    </th>
                    <th>
                        Tue
                    </th>
                    <th>
                        Wed
                    </th>
                    <th>
                        Thu
                    </th>
                    <th>
                        Fri
                    </th>
                    <th>
                        Sat
                    </th>
                </tr>
                <tr>
                    <td colspan="7">
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Panel ID="pnlDay" runat="server" CssClass='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year ? (  
            (    ((bool)Eval("DayOff") 
                    ? ((bool)Eval("CompanyDayOff") 
                        ? (((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Sunday || ((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Saturday ? "WeekEndDayOff" : "DayOff") 
                        : (((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Sunday || ((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Saturday ? "WeekEndDayOff" : "CompanyDayOn")
                      ) 
                    : ((bool)Eval("CompanyDayOff") 
                        ? (((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Sunday || ((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Saturday ? "WeekEndDayOn" : "CompanyDayOff")
                        : (((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Sunday || ((DateTime)Eval("Date")).DayOfWeek == DayOfWeek.Saturday ? "WeekEndDayOn" : "DayOn")
                      )
                )
            )
            ) : "" %>' ToolTip='<%# GetToolTip( (string)Eval("HolidayDescription"), (double?)Eval("ActualHours"),(bool)Eval("IsFloatingHoliday") )%>'>
                    <asp:LinkButton ID="btnDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && !(bool)GetIsReadOnly((bool)Eval("ReadOnly"), (bool)Eval("DayOff"), (bool)Eval("CompanyDayOff"), (DateTime)Eval("Date"), (bool)Eval("IsUnpaidTimeType"))%>'
                        DayOff='<%# (bool)Eval("DayOff") ? "true":"false" %>' Date='<%# Eval("Date") %>'
                        OnClientClick='<%# DayOnClientClick() %>' ToolTip='<%# GetToolTip( (string)Eval("HolidayDescription"), (double?)Eval("ActualHours"),(bool)Eval("IsFloatingHoliday") )%>'
                        HolidayDescription='<%# string.IsNullOrEmpty((string)Eval("HolidayDescription"))? "":((string)Eval("HolidayDescription"))%>'
                        Enabled="true" CompanyDayOff='<%# (bool)Eval("CompanyDayOff") ? "true" : "false" %>'
                        ActualHours='<%# GetDoubleFormat((double?)Eval("ActualHours")) %>' IsFloatingHoliday='<%# (bool)Eval("IsFloatingHoliday") %>'
                        OnClick="btnDay_OnClick" TimeTypeId='<%# (int?)Eval("TimeTypeId") %>'></asp:LinkButton>
                    <asp:Label ID="lblDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year &&  GetIsReadOnly((bool)Eval("ReadOnly"), (bool)Eval("DayOff"), (bool)Eval("CompanyDayOff"), (DateTime)Eval("Date"), (bool)Eval("IsUnpaidTimeType")) %>'></asp:Label>
                </asp:Panel>
            </ItemTemplate>
        </asp:DataList>
    </ContentTemplate>
</asp:UpdatePanel>

