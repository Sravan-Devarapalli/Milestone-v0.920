<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MSBadge.ascx.cs" Inherits="PraticeManagement.Controls.Persons.MSBadge" %>
<%@ Register Src="~/Controls/Persons/PersonBadgeHistory.ascx" TagName="BadgeHistory"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<table>
    <tr>
        <td class="MsBadgeTd1">
            18mos Period Start
        </td>
        <td>
            <%--<asp:TextBox ID="txtBadgeStart" runat="server" ReadOnly="true"></asp:TextBox>--%>
            <uc:DatePicker ID="dpBadgeStart" runat="server" OnSelectionChanged="dpBadgeStart_Changed"
                AutoPostBack="true" />
            <asp:RequiredFieldValidator ID="reqBadgeStart" runat="server" ControlToValidate="dpBadgeStart"
                ErrorMessage="18-Month Badge start date is required." ToolTip="18-Month Badge start date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compBadgeStart" runat="server" ControlToValidate="dpBadgeStart"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter 18-month start date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter 18-month start date in the correct format: MM/DD/YYYY."
                Text="*" Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CustomValidator ID="custBeforeJuly" runat="server" ControlToValidate="dpBadgeStart"
                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="18 months start date cannot be set before 7/1/2014."
                ToolTip="18 months start date cannot be set before 7/1/2014." SetFocusOnError="True"
                OnServerValidate="custBeforeJuly_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="custAftertheBreak" runat="server" ControlToValidate="dpBadgeStart"
                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="18 month start date should be greater than previous break end date."
                ToolTip="18 month start date should be greater than previous break end date." SetFocusOnError="True"
                OnServerValidate="custAftertheBreak_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
        </td>
        <td class="MsBadgeTd2">
            Date Source -
            <asp:Label ID="lblBadgeStartDateSource" runat="server" Text="Available Now"></asp:Label>
        </td>
        <td colspan="2">
            Previously at Microsoft in any capacity?
            <asp:DropDownList ID="ddlPreviousAtMS" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPreviousAtMS_OnIndexChanged">
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                <asp:ListItem Text="No" Value="0" Selected="True"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td style="width: 100px;">
            <asp:LinkButton ID="lnkHistory" runat="server" Text="History" OnClick="lnkHistory_Click"></asp:LinkButton>
        </td>
        <td>
            Badge Deactivated Date:
        </td>
        <td>
            <uc:DatePicker ID="dpDeactivatedDate" runat="server" OnSelectionChanged="dpDeactivatedDate_Change"
                AutoPostBack="true" />
            <asp:CompareValidator ID="compDeactivatedDate" runat="server" ControlToValidate="dpDeactivatedDate"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter badge deactivation date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter badge deactivation date in the correct format: MM/DD/YYYY."
                Text="*" Type="Date" EnableClientScript="false"></asp:CompareValidator>
            <asp:CustomValidator ID="custDeactivateNeed18moDates" runat="server" ControlToValidate="dpDeactivatedDate"
                ErrorMessage="Cannot enter Badge deactivated date when there are no 18 month dates."
                ToolTip="Cannot enter Badge deactivated date when there are no 18 month dates."
                Text="*" Display="Dynamic" EnableTheming="True" SetFocusOnError="True" OnServerValidate="custDeactivateNeed18moDates_ServerValidate"
                EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="custDeactivateDateIn18moDates" runat="server" ControlToValidate="dpDeactivatedDate"
                ErrorMessage="Badge deactivated date should be within 18-Month start date and end dates."
                ToolTip="Badge deactivated date should be within 18-Month start date and end dates."
                Text="*" Display="Dynamic" EnableTheming="True" SetFocusOnError="True" OnServerValidate="custDeactivateDateIn18moDates_ServerValidate"
                EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="custDeactivateDatePriorProjectDates" runat="server" ControlToValidate="dpDeactivatedDate"
                ErrorMessage="You’re entering a deactivation date prior to approved badge dates, please clear or change deactivation date."
                ToolTip="You’re entering a deactivation date prior to approved badge dates, please clear or change deactivation date."
                Text="*" Display="Dynamic" EnableTheming="True" SetFocusOnError="True" OnServerValidate="custDeactivateDatePriorProjectDates_ServerValidate"
                EnableClientScript="false"></asp:CustomValidator>
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            Project Planned End
        </td>
        <td>
            <asp:TextBox ID="txtPlannedEnd" runat="server" ReadOnly="true"></asp:TextBox>
        </td>
        <td class="MsBadgeTd2">
            Date Source -
            <asp:Label ID="lblPlannedDateSource" runat="server" Text="Available Now"></asp:Label>
        </td>
        <td class="MsBadgeTd1">
            Enter previous badge alias:
        </td>
        <td>
            <asp:TextBox ID="txtPreviousBadgeAlias" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="reqPreviousAlias" runat="server" ControlToValidate="txtPreviousBadgeAlias"
                ErrorMessage="Previous badge alias is required." ToolTip="Previous badge alias is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            6-Month Organic Start
        </td>
        <td>
            <asp:TextBox ID="txtOrganicStart" runat="server" ReadOnly="true"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            18mos End
        </td>
        <td>
            <%--<asp:TextBox ID="txtBadgeEnd" runat="server" ReadOnly="true"></asp:TextBox>--%>
            <uc:DatePicker ID="dpBadgeEnd" runat="server" OnSelectionChanged="dpBadgeEnd_Changed"
                AutoPostBack="true" />
            <asp:RequiredFieldValidator ID="reqBadgeEnd" runat="server" ControlToValidate="dpBadgeEnd"
                ErrorMessage="18-Month Badge end date is required." ToolTip="18-Month Badge end date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compBadgeEnd" runat="server" ControlToValidate="dpBadgeEnd"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter 18-month end date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter 18-month end date in the correct format: MM/DD/YYYY."
                Text="*" Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CompareValidator ID="compBadgeEndLess" runat="server" ControlToValidate="dpBadgeEnd"
                ControlToCompare="dpBadgeStart" Operator="GreaterThanEqual" Type="Date" ErrorMessage="18-Month Badge end date should be greater than or equal to 18-Month Badge start date."
                Display="Dynamic" Text="*" ToolTip="18-Month Badge end date should be greater than or equal to 18-Month Badge start date."
                SetFocusOnError="true" EnableClientScript="false"></asp:CompareValidator>
            <asp:CustomValidator ID="custLessThan18Mo" runat="server" ControlToValidate="dpBadgeEnd"
                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="MS badge period cannot be less than 18 months. Please adjust 18 months start date and end date accordingly."
                ToolTip="MS badge period cannot be less than 18 months. Please adjust 18 months start date and end date accordingly."
                SetFocusOnError="True" OnServerValidate="custLessThan18Mo_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="custMoreThan18Mo" runat="server" ControlToValidate="dpBadgeEnd"
                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="MS badge period cannot be more than 18 months. Please adjust 18 months start date and end date accordingly Or add exception dates to make badge period more than 18 months."
                ToolTip="MS badge period cannot be more than 18 months. Please adjust 18 months start date and end date accordingly Or add exception dates to make badge period more than 18 months."
                SetFocusOnError="True" OnServerValidate="custMoreThan18Mo_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="custProjectsAssigned" runat="server" ControlToValidate="dpBadgeEnd"
                ErrorMessage="Cannot change 18 months dates of the person, as the person is assigned to some projects in current 18 mos date ranges."
                ToolTip="Cannot change 18 months dates of the person, as the person is assigned to some projects in current 18 mos date ranges."
                Text="*" Display="Dynamic" EnableTheming="True" SetFocusOnError="True" OnServerValidate="custProjectsAssigned_ServerValidate"
                EnableClientScript="false"></asp:CustomValidator>
            <asp:CustomValidator ID="cust18moNotInEmployment" runat="server" ErrorMessage="Person 18-Month dates are outside of hire/termination date range."
                ToolTip="Person 18-Month dates are outside of hire/termination date range." Display="Dynamic"
                OnServerValidate="cust18moNotInEmployment_ServerValidate" Text="*" EnableClientScript="false"
                SetFocusOnError="true"></asp:CustomValidator>
        </td>
        <td class="MsBadgeTd2">
            Date Source -
            <asp:Label ID="lblBadgeEndDateSource" runat="server" Text="Available Now"></asp:Label>
        </td>
        <td class="MsBadgeTd1">
            Last Badge Start
        </td>
        <td>
            <uc:DatePicker ID="dtpLastBadgeStart" runat="server" />
            <asp:RequiredFieldValidator ID="reqLastBadgeStart" runat="server" ControlToValidate="dtpLastBadgeStart"
                ErrorMessage="Last badge start date is required." ToolTip="Last badge start date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvLastBadgeStart" runat="server" ControlToValidate="dtpLastBadgeStart"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter last badge start date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter last bagde start date in the correct format: MM/DD/YYYY."
                Text="*" Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CustomValidator ID="custNotFuture" runat="server" ControlToValidate="dtpLastBadgeStart"
                Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="Last badge start cannot be in the future."
                ToolTip="Last badge start cannot be in the future." SetFocusOnError="True" OnServerValidate="custNotFuture_ServerValidate"
                EnableClientScript="false"></asp:CustomValidator>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            6-Month Organic End
        </td>
        <td>
            <asp:TextBox ID="txtOrganicEnd" runat="server" ReadOnly="true"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            6mos Break Start
        </td>
        <td>
            <asp:TextBox ID="txtBreakStart" runat="server" ReadOnly="true"></asp:TextBox>
        </td>
        <td>
            &nbsp;
        </td>
        <td class="MsBadgeTd1">
            Last Badge End
        </td>
        <td colspan="3">
            <uc:DatePicker ID="dtpLastBadgeEnd" runat="server" />
            <asp:RequiredFieldValidator ID="reqLastbadgeEnd" runat="server" ControlToValidate="dtpLastBadgeEnd"
                ErrorMessage="Last badge end date is required." ToolTip="Last badge end date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvLastbadgeEnd" runat="server" ControlToValidate="dtpLastBadgeEnd"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter last badge date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter last badge date in the correct format: MM/DD/YYYY."
                Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CompareValidator ID="cvLastBadgeRange" runat="server" ControlToValidate="dtpLastBadgeEnd"
                ControlToCompare="dtpLastBadgeStart" Operator="GreaterThanEqual" Type="Date"
                ErrorMessage="Last Badge end date should be greater than or equal to Last Badge start date."
                Display="Dynamic" Text="*" ToolTip="Last Badge end date should be greater than or equal to Last Badge start date."
                SetFocusOnError="true" EnableClientScript="false"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            6mos Break End
        </td>
        <td>
            <asp:TextBox ID="txtBreakEnd" runat="server" ReadOnly="true"></asp:TextBox>
        </td>
        <td colspan="5">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td colspan="3" style="padding: 10px">
            <asp:CheckBox ID="chbBlockFromMS" runat="server" Checked="false" Text="Block from MS Projects"
                AutoPostBack="true" OnCheckedChanged="chbBlockFromMS_CheckedChanged" />
        </td>
        <td colspan="3">
            <asp:CheckBox ID="chbException" runat="server" Checked="false" Text="Project Override Exception"
                AutoPostBack="true" OnCheckedChanged="chbException_CheckedChanged" />
        </td>
        <td colspan="2" id="tdExcludeInReports" runat="server">
            <asp:CheckBox ID="chbExcludeInReports" runat="server" Checked="false" Text="Exclude from 18-month Reports" />
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            Block Start
        </td>
        <td colspan="2">
            <uc:DatePicker ID="dtpBlockStart" runat="server" />
            <asp:RequiredFieldValidator ID="reqBlockStart" runat="server" ControlToValidate="dtpBlockStart"
                ErrorMessage="Block start date is required." ToolTip="Block start date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvBlockStart" runat="server" ControlToValidate="dtpBlockStart"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter block start date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter block start date in the correct format: MM/DD/YYYY."
                Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CustomValidator ID="custBlockStartAfterJuly" runat="server" ErrorMessage="Block start date should not be before 7/1/2014."
                ToolTip="Block start date should not be before 7/1/2014." Display="Dynamic" OnServerValidate="custBlockStartAfterJuly_ServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
        </td>
        <td class="MsBadgeTd1">
            Exception Start
        </td>
        <td colspan="2">
            <uc:DatePicker ID="dtpExceptionStart" runat="server" />
            <asp:RequiredFieldValidator ID="reqExceptionStart" runat="server" ControlToValidate="dtpExceptionStart"
                ErrorMessage="Exception start date is required." ToolTip="Exception start date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvExceptionStart" runat="server" ControlToValidate="dtpExceptionStart"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter exception start date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter exception start date in the correct format: MM/DD/YYYY."
                Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CustomValidator ID="cvExceptionStartAfterJuly" runat="server" ErrorMessage="Exception start date should not be before 7/1/2014."
                ToolTip="Exception start date should not be before 7/1/2014." Display="Dynamic"
                OnServerValidate="cvExceptionStartAfterJuly_ServerValidate" Text="*" EnableClientScript="false"
                SetFocusOnError="true"></asp:CustomValidator>
        </td>
        <td colspan="2">
            <asp:CheckBox ID="chbManageServiceContract" runat="server" Checked="false" Text="Microsoft Managed Services Contract" />
        </td>
    </tr>
    <tr>
        <td class="MsBadgeTd1">
            Block End
        </td>
        <td colspan="2">
            <uc:DatePicker ID="dtpBlockEnd" runat="server" />
            <asp:RequiredFieldValidator ID="reqBlockEnd" runat="server" ControlToValidate="dtpBlockEnd"
                ErrorMessage="Block end date is required." ToolTip="Block end date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvBlockEnd" runat="server" ControlToValidate="dtpBlockEnd"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter block end date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter block end date in the correct format: MM/DD/YYYY."
                Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CompareValidator ID="cvBlockDateRange" runat="server" ControlToValidate="dtpBlockEnd"
                ControlToCompare="dtpBlockStart" Operator="GreaterThanEqual" Type="Date" ErrorMessage="Block end date should be greater than or equal to Block start date."
                Display="Dynamic" Text="*" ToolTip="Block end date should be greater than or equal to Block start date."
                SetFocusOnError="true" EnableClientScript="false"></asp:CompareValidator>
            <asp:CustomValidator ID="custBlockDatesInEmpHistory" runat="server" ErrorMessage="Person Block dates are outside of hire/termination date range."
                ToolTip="Person Block dates are outside of hire/termination date range." Display="Dynamic"
                OnServerValidate="custBlockDatesInEmpHistory_ServerValidate" Text="*" EnableClientScript="false"
                SetFocusOnError="true"></asp:CustomValidator>
            <asp:CustomValidator ID="custBlockDatesOverlappedException" runat="server" ErrorMessage="This person currently has a block in place, exception cannot be set. If you have questions, please connect with Operations."
                ToolTip="This person currently has a block in place, exception cannot be set. If you have questions, please connect with Operations."
                Display="Dynamic" OnServerValidate="custBlockDatesOverlappedException_ServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
            <asp:CustomValidator ID="custPersonInProject" runat="server" ErrorMessage="This person is currently assigned to a project/milestone during the requested block dates."
                ToolTip="This person is currently assigned to a project/milestone during the requested dates."
                Display="Dynamic" OnServerValidate="custPersonInProject_ServerValidate" Text="*"
                EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
        </td>
        <td class="MsBadgeTd1">
            Exception End
        </td>
        <td colspan="3">
            <uc:DatePicker ID="dtpExceptionEnd" runat="server" />
            <asp:RequiredFieldValidator ID="reqExceptionEnd" runat="server" ControlToValidate="dtpExceptionEnd"
                ErrorMessage="Exception end date is required." ToolTip="Exception end date is required."
                Display="Dynamic" Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvExceptionEnd" runat="server" ControlToValidate="dtpExceptionEnd"
                Display="Dynamic" EnableTheming="True" ErrorMessage="Please enter exception end date in the correct format: MM/DD/YYYY."
                Operator="DataTypeCheck" SetFocusOnError="True" ToolTip="Please enter exception end date in the correct format: MM/DD/YYYY."
                Type="Date" EnableClientScript="false">*</asp:CompareValidator>
            <asp:CompareValidator ID="cvExceptionDateRanges" runat="server" ControlToValidate="dtpExceptionEnd"
                ControlToCompare="dtpExceptionStart" Operator="GreaterThanEqual" Type="Date"
                ErrorMessage="Exception end date should be greater than or equal to Exception start date."
                Display="Dynamic" Text="*" ToolTip="Exception end date should be greater than or equal to Exception start date."
                SetFocusOnError="true" EnableClientScript="false"></asp:CompareValidator>
            <asp:CustomValidator ID="custExceptionInEmpHistory" runat="server" ErrorMessage="Person Exception dates are outside of hire/termination date range."
                ToolTip="Person Exception dates are outside of hire/termination date range."
                Display="Dynamic" OnServerValidate="custExceptionInEmpHistory_ServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
            <asp:CustomValidator ID="custExceptionDatesOverlappsBlock" runat="server" ErrorMessage="This person currently has an exception in place, block cannot be set."
                ToolTip="This person currently has an exception in place, block cannot be set."
                Display="Dynamic" OnServerValidate="custExceptionDatesOverlappsBlock_ServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
            <asp:CustomValidator ID="custExceptionMorethan18" runat="server" ErrorMessage="Exception dates duration cannot be less than 18 months."
                ToolTip="Exception dates duration cannot be less than 18 months." Display="Dynamic"
                OnServerValidate="custExceptionMorethan18_ServerValidate" Text="*" EnableClientScript="false"
                SetFocusOnError="true"></asp:CustomValidator>
            <asp:CustomValidator ID="custExceptionNotMoreThan18moEndDate" runat="server" ErrorMessage="Requested exception will shorten the person’s 18-month clock. Please remove exception."
                ToolTip="Requested exception will shorten the person’s 18-month clock. Please remove exception."
                Display="Dynamic" OnServerValidate="custExceptionNotMoreThan18moEndDate_ServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:CustomValidator>
        </td>
    </tr>
