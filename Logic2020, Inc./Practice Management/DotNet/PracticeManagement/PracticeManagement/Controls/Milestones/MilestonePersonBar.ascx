<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonBar.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonBar" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<tr id="trBar" style="height: 25px; white-space: nowrap;" runat="server">
    <td align="center" style="width: 3%; height: 20px;">
        <asp:ImageButton ID="imgCopy" ToolTip="Copy" runat="server" OnClick="imgCopy_OnClick"
            ImageUrl="~/Images/copy.png" />
    </td>
    <td align="center" style="width: 4%; height: 20px;">
        <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png"
            ValidationGroup="<%# GetValidationGroup() %>" ToolTip="Save" OnClick="btnInsertPerson_Click" />
        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick"
            ToolTip="Cancel" />
    </td>
    <td style="width: 22%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <asp:DropDownList ID="ddlPerson" Width="98%" runat="server" />
                </td>
                <td style="width: 15%;">
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
                    <asp:CustomValidator ID="custDuplicatedPerson" runat="server" ControlToValidate="ddlPerson"
                        ErrorMessage="" ToolTip="The specified person is already assigned on this milestone."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custDuplicatedPersonInsert_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </td>
    <td style="width: 10%; height: 20px;">
        <asp:DropDownList ID="ddlRole" Width="98%" runat="server">
        </asp:DropDownList>
    </td>
    <td align="center" style="width: 11%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <uc2:DatePicker ID="dpPersonStartInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="return true;" AutoPostBack="false" TextBoxWidth="95%" />
                </td>
                <td style="width: 15%;">
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
                        ErrorMessage="" ToolTip="The specified period overlaps with another for this person on the milestone."
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
    <td align="left" style="width: 11%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <uc2:DatePicker ID="dpPersonEndInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>"
                        OnClientChange="return true;" AutoPostBack="false" TextBoxWidth="95%" />
                </td>
                <td style="width: 15%;">
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
    <td align="center" style="width: 11%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <asp:TextBox ID="txtHoursPerDayInsert" runat="server" Width="90%"></asp:TextBox>
                </td>
                <td style="width: 15%;">
                    <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
                        ErrorMessage="" ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
                </td>
            </tr>
        </table>
    </td>
    <td id="tdAmountInsert" runat="server" align="center" style="width: 9%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
                    <asp:TextBox ID="txtAmountInsert" runat="server" Width="80%"></asp:TextBox>
                </td>
                <td style="width: 15%;">
                    <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ValidateEmptyText="true" ErrorMessage="" ToolTip="The Amount is required." Text="*"
                        SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" OnServerValidate="reqHourlyRevenue_ServerValidate"
                        ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
                    <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Revenue."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </td>
    <td align="center" style="width: 7%; height: 20px;">
    </td>
    <td align="center" style="width: 9%; height: 20px;">
        <table width="100%">
            <tr>
                <td style="width: 85%;">
                    <asp:TextBox ID="txtHoursInPeriodInsert" runat="server" Width="90%"></asp:TextBox>
                </td>
                <td style="width: 15%;">
                    <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="" ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
                    <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
                        ErrorMessage="" ToolTip="The Hours In Period must be more then 0 and less or equals to 15,000."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        MinimumValue="0" MaximumValue="15000" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
                </td>
            </tr>
        </table>
    </td>
    <td align="center" style="width: 3%; height: 20px;">
    </td>
</tr>

