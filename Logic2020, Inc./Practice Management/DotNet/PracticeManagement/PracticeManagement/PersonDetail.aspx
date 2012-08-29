<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="True"
    CodeBehind="PersonDetail.aspx.cs" Inherits="PraticeManagement.PersonDetail" Title="Person Details | Practice Management" %>

<%@ Register Src="~/Controls/Generic/Notes.ascx" TagName="Notes" TagPrefix="uc" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="Controls/RecruiterInfo.ascx" TagName="RecruiterInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/WhatIf.ascx" TagName="WhatIf" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/RestrictionPanel.ascx" TagPrefix="uc" TagName="RestrictionPanel" %>
<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLog" %>
<%@ Register Src="~/Controls/Persons/PersonProjects.ascx" TagPrefix="uc" TagName="PersonProjects" %>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/OpportunityList.ascx" TagName="OpportunityList"
    TagPrefix="uc" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="uc" Namespace="PraticeManagement.Controls" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Person Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Person Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" language="javascript">
        /*
        This script is needed to initialize select all/none behavior for checkbox lists
        This is done because tab content is loaded asynchronously and window.load event is not fired
        when the user goes to the Permissions tab. So this method is called each time postback
        goes to the server in any way.
        */

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
        function pageLoadedHandler(sender, args) {
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddClients');
            addListenersToParent('ctl00_body_rpPermissions_msddGroups', 'ctl00_body_rpPermissions_msddClients');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddGroups');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddSalespersons');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddPracticeManagers');
            addListenersToAllCheckBoxes('ctl00_body_rpPermissions_msddPractices');
        }

        function SetTooltipText(descriptionText, hlinkObj) {
            var hlinkObjct = $('#' + hlinkObj.id);
            var displayPanel = $('#<%= personOpportunities.ClientID %>_oppNameToolTipHolder');
            iptop = hlinkObjct.offset().top - hlinkObjct[0].offsetHeight;
            ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 10;
            iptop = iptop;
            ipleft = ipleft;
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();

            var lbloppNameTooltipContent = document.getElementById('<%= personOpportunities.ClientID %>_lbloppNameTooltipContent');
            lbloppNameTooltipContent.innerHTML = descriptionText.toString();
        }

        function HidePanel() {

            var displayPanel = $('#<%= personOpportunities.ClientID %>_oppNameToolTipHolder');
            displayPanel.hide();
        }

        function printform() {
            var printContent = document.getElementById('<%= dvTerminationDateErrors.ClientID %>');
            var windowUrl = 'about:blank';
            var uniqueName = new Date();
            var windowName = 'Print' + uniqueName.getTime();
            var printWindow = window.open(windowUrl, windowName);

            printWindow.document.write(printContent.innerHTML);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
            printWindow.close();
            return false;
        }

        function saveReport() {
            var printContent = document.getElementById('<%= dvTerminationDateErrors.ClientID %>');
            var hdnSaveReportText = document.getElementById('<%= hdnSaveReportText.ClientID %>');
            hdnSaveReportText.value = printContent.innerHTML;

        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {
            ModifyInnerTextToWrapText();
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


        function SetWrapText(str) {
            for (var i = 30; i < str.length; i = i + 10) {
                str = str.slice(0, i) + "<wbr/>" + str.slice(i, str.length);
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
    <%--
        The following script is needed to implement dirty checks on Projects tab        
        Use Page.ClientScript.GetPostBackClientHyperlink(...) method to generate
        personProjects control postback url
    --%>
        function checkDirty(target, entityId) {
            if (showDialod()) {
                __doPostBack('ctl00$body$personProjects', target + ':' + entityId);
                return true;
            }

            return false;
        }
        function setHintPosition(img, displayPnl) {
            var image = $("#" + img);
            var displayPanel = $("#" + displayPnl);
            iptop = image.offset().top;
            ipleft = image.offset().left;
            iptop = iptop + 10;
            ipleft = ipleft - 10;
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop, ipleft);
            displayPanel.show();
        }

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function ChangeHourlyBillRate(IsIncrement) {
            var txtBillRate = document.getElementById('<%= (whatIf.FindControl("txtBillRateSlider_BoundControl") as TextBox).ClientID %>');

            if (txtBillRate != null) {
                BillRate = parseFloat(txtBillRate.value);
                if (IsIncrement && BillRate < 350) {

                    txtBillRate.value = BillRate + 1;
                }
                else if (!IsIncrement && BillRate > 20) {
                    txtBillRate.value = BillRate - 1;
                }
                if (txtBillRate.fireEvent) {
                    txtBillRate.fireEvent('onchange');
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('change', true, true);
                    txtBillRate.dispatchEvent(event);
                }
            }
        }
        function ChangeHoursPerWeek(IsIncrement) {
            var txtHPW = document.getElementById('<%= (whatIf.FindControl("txtHorsPerWeekSlider_BoundControl") as TextBox).ClientID %>');

            if (txtHPW != null) {
                BillRate = parseInt(txtHPW.value);
                if (IsIncrement && BillRate < 60) {

                    if (BillRate < 55)
                        txtHPW.value = BillRate + 5;
                    else
                        txtHPW.value = 60;
                }
                else if (!IsIncrement) {
                    if (BillRate > 15)
                        txtHPW.value = BillRate - 5;
                    else
                        txtHPW.value = 10;
                }
                if (txtHPW.fireEvent) {
                    txtHPW.fireEvent('onchange');
                }
                if (document.createEvent) {
                    var event = document.createEvent('HTMLEvents');
                    event.initEvent('change', true, true);
                    txtHPW.dispatchEvent(event);
                }
            }
        }
        function EnableDisableVacationDays(ddlBasic) {
            var vacationdaysId = ddlBasic.getAttribute("vacationdaysId");
            var vacationdays = document.getElementById(vacationdaysId);
            if (!(ddlBasic.value == 'W2-Salary' || ddlBasic.value == 'W2-Hourly')) {
                vacationdays.setAttribute("disabled", "disabled");
            }
            else {
                vacationdays.removeAttribute("disabled");
            }
            return false;
        }

        function ValidateCompensationAndTerminationDate(btnTerminatePersonOk, popupTerminationDate, ddlPopupTerminationReason) {
            var terminationDateExt =  $find(popupTerminationDate);
            var ddlterminationReason = document.getElementById(ddlPopupTerminationReason);
            var terminationDate = terminationDateExt.get_selectedDate();
            var terminationDateTextValue = terminationDateExt._textbox.get_Value();
            var compensationEndDate = new Date(btnTerminatePersonOk.getAttribute('CompensationEndDate'));
            var hasNotClosedCompensation = btnTerminatePersonOk.getAttribute('HasNotClosedCompensation');
                        
            if(terminationDate != '' && terminationDate != undefined && terminationDate != null && compensationEndDate != null && (compensationEndDate > terminationDate || (hasNotClosedCompensation == 'true')) && ddlterminationReason.value != '')
            {
                var message = '';
                if(hasNotClosedCompensation)
                    message =  message + 'This person has Termination Date, but still has an open-ended compensation record. Click OK to close their compensation as of their termination date, or click Cancel to not to save changes.';
                else
                    message =  message + 'This person has Termination Date, but still has an active compensation record. Click OK to close their compensation as of their termination date, or click Cancel to not to save changes.';

                if(!confirm(message))
                {
                    Page_ClientValidate('PersonTerminate');
                    return false;
                }
            }
            return true;
        }

        function EnableTerminateEmployeeOkButton(btnPersonTerminateOkId, terminationDateTextBoxId, ddlTerminationReasonId)
        {
            var btnPersonTerminateOk = document.getElementById(btnPersonTerminateOkId);
            var terminationDate = document.getElementById(terminationDateTextBoxId);
            var ddlTerminationReason = document.getElementById(ddlTerminationReasonId);

            if(btnPersonTerminateOk != null && ddlTerminationReason != null && terminationDate != null)
            {
                Disable(btnPersonTerminateOk, (ddlTerminationReason.value == '' || terminationDate.value == ''));
            }
        }

        function Disable(control, disable)
        {
            if(disable)
            {
                control.disabled = 'disabled';
            }
            else
            {
                control.disabled = '';
            }
        }
    </script>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hidDirty" runat="server" />
            <table class="PersonForm">
                <tr>
                    <td>
                        <asp:Panel ID="pnlPersonalInfo" runat="server">
                            <table>
                                <tr>
                                    <td>
                                        First Name
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="Width152px" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqFirstName" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtFirstName" ErrorMessage="The First Name is required." EnableClientScript="False"
                                            SetFocusOnError="True" ToolTip="The First Name is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custPersonName" runat="server" ControlToValidate="txtFirstName"
                                            ErrorMessage="There is another Person with the same First Name and Last Name."
                                            ToolTip="There is another Person with the same First Name and Last Name." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custPersonName_ServerValidate"></asp:CustomValidator>
                                        <asp:RegularExpressionValidator ControlToValidate="txtFirstName" ValidationGroup="Person"
                                            ID="valRegFirstName" runat="server" ErrorMessage="First Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ToolTip="First Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-]{2,35}$" />
                                    </td>
                                    <td>
                                        Status
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPersonStatus" runat="server" CssClass="Width158px" onchange="setDirty();"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqPersonStatus" runat="server" ControlToValidate="ddlPersonStatus"
                                            ErrorMessage="The Status is required." ToolTip="The Status is required." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custPersonStatus" runat="server" ControlToValidate="ddlPersonStatus"
                                            ErrorMessage="Only administrator can set a status to Active or Terminated." ToolTip="Only administrator can set a status to Active or Terminated."
                                            ValidationGroup="Person" Text="*" ValidateEmptyText="false" EnableClientScript="false"
                                            SetFocusOnError="true" Display="Dynamic" OnServerValidate="custPersonStatus_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="cvInactiveStatus" runat="server" ErrorMessage="" ToolTip=""
                                            Display="Dynamic" ValidationGroup="Person" Text="*" EnableClientScript="false"
                                            OnServerValidate="cvInactiveStatus_ServerValidate"></asp:CustomValidator>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Last Name
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="Width152px" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqLastName" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtLastName" ErrorMessage="The Last Name is required." EnableClientScript="False"
                                            SetFocusOnError="True" ToolTip="The Last Name is required.">*</asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ControlToValidate="txtLastName" ID="valRegLastName"
                                            runat="server" ValidationGroup="Person" ErrorMessage="Last Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            ToolTip="Last Name should be limited to 2-35 characters in length containing only letters and/or an apostrophe or hyphen."
                                            EnableClientScript="false" Text="*" ValidationExpression="^[a-zA-Z'\-]{2,35}$" />
                                        &nbsp;
                                    </td>
                                    <td>
                                        Career Counselor
                                    </td>
                                    <td class="padRight2">
                                        <asp:UpdatePanel ID="pnlLineManager" runat="server">
                                            <ContentTemplate>
                                                <uc:DefaultManager ID="defaultManager" runat="server" InsertFirtItem="false" PersonsRole="Practice Area Manager"
                                                    CssClass="Width158px" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="lbSetPracticeOwner" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lbSetPracticeOwner" runat="server" PostBackUrl="#" OnClick="lbSetPracticeOwner_Click">Set to Practice Area Owner</asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Default Practice Area
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlDefaultPractice" runat="server" CssClass="Width158px" onchange="setDirty();">
                                            <asp:ListItem Text="Technology Solutions"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:CustomValidator ID="cvPracticeArea" runat="server" ErrorMessage="The Default Practice Area is required for active person."
                                            ToolTip="The Default Practice Area is required for active person." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="cvPracticeArea_ServerValidate"></asp:CustomValidator>&nbsp;
                                    </td>
                                    <td>
                                        Hire&nbsp;Date
                                    </td>
                                    <td class="Width158px">
                                        <uc2:DatePicker ID="dtpHireDate" runat="server" OnSelectionChanged="dtpHireDate_SelectionChanged"
                                            AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <asp:CompareValidator ID="compHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            EnableClientScript="False" ErrorMessage="The Hire Date must be in the format 'MM/dd/yyyy'"
                                            Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Hire Date must be in the format 'MM/dd/yyyy'"
                                            Type="Date">*</asp:CompareValidator>
                                        <asp:RequiredFieldValidator ID="reqHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            Display="Dynamic" EnableClientScript="False" ErrorMessage="The Hire Date is required."
                                            SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Hire Date is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custHireDate" runat="server" ControlToValidate="dtpHireDate"
                                            ErrorMessage="Cannot set a Hire Date outside recruiting commissions period."
                                            ToolTip="Cannot set a Hire Date outside recruiting commissions period." ValidationGroup="Person"
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            OnServerValidate="custHireDate_ServerValidate"></asp:CustomValidator>&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Email&nbsp;Address
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmailAddress" runat="server" CssClass="Width152px" onchange="setDirty();"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RegularExpressionValidator ID="regEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            Display="Dynamic" ErrorMessage="The Email Address is not valid." ValidationGroup="Person"
                                            ToolTip="The Email Address is not valid." Text="*" EnableClientScript="False"
                                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                        <asp:CustomValidator ID="custEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="There is another Person with the same Email." ToolTip="There is another Person with the same Email."
                                            ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" OnServerValidate="custEmailAddress_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custUserName" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="Unknow error occures. Please contact your administrator." ToolTip="Unknow error occures. Please contact your administrator."
                                            ValidateEmptyText="true" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" ValidationGroup="Person" OnServerValidate="custUserName_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custReqEmailAddress" runat="server" ControlToValidate="txtEmailAddress"
                                            ErrorMessage="The Email Address is required for active person." ToolTip="The Email Address is required for active person."
                                            ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                            Display="Dynamic" ValidateEmptyText="true" OnServerValidate="custReqEmailAddress_ServerValidate"></asp:CustomValidator>&nbsp;
                                    </td>
                                    <td nowrap="nowrap">
                                        Termination Date
                                    </td>
                                    <td class="Width158px">
                                        <uc2:DatePicker ID="dtpTerminationDate" runat="server" />
                                    </td>
                                    <td>
                                        <asp:CompareValidator ID="compTerminationDate" runat="server" ControlToValidate="dtpTerminationDate"
                                            Display="Dynamic" EnableClientScript="False" Enabled="False" EnableTheming="True"
                                            ErrorMessage="The Termination Date must be in the format 'MM/dd/yyyy'" Operator="DataTypeCheck"
                                            SetFocusOnError="True" ValidationGroup="Person" ToolTip="The Termination Date must be in the format 'MM/dd/yyyy'"
                                            Type="Date">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="custTerminationDate" runat="server" ErrorMessage="To terminate the person the Termination Date should be specified."
                                            ToolTip="To terminate the person the Termination Date should be specified." ValidationGroup="Person"
                                            Text="*" Display="Dynamic" EnableClientScript="false" OnServerValidate="custTerminationDate_ServerValidate"></asp:CustomValidator>
                                        <asp:CompareValidator ID="cmpTerminateDate" runat="server" ControlToValidate="dtpTerminationDate"
                                            ControlToCompare="dtpHireDate" Operator="GreaterThan" Type="Date" ErrorMessage="Termination date should be greater than Hire date."
                                            Display="Dynamic" Text="*" ValidationGroup="Person" ToolTip="Termination date should be greater than Hire date."
                                            SetFocusOnError="true"></asp:CompareValidator>
                                        <asp:CustomValidator ID="custTerminateDateTE" runat="server" ErrorMessage="" ToolTip=""
                                            Display="Dynamic" ValidationGroup="Person" Text="*" EnableClientScript="false"
                                            OnServerValidate="custTerminationDateTE_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custIsDefautManager" runat="server" ErrorMessage="Unable to set Termination Date for this person because this person is set as default career counselor. Please select another default career counselor and refresh this page to enter termination date for this person."
                                            Display="Dynamic" ValidationGroup="Person" Text="*" EnableClientScript="false"
                                            OnServerValidate="custIsDefautManager_ServerValidate"></asp:CustomValidator>
                                        <asp:DropDownList ID="ddlTerminationReason" runat="server">
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="custTerminationReason" runat="server" ErrorMessage="To terminate the person the Termination Reason should be specified."
                                            ToolTip="To terminate the person the Termination Reason should be specified."
                                            ValidationGroup="Person" Text="*" Display="Static" EnableClientScript="false"
                                            OnServerValidate="custTerminationReason_ServerValidate"></asp:CustomValidator>
                                        <asp:Button ID="btnTerminatePerson" runat="server" Text="Terminate Employee" OnClick="btnTerminatePerson_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td nowrap="nowrap">
                                        Offshore Resource
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPersonType" runat="server" CssClass="Width158px" onchange="setDirty();">
                                            <asp:ListItem Text="NO" Value="0" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="YES" Value="1"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Label ID="lbPayChexID" runat="server" Text="PayChexID" Visible="false"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPayCheckId" runat="server" CssClass="Width152px" onchange="setDirty();"
                                            Visible="false"></asp:TextBox>
                                        <AjaxControlToolkit:FilteredTextBoxExtender ID="ftePayCheckId" TargetControlID="txtPayCheckId"
                                            FilterType="Numbers" FilterMode="ValidChars" runat="server" />
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Telephone number
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTelephoneNumber" runat="server" onchange="setDirty();" CssClass="Width152px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RegularExpressionValidator ID="reqTelphoneNumber" runat="server" ControlToValidate="txtTelephoneNumber"
                                            Display="Dynamic" ErrorMessage="The Telephone number is not valid." ValidationGroup="Person"
                                            ToolTip="The Telephone number is not valid." Text="*" EnableClientScript="False"
                                            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$"></asp:RegularExpressionValidator>&nbsp;
                                    </td>
                                    <td>
                                        <asp:Label ID="lblDivision" runat="server" Text="Division"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlDivision" runat="server" CssClass="Width158px">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblEmployeeNumber" runat="server" Text="PersonID"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEmployeeNumber" runat="server" MaxLength="12" onchange="setDirty();"
                                            onfocus="if (!this.readOnly &amp;&amp; !confirm('This value should not normally be changed once set. Please be cautious about changing this value. Press OK to continue or Cancel to return without changing it.')) this.blur();"
                                            CssClass="Width152px"></asp:TextBox>
                                        <asp:HiddenField ID="hdnPersonId" runat="server" />
                                        <asp:HiddenField ID="hdnIsDefaultManager" runat="server" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="reqEmployeeNumber" runat="server" ValidationGroup="Person"
                                            ControlToValidate="txtEmployeeNumber" EnableClientScript="False" ErrorMessage="The Employee Number is required."
                                            SetFocusOnError="True" ToolTip="The Employee Number is required.">*</asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="custEmployeeNumber" runat="server" ControlToValidate="txtEmployeeNumber"
                                            Display="Dynamic" EnableClientScript="false" ValidationGroup="Person" ErrorMessage="There is another Person with the same Employee Number."
                                            OnServerValidate="custEmployeeNumber_ServerValidate" SetFocusOnError="true" Text="*"
                                            ToolTip="There is another Person with the same Employee Number."></asp:CustomValidator>&nbsp;
                                    </td>
                                    <td>
                                        Practice Areas Owned
                                    </td>
                                    <td class="Width152px">
                                        <asp:Repeater ID="repPracticesOwned" runat="server">
                                            <HeaderTemplate>
                                                <ul class="practices">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li class="practices">
                                                    <%# Eval("HtmlEncodedName") %></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chbLockedOut" runat="server" Checked="false" onclick="setDirty();"
                                            Text="Locked-Out" />
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CustomValidator ID="custPersonData" runat="server" OnServerValidate="custPersonData_ServerValidate"></asp:CustomValidator>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <table class="WholeWidth">
                <tr>
                    <td class="PersonMultiView">
                        <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CommonCustomTabStyle PersonDetailPage_CustomTabStyle">
                            <asp:TableRow ID="rowSwitcher" runat="server">
                                <asp:TableCell ID="cellSecurity" runat="server" CssClass="SelectedSwitch">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnSecurity" runat="server" Text="Application Security" CausesValidation="false"
                                            CssClass="Width80Px" OnCommand="btnView_Command" CommandArgument="0"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellPermissions" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnPermissions" runat="server" Text="Project/Opportunity Permissions"
                                            CssClass="Width120Px" CausesValidation="false" OnCommand="btnView_Command" CommandArgument="1"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellCompensation" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnCompensation" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Compensation"
                                            CausesValidation="false" CssClass="Width83Px" OnCommand="btnView_Command" CommandArgument="2"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellEmploymentHistory" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnEmploymentHistory" runat="server" Text="Employment History"
                                            CausesValidation="false" CssClass="Width83Px" OnCommand="btnView_Command" CommandArgument="3"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellRecruiting" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnViewRecruiting" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Recruiter"
                                            CausesValidation="false" CssClass="width58Px" OnCommand="btnView_Command" CommandArgument="4"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellCommissions" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnCommissions" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Commissions"
                                            CausesValidation="false" CssClass="Width80Px" OnCommand="btnView_Command" CommandArgument="5"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellRates" runat="server" Visible="false">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnRates" runat="server" Text="Overhead and Margin" CausesValidation="false"
                                            OnCommand="btnView_Command" CommandArgument="6"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellWhatIf" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnWhatIf" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; What-If?"
                                            CausesValidation="false" CssClass="width50Px" OnCommand="btnView_Command" CommandArgument="7"
                                            OnClientClick="if (!confirmSaveDirty()) return false;"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellProjects" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnProjects" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Projects"
                                            CausesValidation="false" CssClass="width50Px" OnCommand="btnView_Command" CommandArgument="8"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellOpportunities" runat="server">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnOppportunities" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Opportunities"
                                            CausesValidation="false" CssClass="Width80Px" OnCommand="btnView_Command" CommandArgument="9"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                                <asp:TableCell ID="cellActivityLog" runat="server" Visible="false">
                                    <span class="bg">
                                        <asp:LinkButton ID="btnActivityLog" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; History"
                                            CausesValidation="false" CssClass="Width45Px" OnCommand="btnView_Command" CommandArgument="10"></asp:LinkButton>
                                    </span>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                        <asp:MultiView ID="mvPerson" runat="server" ActiveViewIndex="0">
                            <asp:View ID="vwSecurity" runat="server" OnPreRender="vwPermissions_PreRender">
                                <asp:Panel ID="Panel6" runat="server" CssClass="tab-pane WholeWidth">
                                    <table cellpadding="3">
                                        <tr>
                                            <td class="Width60Px">
                                                <strong>
                                                    <asp:Localize ID="locRolesLabel" runat="server" Text="Roles"></asp:Localize></strong>
                                                <asp:CustomValidator ID="custRoles" runat="server" Display="Dynamic" EnableClientScript="false"
                                                    ValidationGroup="Person" ErrorMessage="Any person who is projected should not have any roles checked."
                                                    OnServerValidate="custRoles_ServerValidate" SetFocusOnError="true" Text="*" ToolTip="Any person who is projected should not have any roles checked."
                                                    ValidateEmptyText="true"></asp:CustomValidator>
                                                <asp:CustomValidator ID="valRecruterRole" runat="server" Display="Dynamic" EnableClientScript="false"
                                                    ValidationGroup="Person" ErrorMessage="Person with Recruiter role should have recruiting commission."
                                                    OnServerValidate="valRecruterRole_OnServerValidate" SetFocusOnError="true" Text="*"
                                                    ToolTip="Person with Recruiter role should have recruiting commission." ValidateEmptyText="true" />
                                                <asp:CustomValidator ID="cvRolesActiveStatus" runat="server" Display="Dynamic" EnableClientScript="false"
                                                    ValidationGroup="Person" ErrorMessage="Any person who is active should have atleast one role checked."
                                                    OnServerValidate="cvRolesActiveStatus_ServerValidate" SetFocusOnError="true"
                                                    Text="*" ToolTip="Any person who is active should have atleast one role checked."></asp:CustomValidator>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <b>Seniority</b>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Width60Px">
                                                <asp:CheckBoxList ID="chblRoles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chblRoles_SelectedIndexChanged"
                                                    CssClass="Width170Px">
                                                </asp:CheckBoxList>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td valign="top">
                                                <asp:DropDownList ID="ddlSeniority" runat="server" onchange="setDirty();" />
                                            </td>
                                            <td>
                                                <asp:CustomValidator ID="custSeniority" runat="server" ValidationGroup="Person" ControlToValidate="ddlPersonStatus"
                                                    Display="Dynamic" EnableClientScript="true" ErrorMessage="The Seniority is required since the person's status is Active."
                                                    OnServerValidate="custSeniority_ServerValidate" Text="*" ToolTip="The Seniority is required since the person's status is Active."></asp:CustomValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="Width60Px">
                                                <asp:Button ID="btnResetPassword" runat="server" OnClick="btnResetPassword_Click"
                                                    OnClientClick="return confirm('Do you really want to reset user\'s password?');"
                                                    Text="Reset Password" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:Label ID="lblPaswordResetted" runat="server" Text="The password was reset, and the person has been notified by email."
                                                    Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <div class="DivPermissionDiffenrece">
                                                    <asp:GridView ID="gvPermissionDiffenrece" runat="server" AutoGenerateColumns="false"
                                                        CssClass="CompPerfTable PermissionDiffenrece">
                                                        <AlternatingRowStyle CssClass="alterrow" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Page</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# Eval("Title") %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Current</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("Old")) ? "Yes" : "No" %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        New</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("New")) ? "Yes" : "No" %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <div class="ie-bg no-wrap">
                                                                        Diff.</div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <%# ((bool)Eval("IsDifferent")) ? "Yes" : "No"%>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwPermissions" runat="server" OnPreRender="vwPermissions_PreRender">
                                <asp:Panel ID="pnlPermissions" runat="server" CssClass="tab-pane">
                                    <asp:HiddenField ID="hfReloadPerms" runat="server" Value="False" />
                                    <uc:RestrictionPanel ID="rpPermissions" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwCompensation" runat="server">
                                <asp:Panel ID="pnlCompensation" runat="server" CssClass="tab-pane">
                                    <div class="filters Margin-Top5Px Margin-Bottom10Px">
                                        <div class="buttons-block">
                                            <span class="colorGray">Click on Start Date column in the grid to edit compensation
                                                history item.</span>
                                            <asp:ShadowedTextButton ID="btnAddCompensation" runat="server" Text="Add Compensation"
                                                OnClick="btnAddCompensation_Click" CssClass="add-btn" OnClientClick="if (!confirmSaveDirty()) return false;" />
                                            &nbsp;<asp:CustomValidator ID="custCompensationCoversMilestone" runat="server" ValidationGroup="Person"
                                                ErrorMessage="This person has a status of Active, but does not have an active compensation record. &nbsp;Go back to their record so you can create a compensation record for them, or set their status as Projected or Terminated."
                                                OnServerValidate="custCompensationCoversMilestone_ServerValidate" Text="*"> </asp:CustomValidator>
                                            <div class="clear0">
                                            </div>
                                        </div>
                                    </div>
                                    <asp:GridView ID="gvCompensationHistory" runat="server" AutoGenerateColumns="False"
                                        OnRowDataBound="gvCompensationHistory_OnRowDataBound" EmptyDataText="No compensation history for this person."
                                        CssClass="CompPerfTable CompensationHistory" ShowFooter="false">
                                        <AlternatingRowStyle CssClass="alterrow" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        &nbsp;
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemStyle CssClass="Width2Percent" />
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
                                                        ImageUrl="~/Images/copy.png" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        &nbsp;
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemStyle CssClass="Width2Percent" />
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgEditCompensation" ToolTip="Edit Compensation" runat="server"
                                                        OnClick="imgEditCompensation_OnClick" ImageUrl="~/Images/icon-edit.png" />
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:ImageButton ID="imgUpdateCompensation" StartDate='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'
                                                        ToolTip="Save" runat="server" ImageUrl="~/Images/icon-check.png" OnClick="imgUpdateCompensation_OnClick"
                                                        operation="Update" />
                                                    <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                                        OnClick="imgCancel_OnClick" />
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:ImageButton ID="imgUpdateCompensation" operation="Insert" ToolTip="Save" runat="server"
                                                        ImageUrl="~/Images/icon-check.png" OnClick="imgUpdateCompensation_OnClick" />
                                                    <asp:ImageButton ID="imgCancel" ToolTip="Cancel" runat="server" ImageUrl="~/Images/no.png"
                                                        OnClick="imgCancelFooter_OnClick" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Start</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'
                                                        CommandArgument='<%# Eval("StartDate") %>' OnCommand="btnStartDate_Command" OnClientClick="if (!confirmSaveDirty()) return false;"></asp:LinkButton>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <uc2:DatePicker ID="dpStartDate" ValidationGroup="CompensationUpdate" runat="server"
                                                            TextBoxWidth="90%" AutoPostBack="false" />
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Start Date is required."
                                                            ToolTip="The Start Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Static"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpStartDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                            Type="Date"></asp:CompareValidator>
                                                    </span>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <uc2:DatePicker ID="dpStartDate" ValidationGroup="CompensationUpdate" runat="server"
                                                            TextBoxWidth="90%" AutoPostBack="false" />
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Start Date is required."
                                                            ToolTip="The Start Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                            Display="Static"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpStartDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                            Type="Date"></asp:CompareValidator>
                                                    </span>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width11Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        End</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label></ItemTemplate>
                                                <EditItemTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <uc2:DatePicker ID="dpEndDate" ValidationGroup="CompensationUpdate" runat="server"
                                                            TextBoxWidth="90%" AutoPostBack="false" />
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:CompareValidator ID="compDateRange" runat="server" ControlToValidate="dpEndDate"
                                                            ValidationGroup="CompensationUpdate" ControlToCompare="dpStartDate" ErrorMessage="The End Date must be greater than the Start Date."
                                                            ToolTip="The End Date must be greater than the Start Date." Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Static" Operator="GreaterThan" Type="Date"></asp:CompareValidator>
                                                        <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                            Type="Date"></asp:CompareValidator>
                                                    </span>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <uc2:DatePicker ID="dpEndDate" ValidationGroup="CompensationUpdate" runat="server"
                                                            TextBoxWidth="90%" AutoPostBack="false" />
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:CompareValidator ID="compDateRange" runat="server" ControlToValidate="dpEndDate"
                                                            ValidationGroup="CompensationUpdate" ControlToCompare="dpStartDate" ErrorMessage="The End Date must be greater than the Start Date."
                                                            ToolTip="The End Date must be greater than the Start Date." Text="*" EnableClientScript="false"
                                                            SetFocusOnError="true" Display="Static" Operator="GreaterThan" Type="Date"></asp:CompareValidator>
                                                        <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                            ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                                            Type="Date"></asp:CompareValidator>
                                                    </span>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width11Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Practice Area</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblpractice" runat="server" Text='<%# Eval("HtmlEncodedPracticeName")%>'></asp:Label></ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ValidationGroup="CompensationUpdate" ID="ddlPractice" runat="server"
                                                        CssClass="Width85Percent">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="custValPractice" runat="server" ToolTip="Please select Practice Area"
                                                        ValidationGroup="CompensationUpdate" Display="Dynamic" Text="*" ErrorMessage="Please select Practice Area"
                                                        OnServerValidate="custValPractice_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList ValidationGroup="CompensationUpdate" ID="ddlPractice" runat="server"
                                                        CssClass="Width85Percent">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="custValPractice" runat="server" ToolTip="Please select Practice Area"
                                                        ValidationGroup="CompensationUpdate" Display="Dynamic" Text="*" ErrorMessage="Please select Practice Area"
                                                        OnServerValidate="custValPractice_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width14Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Seniority</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblseniority" runat="server" Text='<%# Eval("SeniorityName")%>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ValidationGroup="CompensationUpdate" ID="ddlSeniorityName" runat="server"
                                                        CssClass="Width80Percent">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="custValSeniority" runat="server" ToolTip="Please select Seniority"
                                                        ValidationGroup="CompensationUpdate" Display="Dynamic" Text="*" ErrorMessage="Please select Seniority"
                                                        OnServerValidate="custValSeniority_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList ValidationGroup="CompensationUpdate" ID="ddlSeniorityName" runat="server"
                                                        CssClass="Width80Percent">
                                                    </asp:DropDownList>
                                                    <asp:CustomValidator ID="custValSeniority" runat="server" ToolTip="Please select Seniority"
                                                        ValidationGroup="CompensationUpdate" Display="Dynamic" Text="*" ErrorMessage="Please select Seniority"
                                                        OnServerValidate="custValSeniority_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width14Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Basis</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBasis" runat="server" Text='<%# Eval("TimescaleName") %>'></asp:Label></ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlBasis" runat="server" CssClass="Width90Percent" onchange="return EnableDisableVacationDays(this);">
                                                        <asp:ListItem Text="W2-Salary" Value="W2-Salary"></asp:ListItem>
                                                        <asp:ListItem Text="W2-Hourly" Value="W2-Hourly"></asp:ListItem>
                                                        <asp:ListItem Text="1099/Hourly" Value="1099/Hourly"></asp:ListItem>
                                                        <asp:ListItem Text="1099/POR" Value="1099/POR"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:DropDownList ID="ddlBasis" runat="server" CssClass="Width90Percent" onchange="return EnableDisableVacationDays(this);">
                                                        <asp:ListItem Text="W2-Salary" Value="W2-Salary"></asp:ListItem>
                                                        <asp:ListItem Text="W2-Hourly" Value="W2-Hourly"></asp:ListItem>
                                                        <asp:ListItem Text="1099/Hourly" Value="1099/Hourly"></asp:ListItem>
                                                        <asp:ListItem Text="1099/POR" Value="1099/POR"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width8Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Amount</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbAmount" runat="server" Text='<%# Bind("Amount") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <asp:TextBox ID="txtAmount" ValidationGroup="CompensationUpdate" runat="server" CssClass="Width80Percent textRight"></asp:TextBox>
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Amount is required." ToolTip="The Amount is required."
                                                            Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compAmount" runat="server" ControlToValidate="txtAmount"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="A number with 2 decimal digits is allowed for the Amount."
                                                            ToolTip="A number with 2 decimal digits is allowed for the Amount." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck" Type="Currency"
                                                            Display="Dynamic"></asp:CompareValidator>
                                                    </span>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <asp:TextBox ID="txtAmount" ValidationGroup="CompensationUpdate" runat="server" CssClass="Width80Percent textRight"></asp:TextBox>
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="The Amount is required." ToolTip="The Amount is required."
                                                            Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="compAmount" runat="server" ControlToValidate="txtAmount"
                                                            ValidationGroup="CompensationUpdate" ErrorMessage="A number with 2 decimal digits is allowed for the Amount."
                                                            ToolTip="A number with 2 decimal digits is allowed for the Amount." Text="*"
                                                            EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck" Type="Currency"
                                                            Display="Dynamic"></asp:CompareValidator>
                                                    </span>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width6Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Vacation</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbVacationDays" runat="server" Text='<%# Bind("VacationDays") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <asp:TextBox ID="txtVacationDays" ValidationGroup="CompensationUpdate" runat="server"
                                                            CssClass="Width80Percent" Text="0"></asp:TextBox>
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:CompareValidator ID="compVacationDays" runat="server" ControlToValidate="txtVacationDays"
                                                            ValidationGroup="CompensationUpdate" Display="Dynamic" EnableClientScript="False"
                                                            ErrorMessage="The Vacation Days must be an integer number." Operator="DataTypeCheck"
                                                            ToolTip="The Vacation Days must be an integer number." Type="Integer">*</asp:CompareValidator>
                                                        <asp:RequiredFieldValidator ID="rfvVacationDays" runat="server" ControlToValidate="txtVacationDays"
                                                            ValidationGroup="CompensationUpdate" Text="*" EnableClientScript="false" Display="Dynamic"
                                                            ErrorMessage="Vacation days is required" ToolTip="Vacation days is required"></asp:RequiredFieldValidator>
                                                    </span>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <span class="fl-left Width85Percent">
                                                        <asp:TextBox ID="txtVacationDays" ValidationGroup="CompensationUpdate" runat="server"
                                                            CssClass="Width80Percent" Text="0"></asp:TextBox>
                                                    </span><span class="Width15Percent vMiddle">
                                                        <asp:CompareValidator ID="compVacationDays" runat="server" ControlToValidate="txtVacationDays"
                                                            ValidationGroup="CompensationUpdate" Display="Dynamic" EnableClientScript="False"
                                                            ErrorMessage="The Vacation Days must be an integer number." Operator="DataTypeCheck"
                                                            ToolTip="The Vacation Days must be an integer number." Type="Integer">*</asp:CompareValidator>
                                                        <asp:RequiredFieldValidator ID="rfvVacationDays" runat="server" ControlToValidate="txtVacationDays"
                                                            ValidationGroup="CompensationUpdate" Text="*" EnableClientScript="false" Display="Dynamic"
                                                            ErrorMessage="Vacation days is required" ToolTip="Vacation days is required"></asp:RequiredFieldValidator>
                                                    </span>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width6Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Sales Commission %</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSalesCommFraction" runat="server" Text='<%# ((decimal?)Eval("SalesCommissionFractionOfMargin")).HasValue ? 
                                                    ((decimal?)Eval("SalesCommissionFractionOfMargin")).Value.ToString() : string.Empty %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtSalesCommission" ValidationGroup="CompensationUpdate" runat="server"
                                                        CssClass="Width80Percent"></asp:TextBox>
                                                    <asp:CustomValidator ID="custValSalesCommission" runat="server" Display="Dynamic"
                                                        ValidationGroup="CompensationUpdate" Text="*" OnServerValidate="custValSalesCommission_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="txtSalesCommission" ValidationGroup="CompensationUpdate" runat="server"
                                                        CssClass="Width80Percent"></asp:TextBox>
                                                    <asp:CustomValidator ID="custValSalesCommission" runat="server" Display="Dynamic"
                                                        ValidationGroup="CompensationUpdate" Text="*" OnServerValidate="custValSalesCommission_OnServerValidate">
                                                    </asp:CustomValidator>
                                                </FooterTemplate>
                                                <ItemStyle CssClass="Width5Percent" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgCompensationDelete" runat="server" AlternateText="Delete"
                                                        EndDate='<%# ((DateTime?)Eval("EndDate")).HasValue ? ((DateTime?)Eval("EndDate")).Value.ToString("MM/dd/yyyy") : string.Empty %>'
                                                        StartDate='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>' ImageUrl="~/Images/cross_icon.png"
                                                        OnClick="imgCompensationDelete_OnClick" />
                                                    <AjaxControlToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" ConfirmText="Are you sure you want to delete this Compensation?"
                                                        runat="server" TargetControlID="imgCompensationDelete">
                                                    </AjaxControlToolkit:ConfirmButtonExtender>
                                                    <asp:CustomValidator ID="cvDeleteCompensation" runat="server" Text="*" ErrorMessage="The Person is active during this compensation period."
                                                        ToolTip="The Person is active during this compensation period." ValidationGroup="CompensationDelete"></asp:CustomValidator>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                </EditItemTemplate>
                                                <ItemStyle CssClass="Width3Percent" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwEmploymentHistory" runat="server">
                                <asp:Panel ID="pnlEmploymentHistory" runat="server" CssClass="tab-pane">
                                    <asp:GridView ID="gvEmploymentHistory" runat="server" AutoGenerateColumns="false"
                                        CssClass="CompPerfTable CompensationHistory" ShowFooter="false">
                                        <AlternatingRowStyle CssClass="alterrow" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Hire Date
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHireDate" runat="server" Text='<%# ((DateTime)Eval("HireDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Termination Date
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHireDate" runat="server" Text='<%# ((DateTime?)Eval("TerminationDate")).HasValue ? ((DateTime)Eval("TerminationDate")).ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Termination Reason
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHireDate" runat="server" Text='<%# GetTerminationReasonById((int?)Eval("TerminationReasonId")) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwRecruiter" runat="server">
                                <asp:Panel ID="pnlRecruiter" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc1:RecruiterInfo ID="recruiterInfo" NeedFirstItemForRecruiterDropDown="true" runat="server"
                                        OnInfoChanged="recruiterInfo_InfoChanged" ShowCommissionDetails="false" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwCommission" runat="server">
                                <asp:Panel ID="pnlCommission" runat="server" CssClass="tab-pane WholeWidth">
                                    <table>
                                        <tr>
                                            <td class="nowrap">
                                                <asp:CheckBox ID="chbSalesCommissions" runat="server" Text="Person receives sales commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbSalesCommissions_CheckedChanged" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSalesCommissionsGross" runat="server" CssClass="Width60Px" Enabled="False"
                                                    onchange="setDirty();"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqSalesCommissionsGross" runat="server" ControlToValidate="txtSalesCommissionsGross"
                                                    ErrorMessage="The Sales Commission % of Gross Margin is required." ToolTip="The Sales Commission % of Gross Margin is required."
                                                    ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="compSalesCommissionsGross" runat="server" ControlToValidate="txtSalesCommissionsGross"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for Sales Commission % of Gross Margin."
                                                    ToolTip="A number with 2 decimal digits is allowed for Sales Commission % of Gross Margin."
                                                    ValidationGroup="Person" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                    Display="Dynamic" Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                            </td>
                                            <td colspan="3">
                                                &nbsp;%&nbsp;of&nbsp;gross&nbsp;margin
                                            </td>
                                            <td>
                                                &nbsp;&nbsp;&nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="9" align="left">
                                                <h3>
                                                    Recruiting Commissions</h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="8">
                                                <asp:GridView ID="gvRecruitingCommissions" runat="server" EmptyDataText="No commissions."
                                                    AutoGenerateColumns="False" CssClass="CompPerfTable WholeWidth" GridLines="None"
                                                    BackColor="White">
                                                    <AlternatingRowStyle BackColor="#F9FAFF" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Start  Date">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnRecruitingCommissionStartDate" runat="server" CausesValidation="false"
                                                                    OnClientClick="if (!confirmSaveDirty()) return false;" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'
                                                                    CommandArgument='<%# Eval("CommissionHeaderId") %>' OnCommand="btnRecruitingCommissionStartDate_Command"
                                                                    Visible='<%# UserIsAdministrator %>'></asp:LinkButton><asp:Label ID="lblRecruitingCommissionStartDate"
                                                                        runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>'
                                                                        Visible='<%# !UserIsAdministrator %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    Start Date</div>
                                                            </HeaderTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    End Date</div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime?)Eval("EndDate")).Value.AddDays(-1).ToString("MM/dd/yyyy") : string.Empty %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Commissions">
                                                            <HeaderTemplate>
                                                                <div class="ie-bg">
                                                                    Commissions</div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("TextLine") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <EditItemTemplate>
                                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TextLine") %>'></asp:TextBox>
                                                            </EditItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                            <td>
                                                Click on Start Date column to edit an item<br />
                                                <asp:ShadowedTextButton ID="btnAddDefaultRecruitingCommission" runat="server" Text="Add Recruiting Commission"
                                                    OnClientClick="if (!confirmSaveDirty()) return false;" OnClick="btnAddDefaultRecruitingCommission_Click"
                                                    CssClass="add-btn" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="chbManagementCommissions" runat="server" Text="Person receives practice mgmt commission"
                                                    AutoPostBack="True" OnCheckedChanged="chbManagementCommissions_CheckedChanged" />
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtManagementCommission" runat="server" CssClass="Width60Px" Enabled="False"
                                                    onchange="setDirty();"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="reqManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="The Margin is required." ToolTip="The Margin is required." ValidationGroup="Person"
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="compManagementCommission" runat="server" ControlToValidate="txtManagementCommission"
                                                    ErrorMessage="A number with 2 decimal digits is allowed for the Margin." ToolTip="A number with 2 decimal digits is allowed for the Margin."
                                                    Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="Person"
                                                    Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                            </td>
                                            <td>
                                                &nbsp;of&nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td colspan="2">
                                                <asp:RadioButtonList ID="rlstManagementCommission" runat="server" RepeatDirection="Vertical"
                                                    RepeatLayout="Flow" Enabled="False">
                                                    <asp:ListItem Text="Own margin" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Sub-ordinate person margin" Value="2" Selected="True"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwRates" runat="server">
                            </asp:View>
                            <asp:View ID="vwWhatIf" runat="server">
                                <asp:Panel ID="pnlWhatIf" runat="server" CssClass="tab-pane WholeWidth">
                                    <div class="bgColorNone">
                                        <uc1:WhatIf ID="whatIf" runat="server" DisplayTargetMargin="True" TargetMarginReadOnly="True"
                                            DisplayDefinedTermsAndCalcs="true" />
                                    </div>
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwProjects" runat="server">
                                <asp:Panel ID="pnlProjects" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:PersonProjects ID="personProjects" runat="server" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwOpportunities" runat="server">
                                <asp:Panel ID="pnOpportunities" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:OpportunityList ID="personOpportunities" runat="server" AllowAutoRedirectToDetails="false"
                                        FilterMode="ByTargetPerson" />
                                </asp:Panel>
                            </asp:View>
                            <asp:View ID="vwActivityLog" runat="server">
                                <asp:Panel ID="pnlLog" runat="server" CssClass="tab-pane WholeWidth">
                                    <uc:Notes ID="nPerson" runat="server" Target="Person" OnNoteAdded="nPerson_OnNoteAdded" />
                                    <uc:ActivityLog runat="server" ID="activityLog" DisplayDropDownValue="TargetPerson"
                                        DateFilterValue="Year" ShowDisplayDropDown="false" ShowProjectDropDown="false"
                                        ShowPersonDropDown="false" />
                                </asp:Panel>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ValidationSummary ID="valsPerson" runat="server" EnableClientScript="false"
                            ValidationGroup="Person" />
                        <asp:ValidationSummary ID="valsManager" runat="server" EnableClientScript="false"
                            ValidationGroup="ActiveManagers" />
                        <asp:ValidationSummary ID="valSumCompensationDelete" runat="server" EnableClientScript="false"
                            ValidationGroup="CompensationDelete" />
                        <asp:ValidationSummary ID="valSumCompensation" runat="server" EnableClientScript="false"
                            ValidationGroup="CompensationUpdate" />
                        <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green"
                            WarningColor="Orange" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc:MessageLabel ID="mlError" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                            WarningColor="Orange" EnableViewState="false" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="if (!validateStatus()) return false;" />&nbsp;
                        <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnField" runat="server" />
            <asp:HiddenField ID="hdnOpenPopUP" runat="server"></asp:HiddenField>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeViewTerminationDateErrors" runat="server"
                TargetControlID="hdnField" CancelControlID="btnClose" BackgroundCssClass="modalBackground"
                PopupControlID="pnlTerminationDateErrors" DropShadow="false" />
            <asp:Panel ID="pnlTerminationDateErrors" runat="server" CssClass="popUp PopUpPersonDetailPage"
                Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th>
                            Error
                            <asp:Button ID="btnClose" ToolTip="Close" runat="server" CssClass="mini-report-closeNew"
                                Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding6">
                            <div id="dvTerminationDateErrors" runat="server" visible="false" class="errorDivPersonDetailPage">
                                <asp:Label ID="lblTerminationDateError" runat="server" CssClass="PaddingTop7">
                                </asp:Label><br />
                                <asp:Label ID="lblTimeEntriesExist" runat="server" CssClass="PaddingTop7">
                                </asp:Label>
                                <div id="dvProjectMilestomesExist" class="PaddingTop5" runat="server">
                                    <asp:Label ID="lblProjectMilestomesExist" runat="server">
                                    </asp:Label><br />
                                    <asp:DataList ID="dtlProjectMilestones" runat="server" CssClass="WS-Normal">
                                        <ItemTemplate>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-<%# ((DataTransferObjects.Milestone)Container.DataItem).Project.Name+
                                             "-" + ((DataTransferObjects.Milestone)Container.DataItem).Description%>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </div>
                                <div id="divOwnerProjectsExist" class="PaddingTop10" runat="server">
                                    <asp:Label ID="lblOwnerProjectsExist" runat="server" Text="Person is designated as the Owner for the following projects:">
                                    </asp:Label><br />
                                    <asp:DataList ID="dtlOwnerProjects" runat="server" CssClass="WS-Normal">
                                        <ItemTemplate>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-<%# ((DataTransferObjects.Project)Container.DataItem).Name %>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </div>
                                <div id="divOwnerOpportunitiesExist" class="PaddingTop10" runat="server">
                                    <asp:Label ID="lblOwnerOpportunities" runat="server" Text="Person is designated as the Owner for the following Opportunities:">
                                    </asp:Label><br />
                                    <asp:DataList ID="dtlOwnerOpportunities" runat="server" CssClass="WS-Normal">
                                        <ItemTemplate>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-<%# ((DataTransferObjects.Opportunity)Container.DataItem).Name %>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding6">
                            Click OK to terminate this person and set an end date for all applicable projects.
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="Padding6">
                            <table class="Width70Percent">
                                <tr>
                                    <td class="textCenter Width50Percent">
                                        <asp:ImageButton ID="imgPrinter" runat="server" ImageUrl="~/Images/printer.png" ToolTip="Print"
                                            OnClientClick="return printform();" />
                                        <asp:ImageButton ID="lnkSaveReport" runat="server" ImageUrl="~/Images/saveToDisk.png"
                                            class="Margin-Left10Px" OnClientClick="saveReport();" OnClick="lnkSaveReport_OnClick"
                                            ToolTip="Save Report" /><asp:HiddenField ID="hdnSaveReportText" runat="server" />
                                    </td>
                                    <td class="textCenter Width50Percent">
                                        <asp:Button ID="btnTerminationProcessOK" runat="server" Text="OK" OnClick="btnTerminationProcessOK_OnClick"
                                            CssClass="Width60Px" />
                                        &nbsp;
                                        <asp:Button ID="btnTerminationProcessCancel" runat="server" Text="Cancel" OnClick="btnTerminationProcessCancel_OnClick"
                                            CssClass="Width60Px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeViewPersonTerminationPopup" runat="server"
                TargetControlID="hdnOpenPopUP" CancelControlID="btnPersonTerminateCancel" BackgroundCssClass="modalBackground"
                PopupControlID="pnlPersonTermination" DropShadow="false" />
            <asp:Panel ID="pnlPersonTermination" runat="server" CssClass="popUp TerminationPopUpPersonDetailPage"
                Style="display: none;">
                <table>
                    <tr>
                        <td class="textleft Padding6">
                            Termination Date
                        </td>
                        <td class="textleft Padding6 no-wrap">
                            :
                            <uc2:DatePicker ID="dtpPopUpTerminateDate" runat="server" BehaviorID="dtpPopUpTerminateDate" />
                            <asp:RequiredFieldValidator ID="rfvDtpPopUpTerminateDate" runat="server" ControlToValidate="dtpPopUpTerminateDate"
                                Text="*" ErrorMessage="To terminate the person the Termination Date should be specified."
                                ToolTip="To terminate the person the Termination Date should be specified." ValidationGroup="PersonTerminate"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="dtpPopUpTerminateDate"
                                Display="Dynamic" Enabled="False" EnableTheming="True" ErrorMessage="The Termination Date must be in the format 'MM/dd/yyyy'"
                                Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="PersonTerminate"
                                ToolTip="The Termination Date must be in the format 'MM/dd/yyyy'" Type="Date">*</asp:CompareValidator>
                            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="dtpPopUpTerminateDate"
                                ControlToCompare="dtpHireDate" Operator="GreaterThan" Type="Date" ErrorMessage="Termination date should be greater than Hire date."
                                Display="Dynamic" Text="*" ValidationGroup="PersonTerminate" ToolTip="Termination date should be greater than Hire date."
                                SetFocusOnError="true"></asp:CompareValidator>
                            <%--<asp:CustomValidator ID="CustomValidator2" runat="server" ErrorMessage="" ToolTip=""
                                Display="Dynamic" ValidationGroup="PersonTerminate" Text="*"
                                OnServerValidate="custTerminationDateTE_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="CustomValidator3" runat="server" ErrorMessage="Unable to set Termination Date for this person because this person is set as default career counselor. Please select another default career counselor and refresh this page to enter termination date for this person."
                                Display="Dynamic" ValidationGroup="PersonTerminate" Text="*"
                                OnServerValidate="custIsDefautManager_ServerValidate"></asp:CustomValidator>--%>
                        </td>
                    </tr>
                    <tr>
                        <td class="textleft Padding6">
                            Termination Reason
                        </td>
                        <td class="textleft Padding6 no-wrap">
                            :
                            <asp:DropDownList ID="ddlPopUpTerminationReason" runat="server">
                            </asp:DropDownList>
                            <asp:CustomValidator ID="custPopUpTerminationReason" runat="server" ErrorMessage="To terminate the person the Termination Reason should be specified."
                                ToolTip="To terminate the person the Termination Reason should be specified."
                                ValidationGroup="PersonTerminate" Text="*" Display="Dynamic" OnServerValidate="custPopUpTerminationReason_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="Padding6">
                            <asp:ValidationSummary ID="valSummaryTerminationPopup" runat="server" ValidationGroup="PersonTerminate" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="alignCenter Padding6">
                            <asp:Button ID="btnPersonTerminate" Text="OK" runat="server" OnClick="btnPersonTerminate_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnPersonTerminateCancel" Text="Cancel" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="lnkSaveReport" />
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsActivePersons" runat="server" SelectMethod="PersonListAllShort"
        TypeName="PraticeManagement.PersonService.PersonServiceClient">
        <SelectParameters>
            <asp:Parameter Name="practice" Type="Int32" />
            <asp:Parameter DefaultValue="1" Name="statusId" Type="Int32" />
            <asp:Parameter Name="startDate" Type="DateTime" />
            <asp:Parameter Name="endDate" Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

