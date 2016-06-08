<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonBar.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonBar" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<tr id="trBar" runat="server" style="white-space: nowrap;">
    <td class="Width2Percent">
        <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
            ImageUrl="~/Images/copy.png" />
    </td>
    <td class="Width3Percent">
        <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
            ToolTip="Save" OnClick="btnInsertPerson_Click" />
        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
            ToolTip="Cancel" />
    </td>
    <td class="Width2Percent">
        &nbsp;
    </td>
    <td class="Width14Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <cc2:CustomDropDown ID="ddlPerson" CssClass="Width98Percent" runat="server" OnSelectedIndexChanged="ddlPersonName_Changed"
                        AutoPostBack="true" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="reqMilestonePersonName" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="" Text="*" EnableClientScript="false" SetFocusOnError="true"
                        Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPersonInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="cvMaxRows" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="Milestone person with same role cannot have more than 5 entries."
                        ToolTip="Milestone person with same role cannot have more than 5 entries." Text="*"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"
                        OnServerValidate="cvMaxRows_ServerValidate"></asp:CustomValidator>
                    <asp:HiddenField ID="hdnIsFromAddBtn" runat="server" />
                </td>
            </tr>
        </table>
    </td>
    <td class="Width7Percent">
        <asp:DropDownList ID="ddlRole" CssClass="Width98Percent" runat="server">
        </asp:DropDownList>
    </td>
    <td class="Width7Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpPersonStartInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="dtpStartDateInsert_OnClientChange(this)" AutoPostBack="false"
                        TextBoxWidth="95%" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="The Person Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        ToolTip="The Person Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CustomValidator ID="custPersonStartInsert" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="The Person Start Date must be greater than or equal to the Milestone Start Date."
                        ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPersonStartInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="A project resource cannot have more than one bill rate on a project during the same time period. Please adjust the start and end dates to make sure that the time periods for the bill rates do not overlap."
                        ToolTip="A project resource cannot have more than one bill rate on a project during the same time period. Please adjust the start and end dates to make sure that the time periods for the bill rates do not overlap."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodOvberlappingInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="The specified period overlaps with Vacation days for this person on the milestone."
                        ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodVacationOverlappingInsert_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width7Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpPersonEndInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="dtpStartDateInsert_OnClientChange(this)" AutoPostBack="false"
                        TextBoxWidth="95%" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="The Person End Date is required." ToolTip="The Person End Date is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="The Person End Date must be less than or equal to the Milestone End Date."
                        ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        OnServerValidate="custPersonEndInsert_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="The Person End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        ToolTip="The Person End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CompareValidator ID="compPersonEndInsert" runat="server" ControlToValidate="dpPersonEndInsert"
                        ControlToCompare="dpPersonStartInsert" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
                        ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width6Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:TextBox ID="txtHoursPerDayInsert" runat="server" CssClass="Width90Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
                        ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                        Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="The Hours Per Day must be greater than 0 and less or equals to 24."
                        ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursPerDayInsert" runat="server"
                        TargetControlID="txtHoursPerDayInsert" FilterMode="ValidChars" FilterType="Custom,Numbers"
                        ValidChars=".">
                    </AjaxControlToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdAmountInsert" runat="server" class="Width6Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                    <asp:TextBox ID="txtAmountInsert" runat="server" CssClass="Width80Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ValidateEmptyText="true" ErrorMessage="The Amount is required." ToolTip="The Amount is required."
                        Text="*" SetFocusOnError="true" EnableClientScript="false" Display="Dynamic"
                        OnServerValidate="reqHourlyRevenue_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtAmount" runat="server" TargetControlID="txtAmountInsert"
                        FilterMode="ValidChars" FilterType="Custom,Numbers" ValidChars=".">
                    </AjaxControlToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width6Percent">
        &nbsp;
    </td>
    <td class="Width6Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:TextBox ID="txtHoursInPeriodInsert" runat="server" CssClass="Width90Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="A number with 2 decimal digits is allowed for the Hours In Period."
                        ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="The Total Hours must be greater than 0." ToolTip="The Total Hours must be greater than 0."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        MinimumValue="0.01" MaximumValue="15000" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
                    <asp:CustomValidator ID="cvHoursInPeriod" runat="server" ToolTip="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ErrorMessage="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                        OnServerValidate="cvHoursInPeriod_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursInPeriodInsert" runat="server"
                        TargetControlID="txtHoursInPeriodInsert" FilterMode="ValidChars" FilterType="Custom,Numbers"
                        ValidChars=".">
                    </AjaxControlToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdBadgeRequired" runat="server" class="Width4Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:CheckBox ID="chbBadgeRequiredInsert" runat="server" OnCheckedChanged="chbBadgeRequired_CheckedChanged"
                        AutoPostBack="true" />
                </td>
                <td class="Width15Percent">
                    <asp:CustomValidator ID="custBlocked" runat="server" ErrorMessage="Person is blocked from being badged, please choose another resource or uncheck this box or change the dates."
                        ToolTip="Person is blocked from being badged, please choose another resource or uncheck this box or change the dates."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custBlocked_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdBadgeStart" runat="server" class="Width7Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpBadgeStartInsert" runat="server" ValidationGroup='<%# GetValidationGroup() %>'
                        OnClientChange="return true;" TextBoxWidth="95%" AutoPostBack="false" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqBadgeStart" runat="server" ControlToValidate="dpBadgeStartInsert"
                        ErrorMessage="The Badge Start Date is required." ToolTip="The Badge Start Date is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="compBadgeStartType" runat="server" ControlToValidate="dpBadgeStartInsert"
                        ErrorMessage="The Badge Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        ToolTip="The Badge Start Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CustomValidator ID="custBadgeStart" runat="server" ControlToValidate="dpBadgeStartInsert"
                        ErrorMessage="The Badge Start Date must be greater than or equal to the Milestone Start Date."
                        ToolTip="The Badge Start Date must be greater than or equal to the Milestone Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custBadgeStart_ServerValidate"></asp:CustomValidator>
                    <asp:CompareValidator ID="compBadgeStartWithPersonStart" runat="server" ControlToValidate="dpBadgeStartInsert"
                        ControlToCompare="dpPersonStartInsert" ErrorMessage="The Badge Start Date must be after or equal to the Person Start Date in the project."
                        ToolTip="The Badge Start Date must be after or equal to the Person Start Date in the project."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                    <asp:CustomValidator ID="custAftertheBreak" runat="server" ControlToValidate="dpBadgeStartInsert"
                        Text="*" Display="Dynamic" EnableTheming="True" ErrorMessage="18 month start date should be greater than previous break end date."
                        ToolTip="18 month start date should be greater than previous break end date."
                        ValidationGroup="<%# GetValidationGroup() %>" SetFocusOnError="True"
                        OnServerValidate="custAftertheBreak_ServerValidate" EnableClientScript="false"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdBadgeEnd" runat="server" class="Width7Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpBadgeEndInsert" runat="server" ValidationGroup='<%# GetValidationGroup() %>'
                        OnClientChange="return true;" TextBoxWidth="95%" AutoPostBack="false" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqBadgeEnd" runat="server" ControlToValidate="dpBadgeEndInsert"
                        ErrorMessage="The Badge End Date is required." ToolTip="The Badge End Date is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="custBadgeEnd" runat="server" ControlToValidate="dpBadgeEndInsert"
                        ErrorMessage="The Badge End Date must be less than or equal to the Milestone End Date."
                        ToolTip="The Badge End Date must be less than or equal to the Milestone End Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        OnServerValidate="custBadgeEnd_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compBadgeEndType" runat="server" ControlToValidate="dpBadgeEndInsert"
                        ErrorMessage="The Badge End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        ToolTip="The Badge End Date has an incorrect format. It must be 'MM/DD/YYYY'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CompareValidator ID="compBadgeEnd" runat="server" ControlToValidate="dpBadgeEndInsert"
                        ControlToCompare="dpBadgeStartInsert" ErrorMessage="The Badge End Date must be greater than or equal to the badge Start Date."
                        ToolTip="The Badge End Date must be greater than or equal to the badge Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                    <asp:CustomValidator ID="custBadgeInBreakPeriod" runat="server" ErrorMessage="" Text="*"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"
                        OnServerValidate="custBadgeInBreakPeriod_ServerValidate"></asp:CustomValidator>
                    <asp:CompareValidator ID="compBadgeEndWithPersonEnd" runat="server" ControlToValidate="dpBadgeEndInsert"
                        ControlToCompare="dpPersonEndInsert" ErrorMessage="The Badge End Date must be less than or equal to the Person End Date in the project."
                        ToolTip="The Badge End Date must be less than or equal to the Person End Date in the project."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" Operator="LessThanEqual" Type="Date"></asp:CompareValidator>
                    <asp:CustomValidator ID="custBadgeHasMoredays" runat="server" ErrorMessage="Invalid Badge Start/End dates.  Person cannot be assigned to project/phase longer than 18mos without a badge exception."
                        Text="*" ToolTip="Invalid Badge Start/End dates.  Person cannot be assigned to project/phase longer than 18mos without a badge exception."
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"
                        OnServerValidate="custBadgeHasMoredays_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custBadgeNotInEmpHistory" runat="server" ErrorMessage="The person's badge dates in a milestone should be within their hire and termination dates."
                        Text="*" ToolTip="The person's badge dates in a milestone should be within their hire and termination dates."
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"
                        OnServerValidate="custBadgeNotInEmpHistory_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custBadgeAfterJuly" runat="server" ErrorMessage="Person's badge dates cannot be before 7/1/2014."
                        Text="*" ToolTip="Person's badge dates cannot be before 7/1/2014." EnableClientScript="false"
                        SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"
                        OnServerValidate="custBadgeAfterJuly_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdConsultantEnd" runat="server" class="Width5Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:Label ID="lblConsultantsEnd" runat="server"></asp:Label>
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
        </table>
    </td>
    <td id="tdBadgeException" runat="server" class="Width4Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:CheckBox ID="chbBadgeExceptionInsert" runat="server" />
                    <asp:CustomValidator ID="custExceptionNotMoreThan18moEndDate" runat="server" ErrorMessage=""
                        Text="*" ToolTip="" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custExceptionNotMoreThan18moEndDate_ServerValidate"></asp:CustomValidator>
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
        </table>
    </td>
    <td id="tdOpsApproved" runat="server" class="Width4Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:CheckBox ID="chbOpsApprovedInsert" runat="server" />
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
        </table>
    </td>
    <td class="Width3Percent">
        &nbsp;
    </td>
</tr>

