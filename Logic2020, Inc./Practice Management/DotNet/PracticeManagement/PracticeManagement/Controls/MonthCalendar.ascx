<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonthCalendar.ascx.cs"
    Inherits="PraticeManagement.Controls.MonthCalendar" %>
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
            ) : "" %>' ToolTip='<%# GetToolTip( (string)Eval("HolidayDescription"))%>'>
                    <asp:LinkButton ID="btnDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && !(bool)GetIsReadOnly((bool)Eval("ReadOnly"), (bool)Eval("DayOff"), (bool)Eval("CompanyDayOff"), (DateTime)Eval("Date"))%>'
                        DayOff='<%# (bool)Eval("DayOff") ? "true":"false" %>' Date='<%# Eval("Date") %>'
                        OnClientClick='<%# DayOnClientClick((DateTime)Eval("Date")) %>' 
                        IsRecurringHoliday='<%# (bool)Eval("IsRecurringHoliday") %>'
                        ToolTip='<%# GetToolTip( (string)Eval("HolidayDescription") )%>'
                        HolidayDescription='<%# string.IsNullOrEmpty((string)Eval("HolidayDescription"))? "":((string)Eval("HolidayDescription"))%>'
                        RecurringHolidayId='<%# (int?) Eval("RecurringHolidayId")%>' 
                        RecurringHolidayDate='<%# (DateTime?) Eval("RecurringHolidayDate") %>'
                        IsWeekEnd='<%# GetIsWeekend(((DateTime)Eval("Date"))) %>' 
                        Enabled='<%# NeedToEnable((DateTime)Eval("Date")) %>'
                        CompanyDayOff='<%# (bool)Eval("CompanyDayOff") ? "true" : "false" %>' 
                        ActualHours='<%# GetDoubleFormat((double?)Eval("ActualHours")) %>' 
                        IsFloatingHoliday='<%# (bool)Eval("IsFloatingHoliday") %>' 
                        OnClick = "btnDay_OnClick"
                        TimeTypeId = '<%# (int?)Eval("TimeTypeId") %>' 
                        ></asp:LinkButton>
                    <asp:Label ID="lblDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year &&  GetIsReadOnly((bool)Eval("ReadOnly"), (bool)Eval("DayOff"), (bool)Eval("CompanyDayOff"), (DateTime)Eval("Date")) %>'></asp:Label>
                </asp:Panel>
            </ItemTemplate>
        </asp:DataList>
        <asp:HiddenField ID="hdnDummyFieldForModalPopup" runat="server" />
        <asp:HiddenField ID="hndDayOff" runat="server" />
        <asp:HiddenField ID="hdnDate" runat="server" />
        <asp:HiddenField ID="hdnRecurringHolidayId" runat="server" />
        <asp:HiddenField ID="hdnRecurringHolidayDate" runat="server" />
        <asp:Button ID="btnSaveDay" runat="server" OnClick="btnDayOK_OnClick" CssClass="displayNone" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeHoliday" runat="server" TargetControlID="hdnDummyFieldForModalPopup"
            CancelControlID="btnDayCancel" OkControlID="hdnDummyFieldForModalPopup" BackgroundCssClass="modalBackground"
            PopupControlID="pnlHolidayDetails" BehaviorID="bhCompanyHoliday" DropShadow="false" />
        <asp:Panel ID="pnlHolidayDetails" runat="server" CssClass="PanelPerson PanelHolidayDetails"
            Style="display: none;">
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" class="fontBold TextAlignLeft">
                        Date :
                        <label id="lblDate" runat="server" text="">
                        </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="TextAlignLeft">
                        <asp:TextBox ID="txtHolidayDescription" runat="server" placeholder="Enter Holiday Description."
                            TextMode="MultiLine" CssClass="TxtHolidayDescription Height50pxImp"></asp:TextBox>
                        <asp:RadioButton ID="rbPTO" runat="server" Text="PTO" GroupName="PTO" onclick="" />
                        <p>
                            <asp:Label ID="lblActualHours" runat="server" Text="Hours : " CssClass="padLeft20"></asp:Label>
                            <asp:TextBox ID="txtActualHours" runat="server" CssClass="ResizeNone Width50PxImp"></asp:TextBox></p>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="textLeft">
                        <asp:CheckBox ID="chkMakeRecurringHoliday" runat="server" Text="Make Recurring" />
                        <asp:RadioButton ID="rbFloatingHoliday" runat="server" Text="Floating Holiday" GroupName="PTO"
                            onclick="" />
                    </td>
                </tr>
                <tr>
                    <td class="Padding10px0px10px0px TextAlignCenter">
                        <asp:Button ID="btnDayOK" runat="server" Text="OK" HiddenDayOffID="" HiddenDateID=""
                            PersonId="" Date="" SaveDayButtonID="" TextID="" ErrorMessageID="" ExtendarId=""
                            TxtActualHoursID="" RbFloatingID="" OnClientClick="ClickSaveDay(this, 'true'); return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnDayDelete" runat="server" Text="Delete" OnClick="btnDayOK_OnClick" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnDayCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="TextAlignCenter">
                        <asp:Label ID="lblValidationMessage" runat="server" Text="* Please Enter Holiday Description."
                            ForeColor="Red" CssClass="displayNone"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

