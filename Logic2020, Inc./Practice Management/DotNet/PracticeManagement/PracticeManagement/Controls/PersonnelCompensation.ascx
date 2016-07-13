<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonnelCompensation.ascx.cs"
    Inherits="PraticeManagement.Controls.PersonnelCompensation" %>
<%@ Register Src="DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:HiddenField ID="hidOldStartDate" runat="server" />
<asp:HiddenField ID="hidOldEndDate" runat="server" />
<table class="PersonnelCompensationVerticalAlign">
    <tr id="trCompensationDate" runat="server">
        <td>
            Start Date
        </td>
        <td colspan="2" class="PersonnelCompensationPaddinLeftNone">
            <uc2:DatePicker ID="dpStartDate" runat="server" AutoPostBack="true" OnSelectionChanged="Period_SelectionChanged" />
            <asp:Label ID="lblStartDate" runat="server" Visible="false"></asp:Label>
            <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                ErrorMessage="The Start Date is required." ToolTip="The Start Date is required."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpStartDate"
                ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                Type="Date"></asp:CompareValidator>
        </td>
        <td>
            End Date
        </td>
        <td class="PersonnelCompensationPaddinLeftNone">
            <uc2:DatePicker ID="dpEndDate" runat="server" AutoPostBack="true" OnSelectionChanged="Period_SelectionChanged" />
            <asp:Label ID="lblEndDate" runat="server" Visible="false"></asp:Label>
        </td>
        <td colspan="2">
            <asp:CompareValidator ID="compDateRange" runat="server" ControlToValidate="dpEndDate"
                ControlToCompare="dpStartDate" ErrorMessage="The End Date must be greater than or equal to the Start Date."
                ToolTip="The End Date must be greater than or equal to the Start Date." Text="*"
                EnableClientScript="false" SetFocusOnError="true" Display="Static" Operator="GreaterThanEqual"
                Type="Date"></asp:CompareValidator>
            <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                Type="Date"></asp:CompareValidator>
            <asp:CustomValidator ID="custValLockoutDates" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custValLockoutDates_OnServerValidate"></asp:CustomValidator>
        </td>
        <td colspan="4">
            &nbsp;
            <asp:CustomValidator ID="custLockoutBasis" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custLockoutBasis_OnServerValidate"></asp:CustomValidator>
            <asp:CustomValidator ID="custLockoutAmount" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custLockoutAmount_OnServerValidate"></asp:CustomValidator>
            <asp:CustomValidator ID="custLockoutPTO" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custLockoutPTO_OnServerValidate"></asp:CustomValidator>
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnSalaryAnnual" runat="server" GroupName="Compensation" Text="W2-Salary"
                Checked="true" AutoPostBack="True" OnCheckedChanged="Compensation_CheckedChanged"
                onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtSalaryAnnual" runat="server" Enabled="False" onchange="setDirty();"
                MaxLength="16" OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtAmount" runat="server" TargetControlID="txtSalaryAnnual"
                FilterMode="ValidChars" FilterType="Numbers,Custom" ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td class="Left5">
            per Year
        </td>
        <td>
            <asp:RequiredFieldValidator ID="reqSalaryAnnual" runat="server" ControlToValidate="txtSalaryAnnual"
                ErrorMessage="The W2-Salary is required." ToolTip="The W2-Salary is required."
                Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compSalaryAnnual" runat="server" ControlToValidate="txtSalaryAnnual"
                ErrorMessage="A number with 2 decimal digits is allowed for the W2-Salary." ToolTip="A number with 2 decimal digits is allowed for the W2-Salary."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
            <asp:CompareValidator ID="compSalaryWageGreaterThanZero" runat="server" ControlToValidate="txtSalaryAnnual"
                ErrorMessage="Warning - Incorrect Pay: The wage must be greater than $0." ToolTip="Warning - Incorrect Pay: The wage must be greater than $0."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="GreaterThan"
                Type="Currency" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator>
            <asp:CustomValidator ID="cvSLTApprovalValidation" runat="server" OnServerValidate="cvSLTApprovalValidation_OnServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:CustomValidator>
            <asp:CustomValidator ID="cvIsDivisionOrPracticeOwner" runat="server" ErrorMessage="This person is currently assigned as a Practice Area Owner or Division Owner.  Please reassign ownership and then make the change."
                Display="Dynamic" ToolTip="This person is currently assigned as a Practice Area Owner or Division Owner.  Please reassign ownership and then make the change."
                Text="*" Enabled="false"></asp:CustomValidator>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td colspan="6">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnSalaryHourly" runat="server" GroupName="Compensation" Text="W2-Hourly"
                AutoPostBack="True" OnCheckedChanged="Compensation_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtSalaryHourly" runat="server" onchange="setDirty();" OnTextChanged="Compensation_TextChanged"
                MaxLength="16" CssClass="textRight Width120Px"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server"
                TargetControlID="txtSalaryHourly" FilterMode="ValidChars" FilterType="Numbers,Custom"
                ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td class="Left5">
            per Hour
        </td>
        <td>
            <asp:RequiredFieldValidator ID="reqSalaryHourly" runat="server" ControlToValidate="txtSalaryHourly"
                ErrorMessage="The W2-Hourly is required." ToolTip="The W2-Hourly is required."
                Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compSalaryHourly" runat="server" ControlToValidate="txtSalaryHourly"
                ErrorMessage="A number with 2 decimal digits is allowed for the W2-Hourly." ToolTip="A number with 2 decimal digits is allowed for the W2-Hourly."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
            <asp:CompareValidator ID="compHourlyWageGreaterThanZero" runat="server" ControlToValidate="txtSalaryHourly"
                ErrorMessage="Warning - Incorrect Pay: The wage must be greater than $0." ToolTip="Warning - Incorrect Pay: The wage must be greater than $0."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="GreaterThan"
                Type="Currency" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator>
        </td>
        <td colspan="8">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtn1099Ctc" runat="server" GroupName="Compensation" Text="1099/Hourly"
                AutoPostBack="True" OnCheckedChanged="Compensation_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txt1099Ctc" runat="server" Enabled="False" OnTextChanged="Compensation_TextChanged"
                MaxLength="16" CssClass="textRight Width120Px"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server"
                TargetControlID="txt1099Ctc" FilterMode="ValidChars" FilterType="Numbers,Custom"
                ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td class="Left5">
            per Hour
        </td>
        <td>
            <asp:RequiredFieldValidator ID="req1099Ctc" runat="server" ControlToValidate="txt1099Ctc"
                ErrorMessage="The 1099/Hourly is required." ToolTip="The 1099/CTC is required."
                Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="comp1099Ctc" runat="server" ControlToValidate="txt1099Ctc"
                ErrorMessage="A number with 2 decimal digits is allowed for the 1099/Hourly."
                ToolTip="A number with 2 decimal digits is allowed for the 1099/Hourly." Text="*"
                EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck" Type="Currency"
                Display="Dynamic"></asp:CompareValidator>
            <asp:CompareValidator ID="compHourlyGreaterThanZero" runat="server" ControlToValidate="txt1099Ctc"
                ErrorMessage="Warning - Incorrect Pay: The wage must be greater than $0." ToolTip="Warning - Incorrect Pay: The wage must be greater than $0."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="GreaterThan"
                Type="Currency" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator>
        </td>
        <td>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblVendor" runat="server" Text="Vendor Indicator" Visible="false"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlVendor" runat="server" CssClass="Width94Per" onchange="setDirty();"
                            Visible="false">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="reqddlVendor" runat="server" ControlToValidate="ddlVendor"
                            Display="Dynamic" EnableClientScript="false" ErrorMessage="The Vendor Indicator is required."
                            SetFocusOnError="true" Text="*" ToolTip="The Vendor is Indicator required." Enabled="false"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnPercentRevenue" runat="server" AutoPostBack="True" GroupName="Compensation"
                Text="1099/POR" OnCheckedChanged="Compensation_CheckedChanged" onclick="setDirty();" />
        </td>
        <td class="Left12">
            <%-- onkeypress="setTimeout('__doPostBack(\'ctl00$body$personnelCompensation$txtPercRevenue\',\'\')', 0); this.focus();" --%>
            <asp:TextBox ID="txtPercRevenue" runat="server" CssClass="Width120Px" onchange="setDirty();"
                MaxLength="16" OnTextChanged="Compensation_TextChanged" Text="0" AutoPostBack="true"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server"
                TargetControlID="txtPercRevenue" FilterMode="ValidChars" FilterType="Numbers,Custom"
                ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td>
            <asp:RequiredFieldValidator ID="reqPercRevenue" runat="server" ControlToValidate="txtPercRevenue"
                ErrorMessage="Percent of Revenue should be non-empty" ToolTip="Percent of Revenue should be non-empty"
                Text="*" EnableClientScript="false" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compPercRevenue" runat="server" ControlToValidate="txtPercRevenue"
                ErrorMessage="A number with 2 decimal digits is allowed for the Percent of Revenue."
                ToolTip="A number with 2 decimal digits is allowed for the Percent of Revenue."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Double" Display="Dynamic"></asp:CompareValidator>
            <asp:CompareValidator ID="compPercRevenueGreaterThanZero" runat="server" ControlToValidate="txtPercRevenue"
                ErrorMessage="Warning - Incorrect Pay: The wage must be greater than $0." ToolTip="Warning - Incorrect Pay: The wage must be greater than $0."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="GreaterThan"
                Type="Currency" ValueToCompare="0" Display="Dynamic"></asp:CompareValidator>
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
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td colspan="4">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td colspan="12">
            Bonus
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnBonusHourly" runat="server" GroupName="Bonus" Text="Hourly"
                AutoPostBack="True" OnCheckedChanged="Bonus_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtBonusHourly" runat="server" onchange="setDirty();" OnTextChanged="Compensation_TextChanged"
                MaxLength="16" CssClass="textRight Width120Px"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server"
                TargetControlID="txtBonusHourly" FilterMode="ValidChars" FilterType="Numbers,Custom"
                ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td class="Left5" colspan="3">
            <asp:CompareValidator ID="compBonusHourly" runat="server" ControlToValidate="txtBonusHourly"
                ErrorMessage="A number with 2 decimal digits is allowed for the Bonus." ToolTip="A number with 2 decimal digits is allowed for the Bonus."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
            every &nbsp;
            <asp:TextBox ID="txtBonusDuration" runat="server" CssClass="Width80Px" onchange="setDirty();"
                MaxLength="16" OnTextChanged="Compensation_TextChanged">500</asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server"
                TargetControlID="txtBonusDuration" FilterMode="ValidChars" FilterType="Numbers">
            </AjaxControlToolkit:FilteredTextBoxExtender>
            &nbsp;hours
        </td>
        <td>
            <asp:CustomValidator ID="reqBonusDuration" runat="server" ControlToValidate="txtBonusDuration"
                ErrorMessage="The Bonus Every is required since you have entered the Bonus Amount."
                ToolTip="The Bonus Every is required since you have entered the Bonus Amount."
                Text="*" EnableClientScript="false" SetFocusOnError="true" OnServerValidate="reqBonusDuration_ServerValidate"
                ValidateEmptyText="true"></asp:CustomValidator>
            <asp:CompareValidator ID="compBonusDuration" runat="server" ControlToValidate="txtBonusDuration"
                ErrorMessage="The Bonus Every must be an integer number." ToolTip="The Bonus Every must be an integer number."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Integer" Display="Dynamic"></asp:CompareValidator>
        </td>
        <td colspan="5">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnBonusAnnual" runat="server" GroupName="Bonus" Text="Annual"
                Checked="True" AutoPostBack="True" OnCheckedChanged="Bonus_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtBonusAnnual" runat="server" Enabled="False" onchange="setDirty();"
                MaxLength="16" OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
            <AjaxControlToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server"
                TargetControlID="txtBonusAnnual" FilterMode="ValidChars" FilterType="Numbers,Custom"
                ValidChars=".">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td class="Left5">
            per Year
        </td>
        <td colspan="9">
            <asp:CompareValidator ID="compBonusAnnual" runat="server" ControlToValidate="txtBonusAnnual"
                ErrorMessage="A number with 2 decimal digits is allowed for the Bonus." ToolTip="A number with 2 decimal digits is allowed for the Bonus."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td colspan="12">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap">
            <asp:Label ID="lblVacationDays" runat="server" Text="PTO Accrual"></asp:Label>
        </td>
        <td class="Left12">
            <asp:TextBox ID="txtVacationDays" runat="server" CssClass="Width120Px" onchange="setDirty();"
                MaxLength="2" OnTextChanged="Compensation_TextChanged" Text="0"></asp:TextBox>
            <asp:CompareValidator ID="compVacationDays" runat="server" ControlToValidate="txtVacationDays"
                ErrorMessage="PTO Accrual greater than 200 is not allowed." ToolTip="PTO Accrual greater than 200 is not allowed."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="LessThanEqual"
                Type="Integer" ValueToCompare="200" Display="Dynamic"></asp:CompareValidator>
        </td>
        <td class="Left5">
            per Year
        </td>
        <td colspan="9">
            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtVacationDays" runat="server"
                TargetControlID="txtVacationDays" FilterMode="ValidChars" FilterType="Numbers">
            </AjaxControlToolkit:FilteredTextBoxExtender>
            <asp:RequiredFieldValidator ID="rfvVacationDays" runat="server" ControlToValidate="txtVacationDays"
                Text="*" EnableClientScript="false" Display="Dynamic" ErrorMessage="PTO Accrual is required."
                ToolTip="PTO Accrual is required."></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="cvVacationDays" runat="server" Text="*" ErrorMessage="PTO Accrual(In Hours) must be in multiple of 8."
                ToolTip="PTO Accrual(In Hours) must be in multiple of 8." SetFocusOnError="true"
                EnableClientScript="false" Enabled="false" OnServerValidate="cvVacationDays_ServerValidate"></asp:CustomValidator>
            <asp:CustomValidator ID="cvSLTPTOApprovalValidation" runat="server" OnServerValidate="cvSLTPTOApprovalValidation_OnServerValidate"
                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:CustomValidator>
        </td>
    </tr>
    <tr id="trTitle" runat="server">
        <td>
            Title
        </td>
        <td colspan="2" class="Left12">
            <pmc:CustomDropDown ID="ddlTitle" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTitle_OnSelectedIndexChanged"
                CausesValidation="false">
            </pmc:CustomDropDown>
        </td>
        <td class="Left12">
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ToolTip="Please select Title"
                ControlToValidate="ddlTitle" EnableClientScript="false" Display="Dynamic" Text="*"
                ErrorMessage="Please select Title"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="custLockoutTitle" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custLockoutTitle_OnServerValidate">
            </asp:CustomValidator>
        </td>
        <td>
        </td>
        <td colspan="2">
        </td>
        <td colspan="5" class="Left5">
        </td>
    </tr>
    <tr id="trDivisionAndPractice" runat="server">
        <td>
            Division
        </td>
        <td colspan="2" class="Left12">
            <asp:DropDownList ID="ddlDivision" runat="server" Width="100%" AutoPostBack="true"
                OnSelectedIndexChanged="ddlDivision_OnSelectedIndexChanged">
            </asp:DropDownList>
        </td>
        <td class="Left5">
            <asp:RequiredFieldValidator ID="rfvDivision" runat="server" ToolTip="Please select Division"
                ControlToValidate="ddlDivision" EnableClientScript="false" Display="Dynamic"
                Text="*" ErrorMessage="Please select Division"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="custLockOutDivision" runat="server" Display="Dynamic" Text="*"
                OnServerValidate="custLockOutDivision_OnServerValidate"></asp:CustomValidator>
        </td>
        <td colspan="8">
            <table>
                <tr>
                    <td class="Left5">
                        Practice Area
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPractice" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPractice_OnSelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="Left5">
                        <asp:RequiredFieldValidator ID="rfvPractice" runat="server" ToolTip="Please select Practice Area"
                            ControlToValidate="ddlPractice" EnableClientScript="false" Display="Dynamic"
                            Text="*" ErrorMessage="Please select Practice Area"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="custLockOutPractice" runat="server" Display="Dynamic" Text="*"
                            OnServerValidate="custLockOutPractice_OnServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdSLTApproval" runat="server" Value="false" />
