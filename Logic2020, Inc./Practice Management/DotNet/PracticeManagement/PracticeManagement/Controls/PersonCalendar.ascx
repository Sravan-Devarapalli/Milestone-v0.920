<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonCalendar.ascx.cs"
    Inherits="PraticeManagement.Controls.PersonCalendar" %>
<%@ Register Src="~/Controls/PersonMonthCalendar.ascx" TagName="MonthCalendar" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<script type="text/javascript">

    function ShowPopup(dayLink, hiddenDayOffID, hiddenDateID) {
        var hndDayOff = $get(hiddenDayOffID);
        var hdnDate = $get(hiddenDateID);

        hndDayOff.value = dayLink.attributes['DayOff'].value;
        hdnDate.value = dayLink.attributes['Date'].value;

        var date = new Date(hdnDate.value);
        var hdnHolidayDate = document.getElementById('<%= hdnHolidayDate.ClientID %>');

        if (dayLink.attributes['IsFloatingHoliday'].value.toLowerCase() == 'true') {
            var lblDeleteSubstituteDay = document.getElementById('<%= lblDeleteSubstituteDay.ClientID %>');
            var lblDeleteSubstituteDescription = document.getElementById('<%= lblDeleteSubstituteDescription.ClientID %>');
            lblDeleteSubstituteDay.innerHTML = hdnHolidayDate.value = date.format('MM/dd/yyyy');
            lblDeleteSubstituteDescription.innerHTML = EncodeString(dayLink.title.substring(0, dayLink.title.indexOf('Approved')));
            var mpeDeleteSubstituteDay = $find('mpeDeleteSubstituteDay');
            mpeDeleteSubstituteDay.show();
        }
        else {

            return true;
        }
        return false;
    }

    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);


    function endRequestHandle(sender, Args) {

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

    function btnDeleteTimeOffDisable() {
        var btnDeleteTimeOff = document.getElementById('<%=btnDeleteTimeOff.ClientID %>');
        var hdIsTimeOffPopUpDirty = document.getElementById('<%=hdIsTimeOffPopUpDirty.ClientID %>');
        if (btnDeleteTimeOff != null && btnDeleteTimeOff != undefined) {
            btnDeleteTimeOff.setAttribute('disabled', 'disabled');
            hdIsTimeOffPopUpDirty.value = true;
        }
    }

    function btnDeleteSingleDayDisable() {
        var btnDeleteSingleDay = document.getElementById('<%=btnDeleteSingleDay.ClientID %>');
        var hdIsSingleDayPopDirty = document.getElementById('<%=hdIsSingleDayPopDirty.ClientID %>');
        if (btnDeleteSingleDay != null && btnDeleteSingleDay != undefined) {
            btnDeleteSingleDay.setAttribute('disabled', 'disabled');
            hdIsSingleDayPopDirty.value = true;
        }
    }


    function dtpStartDateTimeOff_OnClientChange() {
        var dtpStartDateTimeOff = $find('dtpStartDateTimeOffBehaviorID');
        var dtpEndDateTimeOff = $find('dtpEndDateTimeOffBehaviorID');
        var startDate = new Date(dtpStartDateTimeOff._selectedDate);
        var endDate = new Date(dtpEndDateTimeOff._selectedDate);
        if (startDate > endDate) {
            dtpEndDateTimeOff.set_selectedDate(startDate.format("MM/dd/yyyy"));
        }
    }

    function dtpEndDateTimeOff_OnClientChange() {
        var dtpStartDateTimeOff = $find('dtpStartDateTimeOffBehaviorID');
        var dtpEndDateTimeOff = $find('dtpEndDateTimeOffBehaviorID');
        var startDate = new Date(dtpStartDateTimeOff._selectedDate);
        var endDate = new Date(dtpEndDateTimeOff._selectedDate);
        if (startDate > endDate) {
            dtpStartDateTimeOff.set_selectedDate(endDate.format("MM/dd/yyyy"));
        }
    }

    function DisableButtons(dpModifySubstitutedayBehaviour) {
        var dpModifySubstituteday = $find(dpModifySubstitutedayBehaviour);
        var okButton = document.getElementById('<%= btnModifySubstituteDayOk.ClientID %>');
        var deleteButton = document.getElementById('<%= btnModifySubstituteDayDelete.ClientID %>');
        var previousSubstitutedate = document.getElementById('<%= hdnSubstituteDate.ClientID %>');
        var lblModifySubstituteday = document.getElementById('<%= lblModifySubstituteday.ClientID %>');

        if (dpModifySubstituteday != null && okButton != null && deleteButton != null && previousSubstitutedate != null && lblModifySubstituteday != null) {
            previousSubstitutedate = new Date(previousSubstitutedate.value).format("MM/dd/yyyy");
            var currentSubstituteDate = dpModifySubstituteday.get_selectedDate().format("MM/dd/yyyy");
            lblModifySubstituteday.innerHTML = currentSubstituteDate;

            if (previousSubstitutedate == currentSubstituteDate) {
                deleteButton.disabled = false;
                okButton.disabled = true;
            }
            else {
                okButton.disabled = false;
                deleteButton.disabled = true;
            }
        }
    }

</script>
<table class="Width98Percent">
    <tr>
        <td class="Width15Percent">
            &nbsp;
        </td>
        <td class="vTop Width70Percent TextAlignCenter">
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
                    <table class="CalendarTable Width98Percent TextAlignCenter" align="center">
                        <tr>
                            <td class="textRightImp ValignMiddleImp width30P">
                                Select a Person:
                            </td>
                            <td class="textLeft no-wrap">
                                <pmc:CustomDropDown ID="ddlPerson" runat="server" IsOptionGroupRequired="false">
                                </pmc:CustomDropDown>
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
                            <td class="BtnAddTimeOff">
                                <asp:Button ID="btnAddTimeOff" runat="server" Text="Add Time Off" OnClick="btnAddTimeOff_Click"
                                    ToolTip="Add Time Off" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="PaddingTopBottom10PxImp TextAlignCenterImp">
                                <div id="trAlert" runat="server">
                                    <asp:Label ID="lbAlert1" runat="server" Text="Alert :" CssClass="Bench"></asp:Label>
                                    <asp:Label ID="lbAlert2" runat="server" Text=" You are viewing this calendar as READ-ONLY.  If you believe you should have permissions to make changes to this calendar, please "></asp:Label>
                                    <asp:HyperLink ID="contactSupportMailToLink" runat="server" Text="contact support"
                                        CssClass="color0898E6"></asp:HyperLink>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="TextAlignCenterImp">
                                <asp:Label ID="lblConsultantMessage" runat="server" Visible="false" Text="You can review your vacation days, but cannot change them. Please see your Practice Manager for updates to your vacation schedule."></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table align="center" class="Width98Percent">
                        <tr>
                            <td class="Width15Percent">
                                &nbsp;
                            </td>
                            <td class="Width70Percent" align="center">
                                <table class="CalendarTable">
                                    <tr>
                                        <td colspan="3" class="TextAlignCenterImp">
                                            <table class="Width100Per">
                                                <tr>
                                                    <td class="vMiddle textRightImp">
                                                        <asp:LinkButton ID="btnPrevYear" runat="server" CausesValidation="false" OnClick="btnPrevYear_Click"
                                                            ToolTip="Previous Year">
                                                            <asp:Image ID="imgPrevYear" runat="server" ImageUrl="~/Images/previous.gif" />
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td class="ValignMiddleImp Width15Px">
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
                                        <td rowspan="9" class="setCheckboxesLeft PaddingTop45Left2">
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
                    <asp:HiddenField ID="hdDeleteSubstituteDay" runat="server" />
                    <asp:HiddenField ID="hdEditCondtion" runat="server" />
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeSelectEditCondtion" runat="server"
                        TargetControlID="hdEditCondtion" BackgroundCssClass="modalBackground" PopupControlID="pnlSelectEditCondtion"
                        DropShadow="false" BehaviorID="mpeSelectEditCondtion" CancelControlID="btncancel_EditCondtion" />
                    <asp:Panel ID="pnlSelectEditCondtion" runat="server" CssClass="ConfirmBoxClassError Height175Width320Px PanelPerson">
                        <table class="calendarPopup">
                            <tr>
                                <td colspan="3" class="Width20Px">
                                </td>
                            </tr>
                            <tr>
                                <td class="TextAlignLeft padLeft5" colspan="3">
                                    <asp:Label ID="lbDate" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="RbEdit">
                                    <asp:RadioButton ID="rbEditSeries" runat="server" Text="Edit Series" GroupName="EditDay" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="RbEdit">
                                    <asp:RadioButton ID="rbEditSingleDay" runat="server" Text="Edit Single Day" GroupName="EditDay"
                                        Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="height10Px">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <asp:Button ID="btnOk_EditCondtion" Text="OK" runat="server" ToolTip="Ok" OnClientClick="return  btnOk_EditCondtion();" />&nbsp;
                                    &nbsp;
                                    <asp:Button ID="btncancel_EditCondtion" Text="Cancel" runat="server" ToolTip="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:UpdatePanel ID="upnlTimeOff" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hfAddTimeOff" runat="server" />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeAddTimeOff" runat="server" TargetControlID="hfAddTimeOff"
                                BackgroundCssClass="modalBackground" PopupControlID="pnlAddTimeOff" DropShadow="false"
                                BehaviorID="mpeAddTimeOff" CancelControlID="btnCancelTimeOff" />
                            <asp:Panel ID="pnlAddTimeOff" runat="server" CssClass="ConfirmBoxClassError PanelPerson">
                                <table class="calendarPopup">
                                    <tr>
                                        <td colspan="3" class="height20P">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <table class="Width100Per">
                                                <tr>
                                                    <td class="singleRowTableTd AddTimeOffStartDate">
                                                        Start Date:
                                                    </td>
                                                    <td class="singleRowTableTd AddTimeOffDatePicker">
                                                        <uc:DatePicker ID="dtpStartDateTimeOff" runat="server" TextBoxWidth="75px" OnClientChange="btnDeleteTimeOffDisable(); dtpStartDateTimeOff_OnClientChange();"
                                                            BehaviorID="dtpStartDateTimeOffBehaviorID" />
                                                        <asp:RequiredFieldValidator ID="reqStartDateTimeOff" runat="server" ControlToValidate="dtpStartDateTimeOff"
                                                            ErrorMessage="Start Date is required." ToolTip="Start Date is required." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compStartDateTimeOff" runat="server" ControlToValidate="dtpStartDateTimeOff"
                                                            ErrorMessage="Start Date has an incorrect format. It must be 'MM/dd/yyyy'." ToolTip="Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="TimeOff"></asp:CompareValidator>
                                                    </td>
                                                    <td class="PaddingBottomTop0PxImp">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <table class="Width100Per">
                                                <tr>
                                                    <td class="singleRowTableTd AddTimeOffStartDate">
                                                        End Date:
                                                    </td>
                                                    <td class="singleRowTableTd AddTimeOffDatePicker">
                                                        <uc:DatePicker ID="dtpEndDateTimeOff" runat="server" TextBoxWidth="75px" OnClientChange="btnDeleteTimeOffDisable(); dtpEndDateTimeOff_OnClientChange();"
                                                            BehaviorID="dtpEndDateTimeOffBehaviorID" />
                                                        <asp:RequiredFieldValidator ID="reqEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                                            ErrorMessage="End Date is required." ToolTip="End Date is required." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                                            ErrorMessage="End Date has an incorrect format. It must be 'MM/dd/yyyy'." ToolTip="End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="TimeOff"></asp:CompareValidator>
                                                        <asp:CompareValidator ID="compStartDateEndDateTimeOff" runat="server" ControlToValidate="dtpEndDateTimeOff"
                                                            ControlToCompare="dtpStartDateTimeOff" ErrorMessage="End Date must be greater than or equal to the Start Date."
                                                            ToolTip="End Date must be greater than or equal to the Start Date." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff"
                                                            Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                                                        <asp:CustomValidator ID="cvStartDateEndDateTimeOff" runat="server" ErrorMessage="Selected day(s) are not working day(s). Please select any working day(s)."
                                                            ToolTip="Selected day(s) are not working day(s). Please select any working day(s)."
                                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                            ValidationGroup="TimeOff" OnServerValidate="cvStartDateEndDateTimeOff_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvNotW2Salary" runat="server" Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff" OnServerValidate="cvNotW2Salary_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvNotW2Hourly" runat="server" Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff" OnServerValidate="cvNotW2Hourly_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvPersonNotHired" runat="server" ErrorMessage="Person is not hired in the selected day(s)."
                                                            ToolTip="Person is not hired in the selected day(s)." Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff" OnServerValidate="cvPersonNotHired_ServerValidate"></asp:CustomValidator>
                                                        <asp:CustomValidator ID="cvPersonTerminated" runat="server" ErrorMessage="Person terminated in the selected day(s)."
                                                            ToolTip="Person terminated in the selected day(s)." Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Dynamic" ValidationGroup="TimeOff" OnServerValidate="cvPersonTerminated_ServerValidate"></asp:CustomValidator>
                                                    </td>
                                                    <td class="PaddingBottomTop0PxImp">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="padLeft20 TextAlignLeft">
                                            1. Select type of time to be entered:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="padLeft40 TextAlignLeft">
                                            <pmc:CustomDropDown ID="ddlTimeTypesTimeOff" runat="server" IsOptionGroupRequired="false"
                                                CssClass="width70P" onchange="btnDeleteTimeOffDisable();">
                                            </pmc:CustomDropDown>
                                            <asp:RequiredFieldValidator ID="reqTimeTypesTimeOff" runat="server" ControlToValidate="ddlTimeTypesTimeOff"
                                                ErrorMessage="Work Type is required." ToolTip="Work Type is required." Text="*"
                                                EnableClientScript="false" SetFocusOnError="true" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="height10Px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="padLeft20 TextAlignLeft">
                                            2. Enter the number of hours (per day, if applicable):
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="AddTimeOffHours">
                                            Hours:&nbsp;
                                            <asp:TextBox ID="txthoursTimeOff" runat="server" CssClass="width50Px" MaxLength="4"
                                                onchange="btnDeleteTimeOffDisable();"></asp:TextBox>&nbsp;&nbsp;(8.00 Hrs Max,
                                            0.25 Hr incr.)
                                            <asp:RequiredFieldValidator ID="reqtxthoursTimeOff" runat="server" ControlToValidate="txthoursTimeOff"
                                                ErrorMessage="Hours is required." ToolTip="Hours is required." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" ValidationGroup="TimeOff"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txthoursTimeOff"
                                                ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                                ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                Type="Currency" ValidationGroup="TimeOff"></asp:CompareValidator>
                                            <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txthoursTimeOff"
                                                ErrorMessage="Hours must be between 0.25 and 8.00." ToolTip="Hours must be between 0.25 and 8.00."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                MinimumValue="0.25" MaximumValue="8" Type="Double" ValidationGroup="TimeOff"></asp:RangeValidator>
                                            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursPerDayInsert" runat="server"
                                                TargetControlID="txthoursTimeOff" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                                ValidChars=".">
                                            </AjaxControlToolkit:FilteredTextBoxExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="height10Px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="ValSumTimeOff">
                                            <asp:ValidationSummary ID="valSumTimeOff" runat="server" ValidationGroup="TimeOff" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="Padding10PxImp">
                                            <asp:HiddenField ID="hdIsTimeOffPopUpDirty" runat="server" />
                                            <asp:Button ID="btnOkTimeOff" Text="OK" runat="server" ToolTip="Ok" OnClick="btnOkTimeOff_Click" />
                                            <asp:Button ID="btnDeleteTimeOff" Text="Delete" runat="server" ToolTip="Delete" OnClick="btnDeleteTimeOff_Click" />
                                            <asp:Button ID="btnCancelTimeOff" Text="Cancel" runat="server" ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upnlErrorSingleDay" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hdEditSingleDay" runat="server" />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeEditSingleDay" runat="server" TargetControlID="hdEditSingleDay"
                                BackgroundCssClass="modalBackground" PopupControlID="pnlEditSingleDay" DropShadow="false"
                                BehaviorID="mpeEditSingleDay" CancelControlID="btnCancelEditSingleDay" />
                            <asp:Panel ID="pnlEditSingleDay" runat="server" CssClass="ConfirmBoxClassError PanelPerson"
                                Style="display: none;">
                                <table class="calendarPopup">
                                    <tr>
                                        <td colspan="3" class="height20P">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="textCenter bold padRight5 width100P" colspan="3">
                                            Date:&nbsp;&nbsp;&nbsp;
                                            <asp:Label ID="lbdateSingleDay" runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdnDateSingleDay" runat="server"></asp:HiddenField>
                                            <asp:CustomValidator ID="cvSingleDay" runat="server" Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="SingleDay" OnServerValidate="cvSingleDay_ServerValidate"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="padLeft20 textLeft">
                                            1. Select type of time to be entered:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="textLeft padLeft40">
                                            <pmc:CustomDropDown ID="ddlTimeTypesSingleDay" IsOptionGroupRequired="false" CssClass="width70P"
                                                runat="server" onchange="btnDeleteSingleDayDisable();">
                                            </pmc:CustomDropDown>
                                            <asp:RequiredFieldValidator ID="reqddlTimeTypesSingleDay" runat="server" ControlToValidate="ddlTimeTypesSingleDay"
                                                ErrorMessage="Work Type is required." ToolTip="Work Type is required." Text="*"
                                                EnableClientScript="false" SetFocusOnError="true" ValidationGroup="SingleDay"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="height10P">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="padLeft20 textLeft">
                                            2. Enter the number of hours (per day, if applicable):
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="AddTimeOffHours">
                                            Hours:&nbsp;
                                            <asp:TextBox ID="txtHoursSingleDay" runat="server" CssClass="width50Px" MaxLength="4"
                                                onchange="btnDeleteSingleDayDisable();"></asp:TextBox>&nbsp;&nbsp;(8.00 Hrs
                                            Max, 0.25 Hr incr.)
                                            <asp:RequiredFieldValidator ID="reqHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                                ErrorMessage="Hours is required." ToolTip="Hours is required." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" ValidationGroup="SingleDay"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                                ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                                                ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                Type="Currency" ValidationGroup="SingleDay"></asp:CompareValidator>
                                            <asp:RangeValidator ID="rangeHoursSingleDay" runat="server" ControlToValidate="txtHoursSingleDay"
                                                ErrorMessage="Hours must be between 0.25 and 8.00." ToolTip="Hours must be between 0.25 and 8.00."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                MinimumValue="0.25" MaximumValue="8" Type="Double" ValidationGroup="SingleDay"></asp:RangeValidator>
                                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteHoursSingleDay" runat="server"
                                                TargetControlID="txtHoursSingleDay" FilterMode="ValidChars" FilterType="Custom,Numbers"
                                                ValidChars=".">
                                            </AjaxControlToolkit:FilteredTextBoxExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="height10Px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="textLeft padLeft10">
                                            <asp:ValidationSummary ID="valSumErrorSingleDay" runat="server" ValidationGroup="SingleDay" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <asp:HiddenField ID="hdIsSingleDayPopDirty" runat="server" />
                                            <asp:Button ID="btnOkSingleDay" OnClick="btnOkSingleDay_OnClick" Text="OK" ToolTip="OK"
                                                runat="server" />
                                            <asp:Button ID="btnDeleteSingleDay" OnClick="btnDeleteSingleDay_OnClick" Text="Delete"
                                                ToolTip="Delete" runat="server" />
                                            <asp:Button ID="btnCancelEditSingleDay" Text="Cancel" ToolTip="Cancel" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" class="height10Px">
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upnlValsummary" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hndDayOff" runat="server" />
                            <asp:HiddenField ID="hdnDate" runat="server" />
                            <asp:HiddenField ID="hdnHolidayDate" runat="server" />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeHolidayAndSubStituteDay" runat="server"
                                TargetControlID="hdnHolidayDate" CancelControlID="btnSubstituteDayCancel" BackgroundCssClass="modalBackground"
                                PopupControlID="pnlHolidayAndSubStituteDay" BehaviorID="mpeHolidayAndSubStituteDay"
                                DropShadow="false" />
                            <asp:Panel ID="pnlHolidayAndSubStituteDay" runat="server" Style="display: none;"
                                CssClass="PnlHolidayAndSubStituteDayPadding PanelPerson HolidayAndSubstituteDay">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="HolidayLabel">
                                            You have selected :
                                        </td>
                                        <td class="LabelHolidayDate">
                                            <asp:Label ID="lblHolidayDate" CssClass="fontBold" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td class="LabelHolidayDate">
                                            <asp:Label ID="lblHolidayName" CssClass="fontBold" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="height20P">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="height20P TextAlignLeft">
                                            Please select substitute day for this holiday :
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="DatepickerSubstituteDay">
                                            <uc:DatePicker ID="dpSubstituteDay" runat="server" OnClientChange="return true;"
                                                ValidationGroup="Substituteday" AutoPostBack="false" TextBoxWidth="90px" />
                                            <asp:RequiredFieldValidator ID="rfvSubstituteDay" runat="server" ControlToValidate="dpSubstituteDay"
                                                ErrorMessage="Substitute Date is required." ToolTip="Substitute Date is required."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                ValidationGroup="Substituteday"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compdpSubstituteDay" runat="server" ControlToValidate="dpSubstituteDay"
                                                ErrorMessage="Substitute day Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                ToolTip="Substitute day Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                Operator="DataTypeCheck" Type="Date" ValidationGroup="Substituteday"></asp:CompareValidator>
                                            <asp:CustomValidator ID="cvSubstituteDay" EnableClientScript="false" EnableViewState="false"
                                                Text="*" ValidateEmptyText="true" ToolTip="The selected date is not a working day."
                                                ErrorMessage="The selected date is not a working day." ValidationGroup="Substituteday"
                                                runat="server" OnServerValidate="cvSubstituteDay_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvSubstituteDayNotHavingW2Salary" runat="server" ErrorMessage="Invalid Pay Type: Employee is not a W2-Salary employee for the selected Substitute day."
                                                ToolTip="Invalid Pay Type: Employee is not a W2-Salary employee for the selected Substitute day."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                ValidationGroup="Substituteday" OnServerValidate="cvSubstituteDayNotHavingW2Salary_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvValidateSubDateWithHireDate" runat="server" ErrorMessage="Person is not hired in the selected  Substitute day."
                                                ToolTip="Person is not hired in the selected Substitute day." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="Substituteday" OnServerValidate="cvValidateSubDateWithHireDate_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvValidateSubDateWithTermDate" runat="server" ErrorMessage="Person terminated in the selected  Substitute day."
                                                ToolTip="Person terminated in the selected Substitute day." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="Substituteday" OnServerValidate="cvValidateSubDateWithTermDate_ServerValidate"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="TextAlignCenter vMiddle">
                                            <asp:ValidationSummary ID="valSumsubstituteday" runat="server" ValidationGroup="Substituteday" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="DatepickerSubstituteDay">
                                            <asp:Button ID="btnSubstituteDayOK" OnClick="btnSubstituteDayOK_Click" runat="server"
                                                ValidationGroup="Substituteday" ToolTip="OK" Text="OK" />
                                            &nbsp; &nbsp;
                                            <asp:Button ID="btnSubstituteDayCancel" runat="server" Text="Cancel" ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="Height5Px">
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="upnlModifySubstituteDay" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hdnHolidayDay" runat="server" />
                            <asp:HiddenField ID="hdnSubstituteDate" runat="server" />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeModifySubstituteDay" runat="server"
                                TargetControlID="hdnSubstituteDate" CancelControlID="btnModifySubstituteDayCancel"
                                BackgroundCssClass="modalBackground" PopupControlID="pnlModifySubstituteDay"
                                BehaviorID="mpeModifySubstituteDay" DropShadow="false" />
                            <asp:Panel ID="pnlModifySubstituteDay" runat="server" Style="display: none;" CssClass="PnlHolidayAndSubStituteDayPadding PanelPerson HolidayAndSubstituteDay">
                                <table class="WholeWidth">
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblHolidayDetails" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="LabelModifySubstituteDate Width75Percent">
                                            Substitute Date:&nbsp;
                                            <asp:Label ID="lblModifySubstituteday" runat="server" CssClass="fontBold"></asp:Label>
                                        </td>
                                        <td class="LabelModifySubstituteDate Width25Percent textLeft">
                                            <uc:DatePicker ID="dpModifySubstituteday" runat="server" OnClientChange="DisableButtons('dpModifySubstituteday')"
                                                VisibleTextBox="hidden" ValidationGroup="Substituteday" AutoPostBack="false"
                                                TextBoxWidth="0px" BehaviorID="dpModifySubstituteday" />
                                            <asp:CustomValidator ID="cvModifySubstituteday" EnableClientScript="false" EnableViewState="false"
                                                Text="*" ValidateEmptyText="true" ToolTip="The selected date is not a working day."
                                                ErrorMessage="The selected date is not a working day." ValidationGroup="ModifySubstituteDay"
                                                runat="server" OnServerValidate="cvModifySubstituteday_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvValidateModifiedSubDateWithHireDate" runat="server" ErrorMessage="Person is not hired in the selected  Substitute day."
                                                ToolTip="Person is not hired in the selected Substitute day." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="ModifySubstituteDay"
                                                OnServerValidate="cvValidateModifiedSubDateWithHireDate_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvValidateModifiedSubDateWithTermDate" runat="server" ErrorMessage="Person terminated in the selected  Substitute day."
                                                ToolTip="Person terminated in the selected Substitute day." Text="*" EnableClientScript="false"
                                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="ModifySubstituteDay"
                                                OnServerValidate="cvValidateModifiedSubDateWithTermDate_ServerValidate"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvModifiedSubstituteDayNotHavingW2Salary" runat="server"
                                                ErrorMessage="Invalid Pay Type: Employee is not a W2-Salary employee for the selected Substitute day."
                                                ToolTip="Invalid Pay Type: Employee is not a W2-Salary employee for the selected Substitute day."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                ValidationGroup="ModifySubstituteDay" OnServerValidate="cvModifiedSubstituteDayNotHavingW2Salary_ServerValidate"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:ValidationSummary ID="valSumModifySubstituteDay" runat="server" ValidationGroup="ModifySubstituteDay" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="Padding10PxImp" colspan="2">
                                            <asp:Button ID="btnModifySubstituteDayOk" runat="server" Text="OK" ToolTip="OK" ValidationGroup="ModifySubstituteDay"
                                                OnClick="btnModifySubstituteDayOk_Click" />
                                            &nbsp;<asp:Button ID="btnModifySubstituteDayDelete" runat="server" Text="Delete"
                                                ToolTip="Delete" OnClick="btnModifySubstituteDayDelete_Click" />
                                            &nbsp;<asp:Button ID="btnModifySubstituteDayCancel" runat="server" Text="Cancel"
                                                ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <AjaxControlToolkit:ModalPopupExtender ID="mpeDeleteSubstituteDay" runat="server"
                        TargetControlID="hdDeleteSubstituteDay" CancelControlID="btnCancelSubstituteDay"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlDeleteSubstituteDay"
                        BehaviorID="mpeDeleteSubstituteDay" DropShadow="false" />
                    <asp:Panel ID="pnlDeleteSubstituteDay" runat="server" Style="display: none;" CssClass="PnlDeleteSubstitutionDate PnlHolidayAndSubStituteDayPadding PanelPerson">
                        <table class="WholeWidth">
                            <tr>
                                <td class="TextAlignCenter">
                                    Date<span class="fontBold">:
                                        <asp:Label ID="lblDeleteSubstituteDay" runat="server"></asp:Label></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="Height5Px">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDeleteSubstituteDescription" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="Height5Px">
                                </td>
                            </tr>
                            <tr>
                                <td align="center" class="Padding10px0px10px0px">
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

