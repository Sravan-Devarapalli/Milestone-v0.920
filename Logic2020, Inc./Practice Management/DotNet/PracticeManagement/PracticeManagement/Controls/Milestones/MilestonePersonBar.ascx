<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonBar.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonBar" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<tr id="trBar" runat="server">
    <td align="center" style="width: 4%; padding-top: 10px;">
        <asp:ImageButton ID="btnInsert" runat="server" ImageUrl="~/Images/icon-check.png" OnClientClick="SetValueForhdnField();"
            ValidationGroup="<%# GetValidationGroup() %>" ToolTip="Save" OnClick="btnInsertPerson_Click" />
        <asp:ImageButton ID="btnCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="btnCancel_OnClick" OnClientClick="SetValueForhdnField();"
            ToolTip="Cancel" />
    </td>
    <td style="width: 28%; white-space: nowrap;">
        <asp:DropDownList ID="ddlPerson" onchange="setDirty();" Width="98%" runat="server" />
        <asp:RequiredFieldValidator ID="reqPersonName" runat="server" ControlToValidate="ddlPerson"
            ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="MilestonePerson"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="reqMilestonePersonName" runat="server" ControlToValidate="ddlPerson"
            ErrorMessage="The Person Name is required." ToolTip="The Person Name is required."
            Text="*" EnableClientScript="false" SetFocusOnError="true" ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
        <asp:CustomValidator ID="custPeriod" runat="server" ControlToValidate="ddlPerson"
            ErrorMessage="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
            ToolTip="The person you are trying to add is not set as being active during the entire length of their participation in the milestone.  Please adjust the person's hire and compensation records, or change the dates that they are attached to this milestone."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="MilestonePerson" OnServerValidate="custPersonInsert_ServerValidate"></asp:CustomValidator>
        <asp:CustomValidator ID="custEntries" runat="server" ControlToValidate="ddlPerson"
            ErrorMessage="You must specify at least one detail record." ToolTip="You must specify at least one detail record."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="MilestonePerson" OnServerValidate="custEntriesInsert_ServerValidate"></asp:CustomValidator>
        <asp:CustomValidator ID="custDuplicatedPerson" runat="server" ControlToValidate="ddlPerson"
            ErrorMessage="The specified person is already assigned on this milestone." ToolTip="The specified person is already assigned on this milestone."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="MilestonePerson" OnServerValidate="custDuplicatedPersonInsert_ServerValidate"></asp:CustomValidator>
    </td>
    <td style="width: 10%;">
        <asp:DropDownList ID="ddlRole" onchange="setDirty();" Width="98%" runat="server">
        </asp:DropDownList>
    </td>
    <td align="center" style="width: 11%;">
        <uc2:DatePicker ID="dpPersonStartInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>" OnClientChange="setDirty();"
            OnSelectionChanged="dpPersonStart_SelectionChanged" AutoPostBack="true" TextBoxWidth="85%" />
        <asp:RequiredFieldValidator ID="reqPersonStart" runat="server" ControlToValidate="dpPersonStartInsert"
            ErrorMessage="The Person Start Date is required." ToolTip="The Person Start Date is required."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="<%# GetValidationGroup() %>"></asp:RequiredFieldValidator>
        <asp:CompareValidator ID="compPersonStartType" runat="server" ControlToValidate="dpPersonStartInsert"
            ErrorMessage="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
            ToolTip="The Person Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
        <asp:CustomValidator ID="custPersonStartInsert" runat="server" ControlToValidate="dpPersonStartInsert"
            ErrorMessage="The Person Start Date must be greater than or equal to the Milestone Start Date."
            ToolTip="The Person Start Date must be greater than or equal to the Milestone Start Date."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPersonStartInsert_ServerValidate"></asp:CustomValidator>
        <asp:CustomValidator ID="custPeriodOvberlapping" runat="server" ControlToValidate="dpPersonStartInsert"
            ErrorMessage="The specified period overlaps with another for this person on the milestone."
            ToolTip="The specified period overlaps with another for this person on the milestone."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodOvberlappingInsert_ServerValidate"></asp:CustomValidator>
        <asp:CustomValidator ID="custPeriodVacationOverlapping" runat="server" ControlToValidate="dpPersonStartInsert"
            ErrorMessage="The specified period overlaps with Vacation days for this person on the milestone."
            ToolTip="The specified period overlaps with Vacation days for this person on the milestone."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidateEmptyText="false" ValidationGroup="<%# GetValidationGroup() %>" OnServerValidate="custPeriodVacationOverlappingInsert_ServerValidate"></asp:CustomValidator>
    </td>
    <td align="center" style="width: 11%;">
        <uc2:DatePicker ID="dpPersonEndInsert" runat="server" ValidationGroup="<%# GetValidationGroup() %>" OnClientChange="setDirty();"
            OnSelectionChanged="dpPersonEnd_SelectionChanged" AutoPostBack="true" TextBoxWidth="85%" />
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
            ErrorMessage="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
            ToolTip="The Person End Date has an incorrect format. It must be 'MM/dd/yyyy'."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            Operator="DataTypeCheck" Type="Date" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
    </td>
    <td align="center" style="width: 8%;">
        <asp:TextBox ID="txtHoursPerDayInsert" runat="server" onchange="setDirty();" Width="70%"></asp:TextBox>
         <asp:CompareValidator ID="compHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
            ErrorMessage="A number with 2 decimal digits is allowed for the Hours Per Day."
            ToolTip="A number with 2 decimal digits is allowed for the Hours Per Day." Text="*"
            EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
            Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
        <asp:RangeValidator ID="rangHoursPerDay" runat="server" ControlToValidate="txtHoursPerDayInsert"
            ErrorMessage=" The Hours Per Day must be greater than 0 and less or equals to 24."
            ToolTip=" The Hours Per Day must be greater than 0 and less or equals to 24."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            MinimumValue="0.01" MaximumValue="24" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
    </td>
    <td align="center" style="width: 8%;">
        <asp:Label ID="lblAmountInsert" runat="server" Text="$"></asp:Label>
        <asp:TextBox ID="txtAmountInsert" runat="server" onchange="setDirty();" Width="70%"></asp:TextBox>
         <asp:CustomValidator ID="reqHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
            ErrorMessage="The Amount is required." ToolTip="The Amount is required." Text="*"
            SetFocusOnError="true" EnableClientScript="false" Display="Dynamic" OnServerValidate="reqHourlyRevenue_ServerValidate"
            ValidationGroup="<%# GetValidationGroup() %>"></asp:CustomValidator>
        <asp:CompareValidator ID="compHourlyRevenue" runat="server" ControlToValidate="txtAmountInsert"
            ErrorMessage="A number with 2 decimal digits is allowed for the Revenue." ToolTip="A number with 2 decimal digits is allowed for the Revenue."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
    </td>
    <td align="center" style="width: 7%;">
    </td>
    <td align="center" style="width: 12%;">
        <asp:TextBox ID="txtHoursInPeriodInsert" onchange="setDirty();" runat="server" Width="50%"></asp:TextBox>
        <asp:CompareValidator ID="compHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
            ErrorMessage="A number with 2 decimal digits is allowed for the Hours In Period."
            ToolTip="A number with 2 decimal digits is allowed for the Hours In Period."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            Operator="DataTypeCheck" Type="Currency" ValidationGroup="<%# GetValidationGroup() %>"></asp:CompareValidator>
        <asp:RangeValidator ID="rangHoursInPeriod" runat="server" ControlToValidate="txtHoursInPeriodInsert"
            ErrorMessage="The Hours In Period must be more then 0 and less or equals to 15,000."
            ToolTip="The Hours In Period must be more then 0 and less or equals to 15,000."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            MinimumValue="0" MaximumValue="15000" Type="Double" ValidationGroup="<%# GetValidationGroup() %>"></asp:RangeValidator>
    </td>
    <td style="width: 1%;">
        <asp:CompareValidator ID="compPersonEndInsert" runat="server" ControlToValidate="dpPersonEndInsert"
            ControlToCompare="dpPersonStartInsert" ErrorMessage="The Person End Date must be greater than or equal to the Person Start Date."
            ToolTip="The Person End Date must be greater than or equal to the Person Start Date."
            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
            ValidationGroup="<%# GetValidationGroup() %>" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
    </td>
</tr>

