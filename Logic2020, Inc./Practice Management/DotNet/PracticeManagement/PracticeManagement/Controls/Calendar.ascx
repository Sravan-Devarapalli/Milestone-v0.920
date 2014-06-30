<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="PraticeManagement.Controls.Calendar" %>
<%@ Register Src="~/Controls/MonthCalendar.ascx" TagName="MonthCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>

<script type="text/javascript">
    var updatingCalendarContainer = null;

    function changeAlternateitemscolrsForCBL() {
        var cbl = document.getElementById('<%=cblRecurringHolidays.ClientID %>');
        if (cbl != null)
            SetAlternateColors(cbl);
    }

    function SetAlternateColors(chkboxList) {
        var chkboxes = chkboxList.getElementsByTagName('input');
        var index = 0;
        if (chkboxes[0].parentNode.style.display != "none") {
            chkboxes[0].parentNode.style.paddingTop = "6px";
            chkboxes[0].parentNode.style.paddingBottom = "6px";
            chkboxes[0].parentNode.style.borderBottom = "1px solid black";
        }
        for (var i = 0; i < chkboxes.length; i++) {
            if (chkboxes[i].parentNode.style.display != "none") {
                index++;
                if ((index) % 2 == 0) {
                    chkboxes[i].parentNode.style.backgroundColor = "#f9faff";
                }
                else {
                    chkboxes[i].parentNode.style.backgroundColor = "";
                }
                chkboxes[i].parentNode.style.paddingRight = "2px";
                chkboxes[i].parentNode.style.borderRight = "5px solid white";
                chkboxes[i].parentNode.style.borderLeft = "5px solid white";
            }
        }
        chkboxList.parentNode.style.overflow = "hidden";
        chkboxList.parentNode.style.height = ((chkboxes.length * 40) - 20) + "px";
    }
    function ShowPopup(dayLink, peBehaviourId, saveDayButtonID, hiddenDayOffID, hiddenDateID,
                        txtHolidayDescriptionID, chkMakeRecurringHolidayId, hdnRecurringHolidayIdClientID, hdnRecurringHolidayDateClientID, lblDateID,
                        ErrorMessageID, btnOkID, personId, txtActualHoursID, lblActualHoursClientID, rbPTOClientID, rbFloatingHolidayClientID, btnDeleteId) {
        var txtHolidayDescription = $get(txtHolidayDescriptionID);
        var txtActualHours = $get(txtActualHoursID);
        var lblDateDescription = $get(lblDateID);
        var chkMakeRecurringHoliday = $get(chkMakeRecurringHolidayId);
        var hndDayOff = $get(hiddenDayOffID);
        var hdnDate = $get(hiddenDateID);
        var hdnRecurringHolidayId = $get(hdnRecurringHolidayIdClientID);
        var hdnRecurringHolidayDate = $get(hdnRecurringHolidayDateClientID);
        var DeleteButton = $get(btnDeleteId);
        hndDayOff.value = dayLink.attributes['DayOff'].value;
        hdnDate.value = dayLink.attributes['Date'].value;
        hdnRecurringHolidayId.value = dayLink.attributes['RecurringHolidayId'].value;
        hdnRecurringHolidayDate.value = dayLink.attributes['RecurringHolidayDate'].value;
        DeleteButton.disabled = 'disabled';


        if (hndDayOff.value == 'true'
                && dayLink.attributes['IsRecurringHoliday'].value == 'True'
                && hdnRecurringHolidayId.value == "") {
            if (confirm("This is a recurring holiday. Do you want to remove all instances of this holiday going forward?")) {
                chkMakeRecurringHoliday.checked = true;

                if (hdnRecurringHolidayId != null) {
                    var cbl = document.getElementById('<%=cblRecurringHolidays.ClientID %>');
                }
            }
            else {
                chkMakeRecurringHoliday.checked = false;
            }
            $get(saveDayButtonID).click();
        }
        else if (dayLink.attributes['IsWeekEnd'].value == 'true' || hdnRecurringHolidayId.value != "" ||
                    (dayLink.attributes['DayOff'].value == 'true' && dayLink.attributes['IsRecurringHoliday'].value == 'False')) {
            if (dayLink.attributes['IsWeekEnd'].value == 'true') {
                chkMakeRecurringHoliday.checked = false;
            }
            else {
                chkMakeRecurringHoliday.checked = (dayLink.attributes['IsRecurringHoliday'].value == 'true');
            }
            $get(saveDayButtonID).click();
        }
        else {
            var date = new Date(hdnDate.value);
            var popupExtendar = $find(peBehaviourId);
            var OkButton = $get(btnOkID);
            var errorMessage = $get(ErrorMessageID);
            var lblActualHours = $get(lblActualHoursClientID);
            var rbPTO = $get(rbPTOClientID);
            var rbFloatingHoliday = $get(rbFloatingHolidayClientID);
            OkButton.attributes['SaveDayButtonID'].value = saveDayButtonID;
            OkButton.attributes['ErrorMessageID'].value = ErrorMessageID;
            OkButton.attributes['TextID'].value = txtHolidayDescriptionID;
            OkButton.attributes['ExtendarId'].value = peBehaviourId;

            errorMessage.style.display = 'none';
            lblActualHours.style.display = 'none';
            txtActualHours.style.display = 'none';
            rbPTO.style.display = 'none';
            rbPTO.nextSibling.style.display = 'none';
            rbFloatingHoliday.style.display = 'none';
            rbFloatingHoliday.nextSibling.style.display = 'none';
            DeleteButton.style.display = 'none';
            txtActualHours.value = '';
            lblDateDescription.innerHTML = date.format('MM/dd/yyyy');
            txtHolidayDescription.value = dayLink.attributes['HolidayDescription'].value;
            chkMakeRecurringHoliday.checked = (dayLink.attributes['IsRecurringHoliday'].value == 'true');
            popupExtendar.show();
        }

        return false;
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);


    function endRequestHandle(sender, Args) {

        changeAlternateitemscolrsForCBL();

    }




    function ClickSaveDay(btnOk) {
        var noteText = $get(btnOk.attributes['TextID'].value);
        var popupExtendar = $find(btnOk.attributes['ExtendarId'].value);
        var actualHoursText = $get(btnOk.attributes['TxtActualHoursID'].value);
        var errorText = $get(btnOk.attributes['ErrorMessageID'].value);
        var rbFloating = $get(btnOk.attributes['RbFloatingID'].value);

        if (actualHoursText == null && noteText != '') {
            var noteTextStr = noteText.value.toString();
            if (noteTextStr.length >= 3 && noteTextStr.length <= 1000) {
                SaveDetails(popupExtendar, btnOk);
            }
            else {
                errorText.innerHTML = 'Holiday Description should be 3-1000 characters long';
            }
        }
        else {
            var hdnDayOff = $get(btnOk.attributes['HiddenDayOffID'].value);

            if (rbFloating.checked) {
                hdnDayOff.value = 'false'; //For Updating the Floating Holiday details.
                SaveDetails(popupExtendar, btnOk);
            }
            else {
                var hoursTextStr = actualHoursText.value.toString();
                if (hoursTextStr.length > 0) {
                    var hours = parseFloat(hoursTextStr);
                    if (hours >= 0.0 && hours <= 8.0 && hours == hoursTextStr) {
                        hdnDayOff.value = 'false'; //For Updating the PTO details.
                        SaveDetails(popupExtendar, btnOk);
                    }
                    else {
                        errorText.innerHTML = '* Hours should be real and 0.00-8.00.';
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

</script>

<table class="Width98Percent">
    <tr>
        <td class="Width15Percent">
            &nbsp;
        </td>
        <td id="tdDescription" runat="server" class="vTop Width70Percent TextAlignCenter">
            <div class="HolidaysDescription">
                <p>
                    Days selected on this calendar will be highlighted as Company Holidays throughout
                    Practice Management.</p>
                <p  class="PaddingTop8">
                    Common Recurring Holidays can be selected from the drop-down as well. Once selected
                    they will be highlighted as Company Holidays throughout Practice Management for
                    the current year as well as in future years.</p>
            </div>
        </td>
        <td class="Width15Percent">
            &nbsp;
        </td>
    </tr>
</table>
<table class="Width98Percent">
    <tr>
        <td class="Width100Per TextAlignCenter">
            <asp:UpdatePanel ID="upnlBody" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc3:LoadingProgress ID="ldProgress" runat="server" />
                    <table class="CalendarTable Width98Percent TextAlignCenter">
                        <tr>
                            <td colspan="3" class="TextAlignCenterImp">
                                <asp:Label ID="lblConsultantMessage" runat="server" Visible="false" Text="You can review your vacation days, but cannot change them. Please see your Practice Manager for updates to your vacation schedule."></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table class="Width98Percent" align="center">
                        <tr>
                            <td class="Width15Percent">
                                &nbsp;
                            </td>
                            <td class="Width70Percent TextAlignCenter">
                                <table class="CalendarTable">
                                    <tr>
                                        <td colspan="3" align="center">
                                            <table class="Width100Per">
                                                <tr>
                                                    <td class="vMiddle textRightImp">
                                                        <asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" OnClick="btnPrevYear_Click"
                                                            ToolTip="Previous Year">
                                                            <asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td class="vMiddle Width15Px">
                                                        <asp:Label ID="lblYear" CssClass="FontSizeXLarge" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="vMiddle textLeft">
                                                        <asp:LinkButton ID="btnNextYear" runat="server" CausesValidation="false" OnClick="btnNextYear_Click"
                                                            ToolTip="Next Year">
                                                            <asp:Image ID="imgNextYear" runat="server" ImageUrl="~/Images/next.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td id="tdRecurringHolidaysDetails" runat="server" rowspan="9" class="setCheckboxesLeft PaddingTop45pxLeft2Per">
                                            <uc:ScrollingDropDown ID="cblRecurringHolidays" runat="server" SetDirty="false" AllSelectedReturnType="AllItems"
                                                OnSelectedIndexChanged="cblRecurringHolidays_OnSelectedIndexChanged" AutoPostBack="true" />
                                        </td>
                                    </tr>
                                    <tr class="HeadRow">
                                        <td class="setColorToCalendar">
                                            January
                                        </td>
                                        <td>
                                            February
                                        </td>
                                        <td class="setColorToCalendar">
                                            March
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcJanuary" runat="server" Month="1" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcFebruary" runat="server" Month="2" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcMarch" runat="server" Month="3" />
                                        </td>
                                    </tr>
                                    <tr class="HeadRow">
                                        <td>
                                            April
                                        </td>
                                        <td class="setColorToCalendar">
                                            May
                                        </td>
                                        <td>
                                            June
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:MonthCalendar ID="mcApril" runat="server" Month="4" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcMay" runat="server" Month="5" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcJune" runat="server" Month="6" />
                                        </td>
                                    </tr>
                                    <tr class="HeadRow">
                                        <td class="setColorToCalendar">
                                            July
                                        </td>
                                        <td>
                                            August
                                        </td>
                                        <td class="setColorToCalendar">
                                            September
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcJuly" runat="server" Month="7" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcAugust" runat="server" Month="8" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcSeptember" runat="server" Month="9" />
                                        </td>
                                    </tr>
                                    <tr class="HeadRow">
                                        <td>
                                            October
                                        </td>
                                        <td class="setColorToCalendar">
                                            November
                                        </td>
                                        <td>
                                            December
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:MonthCalendar ID="mcOctober" runat="server" Month="10" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcNovember" runat="server" Month="11" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcDecember" runat="server" Month="12" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width15Percent">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>

