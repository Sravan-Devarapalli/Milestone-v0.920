<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="PraticeManagement.Controls.Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>
<%@ Register Src="~/Controls/MonthCalendar.ascx" TagName="MonthCalendar" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
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
        }
        else {
            var date = new Date(hdnDate.value);
            var hdnHolidayDate = document.getElementById('<%= hdnHolidayDate.ClientID %>');

            if (dayLink.attributes['IsFloatingHoliday'].value.toLowerCase() == 'true') {
                var lblDeleteSubstituteDay = document.getElementById('<%= lblDeleteSubstituteDay.ClientID %>');
                lblDeleteSubstituteDay.innerHTML = hdnHolidayDate.value = date.format('MM/dd/yyyy');
                var mpeDeleteSubstituteDay = $find('mpeDeleteSubstituteDay');
                mpeDeleteSubstituteDay.show();
            }
            else if (dayLink.attributes['CompanyDayOff'].value == 'false' && dayLink.attributes['IsWeekEnd'].value == 'false' && dayLink.attributes['HolidayDescription'].value == '') {


                var popupExtendar = $find(peBehaviourId);
                var OkButton = $get(btnOkID);
                var errorMessage = $get(ErrorMessageID);
                var rbPTO = $get(rbPTOClientID);
                var rbFloatingHoliday = $get(rbFloatingHolidayClientID);
                OkButton.attributes['ErrorMessageID'].value = ErrorMessageID;
                OkButton.attributes['SaveDayButtonID'].value = saveDayButtonID;
                OkButton.attributes['TxtActualHoursID'].value = txtActualHoursID;
                OkButton.attributes['ExtendarId'].value = peBehaviourId;
                OkButton.attributes['RbFloatingID'].value = rbFloatingHolidayClientID;
                OkButton.attributes['HiddenDayOffID'].value = hiddenDayOffID;
                OkButton.attributes['PersonId'].value = personId;
                OkButton.attributes['Date'].value = date.format('MM/dd/yyyy');
                rbPTO.attributes['onclick'].value = "disableActualHours( " + txtActualHoursID + ", 'false')";
                rbFloatingHoliday.attributes['onclick'].value = "disableActualHours( " + txtActualHoursID + ", 'true')";

                DeleteButton.disabled = (dayLink.attributes['DayOff'].value == 'false') ? 'disabled' : '';
                if (dayLink.attributes['IsFloatingHoliday'].value.toLowerCase() == 'true') {
                    rbFloatingHoliday.checked = true;
                }
                else {
                    rbPTO.checked = true;
                }
                txtActualHours.value = (dayLink.attributes['DayOff'].value == 'false' || rbFloatingHoliday.checked) ? '8.00' : parseFloat(dayLink.attributes['ActualHours'].value.toString());
                txtActualHours.disabled = rbFloatingHoliday.checked ? 'disabled' : '';
                lblDateDescription.innerHTML = date.format('MM/dd/yyyy');
                errorMessage.style.display = 'none';
                txtHolidayDescription.style.display = 'none';
                chkMakeRecurringHoliday.nextSibling.style.display = 'none'
                chkMakeRecurringHoliday.style.display = 'none';
                popupExtendar.show();
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

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);


    function endRequestHandle(sender, Args) {

        changeAlternateitemscolrsForCBL();
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
                    <script type="text/javascript">
                        function btnOk_EditCondtion() {
                            $find('mpeSelectEditCondtion').hide();
                            var rbEditSingleDay = document.getElementById('<%=rbEditSingleDay.ClientID %>');
                            var rbEditSeries = document.getElementById('<%=rbEditSeries.ClientID %>');
                            alert(rbEditSingleDay.value);
                            if (rbEditSingleDay.value) {
                                $find('mpeEditSingleDay').show();
                            } else {
                                $find('mpeAddTimeOffPopup').show();
                            }

                        }
                    </script>
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
                                <asp:Button ID="btnAddTimeOff" runat="server" Text="Add Time Off" />
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
                                                        <asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" OnClick="btnPrevYear_Click">
                                                            <asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td valign="middle" style="vertical-align: middle !important;" width="15px">
                                                        <asp:Label ID="lblYear" Style="font-size: x-large;" runat="server"></asp:Label>
                                                    </td>
                                                    <td valign="middle" style="text-align: left;">
                                                        <asp:LinkButton ID="btnNextYear" runat="server" CausesValidation="false" OnClick="btnNextYear_Click">
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
                    <asp:Button ID="hdEditSingleDay" runat="server" Text="edit single day" />
                    <asp:Button ID="hdEditCondtion" runat="server" Text="Edit Condition" />
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeSelectEditCondtion" runat="server"
                        TargetControlID="hdEditCondtion" BackgroundCssClass="modalBackground" PopupControlID="pnlSelectEditCondtion"
                        DropShadow="false" BehaviorID="mpeSelectEditCondtion" CancelControlID="btncancel_EditCondtion" />
                    <asp:Panel ID="pnlSelectEditCondtion" runat="server" BackColor="White" BorderColor="Black"
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" Height="270px"
                        Width="320px">
                        <table width="100%" class="calendarPopup">
                            <tr>
                                <td colspan="3" style="height: 20px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%; text-align: right; font-weight: bold; padding-right: 5px;">
                                    Date:
                                </td>
                                <td style="width: 40%; text-align: left; padding-left: 5px;">
                                    <asp:Label ID="lbDate" runat="server" Text="2012/02/23"></asp:Label>
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
                                    <asp:Button ID="btnOk_EditCondtion" Text="OK" runat="server" Style="padding-left: 10px"
                                        OnClientClick="javascript:btnOk_EditCondtion();" />
                                    <asp:Button ID="btncancel_EditCondtion" Text="Cancel" runat="server" Style="padding-left: 10px" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeAddTimeOffPopup" runat="server" TargetControlID="btnAddTimeOff"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlAddTimeOffPopup" DropShadow="false"
                        BehaviorID="mpeAddTimeOffPopup" CancelControlID="btnCancelTimeOff" />
                    <asp:Panel ID="pnlAddTimeOffPopup" runat="server" BackColor="White" BorderColor="Black"
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" Height="270px"
                        Width="320px">
                        <table width="100%" class="calendarPopup">
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
                                    <asp:Button ID="btnOkTimeOff" Text="OK" ValidationGroup="TimeOff" runat="server"
                                        OnClick="btnOkTimeOff_Click" Style="padding-left: 10px" />
                                    <asp:Button ID="btnDeleteTimeOff" Text="Delete" runat="server" ValidationGroup="TimeOff"
                                        OnClick="btnDeleteTimeOff_Click" Style="padding-left: 10px" />
                                    <asp:Button ID="btnCancelTimeOff" Text="Cancel" runat="server" Style="padding-left: 10px" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
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
                        CssClass="ConfirmBoxClassError" Style="display: none;" BorderWidth="2px" Height="200px"
                        Width="320px">
                        <table width="100%" class="calendarPopup">
                            <tr>
                                <td colspan="3" style="height: 20px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center; font-weight: bold; padding-right: 5px; width: 100%;"
                                    colspan="3">
                                    Date:&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lbdateSingleDay" runat="server" Text="2012/02/23"></asp:Label>
                                    <asp:HiddenField ID="hdnDateSingleDay" runat="server" ></asp:HiddenField>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 20px; text-align: left;">
                                    1. Select type of time to be entered:
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-left: 40px; text-align: left;">
                                    <asp:DropDownList ID="ddlTimeTypesSingleDay" runat="server" Style="width: 70%;">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="reqddlTimeTypesSingleDay" runat="server" ControlToValidate="ddlTimeTypesSingleDay"
                                        ErrorMessage="The Work Type is required." ToolTip="The Work Type is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="SingleDay"></asp:RequiredFieldValidator>
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
                                <td width="30%" style="padding-left: 10px; text-align: right; padding-right: 5px;">
                                    Hours:
                                </td>
                                <td width="10%" style="padding-left: 5px; text-align: left;">
                                    <asp:TextBox ID="txtHoursSingleDay" runat="server" Style="width: 50px;"></asp:TextBox>
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
                                <td width="60%">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="height: 10px;">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Button ID="btnOkSingleDay" OnClick="btnOkSingleDay_OnClick" Text="OK" ToolTip="OK" ValidationGroup="SingleDay"
                                        runat="server" Style="padding-left: 10px" />
                                    <asp:Button ID="btnDeleteSingleDay" OnClick="btnDeleteSingleDay_OnClick" ValidationGroup="SingleDay"
                                        Text="Delete" ToolTip="Delete" runat="server" Style="padding-left: 10px" />
                                    <asp:Button ID="btnCancelEditSingleDay" Text="Cancel" ToolTip="Cancel" runat="server" Style="padding-left: 10px" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" style="height: 10px;">
                                    <asp:UpdatePanel ID="upnlErrorSingleDay" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:ValidationSummary ID="valSumErrorSingleDay" runat="server" ValidationGroup="SingleDay" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeHolidayAndSubStituteDay" runat="server"
                        TargetControlID="hdnHolidayDate" CancelControlID="btnSubstituteDayCancel" BackgroundCssClass="modalBackground"
                        PopupControlID="pnlHolidayAndSubStituteDay" BehaviorID="mpeHolidayAndSubStituteDay"
                        DropShadow="false" />
                    <asp:Panel ID="pnlHolidayAndSubStituteDay" runat="server" BackColor="White" BorderColor="Black"
                        Style="padding-top: 20px; padding-left: 10px; padding-right: 10px; display: none;">
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
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnPrevYear" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnNextYear" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="cblRecurringHolidays" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>
<br />
<uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
<br />

