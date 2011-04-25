<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="OverheadDetail.aspx.cs" Inherits="PraticeManagement.OverheadDetail"
    Title="Practice Management - Overhead Details" %>

<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Overhead Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Overhead Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <table border="0" width="100%">
                <tr>
                    <td width="200px">
                        <asp:CheckBox ID="chbActive" runat="server" Checked="true" Text="Is Active?" onclick="setDirty();" />
                    </td>
                    <td width="25px">
                        &nbsp;
                    </td>
                    <td colspan="3">
                        Is this overhead item active? If inactive, it will not take<br />
                        effect regardless of its start or end dates.
                    </td>
                </tr>
                <tr>
                    <td>
                        Overhead Name
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtOverheadName" runat="server" onchange="setDirty();"></asp:TextBox><asp:RequiredFieldValidator
                            ID="reqOverheadName" runat="server" ControlToValidate="txtOverheadName" ErrorMessage="The Overhead Name is required."
                            ToolTip="The Overhead Name is required." Text="*" SetFocusOnError="true" EnableClientScript="false"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Basis
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlBasis" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlBasis_SelectedIndexChanged"
                            onchange="setDirty();">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="reqBasis" runat="server" ControlToValidate="ddlBasis"
                            ErrorMessage="The Basis is required." ToolTip="The Basis is required." Text="*"
                            SetFocusOnError="true" EnableClientScript="false"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        Cost or Multiplier
                    </td>
                    <td>
                        <asp:Label ID="lblCurrencySign" runat="server" Text="$" Visible="False" Font-Bold="True"></asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtRate" runat="server" onchange="setDirty();"></asp:TextBox><asp:Label
                            ID="lblPersantageSign" runat="server" Text="For a $5/hour surcharge, enter 5.  For a 1% surcharge, enter 1"
                            Visible="False" Font-Bold="True"></asp:Label><asp:RequiredFieldValidator ID="reqRate"
                                runat="server" ControlToValidate="txtRate" ErrorMessage="The Cost or Multiplier is required."
                                ToolTip="The Cost or Multiplier is required." Text="*" SetFocusOnError="true"
                                EnableClientScript="false" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="regRate" runat="server" ControlToValidate="txtRate"
                            ErrorMessage="A number with 5 decimal digits is allowed for the Cost or Multiplier."
                            ToolTip="A number with 5 decimal digits is allowed for the Cost or Multiplier."
                            Text="*" SetFocusOnError="true" EnableClientScript="false" ValidationExpression="(\d{0,16}\.\d{1,5})|(\d{1,16})|(\d{1,16}\.\d{0,5})"
                            Display="Dynamic"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" width="670px">
                    <table>
                        <tr>
                            <td>
                                This overhead item should take effect on &nbsp; 
                            </td>
                            <td><uc2:DatePicker ID="dpStartDate" runat="server"
                                AutoPostBack="true" OnSelectionChanged="Period_SelectionChanged" />
                                </td>
                            <td>
                            <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                                ErrorMessage="The Start Date is required." ToolTip="The Start Date is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="compStartDateType" runat="server" ControlToValidate="dpStartDate"
                                ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                Type="Date"></asp:CompareValidator>
                            </td>
                            <td>&nbsp;and should cease on &nbsp;
                            </td>
                            <td><uc2:DatePicker ID="dpEndDate" runat="server" AutoPostBack="true"
                                OnSelectionChanged="Period_SelectionChanged" />
                            </td>
                            <td>
                            <asp:CompareValidator ID="compEndDateType" runat="server" ControlToValidate="dpEndDate"
                                ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                                EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                                Type="Date"></asp:CompareValidator>
                            <asp:CompareValidator ID="compPeriodStartEnd" runat="server" ControlToValidate="dpStartDate"
                                ControlToCompare="dpEndDate" ErrorMessage="The Start Date must be greater than the End Date."
                                ToolTip="The Start Date must be greater than the End Date." Text="*" EnableClientScript="false"
                                SetFocusOnError="true" Display="Dynamic" Operator="LessThan" Type="Date"></asp:CompareValidator>
                            </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButtonList ID="rlstCogsExpense" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rlstCogsExpense_SelectedIndexChanged">
                            <asp:ListItem Text="COGS" Value="True"></asp:ListItem>
                            <asp:ListItem Text="Expense" Value="False"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td colspan="4">
                        <asp:RequiredFieldValidator ID="reqCogsExpense" runat="server" ControlToValidate="rlstCogsExpense"
                            ErrorMessage="The COGS or Expense field is required." ToolTip="The COGS or Expense field is required."
                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        The overhead item should apply to persons being compensated as follows:
                        <ul>
                            <li>
                                <asp:CheckBox ID="chbHourly" runat="server" Text="W2-Hourly" onclick="setDirty();" /></li>
                            <li>
                                <asp:CheckBox ID="chbSalary" runat="server" Text="W2-Salary" onclick="setDirty();" /></li>
                            <li>
                                <asp:CheckBox ID="chb1099" runat="server" Text="1099" onclick="setDirty();" /></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <asp:ValidationSummary ID="vsumOverhead" runat="server" EnableClientScript="false" />
                        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5" align="center">
                        <asp:HiddenField ID="hdnOverheadId" runat="server" />
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                        <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

