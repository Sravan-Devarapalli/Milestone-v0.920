﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonnelCompensation.ascx.cs"
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
                ToolTip="The End Date must be greater than or equal to the Start Date." Text="*" EnableClientScript="false"
                SetFocusOnError="true" Display="Static" Operator="GreaterThanEqual" Type="Date"></asp:CompareValidator>
            <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                Type="Date"></asp:CompareValidator>
        </td>
        <td colspan="4">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td>
            <asp:RadioButton ID="rbtnSalaryAnnual" runat="server" GroupName="Compensation" Text="W2-Salary"
                AutoPostBack="True" OnCheckedChanged="Compensation_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtSalaryAnnual" runat="server" Enabled="False" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
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
                Checked="true" AutoPostBack="True" OnCheckedChanged="Compensation_CheckedChanged"
                onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtSalaryHourly" runat="server" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
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
                CssClass="textRight Width120Px"></asp:TextBox>
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
        </td>
        <td>
            &nbsp;
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
                OnTextChanged="Compensation_TextChanged" Text="0" AutoPostBack="true"></asp:TextBox>
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
                AutoPostBack="True" Checked="True" OnCheckedChanged="Bonus_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtBonusHourly" runat="server" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
        </td>
        <td class="Left5" colspan="3">
            <asp:CompareValidator ID="compBonusHourly" runat="server" ControlToValidate="txtBonusHourly"
                ErrorMessage="A number with 2 decimal digits is allowed for the Bonus." ToolTip="A number with 2 decimal digits is allowed for the Bonus."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
            every &nbsp;
            <asp:TextBox ID="txtBonusDuration" runat="server" CssClass="Width80Px" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged">500</asp:TextBox>
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
                AutoPostBack="True" OnCheckedChanged="Bonus_CheckedChanged" onclick="setDirty();" />
        </td>
        <td>
            $<asp:TextBox ID="txtBonusAnnual" runat="server" Enabled="False" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged" CssClass="textRight Width120Px"></asp:TextBox>
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
    <tr id="trDefaultHoursPerDay" runat="server">
        <td>
            Default hours per Day
        </td>
        <td class="Left12">
            <asp:TextBox ID="txtDefaultHoursPerDay" runat="server" Text="8" CssClass="Width120Px" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged"></asp:TextBox>
        </td>
        <td>
            <asp:RequiredFieldValidator ID="reqDefaultHoursPerDay" runat="server" ControlToValidate="txtDefaultHoursPerDay"
                ErrorMessage="The Default hours per day is required." ToolTip="The Default hours per day is required."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="compDefaultHoursPerDay" runat="server" ControlToValidate="txtDefaultHoursPerDay"
                ErrorMessage="A number with 2 decimal digits is allowed for the Default hours per day."
                ToolTip="A number with 2 decimal digits is allowed for the Default hours per day."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Operator="DataTypeCheck"
                Type="Currency" Display="Dynamic"></asp:CompareValidator>
        </td>
        <td colspan="9">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td colspan="12">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap">
            <asp:Label ID="lblVacationDays" runat="server" Text="Vacation Days"></asp:Label>
        </td>
        <td class="Left12">
            <asp:TextBox ID="txtVacationDays" runat="server" CssClass="Width120Px" onchange="setDirty();"
                OnTextChanged="Compensation_TextChanged" Text="0"></asp:TextBox>
        </td>
        <td class="Left5">
            per Year
        </td>
        <td colspan="9">
            <AjaxControlToolkit:FilteredTextBoxExtender ID="ftetxtVacationDays" runat="server"
                TargetControlID="txtVacationDays" FilterMode="ValidChars" FilterType="Numbers">
            </AjaxControlToolkit:FilteredTextBoxExtender>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtVacationDays"
                Text="*" EnableClientScript="false" Display="Dynamic" ErrorMessage="Vacation days is required"
                ToolTip="Vacation days is required"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="cvVacationDays" runat="server" Text="*" ErrorMessage="VacationDays(In Hours) must be in multiple of 8."
                ToolTip="VacationDays(In Hours) must be in multiple of 8." SetFocusOnError="true" EnableClientScript="false"
                Enabled="false" OnServerValidate="cvVacationDays_ServerValidate"></asp:CustomValidator>
        </td>
    </tr>
    <tr id="trPayments" runat="server">
        <td nowrap="nowrap">
            Times paid per month
        </td>
        <td class="Left12">
            <asp:DropDownList ID="ddlPaidPerMonth" runat="server" CssClass="Width80Px" onchange="setDirty();">
                <asp:ListItem></asp:ListItem>
                <asp:ListItem Text="1"></asp:ListItem>
                <asp:ListItem Text="2" Selected="True"></asp:ListItem>
                <asp:ListItem Text="4"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td colspan="2">
            &nbsp;
        </td>
        <td>
            Payment&nbsp;Terms&nbsp;
        </td>
        <td colspan="2">
            <asp:DropDownList ID="ddlPaymentTerms" runat="server" DataTextField="Name" DataValueField="Frequency"
                AppendDataBoundItems="true" onchange="setDirty();">
                <asp:ListItem Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <asp:RequiredFieldValidator ID="reqPaymentTerms" runat="server" ControlToValidate="ddlPaymentTerms"
                ErrorMessage="The Payment Terms is required." ToolTip="The Payment Terms is required."
                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
        </td>
        <td colspan="4">
            &nbsp;
        </td>
    </tr>
    <tr id="trSeniorityAndPractice" runat="server">
        <td>
            Seniority
        </td>
        <td colspan="2" class="Left12">
            <asp:DropDownList ID="ddlSeniority" runat="server">
            </asp:DropDownList>
        </td>
        <td class="Left12">
            <asp:CustomValidator ID="custValSeniority" runat="server" ToolTip="Please select Seniority"
                Display="Dynamic" Text="*" ErrorMessage="Please select Seniority" OnServerValidate="custValSeniority_OnServerValidate">
            </asp:CustomValidator>
        </td>
        <td>
            Practice Area
        </td>
        <td colspan="2">
            <asp:DropDownList ID="ddlPractice" runat="server">
            </asp:DropDownList>
        </td>
        <td colspan="5" class="Left5">
            <asp:CustomValidator ID="custValPractice" runat="server" ToolTip="Please select Practice Area"
                Display="Dynamic" Text="*" ErrorMessage="Please select Practice Area" OnServerValidate="custValPractice_OnServerValidate">
            </asp:CustomValidator>
        </td>
    </tr>
    <tr id="trSalesCommisiion" runat="server">
        <td>
            Sales Commission
        </td>
        <td class="Left12">
            <asp:TextBox ID="txtSalesCommission" runat="server"></asp:TextBox>
        </td>
        <td colspan="9">
            <asp:CustomValidator ID="custValSalesCommission" runat="server" Display="Dynamic"
                Text="*" OnServerValidate="custValSalesCommission_OnServerValidate">
            </asp:CustomValidator>
            &nbsp;%&nbsp;of&nbsp;gross&nbsp;margin.
        </td>
    </tr>
</table>

