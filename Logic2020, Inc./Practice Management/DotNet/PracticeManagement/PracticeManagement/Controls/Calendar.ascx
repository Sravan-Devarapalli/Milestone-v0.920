<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="PraticeManagement.Controls.Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<%@ Register Src="~/Controls/MonthCalendar.ascx" TagName="MonthCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
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

        if (personId == "") {
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
        }//for person calander
        else {
            var date = new Date(hdnDate.value);
            var hdnHolidayDate = document.getElementById('<%= hdnHolidayDate.ClientID %>');

            if (dayLink.attributes['IsFloatingHoliday'].value.toLowerCase() == 'true') {
                var lblDeleteSubstituteDay = document.getElementById('<%= lblDeleteSubstituteDay.ClientID %>');
                lblDeleteSubstituteDay.innerHTML = hdnHolidayDate.value = date.format('MM/dd/yyyy');
                var mpeDeleteSubstituteDay = $find('mpeDeleteSubstituteDay');
                mpeDeleteSubstituteDay.show();
            }
            else if (dayLink.attributes['CompanyDayOff'].value == 'false' && dayLink.attributes['IsWeekEnd'].value == 'false' ) {

               return true;
            }
            else if (hndDayOff.value == 'true' && dayLink.attributes['CompanyDayOff'].value == 'true') {

                var lblHolidayDate = document.getElementById('<%= lblHolidayDate.ClientID %>');
                var lblHolidayName = document.getElementById('<%= lblHolidayName.ClientID %>');

                lblHolidayDate.innerHTML = hdnHolidayDate.value = date.format('MM/dd/yyyy');
                lblHolidayName.innerHTML = dayLink.attributes['HolidayDescription'].value;
                var dpSubstituteDay = document.getElementById('<%= (dpSubstituteDay.FindControl("txtDate") as TextBox).ClientID %>');
                dpSubstituteDay.value = '';

                var mpeHolidayAndSubStituteDay = $find('mpeHolidayAndSubStituteDay');
                mpeHolidayAndSubStituteDay.show();
            }
        }
        return false;
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest( endRequestHandle );


    function endRequestHandle(sender, Args) {

        changeAlternateitemscolrsForCBL();
    }



</script>
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

    function disableActualHours(txtBox, isFloatingHoliday) {
        var item = txtBox;
        if (isFloatingHoliday == 'true') {
            item.disabled = 'disabled';
        }
        else {
            item.disabled = '';
        }
    } 

    function btnOk_EditCondtion() {
        $find('mpeSelectEditCondtion').hide();
        var rbEditSingleDay = document.getElementById('<%=rbEditSingleDay.ClientID %>');
        var rbEditSeries = document.getElementById('<%=rbEditSeries.ClientID %>');
        if (rbEditSingleDay.checked) {
            $find('mpeEditSingleDay').show();
        } else {
            $find('mpeAddTimeOff').show();
        }
        return false;
    }

    function btnAddTimeOff_Click() {
        $find( 'mpeAddTimeOff' ).show();
        var btnDeleteTimeOff = document.getElementById( '<%=btnDeleteTimeOff.ClientID %>' );
        var dtpStartDateTimeOff = document.getElementById( '<%=dtpStartDateTimeOff.ClientID %>' );
        var dtpEndDateTimeOff = document.getElementById( '<%=dtpEndDateTimeOff.ClientID %>' );
        var ddlTimeTypesTimeOff = document.getElementById( '<%=ddlTimeTypesTimeOff.ClientID %>' );
        var txthoursTimeOff = document.getElementById( '<%=txthoursTimeOff.ClientID %>' );
        btnDeleteTimeOff.style.display = 'none';
        txthoursTimeOff.value = '';
        ddlTimeTypesTimeOff.value = '';
        ddlTimeTypesTimeOff.removeAttribute( 'disabled' );
        var txtStartDate = document.getElementById( '<%=(dtpStartDateTimeOff.FindControl("txtDate") as TextBox).ClientID %>' );
        var txtEndDate = document.getElementById( '<%=(dtpEndDateTimeOff.FindControl("txtDate") as TextBox).ClientID %>' );
        txtStartDate.value = '';
        txtEndDate.value = '';
        return false;
    }
