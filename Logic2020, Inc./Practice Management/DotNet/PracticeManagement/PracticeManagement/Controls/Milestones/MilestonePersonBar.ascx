<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonBar.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonBar" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<tr id="trBar" runat="server">
    <td class="Width2Percent">
        <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
            ImageUrl="~/Images/copy.png" />
    </td>
    <td class="Width4Percent">
        <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
            ToolTip="Save" OnClick="btnInsertPerson_Click" />
        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
            ToolTip="Cancel" />
    </td>
    <td class="Width2Percent">
    </td>
    <td class="Width21Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <cc2:CustomDropDown ID="ddlPerson" CssClass="Width98Percent" runat="server" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="The Person Name is required." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="reqMilestonePersonName" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="The Person Name is required." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPersonInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="cvMaxRows" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="Milestone person with same role cannot have more than 5 entries."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="cvMaxRows_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width10Percent">
        <asp:DropDownList ID="ddlRole" CssClass="Width98Percent" runat="server">
        </asp:DropDownList>
    </td>
    <td class="Width11Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpPersonStartInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="return true;" AutoPostBack="false" TextBoxWidth="95%" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="" ToolTip="The Person Start Date is required." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="" ToolTip="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CustomValidator ID="custPersonStartInsert" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="" ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPersonStartInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="" ToolTip="The specified period overlaps with another for this person with same role on the milestone."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodOvberlappingInsert_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStartInsert"
                        ErrorMessage="" ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodVacationOverlappingInsert_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width11Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <uc2:DatePicker ID="dpPersonEndInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="return true;" AutoPostBack="false" TextBoxWidth="95%" />
                </td>
                <td class="Width15Percent">
                    <asp:RequiredFieldValidator ID="reqPersonEnd" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="" ToolTip="The Person End Date is required." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" Display="Dynamic" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="custPersonEnd" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="" ToolTip="The Person End Date must be less than or equal to the Milestone End Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        OnServerValidate="custPersonEndInsert_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compPersonEndType" runat="server" ControlToValidate="dpPersonEndInsert"
                        ErrorMessage="" ToolTip="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:CompareValidator ID="compPersonEndInsert" runat="server" ControlToValidate="dpPersonEndInsert"
                        ControlToCompare="dpPersonStartInsert" ErrorMessage="" ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width11Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:TextBox ID="txtHoursPerDayInsert" runat="server" CssClass="Width90Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="" ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
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
    <td id="tdAmountInsert" runat="server" class="Width9Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                    <asp:TextBox ID="txtAmountInsert" runat="server" CssClass="Width80Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ValidateEmptyText="true" ErrorMessage="" ToolTip="The Amount is required." Text="*"
                        SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" OnServerValidate="reqHourlyRevenue_ServerValidate"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtAmount" runat="server" TargetControlID="txtAmountInsert"
                        FilterMode="ValidChars" FilterType="Custom,Numbers" ValidChars=".">
                    </AjaxControlToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width7Percent">
    </td>
    <td class="Width9Percent">
        <table class="WholeWidth">
            <tr>
                <td class="Width85Percent">
                    <asp:TextBox ID="txtHoursInPeriodInsert" runat="server" CssClass="Width90Percent"></asp:TextBox>
                </td>
                <td class="Width15Percent">
                    <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="" ToolTip="The Total Hours must be greater than 0." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" Display="Dynamic" MinimumValue="0.01" MaximumValue="15000"
                        Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
                    <asp:CustomValidator ID="cvHoursInPeriod" runat="server" ToolTip="Total hours should be a larger value so that Hoursperday will be greater than Zero after rounding."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        OnServerValidate="cvHoursInPeriod_ServerValidate" ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtHoursInPeriodInsert" runat="server"
                        TargetControlID="txtHoursInPeriodInsert" FilterMode="ValidChars" FilterType="Custom,Numbers"
                        ValidChars=".">
                    </AjaxControlToolkit:FilteredTextBoxExtender>
                </td>
            </tr>
        </table>
    </td>
    <td class="Width3Percent">
    </td>
</tr>

