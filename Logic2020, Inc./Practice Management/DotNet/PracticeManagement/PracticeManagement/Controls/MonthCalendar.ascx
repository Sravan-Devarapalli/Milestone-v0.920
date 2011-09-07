<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonthCalendar.ascx.cs"
    Inherits="PraticeManagement.Controls.MonthCalendar" %>
<asp:UpdatePanel ID="pnlMonth" runat="server">
    <ContentTemplate>
        <script type="text/javascript" language="javascript">
            function ClickSaveDay(btnOk) {
                var noteText = $get(btnOk.attributes['TextID'].value);
                var popupExtendar = $find(btnOk.attributes['ExtendarId'].value);
                var actualHoursText = $get(btnOk.attributes['TxtActualHoursID'].value);
                var errorText = $get(btnOk.attributes['ErrorMessageID'].value);
                var rbFloating = $get(btnOk.attributes['RbFloatingID'].value);
                
                if (actualHoursText == null && noteText != '') {
                    var noteTextStr = noteText.value.toString();
                    if (noteTextStr.length > 0) {
                        SaveDetails(popupExtendar, btnOk);
                    }
                    else {
                        errorText.innerHTML = '* Please Enter Holiday Description.';
                    }
                }
                else {
                    if (rbFloating.checked) {
                        SaveDetails(popupExtendar, btnOk);
                    }
                    else {
                        var hoursTextStr = actualHoursText.value.toString();
                        if (hoursTextStr.length > 0) {
                            var hours = parseFloat(hoursTextStr);
                            if (hours >= 0.0 && hours <= 24.0 && hours == hoursTextStr) {

                                SaveDetails(popupExtendar, btnOk);
                            }
                            else {
                                errorText.innerHTML = '* Hours should be real and 0.00-24.00.';
                            }
                        }
                        else {
                            errorText.innerHTML = '* Please Enter Hours';
                        }
                    }
                }
                errorText.style.display = 'block';
                return false;
            }

            function SaveDetails(popupExtendar, btnOk) {
                btnSave = $get(btnOk.attributes['SaveDayButtonID'].value);
                popupExtendar.hide();
                btnSave.click();
            }

            function disableActualHours(txtBox, isFloatingHoliday) {
                var item = txtBox;
                if (isFloatingHoliday == 'true') {
                    item.disabled = 'disabled';
                }
                else {
                    item.disabled = '';
                }
            }
        </script>
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
            ) : "" %>' ToolTip='<%# string.IsNullOrEmpty((string)Eval("HolidayDescription"))? "":((string)Eval("HolidayDescription"))%>'>
                    <asp:LinkButton ID="btnDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && !(bool)Eval("ReadOnly") %>'
                        DayOff='<%# (bool)Eval("DayOff") ? "true":"false" %>' Date='<%# Eval("Date") %>'
                        OnClientClick='<%# DayOnClientClick((DateTime)Eval("Date")) %>' IsRecurringHoliday='<%# (bool)Eval("IsRecurringHoliday") %>'
                        ToolTip='<%# string.IsNullOrEmpty((string)Eval("HolidayDescription"))? "":((string)Eval("HolidayDescription"))%>'
                        HolidayDescription='<%# string.IsNullOrEmpty((string)Eval("HolidayDescription"))? "":((string)Eval("HolidayDescription"))%>'
                        RecurringHolidayId='<%# (int?) Eval("RecurringHolidayId")%>' RecurringHolidayDate='<%# (DateTime?) Eval("RecurringHolidayDate") %>'
                        IsWeekEnd='<%# GetIsWeekend(((DateTime)Eval("Date"))) %>' Enabled='<%# NeedToEnable((DateTime)Eval("Date")) %>'
                        CompanyDayOff='<%# (bool)Eval("CompanyDayOff") ? "true" : "false" %>'></asp:LinkButton>
                    <asp:Label ID="lblDay" runat="server" Text='<%# Eval("Date.Day") %>' Visible='<%# ((DateTime)Eval("Date")).Month == Month && ((DateTime)Eval("Date")).Year == Year && (bool)Eval("ReadOnly") %>'></asp:Label>
                    <%--<asp:Label ID="lblDayOut" runat="server" Text='<%# Eval("Date.Day") %>'
						Visible='<%# ((DateTime)Eval("Date")).Month != Month || ((DateTime)Eval("Date")).Year != Year %>'></asp:Label>--%>
                </asp:Panel>
            </ItemTemplate>
        </asp:DataList>
        <asp:HiddenField ID="hdnDummyFieldForModalPopup" runat="server" />
        <asp:HiddenField ID="hndDayOff" runat="server" />
        <asp:HiddenField ID="hdnDate" runat="server" />
        <asp:HiddenField ID="hdnRecurringHolidayId" runat="server" />
        <asp:HiddenField ID="hdnRecurringHolidayDate" runat="server" />
        <asp:Button ID="btnSaveDay" runat="server" OnClick="btnDayOK_OnClick" Style="display: none;" />
        <AjaxControlToolkit:ModalPopupExtender ID="mpeHoliday" runat="server" TargetControlID="hdnDummyFieldForModalPopup"
            CancelControlID="btnDayCancel" OkControlID="hdnDummyFieldForModalPopup" BackgroundCssClass="modalBackground"
            PopupControlID="pnlHolidayDetails" BehaviorID="bhCompanyHoliday" DropShadow="false" />
        <asp:Panel ID="pnlHolidayDetails" runat="server" BackColor="White" BorderColor="Black"
            Style="padding-top: 20px; padding-left: 10px; padding-right: 10px; min-width:250px; max-width:700px; min-height:60px;
            display: none;" BorderWidth="2px">
            <table class="WholeWidth">
                <tr>
                    <td colspan="2" align="left" style="font-weight: bold;">
                        Date :
                        <label id="lblDate" runat="server" text="">
                        </label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:left;">
                        <asp:TextBox ID="txtHolidayDescription" runat="server" placeholder="Enter Holiday Description."
                            TextMode="MultiLine" Height="50px" Style="resize: none; width: 300px; overflow: auto; margin-left:4px;"></asp:TextBox>
                        <asp:RadioButton ID="rbPTO" runat="server" Text="PTO" GroupName="PTO" onclick=""/>
                        <p>
                        <asp:Label ID="lblActualHours" runat="server" Text="Hours : " style="padding-left:20px;"></asp:Label>
                        <asp:TextBox ID="txtActualHours" runat="server" Width="50px" Style="resize: none;"></asp:TextBox></p>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: left;">
                        <asp:CheckBox ID="chkMakeRecurringHoliday" runat="server" Text="Make Recurring" />
                        <asp:RadioButton ID="rbFloatingHoliday" runat="server" Text="Floating Holiday" GroupName="PTO" onclick=""/>
                    </td>
                </tr>
                <tr>
                    <td align="center" style="padding: 10px 0px 10px 0px;">
                        <asp:Button ID="btnDayOK" runat="server" Text="OK" HiddenDayOffID="" HiddenDateID=""
                            SaveDayButtonID="" TextID="" ErrorMessageID="" ExtendarId="" TxtActualHoursID="" RbFloatingID="" OnClientClick="ClickSaveDay(this, 'true'); return false;" />
                        &nbsp; &nbsp;
                        <asp:Button ID="btnDayCancel" runat="server" Text="Cancel" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label ID="lblValidationMessage" runat="server" Text="* Please Enter Holiday Description."
                            ForeColor="Red" Style="display: none;"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<%--<AjaxControlToolkit:UpdatePanelAnimationExtender ID="pnlMonth_UpdatePanelAnimationExtender" 
	runat="server" Enabled="True" TargetControlID="pnlMonth">
	<Animations>
		<OnUpdated>
			<ScriptAction Script="hideInProcessImage($get('divWait1'));" />
		</OnUpdated>
	</Animations>
</AjaxControlToolkit:UpdatePanelAnimationExtender>--%>