</script>
<style>
    .setCheckboxesLeft TD, .setCheckboxesLeft div
    {
        text-align: left !important;
    }
    
    .setCheckboxesLeft input
    {
        vertical-align: middle;
        float: right;
    }
    .setCheckboxesLeft Table
    {
        width: 100%;
    }
    .setColorToCalendar
    {
        background-color: #E9F0F9;
    }
    .AlertColor
    {
        color: Red;
    }
</style>
<table width="98%">
    <tr>
        <td style="width: 15%;">
            &nbsp;
        </td>
        <td id="tdDescription" align="center" runat="server" style="vertical-align: top;
            width: 70%;">
            <div style="border: 1px solid black; padding: 5px; white-space: normal;">
                <p>
                    Days selected on this calendar will be highlighted as Company Holidays throughout
                    Practice Management.</p>
                <p style="padding-top: 8px;">
                    Common Recurring Holidays can be selected from the drop-down as well. Once selected
                    they will be highlighted as Company Holidays throughout Practice Management for
                    the current year as well as in future years.</p>
            </div>
        </td>
        <td style="width: 15%;">
            &nbsp;
        </td>
    </tr>
</table>
<table width="98%">
    <tr>
        <td style="width: 100%; text-align: center;">
            <asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc3:LoadingProgress ID="ldProgress" runat="server" />
                    <table width="98%" align="center" style="text-align: center;" class="CalendarTable">
                        <tr id="trPersonDetails" runat="server">
                            <td style="text-align: right; vertical-align: middle; width: 30%">
                                Select a Person:
                            </td>
                            <td nowrap="nowrap" style="text-align: left;">
                                <uc:CustomDropDown ID="ddlPerson" runat="server" IsOptionGroupRequired="false">
                                </uc:CustomDropDown>
                                <asp:UpdatePanel ID="pnlButton" runat="server" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:Button ID="btnRetrieveCalendar" runat="server" Text="Retrieve Calendar" OnClick="btnRetrieveCalendar_Click" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <AjaxControlToolkit:UpdatePanelAnimationExtender ID="pnlButton_UpdatePanelAnimationExtender"
                                    runat="server" Enabled="True" TargetControlID="pnlButton">
                                    <Animations>
								<OnUpdating>
									<EnableAction AnimationTarget="btnRetrieveCalendar" Enabled="false" />
								</OnUpdating>
								<OnUpdated>
									<EnableAction AnimationTarget="btnRetrieveCalendar" Enabled="true" />
								</OnUpdated>
                                    </Animations>
                                </AjaxControlToolkit:UpdatePanelAnimationExtender>
                            </td>
                            <td style="text-align: left; vertical-align: middle; width: 30%">
                                <asp:Button ID="btnAddTimeOff" runat="server" Text="Add Time Off"  OnClientClick="return btnAddTimeOff_Click();" ToolTip="Add Time Off"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" style="padding-top: 10px; padding-bottom: 10px;">
                                <div id="trAlert" runat="server">
                                    <asp:Label ID="lbAlert1" runat="server" Text="Alert :" CssClass="AlertColor"></asp:Label>
                                    <asp:Label ID="lbAlert2" runat="server" Text=" You are viewing this calendar as READ-ONLY.  If you believe you should have permissions to make changes to this calendar, please "></asp:Label>
                                    <asp:HyperLink ID="contactSupportMailToLink" runat="server" Text="contact support"
                                        ForeColor="#0898E6"></asp:HyperLink>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <asp:Label ID="lblConsultantMessage" runat="server" Visible="false" Text="You can review your vacation days, but cannot change them. Please see your Practice Manager for updates to your vacation schedule."></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table width="98%" align="center">
                        <tr>
                            <td style="width: 15%">
                                &nbsp;
                            </td>
                            <td style="width: 70%" align="center">
                                <table class="CalendarTable">
                                    <tr>
                                        <td colspan="3" align="center">
                                            <table width="100%">
                                                <tr>
                                                    <td valign="middle" style="text-align: right;">
                                                        <asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" OnClick="btnPrevYear_Click" ToolTip="Previous Year">
                                                            <asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td valign="middle" style="vertical-align: middle !important;" width="15px">
                                                        <asp:Label ID="lblYear" Style="font-size: x-large;" runat="server"></asp:Label>
                                                    </td>
                                                    <td valign="middle" style="text-align: left;">
                                                        <asp:LinkButton ID="btnNextYear" runat="server" CausesValidation="false" OnClick="btnNextYear_Click" ToolTip="Next Year">
                                                            <asp:Image ID="imgNextYear" runat="server" ImageUrl="~/Images/next.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td id="tdRecurringHolidaysDetails" runat="server" rowspan="9" class="setCheckboxesLeft"
                                            style="padding-top: 45px; padding-left: 2%;">
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
                                            <uc1:MonthCalendar ID="mcJanuary" runat="server" Year="2008" Month="1" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcFebruary" runat="server" Year="2008" Month="2" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcMarch" runat="server" Year="2008" Month="3" OnPreRender="calendar_PreRender" />
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
                                            <uc1:MonthCalendar ID="mcApril" runat="server" Year="2008" Month="4" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcMay" runat="server" Year="2008" Month="5" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcJune" runat="server" Year="2008" Month="6" OnPreRender="calendar_PreRender" />
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
                                            <uc1:MonthCalendar ID="mcJuly" runat="server" Year="2008" Month="7" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcAugust" runat="server" Year="2008" Month="8" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcSeptember" runat="server" Year="2008" Month="9" OnPreRender="calendar_PreRender" />
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
                                            <uc1:MonthCalendar ID="mcOctober" runat="server" Year="2008" Month="10" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td class="setColorToCalendar">
                                            <uc1:MonthCalendar ID="mcNovember" runat="server" Year="2008" Month="11" OnPreRender="calendar_PreRender" />
                                        </td>
                                        <td>
                                            <uc1:MonthCalendar ID="mcDecember" runat="server" Year="2008" Month="12" OnPreRender="calendar_PreRender" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 15%">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <asp:HiddenField ID="hdnHolidayDate" runat="server" />
                    <asp:HiddenField ID="hdDeleteSubstituteDay" runat="server" />
                    <asp:HiddenField ID="hdEditSingleDay" runat="server" />
                    <asp:HiddenField ID="hdEditCondtion" runat="server" />
                    <asp:HiddenField ID="hfAddTimeOff" runat="server" />
                    
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeSelectEditCondtion" runat="server"
                        TargetControlID="hdEditCondtion" BackgroundCssClass="modalBackground" PopupControlID="pnlSelectEditCondtion"
                        DropShadow="false" BehaviorID="mpeSelectEditCondtion" CancelControlID="btncancel_EditCondtion" />
                    <asp:Panel ID="pnlSelectEditCondtion" runat="server" BackColor="White" BorderColor="Black"
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" Height="150px"
                        Width="200px">
                        <table class="calendarPopup">
                            <tr>
                                <td colspan="3" style="height: 20px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%; text-align: right; font-weight: bold; padding-right: 5px;">
                                    Date:
                                </td>
                                <td style="width: 40%; text-align: left; padding-left: 5px;">
                                    <asp:Label ID="lbDate" runat="server" ></asp:Label>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 40px; text-align: left;">
                                    <asp:RadioButton ID="rbEditSingleDay" runat="server" Text="Edit Single Day" GroupName="EditDay"
                                        Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 40px; text-align: left;">
                                    <asp:RadioButton ID="rbEditSeries" runat="server" Text="Edit Series" GroupName="EditDay" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="height: 10px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Button ID="btnOk_EditCondtion" Text="OK" runat="server" Style="padding-left: 10px" ToolTip="Ok"
                                        OnClientClick="return  btnOk_EditCondtion();" />
                                    <asp:Button ID="btncancel_EditCondtion" Text="Cancel" runat="server" Style="padding-left: 10px"  ToolTip="Cancel"/>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAddTimeOff" runat="server" TargetControlID="hfAddTimeOff"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlAddTimeOff" DropShadow="false"
                        BehaviorID="mpeAddTimeOff" CancelControlID="btnCancelTimeOff" />
                    <asp:Panel ID="pnlAddTimeOff" runat="server" BackColor="White" BorderColor="Black" 
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" min-Height="270px" max-Height="500px"
                        Width="320px">
                        <table  class="calendarPopup">
                            <tr>
                                <td colspan="3" style="height: 20px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%; text-align: right; font-weight: bold; padding-right: 5px;">
                                    Start Date:
                                </td>
                                <td style="width: 40%; text-align: left; padding-left: 5px;">
                                    <uc:DatePicker ID="dtpStartDateTimeOff" runat="server" TextBoxWidth="75px" />
                                    <asp:RequiredFieldValidator ID="reqStartDateTimeOff" runat="server" ControlToValidate="dtpStartDateTimeOff"
                                        ErrorMessage="The Start Date is required." ToolTip="The Start Date is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compStartDateTimeOff" runat="server" ControlToValidate="dtpStartDateTimeOff"
                                        ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                        Type="Date" ValidationGroup="TimeOff"></asp:CompareValidator>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%; text-align: right; font-weight: bold; padding-right: 5px;">
                                    End Date:
                                </td>
                                <td style="width: 40%; text-align: left; padding-left: 5px;">
                                    <uc:DatePicker ID="dtpEndDateTimeOff" runat="server" TextBoxWidth="75px" />
                                    <asp:RequiredFieldValidator ID="reqEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                        ErrorMessage="The End Date is required." ToolTip="The End Date is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                        ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                        Type="Date" ValidationGroup="TimeOff"></asp:CompareValidator>
                                    <asp:CompareValidator ID="compStartDateEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                        ControlToCompare="dtpStartDateTimeOff" ErrorMessage="The End Date must be greater than or equal to the Start Date."
                                        ToolTip="The End Date must be greater than or equal to the Start Date." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff"
                                        Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 20px; text-align: left;">
                                    1. Select type of time to be entered:
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 40px; text-align: left;">
                                    <asp:DropDownList ID="ddlTimeTypesTimeOff" runat="server" Style="width: 70%;">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqTimeTypesTimeOff" runat="server" ControlToValidate="ddlTimeTypesTimeOff"
                                        ErrorMessage="The Work Type is required." ToolTip="The Work Type is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="height: 10px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 20px; text-align: left;">
                                    2. Enter the number of hours (per day, if applicable):
                                </td>
                            </tr>
                            <tr>
                                <td width="10%" style="padding-left: 10px; text-align: right; padding-right: 5px;">
                                    Hours:
                                </td>
                                <td width="10%" style="padding-left: 5px; text-align: left;">
                                    <asp:TextBox ID="txthoursTimeOff" runat="server" Style="width: 50px;"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqtxthoursTimeOff" runat="server" ControlToValidate="txthoursTimeOff"
                                        ErrorMessage="The Hours is required." ToolTip="The Hours is required." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txthoursTimeOff"
                                        ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                        ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                        Type="Currency" ValidationGroup="TimeOff"></asp:CompareValidator>
                                    <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txthoursTimeOff"
                                        ErrorMessage="The Hours Per Day must be greater than 0 and less or equals to 8."
                                        ToolTip="The Hours Per Day must be greater than 0 and less or equals to 8." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" MinimumValue="0.01"
                                        MaximumValue="8" Type="Double" ValidationGroup="TimeOff"></asp:RangeValidator>
                                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursPerDayInsert" runat="server"
                                        TargetControlID="txthoursTimeOff" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                        ValidChars=".">
                                    </AjaxControlToolkit:FilteredTextBoxExtender>
                                </td>
                                <td width="80%">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="height: 10px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Button ID="btnOkTimeOff" Text="OK" ValidationGroup="TimeOff" runat="server" ToolTip="Ok"
                                        OnClick="btnOkTimeOff_Click" Style="padding-left: 10px" />
                                    <asp:Button ID="btnDeleteTimeOff" Text="Delete" runat="server" ValidationGroup="TimeOff" ToolTip="Delete"
                                        OnClick="btnDeleteTimeOff_Click" Style="padding-left: 10px" />
                                    <asp:Button ID="btnCancelTimeOff" Text="Cancel" runat="server" Style="padding-left: 10px" ToolTip="Cancel"/>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="text-align:left; padding:10px;">
                                    <asp:UpdatePanel ID="upnlErrorsTimeOff" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:ValidationSummary ID="valSumTimeOff" runat="server" ValidationGroup="TimeOff" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeEditSingleDay" runat="server" TargetControlID="hdEditSingleDay"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlEditSingleDay" DropShadow="false"
                        BehaviorID="mpeEditSingleDay" CancelControlID="btnCancelEditSingleDay" />
                    <asp:Panel ID="pnlEditSingleDay" runat="server" BackColor="White" BorderColor="Black"
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" min-Height="230px" max-Height = "500px"
                        Width="320px">
                        <table class="calendarPopup">
                            <tr>
                                <td colspan="3" class="height20P">
                                </td> 
                            </tr>
                            <tr>
                                <td class = "textCenter bold padRight5 width100P" colspan="3">
                                    Date:&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lbdateSingleDay" runat="server" ></asp:Label>
                                    <asp:HiddenField ID="hdnDateSingleDay" runat="server" ></asp:HiddenField>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class = "padLeft20 textLeft">
                                    1. Select type of time to be entered:
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="textLeft padLeft40">
                                    <asp:DropDownList ID="ddlTimeTypesSingleDay" CssClass = "width70P" runat="server" Enabled="false">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqddlTimeTypesSingleDay" runat="server" ControlToValidate="ddlTimeTypesSingleDay"
                                        ErrorMessage="The Work Type is required." ToolTip="The Work Type is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="SingleDay"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="height10P" >
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="padLeft20 textLeft">
                                    2. Enter the number of hours (per day, if applicable):
                                </td>
                            </tr>
                            <tr>
                                <td class="padLeft10 textRight padRight5 width30P">
                                    Hours:
                                </td>
                                <td class="width10P padLeft5 textLeft" >
                                    <asp:TextBox ID="txtHoursSingleDay" runat="server" CssClass="width50Px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                        ErrorMessage="The Hours is required." ToolTip="The Hours is required." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" ValidationGroup="SingleDay"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                        ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                        ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                        Type="Currency" ValidationGroup="SingleDay"></asp:CompareValidator>
                                    <asp:RangeValidator ID="rangeHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                        ErrorMessage="The Hours Per Day must be greater than 0 and less or equals to 8."
                                        ToolTip="The Hours Per Day must be greater than 0 and less or equals to 8." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" MinimumValue="0.01"
                                        MaximumValue="8" Type="Double" ValidationGroup="SingleDay"></asp:RangeValidator>
                                    <AjaxControlToolkit:FilteredTextBoxExtender ID="fteHoursSingleDay" runat="server"
                                        TargetControlID="txtHoursSingleDay" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                        ValidChars=".">
                                    </AjaxControlToolkit:FilteredTextBoxExtender>
                                </td>
                                <td class="width60P">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="height10Px" >
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Button ID="btnOkSingleDay" OnClick="btnOkSingleDay_OnClick" Text="OK" ToolTip="OK" ValidationGroup="SingleDay"
                                        runat="server" class="padLeft10" />
                                    <asp:Button ID="btnDeleteSingleDay" OnClick="btnDeleteSingleDay_OnClick" ValidationGroup="SingleDay"
                                        Text="Delete" ToolTip="Delete" runat="server" class="padLeft10" />
                                    <asp:Button ID="btnCancelEditSingleDay" Text="Cancel" ToolTip="Cancel" runat="server" class="padLeft10" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="textLeft padLeft10">
                                    <asp:UpdatePanel ID="upnlErrorSingleDay" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:ValidationSummary ID="valSumErrorSingleDay" runat="server" ValidationGroup="SingleDay" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="height10Px" >
                                </td>
                             </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeHolidayAndSubStituteDay" runat="server"
                        TargetControlID="hdnHolidayDate" CancelControlID="btnSubstituteDayCancel" BackgroundCssClass="modalBackground"
                        PopupControlID="pnlHolidayAndSubStituteDay" BehaviorID="mpeHolidayAndSubStituteDay"
                        DropShadow="false" />
                    <asp:Panel ID="pnlHolidayAndSubStituteDay" runat="server" BackColor="White" BorderColor="Black"
                        Style="padding-top: 20px; padding-left: 10px; padding-right: 10px; display: none;"
                        BorderWidth="2px" min-Height="100px" Width="280px" max-Height="205px">
                        <table class="WholeWidth">
                            <tr>
                                <td style="width: 40%; text-align: left; height: 20px;">
                                    You have selected :
                                </td>
                                <td style="width: 60%; text-align: left; padding-left: 3px; height: 20px;">
                                    <asp:Label ID="lblHolidayDate" Font-Bold="true" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="width: 60%; text-align: left; padding-left: 3px; height: 20px;">
                                    <asp:Label ID="lblHolidayName" Font-Bold="true" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 20px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 20px; text-align: left;">
                                    Please select substitute day for this holiday :
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 50px; text-align: center; vertical-align: middle;">
                                    <uc:DatePicker ID="dpSubstituteDay" runat="server" OnClientChange="return true;"
                                        ValidationGroup="Substituteday" AutoPostBack="false" TextBoxWidth="90px" />
                                    <asp:CustomValidator ID="cvSubstituteDay" EnableClientScript="false" EnableViewState="false"
                                        Text="*" ValidateEmptyText="true" ToolTip="Selected date is not a Working day.Please select any Working day."
                                        ErrorMessage="Selected date is not a Working day.Please select any Working day."
                                        ValidationGroup="Substituteday" runat="server" OnServerValidate="cvSubstituteDay_ServerValidate"></asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 50px; text-align: center; vertical-align: middle;">
                                    <asp:Button ID="btnSubstituteDayOK" OnClick="btnSubstituteDayOK_Click" runat="server"
                                        ValidationGroup="Substituteday" ToolTip="OK" Text="OK" />
                                    &nbsp; &nbsp;
                                    <asp:Button ID="btnSubstituteDayCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: left; vertical-align: middle;">
                                    <asp:UpdatePanel ID="upnlValsummary" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:ValidationSummary ID="valSumsubstituteday" runat="server" ValidationGroup="Substituteday" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 5px;">
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeDeleteSubstituteDay" runat="server"
                        TargetControlID="hdDeleteSubstituteDay" CancelControlID="btnCancelSubstituteDay"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlDeleteSubstituteDay"
                        BehaviorID="mpeDeleteSubstituteDay" DropShadow="false" />
                    <asp:Panel ID="pnlDeleteSubstituteDay" runat="server" BackColor="White" BorderColor="Black"
                        Style="padding-top: 20px; padding-left: 10px; padding-right: 10px; min-width: 250px;
                        max-width: 700px; min-height: 60px; display: none;" BorderWidth="2px">
                        <table class="WholeWidth">
                            <tr>
                                <td colspan="2">
                                    Date :
                                    <asp:Label ID="lblDeleteSubstituteDay" Font-Bold="true" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    Work type selected : Holiday
                                </td>
                            </tr>
                            <tr>
                                <td align="center" style="padding: 10px 0px 10px 0px;">
                                    <asp:Button ID="btnDeleteSubstituteDay" OnClick="btnDeleteSubstituteDay_Click" runat="server"
                                        Text="Delete" ToolTip="Delete" />
                                    &nbsp; &nbsp;
                                    <asp:Button ID="btnCancelSubstituteDay" runat="server" Text="Cancel" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>