</table>
<h3>
    Logic20/20 MS Badge History:</h3>
<asp:Repeater ID="repMSBadge" runat="server" OnItemDataBound="repMSBadge_DataBound">
    <HeaderTemplate>
        <div class="BadgeHistory" style="width: 80%">
            <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport zebra WholeWidth">
                <thead>
                    <tr>
                        <th class="TextAlignLeftImp Width20Percent">
                            Project Name
                        </th>
                        <th class="Width7Percent">
                            Project #
                        </th>
                        <th class="Width8Per">
                            Project Stage
                        </th>
                        <th class="Width10Per">
                            Project Start
                        </th>
                        <th class="Width10Per">
                            Project End
                        </th>
                        <th class="Width10Per">
                            Badge Start
                        </th>
                        <th class="Width8Per">
                            Badge End
                        </th>
                        <th class="Width20Percent">
                            Badged Time on Project(in Months)
                        </th>
                        <th class="Width7Percent">
                            Is Approved?
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="padLeft5 textLeft">
                <%# Eval("Project.HtmlEncodedName")%>
            </td>
            <td>
                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("Project.ProjectNumber")%> '
                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                </asp:HyperLink>
            </td>
            <td>
                <asp:Label ID="lblProjectStatus" runat="server"></asp:Label>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("Project.StartDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("Project.EndDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("BadgeStartDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("BadgeEndDate"))%>
            </td>
            <td>
                <%# Eval("BadgeDuration")%>
            </td>
            <td>
                <asp:Label ID="lblIsApproved" runat="server"></asp:Label>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td class="padLeft5 textLeft">
                <%# Eval("Project.HtmlEncodedName")%>
            </td>
            <td>
                <asp:HyperLink ID="hlProjectNumber" runat="server" Text=' <%# Eval("Project.ProjectNumber")%> '
                    Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                </asp:HyperLink>
            </td>
            <td>
                <asp:Label ID="lblProjectStatus" runat="server"></asp:Label>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("Project.StartDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("Project.EndDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("BadgeStartDate"))%>
            </td>
            <td>
                <%# GetDateFormat((DateTime)Eval("BadgeEndDate"))%>
            </td>
            <td>
                <%# Eval("BadgeDuration")%>
            </td>
            <td>
                <asp:Label ID="lblIsApproved" runat="server"></asp:Label>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" class="MSBadgeEmptyDiv" style="display: none;" runat="server">
    There are no previous MS projects for this person.
</div>
<asp:HiddenField ID="hdnHistoryPanel" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeBadgeHistoryPanel" runat="server" BehaviorID="mpeBadgePanelBehaviourId"
    TargetControlID="lnkHistory" BackgroundCssClass="modalBackground" PopupControlID="pnlHistoryPanel"
    OkControlID="btnOKValidationPanel" CancelControlID="btnOKValidationPanel" DropShadow="false" />
<asp:Panel ID="pnlHistoryPanel" runat="server" CssClass="BadgeHistorypopUp" Style="display: none;">
    <table class="WholeWidth">
        <tr>
            <td>
                <uc:BadgeHistory ID="badgeHistory" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="Padding10 textCenter">
                <asp:Button ID="btnOKValidationPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100Px" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdnDeactivateWithinProject" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeDeactivateWithinProject" runat="server"
    TargetControlID="hdnDeactivateWithinProject" BehaviorID="mpeBadgeBehaviourId"
    BackgroundCssClass="modalBackground" PopupControlID="pnlDeactivateWithinProject"
    CancelControlID="btnCancel" DropShadow="false" />
<asp:Panel ID="pnlDeactivateWithinProject" runat="server" CssClass="DeactivateDateValidationpopUp"
    Style="display: none;">
    <table class="WholeWidth">
        <tr>
            <td>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:CustomValidator ID="custDeactivateWithinProject" runat="server" ControlToValidate="dpDeactivatedDate"
                    ErrorMessage="You are trying to enter a badge deactivation date, but the resource has an active badge. Click ‘Change Badge End Date(s)’ to change the badge end date to the deactivation date. Click ‘Cancel’ to change the deactivation date."
                    ToolTip="You are trying to enter a badge deactivation date, but the resource has an active badge. Click ‘Change Badge End Date(s)’ to change the badge end date to the deactivation date. Click ‘Cancel’ to change the deactivation date."
                    Text="You are trying to enter a badge deactivation date, but the resource has an active badge. Click ‘Change Badge End Date(s)’ to change the badge end date to the deactivation date. Click ‘Cancel’ to change the deactivation date."
                    Display="Dynamic" EnableTheming="True" SetFocusOnError="True" OnServerValidate="custDeactivateWithinProject_ServerValidate"
                    EnableClientScript="false"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td class="Padding10 textCenter">
                <asp:Button ID="btnOk" runat="server" ToolTip="Change Badge End Date(s)" Text="Change Badge End Date(s)"
                    OnClick="btnOk_Click" />
                <asp:Button ID="btnCancel" runat="server" ToolTip="Cancel" Text="Cancel" CssClass="Width100Px" />
            </td>
        </tr>
    </table>
</asp:Panel>

