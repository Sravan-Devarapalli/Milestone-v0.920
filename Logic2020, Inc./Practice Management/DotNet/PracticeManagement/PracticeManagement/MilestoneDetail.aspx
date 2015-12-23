<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="MilestoneDetail.aspx.cs" Inherits="PraticeManagement.MilestoneDetail"
    Title="Milestone Details | Practice Management" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/ProjectInfo.ascx" TagName="ProjectInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="Controls/MilestonePersons/MilestonePersonActivity.ascx" TagName="Activity"
    TagPrefix="mp" %>
<%@ Register Src="Controls/Milestones/MilestoneExpenses.ascx" TagName="Expenses"
    TagPrefix="m" %>
<%@ Register Src="Controls/MilestonePersons/CumulativeDailyActivity.ascx" TagName="Cumulative"
    TagPrefix="mp" %>
<%@ Register Src="Controls/MilestonePersons/CumulativeActivity.ascx" TagName="CumulativeTotal"
    TagPrefix="mp" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="uc" TagName="MessageLabel" Src="~/Controls/MessageLabel.ascx" %>
<%@ Register Src="~/Controls/Milestones/MilestonePersonList.ascx" TagName="MilestonePersonList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Milestone Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Milestone Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function checkDirty(mpId) {
            if (showDialod()) {
                __doPostBack('__Page', mpId);
                return true;
            }
            return false;
        }
        function dtpStartDate_OnClientChange(dtp) {
            var index = dtp.id.indexOf('dpPerson');
            var row = dtp.id.substring(0, index);
            var chbBadgeRequired = document.getElementById(row + 'chbBadgeRequired');
            var chbOpsApproved = document.getElementById(row + 'chbOpsApproved');
            var dpPersonStart = document.getElementById(row + 'dpPersonStart_txtDate');
            var dpPersonEnd = document.getElementById(row + 'dpPersonEnd_txtDate');
            var dpBadgeStart = document.getElementById(row + 'dpBadgeStart_txtDate');
            var dpBadgeEnd = document.getElementById(row + 'dpBadgeEnd_txtDate');
            var hdnStartDateValue = document.getElementById(row + 'hdnStartDateValue');
            var hdnEndDateValue = document.getElementById(row + 'hdnEndDateValue');
            var hdnApproved = document.getElementById(row + 'hdnApproved');
            var hdnPersonId = document.getElementById(row + 'hdnPersonId');
            var ddlPersonName = document.getElementById(row + 'ddlPersonName');
            if (hdnPersonId.value != ddlPersonName.value)
                return;
            var oldPersonStartDate = new Date(hdnStartDateValue.value);
            var oldPersonEnddate = new Date(hdnEndDateValue.value);
            var newPersonStartdate = new Date(dpPersonStart.value);
            var newPersonEnddate = new Date(dpPersonEnd.value);
            var previousApproved = hdnApproved.value == 'Yes';
            if (chbBadgeRequired.checked) {
                if (oldPersonStartDate != newPersonStartdate || oldPersonEnddate != newPersonEnddate) {
                    dpBadgeStart.value = dpPersonStart.value;
                    dpBadgeEnd.value = dpPersonEnd.value;
                    dtpBadgeStartDate_OnClientChange(dpBadgeStart);
                }
                else {
                    chbOpsApproved.checked = previousApproved;
                }
            }
        }

        function dtpBadgeStartDate_OnClientChange(dtp) {
            var index = dtp.id.indexOf('dpBadge');
            var row = dtp.id.substring(0, index);
            var chbBadgeRequired = document.getElementById(row + 'chbBadgeRequired');
            var chbOpsApproved = document.getElementById(row + 'chbOpsApproved');
            var dpBadgeStart = document.getElementById(row + 'dpBadgeStart_txtDate');
            var dpBadgeEnd = document.getElementById(row + 'dpBadgeEnd_txtDate');
            var hdnStartDateValue = document.getElementById(row + 'hdnBadgeStartDateValue');
            var hdnEndDateValue = document.getElementById(row + 'hdnBadgeEndDateValue');
            var hdnApproved = document.getElementById(row + 'hdnApproved');
            var hdnApprovedChange = document.getElementById(row + 'hdnApprovedChange');
            var hdnPersonId = document.getElementById(row + 'hdnPersonId');
            var ddlPersonName = document.getElementById(row + 'ddlPersonName');
            if (hdnPersonId.value != ddlPersonName.value)
                return;
            var oldPersonStartDate = new Date(hdnStartDateValue.value);
            var oldPersonEnddate = new Date(hdnEndDateValue.value);
            var newPersonStartdate = new Date(dpBadgeStart.value);
            var newPersonEnddate = new Date(dpBadgeEnd.value);
            var previousApproved = hdnApproved.value == 'Yes';
            if (chbBadgeRequired.checked) {
                if (oldPersonStartDate != newPersonStartdate || oldPersonEnddate != newPersonEnddate) {
                    if (oldPersonStartDate > newPersonStartdate || oldPersonEnddate < newPersonEnddate) {
                        hdnApprovedChange.value = 'false';
                        chbOpsApproved.checked = false;
                    }
                    else {
                        hdnApprovedChange.value = 'true';
                        chbOpsApproved.checked = previousApproved;
                    }
                }
                else {
                    chbOpsApproved.checked = previousApproved;
                }
            }
        }

        function dtpStartDateInsert_OnClientChange(dtp) {
            var index = dtp.id.indexOf('dpPerson');
            var row = dtp.id.substring(0, index);
            var chbBadgeRequired = document.getElementById(row + 'chbBadgeRequiredInsert');
            var dpPersonStart = document.getElementById(row + 'dpPersonStartInsert_txtDate');
            var dpPersonEnd = document.getElementById(row + 'dpPersonEndInsert_txtDate');
            var dpBadgeStart = document.getElementById(row + 'dpBadgeStartInsert_txtDate');
            var dpBadgeEnd = document.getElementById(row + 'dpBadgeEndInsert_txtDate');
            if (chbBadgeRequired.checked) {
                dpBadgeStart.value = dpPersonStart.value;
                dpBadgeEnd.value = dpPersonEnd.value;
            }
        }

        function ShowConfirmDialogForStartDate(chb) {
            if (confirm("Are you sure you want to Cancel Changes?")) {
                var btnCancelSaving = document.getElementById("<%= btnCancelSaving.ClientID %>");
                btnCancelSaving.click();
            }
            else {
                chb.checked = false;
                document.getElementById("<%= rbtnRemovePersonsStartDate.ClientID %>").checked = true;

            }

        }

        function ShowConfirmDialogForEndDate(chb) {
            if (confirm("Are you sure you want to Cancel Changes?")) {
                var btnCancelSaving = document.getElementById("<%= btnCancelSaving.ClientID %>");
                btnCancelSaving.click();
            }
            else {
                chb.checked = false;
                document.getElementById("<%= rbtnRemovePersonsEndDate.ClientID %>").checked = true;
            }

        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

        function SetWrapText(str) {
            for (var i = 30; i < str.length; i = i + 10) {
                str = str.slice(0, i) + "<wbr />" + str.slice(i, str.length);
            }
            return str;
        }

        function GetWrappedText(childObj) {
            if (childObj != null) {

                for (var i = 0; i < childObj.children.length; i++) {
                    if (childObj.children[i] != null) {
                        if (childObj.children[i].children.length == 0) {
                            if (childObj.children[i].innerHTML != null && childObj.children[i].innerHTML != "undefined" && childObj.children[i].innerHTML.length > 70) {
                                childObj.children[i].innerHTML = SetWrapText(childObj.children[i].innerHTML);
                            }
                        }
                    }

                }
            }
        }

        function ModifyInnerTextToWrapText() {
            if (navigator.userAgent.indexOf(" Firefox/") > -1) {
                var tbl = $("table[id*='gvActivities']");
                if (tbl != null && tbl.length > 0) {
                    var gvActivitiesclientId = tbl[0].id;
                    var lastTds = $('#' + gvActivitiesclientId + ' tr td:nth-child(3)');

                    for (var i = 0; i < lastTds.length; i++) {
                        GetWrappedText(lastTds[i]);
                    }
                }
            }
        }

        function ChangeActiveViewIndex(cntrl) {
            var mpEntryId = cntrl.attributes["mpEntryId"].value;
            var hdnEditEntryIdIndex = document.getElementById("<%= hdnEditEntryIdIndex.ClientID %>");
            hdnEditEntryIdIndex.value = mpEntryId;
            return true;
        }

        function CheckIfDatesValid() {

            txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
            txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
            var startDate = new Date(txtStartDate.value);
            var endDate = new Date(txtEndDate.value);
            if (txtStartDate.value != '' && txtEndDate.value != ''
            && startDate <= endDate) {
                var btnCustDatesClose = document.getElementById('<%= activityLog.ClientID %>_btnCustDatesClose');
                hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
                hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
                lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                var startDate = new Date(txtStartDate.value);
                var endDate = new Date(txtEndDate.value);
                var startDateStr = startDate.format("MM/dd/yyyy");
                var endDateStr = endDate.format("MM/dd/yyyy");
                hdnStartDate.value = startDateStr;
                hdnEndDate.value = endDateStr;
                lblCustomDateRange.innerHTML = '(' + startDateStr + '&nbsp;-&nbsp;' + endDateStr + ')';
                btnCustDatesClose.click();

            }
            return false;
        }

        function CheckAndShowCustomDatesPoup(ddlPeriod) {
            imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
            lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
            if (ddlPeriod.value == '0') {
                imgCalender.attributes["class"].value = "";
                lblCustomDateRange.attributes["class"].value = "fontBold";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "";
                    lblCustomDateRange.style.display = "";
                    imgCalender.click();
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('click', true, true);
                    imgCalender.dispatchEvent(event);
                }
            }
            else {
                imgCalender.attributes["class"].value = "displayNone";
                lblCustomDateRange.attributes["class"].value = "displayNone";
                if (imgCalender.fireEvent) {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }
        }
        function ReAssignStartDateEndDates() {
            hdnStartDate = document.getElementById('<%= activityLog.ClientID %>_hdnStartDate');
            hdnEndDate = document.getElementById('<%= activityLog.ClientID %>_hdnEndDate');
            txtStartDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbFrom');
            txtEndDate = document.getElementById('<%= activityLog.ClientID %>_diRange_tbTo');
            hdnStartDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnStartDateCalExtenderBehaviourId');
            hdnEndDateCalExtenderBehaviourId = document.getElementById('<%= activityLog.ClientID %>_hdnEndDateCalExtenderBehaviourId');

            var endDateCalExtender = $find(hdnEndDateCalExtenderBehaviourId.value);
            var startDateCalExtender = $find(hdnStartDateCalExtenderBehaviourId.value);
            if (startDateCalExtender != null) {
                startDateCalExtender.set_selectedDate(hdnStartDate.value);
            }
            if (endDateCalExtender != null) {
                endDateCalExtender.set_selectedDate(hdnEndDate.value);
            }
            CheckIfDatesValid();
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            var activityLog = document.getElementById('<%= activityLog.ClientID%>');
            if (activityLog != null) {
                imgCalender = document.getElementById('<%= activityLog.ClientID %>_imgCalender');
                lblCustomDateRange = document.getElementById('<%= activityLog.ClientID %>_lblCustomDateRange');
                ddlPeriod = document.getElementById('<%=  activityLog.ClientID %>_ddlPeriod');
                if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                    imgCalender.style.display = "none";
                    lblCustomDateRange.style.display = "none";
                }
            }
        }

        //milestonePersonList.ascx
        function imgMilestonePersonDelete_OnClientClick(imgDelete) {
            var hdMilestonePersonEntryId = document.getElementById('<%= MilestonePersonEntryListControl.ClientID%>' + '_hdMilestonePersonEntryId');
            var btnDeleteAllPersonEntries = document.getElementById('<%= MilestonePersonEntryListControl.ClientID%>' + '_btnDeleteAllPersonEntries');
            var btnDeletePersonEntry = document.getElementById('<%= MilestonePersonEntryListControl.ClientID%>' + '_btnDeletePersonEntry');
            var trDeleteOriginalEntry = document.getElementById('trDeleteOriginalEntry');
            var trDeleteExtendedEntry = document.getElementById('trDeleteExtendedEntry');
            var IsOriginalResource = imgDelete.getAttribute("IsOriginalResource");
            hdMilestonePersonEntryId.value = imgDelete.getAttribute("MilestonePersonEntryId");

            if (IsOriginalResource == 'false') {
                btnDeleteAllPersonEntries.style.display = "none";
                btnDeletePersonEntry.value = "   OK   ";
                trDeleteOriginalEntry.style.display = "none";
                trDeleteExtendedEntry.style.display = "block";
            }
            else {
                btnDeleteAllPersonEntries.style.display = "block";
                btnDeletePersonEntry.value = "Delete";
                trDeleteOriginalEntry.style.display = "block";
                trDeleteExtendedEntry.style.display = "none";
            }

            $find('mpeDeleteMileStonePersons').show();
            return false;
        }
        function btnClose_OnClientClick() {
            $find('mpeDeleteMileStonePersons').hide();
            return false;
        }

        function ShowPanel(object, displaypnl, lblTimeOffHoursId, lblProjectAffectedHoursId, TimeOffHours, ProjectAffectedHours) {
            var lblTimeOffHours = document.getElementById(lblTimeOffHoursId);
            var lblProjectAffectedHours = document.getElementById(lblProjectAffectedHoursId);
            lblTimeOffHours.innerHTML = TimeOffHours;
            lblProjectAffectedHours.innerHTML = ProjectAffectedHours;
            var obj = $("#" + object);
            var displayPanel = $("#" + displaypnl);

            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left - 70;
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }

        function HidePanel(hiddenpnl) {

            var displayPanel = $("#" + hiddenpnl);
            displayPanel.hide();
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td>
                        <uc1:ProjectInfo ID="pdProjectInfo" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="WholeWidth">
                        <div id="divPrevNextMainContent" class="main-content" runat="server">
                            <div class="page-hscroll-wrapper">
                                <div class="side-r">
                                </div>
                                <div class="side-l">
                                </div>
                                <div id="divLeft" class="scroll-left" runat="server" visible="false">
                                    <asp:HyperLink ID="lnkPrevMilestone" runat="server" ToolTip="Previous milestone">
                                        <span id="captionLeft" runat="server"></span>
                                    </asp:HyperLink>
                                    <label id="lblLeft" runat="server">
                                    </label>
                                </div>
                                <div id="divRight" class="scroll-right" runat="server" visible="false">
                                    <asp:HyperLink ID="lnkNextMilestone" runat="server" ToolTip="Next milestone">
                                        <span id="captionRight" runat="server"></span>
                                    </asp:HyperLink>
                                    <label id="lblRight" runat="server">
                                    </label>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hidDirty" runat="server" />
            <asp:HiddenField ID="hdnEditEntryIdIndex" runat="server" Value="" />
            <div class="MileStoneDetailBasicInfo">
                <table>
                    <tr>
                        <td>
                            Milestone Name
                            <asp:TextBox ID="txtMilestoneName" runat="server" onchange="setDirty();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqMilestoneName" runat="server" ControlToValidate="txtMilestoneName"
                                ErrorMessage="The Milestone Name is required." ToolTip="The Milestone Name is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cvMilestoneName" runat="server" ControlToValidate="txtMilestoneName"
                                OnServerValidate="cvMilestoneName_Validate" ErrorMessage="The Milestone Name should be less than or equal to 55 characters"
                                ToolTip="The Milestone Name should be less than or equal to 55 characters" Text="*"
                                EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:CustomValidator>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        From
                                    </td>
                                    <td>
                                        <uc2:DatePicker ID="dtpPeriodFrom" runat="server" ValidationGroup="Milestone" OnSelectionChanged="dtpPeriod_SelectionChanged" />
                                        <asp:HiddenField ID="hdnPeriodFrom" runat="server" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPeriodFrom" runat="server" ControlToValidate="dtpPeriodFrom"
                                            ErrorMessage="The Period From is required." ToolTip="The Period From is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPeriodFrom" runat="server" ControlToValidate="dtpPeriodFrom"
                                            ErrorMessage="The Period From has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Project Start has an incorrect format. It must be 'MM/dd/yyyy'."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            Operator="DataTypeCheck" Type="Date" ValidationGroup="Milestone"></asp:CompareValidator>
                                    </td>
                                    <td>
                                        &nbsp;to&nbsp;
                                    </td>
                                    <td>
                                        <uc2:DatePicker ID="dtpPeriodTo" runat="server" ValidationGroup="Milestone" OnSelectionChanged="dtpPeriod_SelectionChanged" />
                                        <asp:HiddenField ID="hdnPeriodTo" runat="server" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPeriodTo" runat="server" ControlToValidate="dtpPeriodTo"
                                            ErrorMessage="The Period To is required." ToolTip="The Period To is required."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compPeriod" runat="server" ControlToCompare="dtpPeriodTo"
                                            ControlToValidate="dtpPeriodFrom" Display="Dynamic" EnableClientScript="False"
                                            ErrorMessage="The Period To must be greater than or equal to the Period From."
                                            Operator="LessThanEqual" SetFocusOnError="True" Type="Date" ToolTip="The Period To must be greater than or equal to the Period From."
                                            ValidationGroup="Milestone">*</asp:CompareValidator>
                                        <asp:CompareValidator ID="compPeriodTo" runat="server" ControlToValidate="dtpPeriodTo"
                                            ErrorMessage="The Period To has an incorrect format. It must be 'MM/dd/yyyy'."
                                            ToolTip="The Project To has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                            Type="Date" ValidationGroup="Milestone"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td rowspan="2" class="vMiddle">
                                        Revenue
                                    </td>
                                    <td colspan="3">
                                        <asp:RadioButton ID="rbtnHourlyRevenue" runat="server" AutoPostBack="true" Checked="true"
                                            GroupName="Revenue" OnCheckedChanged="Revenue_CheckedChanged" onclick="setDirty();"
                                            Text="Hourly - Set hourly rate when you add people to this milestone" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbtnFixedRevenue" runat="server" AutoPostBack="true" GroupName="Revenue"
                                            OnCheckedChanged="Revenue_CheckedChanged" onclick="setDirty();" Text="Fixed Milestone Payment of" />
                                    </td>
                                    <td>
                                        $
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtFixedRevenue" runat="server" onchange="setDirty();"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqFixedRevenue" runat="server" ControlToValidate="txtFixedRevenue"
                                            Display="Dynamic" EnableClientScript="false" ErrorMessage="The Revenue is required."
                                            SetFocusOnError="true" Text="*" ToolTip="The Revenue is required." ValidationGroup="Milestone"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="compFixedRevenue" runat="server" ControlToValidate="txtFixedRevenue"
                                            Display="Dynamic" EnableClientScript="false" ErrorMessage="A number with 2 decimal digits is allowed for the Revenue."
                                            Operator="DataTypeCheck" SetFocusOnError="true" Text="*" ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                                            Type="Currency" ValidationGroup="Milestone"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbIsChargeable" runat="server" onclick="setDirty();" Text="Time entries in this milestone are billable by default" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbConsultantsCanAdjust" runat="server" onclick="setDirty();" Text="Chargeability of time entries in this milestone can be adjusted by consultants" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Table ID="tblMilestoneDetailTabViewSwitch" runat="server" CssClass="CommonCustomTabStyle">
                <asp:TableRow ID="rowSwitcher" runat="server">
                    <asp:TableCell ID="cellFinancials" runat="server" CssClass="SelectedSwitch">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnFinancials" runat="server" Text="Financials" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDetail" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnDetail" runat="server" Text="Detail" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellResources" Visible='<%# GetVisibleValue()  %>' runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnResources" runat="server" Text="Resources" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellExpenses" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnExpenses" runat="server" Text="Expenses" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellDailyActivity" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnDailyActivity" runat="server" Text="Daily Activity" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellCumulativeActivity" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnCumulativeActivity" runat="server" Text="Cumulative Activity"
                                CausesValidation="false" OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellActivitySummary" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnActivitySummary" runat="server" Text="Activity Summary" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellHistory" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnHistory" runat="server" Text="History" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="7"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                    <asp:TableCell ID="cellTools" runat="server">
                        <span class="bg"><span>
                            <asp:LinkButton ID="btnTools" runat="server" Text="Tools" CausesValidation="false"
                                OnCommand="btnView_Command" CommandArgument="8"></asp:LinkButton></span>
                        </span>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:MultiView ID="mvMilestoneDetailTab" runat="server" ActiveViewIndex="0">
                <asp:View ID="vwFinancials" runat="server">
                    <asp:Panel ID="pnlFinancials" runat="server" CssClass="tab-pane">
                        <table class="alterrow">
                            <tr>
                                <td>
                                    Estimated Services Revenue
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblTotalRevenue" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Reimbursed Expenses
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblReimbursedExpenses" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Account discount (<asp:Label ID="lblClientDiscount" runat="server"></asp:Label>
                                    &nbsp;%)
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblClientDiscountAmount" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Revenue net of Discounts
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblTotalRevenueNet" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    COGS
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblTotalCogs" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Total Project Expenses
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblExpenses" runat="server" class="fontBold" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Contribution Margin
                                </td>
                                <td class="textRight no-wrap">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblTotalMargin" runat="server" class="fontBold" />
                                            </td>
                                            <td id="tdTargetMargin" runat="server">
                                                &nbsp;(<asp:Label ID="lblTargetMargin" runat="server" />)
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Net Margin
                                </td>
                                <td class="textRight">
                                    <asp:Label ID="lblFinalMilestoneMargin" runat="server" class="fontBold"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwDetails" runat="server">
                    <asp:Panel ID="pnlDetails" runat="server" CssClass="tab-pane" Style="overflow: auto;
                        margin-bottom: 5px;">
                        <asp:GridView EnableViewState="false" ID="gvPeople" runat="server" AutoGenerateColumns="False"
                            EmptyDataText="There is nothing to be displayed." OnRowDataBound="gvPeople_RowDataBound"
                            ShowFooter="true" CssClass="CompPerfTable MileStoneDetailPageDetailTab" OnDataBound="gvPeople_OnDataBound">
                            <AlternatingRowStyle CssClass="alterrow" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">
                                            &nbsp;
                                        </div>
                                    </HeaderTemplate>
                                    <HeaderStyle CssClass="textCenter" />
                                    <ItemStyle CssClass="textCenter" />
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgEdit" ToolTip="Edit" runat="server" OnClientClick="return ChangeActiveViewIndex(this);"
                                            ImageUrl="~/Images/icon-edit.png" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Person Name</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:HyperLink ID="btnPersonLink" runat="server" NavigateUrl='<%# GetMpeRedirectUrl(Eval("Id")) %>'
                                            Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("Person.LastName"), Eval("Person.FirstName"))) %>'
                                            onclick="return checkDirtyWithRedirect(this.href);" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Role</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblPersonRole" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].Role.Name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Start Date</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartDate" runat="server" CssClass="spacing" Text='<%# ((DateTime)Eval("Entries[0].StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            End Date</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEndDate" runat="server" CssClass="spacing" Text='<%# ((DateTime?)Eval("Entries[0].EndDate")).HasValue ? ((DateTime)Eval("Entries[0].EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle CssClass="textRight" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Eff. Bill Rate</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEffectiveBillRate" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].ComputedFinancials.BillRateMinusDiscount")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        Totals by months
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle CssClass="textRight" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Hours</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalHours" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.HoursBilled") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle CssClass="textRight" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle CssClass="textRight fontBold" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Revenue</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblRevenueContribution" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle CssClass="textRight fontBold" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            Margin</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblMarginContribution" runat="server" Text='<%# GetText(Eval("Entries[0].ComputedFinancials.GrossMargin"), (DataTransferObjects.Person)Eval("Person") ) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle CssClass="textRight fontBold" />
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Revenue</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalRevenue" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle CssClass="vTop" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg no-wrap">
                                            &#931 Contribution Margin</div>
                                        </th>
                                    </HeaderTemplate>
                                    <ItemStyle CssClass="textRight fontBold" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTotalMargin" runat="server" Text='<%# GetText(Eval("Entries[0].ComputedFinancials.GrossMargin"), (DataTransferObjects.Person)Eval("Person") ) %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle CssClass="textRight" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwResources" OnActivate="vwResources_OnActivate" runat="server">
                    <asp:Panel ID="pnlResources" runat="server" CssClass="tab-pane">
                        <asp:UpdatePanel ID="upnlMilestonePersons" ChildrenAsTriggers="true" runat="server">
                            <ContentTemplate>
                                <% if (IsShowResources)
                                   { %>
                                <uc:MilestonePersonList runat="server" ID="MilestonePersonEntryListControl"></uc:MilestonePersonList>
                                <% } %>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwExpenses" runat="server">
                    <asp:Panel ID="pnlExpenses" runat="server" CssClass="tab-pane">
                        <m:Expenses ID="milestoneExpenses" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwDaily" runat="server">
                    <asp:Panel ID="pnlDaily" runat="server" CssClass="tab-pane">
                        <% if (IsShowResources)
                           { %>
                        <mp:Activity ID="mpaDaily" runat="server" />
                        <% } %>
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwCumulativeActivity" runat="server">
                    <asp:Panel ID="plnCumulativeActivity" runat="server" CssClass="tab-pane">
                        <mp:Cumulative ID="mpaCumulative" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwActivitySummary" runat="server">
                    <asp:Panel ID="pnlActivitySummary" runat="server" CssClass="tab-pane">
                        <mp:CumulativeTotal ID="mpaTotal" runat="server" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwHistory" runat="server">
                    <asp:Panel ID="pnlHistory" runat="server" CssClass="tab-pane">
                        <uc:Notes ID="nMilestone" runat="server" Target="Milestone" OnNoteAdded="nMilestone_OnNoteAdded" />
                        <uc:ActivityLogControl runat="server" ID="activityLog" DisplayDropDownValue="Milestone"
                            DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false" />
                    </asp:Panel>
                </asp:View>
                <asp:View ID="vwTools" runat="server">
                    <asp:Panel ID="pnlTools" runat="server" CssClass="tab-pane">
                        <table class="vTop">
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlMoveMilestone" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    Move milestone
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtShiftDays" runat="server" TabIndex="2"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="reqShiftDays" runat="server" ControlToValidate="txtShiftDays"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Shift for Days is required."
                                                        SetFocusOnError="true" Text="*" ToolTip="The Shift for Days is required." ValidationGroup="ShiftDays"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="compShiftDays" runat="server" ControlToValidate="txtShiftDays"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Shift for Days must be an integer number."
                                                        Operator="DataTypeCheck" SetFocusOnError="true" Text="*" ToolTip="The Shift for Days must be an integer number."
                                                        Type="Integer" ValidationGroup="ShiftDays"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    days. (e.g. 3 or -3)
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" class="textCenter">
                                                    <asp:CheckBox ID="chbMoveFutureMilestones" runat="server" TabIndex="2" Text="Move Future Milestones" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnMoveMilestone" runat="server" OnClick="btnMoveMilestone_Click"
                                                        OnClientClick="if (!confirm('Do you really want to move the milestone?')) return false;"
                                                        TabIndex="2" Text="Move Milestone" ValidationGroup="ShiftDays" />
                                                    <ext:ElementDisablerExtender ID="ElementDisablerExtender1" runat="server" TargetControlID="btnMoveMilestone"
                                                        ControlToDisableID="btnClone" />
                                                    <ext:ElementDisablerExtender ID="ElementDisablerExtender2" runat="server" TargetControlID="btnMoveMilestone"
                                                        ControlToDisableID="btnMoveMilestone" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                                <td>
                                    <asp:Panel ID="pnlCloneMilestone" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    Duration
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtCloneDuration" runat="server" TabIndex="3"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="reqCloneDuration" runat="server" ControlToValidate="txtCloneDuration"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Duration is required."
                                                        SetFocusOnError="true" Text="*" ToolTip="The Duration is required." ValidationGroup="Clone"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="compCloneDuration" runat="server" ControlToValidate="txtCloneDuration"
                                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="The Duration must be a positive integer."
                                                        Operator="GreaterThan" SetFocusOnError="true" Text="*" ToolTip="The Duration must be a positive integer."
                                                        Type="Integer" ValidationGroup="Clone" ValueToCompare="0"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    days. (e.g. 3)
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="height19Px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btnClone" runat="server" OnClick="btnClone_Click" TabIndex="3" Text="Clone Milestone"
                                                        ValidationGroup="Clone" />
                                                    <ext:ElementDisablerExtender ID="edeClone1" runat="server" TargetControlID="btnClone"
                                                        ControlToDisableID="btnClone" />
                                                    <ext:ElementDisablerExtender ID="edeClone2" runat="server" TargetControlID="btnClone"
                                                        ControlToDisableID="btnMoveMilestone" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </asp:View>
            </asp:MultiView>
            <table>
                <tr>
                    <td align="left" colspan="2" style="padding-top: 5px;">
                        <asp:HiddenField ID="hdnMilestoneId" runat="server" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete Milestone" ToolTip="Delete Milestone"
                            CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="if (!confirm('Do you really want to delete the milestone?')) return false;" />&nbsp;
                        <asp:Button ID="btnSave" runat="server" Text="Save All" ToolTip="Save All" OnClick="btnSave_Click"
                            CausesValidation="true" ValidationGroup="Milestone" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                        <script type="text/javascript">
                            function disableSaveButton() {
                                document.getElementById('<%= btnSave.ClientID %>').disabled = true;
                            }
                        </script>
                        <AjaxControlToolkit:AnimationExtender ID="aeBtnSave" runat="server" TargetControlID="btnSave">
                            <Animations>
					            <OnClick>
					                <ScriptAction Script="disableSaveButton();" />
					            </OnClick>
                            </Animations>
                        </AjaxControlToolkit:AnimationExtender>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <uc:MessageLabel ID="lblError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" />
                        <uc:MessageLabel ID="lblResult" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" />
                        <asp:ValidationSummary ID="vsumMilestone" runat="server" EnableClientScript="false"
                            ValidationGroup="Milestone" />
                        <asp:ValidationSummary ID="vsumPopup" runat="server" EnableClientScript="false" ValidationGroup="MilestonePopup" />
                        <asp:CustomValidator ID="custExpenseValidate" ValidationGroup="MilestoneDelete" runat="server"
                            ErrorMessage="This milestone cannot be deleted, because project has expenses during the milestone period."
                            OnServerValidate="custExpenseValidate_OnServerValidate" Display="Dynamic"></asp:CustomValidator><br />
                        <asp:CustomValidator ID="custProjectStatus" ValidationGroup="MilestoneDelete" runat="server"
                            ErrorMessage="Projects with Active status should have atleast one milestone added to it."
                            OnServerValidate="custProjectStatus_OnServerValidate" Display="Dynamic"></asp:CustomValidator><br />
                        <asp:CustomValidator ID="custCSATValidate" ValidationGroup="MilestoneDelete" runat="server"
                            ErrorMessage="Milestone cannot be deleted as project has CSAT data added to it."
                            OnServerValidate="custCSATValidate_OnServerValidate" Display="Dynamic"></asp:CustomValidator><br />
                        <asp:CustomValidator ID="custAttribution" ValidationGroup="MilestoneDelete" runat="server"
                            ErrorMessage="Milestone cannot be deleted as project has Attribution data added to it."
                            OnServerValidate="custAttribution_OnServerValidate" Display="Dynamic"></asp:CustomValidator><br />
                        <asp:CustomValidator ID="custFeedback" ValidationGroup="MilestoneDelete" runat="server"
                            ErrorMessage="The milestone cannot be deleted because there are project feedback records has been marked as completed.  The milestone can be deleted if the status of all the feedbacks changed to 'Not Completed' or 'Canceled'. Please navigate to the 'Project Feedback' tab for more information to make the necessary adjustments."
                            OnServerValidate="custFeedback_OnServerValidate" Display="Dynamic"></asp:CustomValidator>
                        <asp:ValidationSummary ID="vsumShiftDays" runat="server" EnableClientScript="false"
                            ValidationGroup="ShiftDays" />
                        <asp:ValidationSummary ID="vsumClone" runat="server" EnableClientScript="false" ValidationGroup="Clone" />
                    </td>
                </tr>
            </table>
            <asp:CustomValidator ID="custStartandEndDate" runat="server" ErrorMessage="" ToolTip=""
                ValidationGroup="MilestonePopup" Text="*" EnableClientScript="false" OnServerValidate="custStartandEndDate_ServerValidate"></asp:CustomValidator>
            <asp:HiddenField ID="hdnCanShowPopup" Value="false" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="hdnCanShowPopup"
                BackgroundCssClass="modalBackground" PopupControlID="pnlPopup" DropShadow="false" />
            <asp:Panel ID="pnlPopup" runat="server" CssClass="popUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th colspan="2">
                            Attention!
                            <asp:Button ID="btnClose" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel Changes"
                                OnClick="btnCancel_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10" colspan="2">
                            <table id="tblHasTimeentriesTowardsMileStone" visible="false" runat="server">
                                <tr>
                                    <td>
                                        <p>
                                            You are attempting to change the milestone start/end date, but there is already
                                            time entered for the days you are skipping.</p>
                                        <br />
                                        <p>
                                            You will need to reassign the time entries to a new milestone before you are allowed
                                            to change this date.</p>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblchangeMilestonePersonsForStartDate" visible="false" runat="server">
                                <tr>
                                    <td class="PaddingTop10">
                                        <asp:Label ID="lblchangeMilestonePersonsPopupMessageForStartDate" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter vMiddle">
                                        <asp:RadioButton ID="rbtnchangeMileStoneAndPersonsStartDate" runat="server" AutoPostBack="false"
                                            Checked="true" GroupName="changedMileStoneStartDate" /><b class="font13Px">Change Milestone
                                                and Resource(s)</b>
                                        <asp:RadioButton ID="rbtnchangeMileStoneStartDate" runat="server" AutoPostBack="false"
                                            GroupName="changedMileStoneStartDate" /><b class="font13Px">Change Milestone Only</b>
                                    </td>
                                </tr>
                            </table>
                            <hr class="MilestoneDetailPopUpHR" id="hrBetweenCMSDandCMED" runat="server" visible="false" />
                            <table id="tblchangeMilestonePersonsForEndDate" visible="false" runat="server">
                                <tr>
                                    <td class="PaddingTop10">
                                        <asp:Label ID="lblchangeMilestonePersonsForEndDate" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter vMiddle">
                                        <asp:RadioButton ID="rbtnchangeMileStoneAndPersonsEndDate" runat="server" AutoPostBack="false"
                                            Checked="true" GroupName="changedMileStoneEndDate" /><b class="font13Px">Change Milestone
                                                and Resource(s)</b>
                                        <asp:RadioButton ID="rbtnchangeMileStoneEndDate" runat="server" AutoPostBack="false"
                                            GroupName="changedMileStoneEndDate" /><b class="font13Px">Change Milestone Only</b>
                                    </td>
                                </tr>
                            </table>
                            <hr class="MilestoneDetailPopUpHR" id="hrBetweenCMEDandRMSD" runat="server" visible="false" />
                            <table id="tblRemoveMilestonePersonsForStartDate" visible="false" runat="server">
                                <tr>
                                    <td class="PaddingTop10">
                                        <asp:Label ID="lblRemoveMilestonePersonsForStartDate" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter vMiddle">
                                        <asp:RadioButton ID="rbtnRemovePersonsStartDate" runat="server" AutoPostBack="false"
                                            Checked="true" GroupName="removeStart" /><b class="font13Px">OK</b>&nbsp;&nbsp;&nbsp;
                                        <asp:RadioButton ID="rbtnCancelStartDate" runat="server" AutoPostBack="false" GroupName="removeStart"
                                            onclick="ShowConfirmDialogForStartDate(this);" /><b class="font13Px">Cancel</b>
                                    </td>
                                </tr>
                            </table>
                            <hr class="MilestoneDetailPopUpHR" id="hrBetweenRMSDandRMED" runat="server" visible="false" />
                            <table id="tblRemoveMilestonePersonsForEndDate" visible="false" runat="server">
                                <tr>
                                    <td class="PaddingTop10">
                                        <asp:Label ID="lblRemoveMilestonePersonsForEndDate" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="textCenter vMiddle">
                                        <asp:RadioButton ID="rbtnRemovePersonsEndDate" runat="server" AutoPostBack="false"
                                            GroupName="removeEnd" Checked="true" /><b class="font13Px">OK</b>&nbsp;&nbsp;&nbsp;
                                        <asp:RadioButton ID="rbtnCancelEndDate" runat="server" AutoPostBack="false" GroupName="removeEnd"
                                            onclick="ShowConfirmDialogForEndDate(this);" /><b class="font13Px">Cancel</b>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="trShowSaveandCancel" runat="server">
                        <td colspan="2" class="Padding6 padBottom15" align="center">
                            <table>
                                <tr>
                                    <td class="padRight3">
                                        <asp:Button ID="btnSavePopup" runat="server" Text="Save Changes" ToolTip="Save Changes"
                                            OnClick="btnSavePopup_OnClick" />
                                    </td>
                                    <td class="padLeft3">
                                        <asp:Button ID="btnCancelSaving" runat="server" Text="Cancel Changes" ToolTip="Cancel Changes"
                                            OnClick="btnCancel_OnClick" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:CustomValidator runat="server" ID="cvAttributionPopup" OnServerValidate="cvAttributionPopup_ServerValidate"
                ValidationGroup="AttributionPopup"></asp:CustomValidator>
            <asp:HiddenField ID="hdnAttribution" Value="false" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeAttribution" runat="server" TargetControlID="hdnAttribution"
                BehaviorID="mpeAttributionBehaviourId" BackgroundCssClass="modalBackground" PopupControlID="pnlAttribution"
                DropShadow="false" />
            <asp:Panel ID="pnlAttribution" runat="server" CssClass="popUp yScrollAuto" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th colspan="2">
                            Attention!
                        </th>
                    </tr>
                    <tr id="trAttributionRecord" runat="server">
                        <td>
                            <p>
                                &nbsp;&nbsp;&nbsp; This action cannot be done as the following attribution records
                                have the person start date and end date out of the project's start date and project's
                                end date in commissions tab.</p>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater runat="server" ID="repDeliveryPersons">
                                <HeaderTemplate>
                                    &nbsp;&nbsp;Delivery Attribution:
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <br />
                                    &nbsp;&nbsp;&nbsp;&nbsp; <b>
                                        <%# Eval("TargetName") %></b>&nbsp;has&nbsp;startdate&nbsp;<b><%# ((DateTime)Eval("StartDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%></b>&nbsp;and
                                    enddate&nbsp;<b><%# ((DateTime)Eval("EndDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat) %></b>.
                                </ItemTemplate>
                                <FooterTemplate>
                                    <br />
                                    <br />
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:HiddenField runat="server" ID="hdnIsUpdate" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Repeater runat="server" ID="repSalesPersons">
                                <HeaderTemplate>
                                    &nbsp;&nbsp;Sales Attribution:</HeaderTemplate>
                                <ItemTemplate>
                                    <br />
                                    &nbsp;&nbsp;&nbsp;&nbsp; <b>
                                        <%# Eval("TargetName") %></b>&nbsp;has&nbsp;startdate&nbsp;<b><%# ((DateTime)Eval("StartDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%></b>&nbsp;and
                                    enddate&nbsp;<b><%# ((DateTime)Eval("EndDate")).ToString(PraticeManagement.Constants.Formatting.EntryDateFormat) %></b>.
                                </ItemTemplate>
                                <FooterTemplate>
                                    <br />
                                    <br />
                                </FooterTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr id="trCommissionsStartDateExtend" runat="server">
                        <td>
                            <p>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label runat="server" ID="lblCommissionsStartDateExtendMessage"></asp:Label>
                            </p>
                            <br />
                        </td>
                    </tr>
                    <tr id="trCommissionsEndDateExtend" runat="server">
                        <td>
                            <p>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label runat="server" ID="lblCommissionsEndDateExtendMessage"></asp:Label>
                            </p>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                &nbsp;&nbsp;&nbsp; Please Click "OK" to accept the change.
                            </p>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter">
                            <asp:Button ID="btnOkAttribution" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                                OnClick="btnOkAttribution_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancelAttribution" runat="server" ToolTip="Cancel" Text="Cancel"
                                OnClick="btnCancelAttribution_Click" CssClass="Width100PxImp" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdnApprovedByOps" Value="false" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeApprovedByOpsWhenCompleteOut" runat="server"
                TargetControlID="hdnApprovedByOps" BehaviorID="mpeApprovedbyOpsBehaviourId" BackgroundCssClass="modalBackground"
                OkControlID="btnOk" PopupControlID="pnlApprovedByOps" DropShadow="false" />
            <asp:Panel ID="pnlApprovedByOps" runat="server" CssClass="popUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Attention!
                            <asp:Button ID="Button1" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel Changes"
                                OnClick="btnCancel_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td style="padding: 10px;">
                            “Approved by Ops” has been unchecked as person badge dates are changed with change
                            in milestone dates. Request for Operations for the approval has been sent.
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter" style="padding-bottom: 5px;">
                            <asp:Button ID="btnOk" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:CustomValidator runat="server" ID="custMilestoneDatesConflict" OnServerValidate="custMilestoneDatesConflict_ServerValidate"
                ValidationGroup="MilestoneDatesConflict"></asp:CustomValidator>
            <asp:HiddenField ID="hdnMilestoneDatesConflict" Value="false" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeMilestoneDatesConflict" runat="server"
                TargetControlID="hdnMilestoneDatesConflict" BehaviorID="mpeMilestoneDatesConflictBehaviourId"
                BackgroundCssClass="modalBackground" OkControlID="btnnOk" PopupControlID="pnlMilestoneDatesConflict"
                DropShadow="false" />
            <asp:Panel ID="pnlMilestoneDatesConflict" runat="server" CssClass="popUp" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Attention!
                            <asp:Button ID="btnCancel" runat="server" CssClass="mini-report-closeNew" ToolTip="Cancel Changes"
                                OnClick="btnCancel_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td style="padding: 10px;">
                            Milestone cannot be moved as the following people are assigned to projects which
                            are after 18-Month End date.
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding5Imp">
                            <asp:Repeater runat="server" ID="repBadgePeople">
                                <HeaderTemplate>
                                    <table class="border1Px WholeWidth">
                                        <thead>
                                            <tr class="textLeft border1Px Height25Px">
                                                <th class="paddingLeft5pxImp">
                                                    Person
                                                </th>
                                                <th class="borderLeft paddingLeft5pxImp">
                                                    Project
                                                </th>
                                                <th class="borderLeft paddingLeft5pxImp">
                                                    Badge Start Date
                                                </th>
                                                <th class="borderLeft paddingLeft5pxImp">
                                                    Badge End Date
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr class="border1Px Height25Px">
                                        <td class="paddingLeft5pxImp">
                                            <%# Eval("Person.HtmlEncodedName")%>
                                        </td>
                                        <td class="borderLeft paddingLeft5pxImp">
                                            <%# Eval("Project.HtmlEncodedName")%>
                                        </td>
                                        <td class="borderLeft paddingLeft5pxImp">
                                            <%# GetDateFormat((DateTime)Eval("BadgeStartDate"))%>
                                        </td>
                                        <td class="borderLeft paddingLeft5pxImp">
                                            <%# GetDateFormat((DateTime)Eval("BadgeEndDate"))%>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody> </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter paddingBottom5px PaddingTop20">
                            <asp:Button ID="btnnOk" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <uc:LoadingProgress ID="lpOpportunityDetails" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