<asp:HiddenField ID="hdSLTPTOApproval" runat="server" Value="false" />
<asp:HiddenField ID="hdSLTApprovalPopUp" runat="server" Value="" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeSLTApprovalPopUp" runat="server" TargetControlID="hdSLTApprovalPopUp"
    BackgroundCssClass="modalBackground" PopupControlID="pnlSLTApprovalPopUp" DropShadow="false" />
<asp:Panel ID="pnlSLTApprovalPopUp" runat="server" CssClass="popUp PopUpPersonDetailPage"
    Style="display: none;">
    <table class="WholeWidth">
        <tr>
            <td class="Padding6">
                The inputted value is outside of the approved salary band for this level. A salary
                figure outside of the band requires approval from a member of the Senior Leadership
                Team. Please ensure that you have received that approval before continuing.
            </td>
        </tr>
        <tr>
            <td align="center" class="Padding6">
                <asp:Button ID="btnSLTApproval" runat="server" Text="SLT Approval Received" OnClick="btnSLTApproval_OnClick"
                    UseSubmitBehavior="false" CssClass="Width160px" />
                &nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_OnClick"
                    UseSubmitBehavior="false" CssClass="Width60Px" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="hdSLTPTOApprovalPopUp" runat="server" Value="" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeSLTPTOApprovalPopUp" runat="server"
    TargetControlID="hdSLTPTOApprovalPopUp" BackgroundCssClass="modalBackground"
    PopupControlID="pnlSLTPTOApprovalPopUp" DropShadow="false" />
<asp:Panel ID="pnlSLTPTOApprovalPopUp" runat="server" CssClass="popUp PopUpPersonDetailPage"
    Style="display: none;">
    <table class="WholeWidth">
        <tr>
            <td class="Padding6">
                Any change to a person's allotted PTO accrual requires approval from a member of
                the Senior Leadership Team. Please ensure that you have received that approval before
                continuing.
            </td>
        </tr>
        <tr>
            <td align="center" class="Padding6">
                <asp:Button ID="btnSLTPTOApproval" runat="server" Text="SLT Approval Received" OnClick="btnSLTPTOApproval_OnClick"
                    UseSubmitBehavior="false" CssClass="Width160px" />
                &nbsp;
                <asp:Button ID="btnCancelSLTPTOApproval" runat="server" Text="Cancel" OnClick="btnCancelSLTPTOApproval_OnClick"
                    UseSubmitBehavior="false" CssClass="Width60Px" />
            </td>
        </tr>
    </table>
</asp:Panel>

