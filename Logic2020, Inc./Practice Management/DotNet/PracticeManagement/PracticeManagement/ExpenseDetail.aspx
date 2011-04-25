<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ExpenseDetail.aspx.cs" Inherits="PraticeManagement.ExpenseDetail"
    Title="Practice Management - Expense Details" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc1" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Expense Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Expense Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <table>
        <tr>
            <td>
                Expense Name
            </td>
            <td>
                <asp:TextBox ID="txtExpenseName" runat="server" MaxLength="50"></asp:TextBox>
                <asp:HiddenField ID="hidOldExpenseName" runat="server" />
            </td>
            <td>
                <asp:RequiredFieldValidator ID="reqExpenseName" runat="server" ControlToValidate="txtExpenseName"
                    ErrorMessage="The Expense name is required." ToolTip="The Expense name is required."
                    EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ExpenseDetail">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Expense Category
            </td>
            <td>
                <asp:DropDownList ID="ddlExpenseCategory" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="reqExpenseCategory" runat="server" ControlToValidate="ddlExpenseCategory"
                    ErrorMessage="The Expense Category is required." ToolTip="The Expense Category is required."
                    EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ExpenseDetail">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="custExpenseCategoryDeleted" runat="server" ControlToValidate="ddlExpenseCategory"
                    ErrorMessage="The expense category was deleted." ToolTip="The expense category was deleted."
                    EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ExpenseDetail"
                    OnServerValidate="custExpenseCategoryDeleted_ServerValidate">*</asp:CustomValidator>
                <asp:CustomValidator ID="custExpenseCategory" runat="server" ControlToValidate="ddlExpenseCategory"
                    EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ExpenseDetail"
                    OnServerValidate="custExpenseCategory_ServerValidate">*</asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td>
                Basis
            </td>
            <td>
                <asp:DropDownList ID="ddlBasis" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="reqBasis" runat="server" ControlToValidate="ddlBasis"
                    ErrorMessage="The Basis is required." ToolTip="The Basis is required." EnableClientScript="false"
                    SetFocusOnError="true" ValidationGroup="ExpenseDetail">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                Week paid
            </td>
            <td>
                <asp:DropDownList ID="ddlWeekPaid" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="reqWeekPaid" runat="server" ControlToValidate="ddlWeekPaid"
                    ErrorMessage="The Week paid is required." ToolTip="The Week paid is required."
                    EnableClientScript="false" SetFocusOnError="true" ValidationGroup="ExpenseDetail">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gvExpenses" runat="server" EmptyDataText="There is nothing to be displayed here"
                    AutoGenerateColumns="False" DataKeyNames="Key" OnRowUpdating="gvExpenses_RowUpdating"
                    OnRowDeleting="gvExpenses_RowDeleting" OnRowEditing="gvExpenses_RowEditing" OnRowCancelingEdit="gvExpenses_RowCancelingEdit"
                    CssClass="CompPerfTable" GridLines="None" BackColor="White">
                    <AlternatingRowStyle BackColor="#F9FAFF" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="250px" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Month</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblMonth" runat="server" Text='<%# ((DateTime)Eval("Key")).ToString("MMM-yyyy") %>'></asp:Label>
                                <asp:HiddenField ID="hidKey" runat="server" Value='<%# Eval("Key") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemStyle Width="150px" />
                            <HeaderTemplate>
                                <div class="ie-bg">
                                    Amount</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAmount" runat="server" Text='<%# SelectedBasisId.HasValue && SelectedBasisId.Value == (int)ExpenseBasisType.AbsolutAmount ? ((PracticeManagementCurrency)(decimal)Eval("Value")).ToString() : string.Format("{0}%", Eval("Value")) %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAmount" runat="server" Text='<%# Eval("Value") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                                    ErrorMessage="The Amount is required." ToolTip="The Amount is required." EnableClientScript="false"
                                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="MonthExpense">*</asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="custAmount" runat="server" ControlToValidate="txtAmount"
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                    ValidationGroup="MonthExpense" OnServerValidate="custAmount_ServerValidate"></asp:CustomValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField InsertVisible="False" ShowDeleteButton="True" ShowEditButton="True" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <h3>
                    New Expense</h3>
            </td>
        </tr>
        <tr>
            <td>
                Month
            </td>
            <td>
                Amount
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td nowrap="nowrap">
                <uc1:MonthPicker ID="mpNewExpense" runat="server" AutoPostBack="false" />
                <asp:CustomValidator ID="custNewExpense" runat="server" ControlToValidate="txtAmount"
                    ErrorMessage="The expense for the month already exists." ToolTip="The expense for the month already exists."
                    EnableClientScript="false" SetFocusOnError="false" Display="Dynamic" Text="*"
                    OnServerValidate="custNewExpense_ServerValidate" ValidationGroup="NewExpense"></asp:CustomValidator>
            </td>
            <td>
                <asp:TextBox ID="txtAmount" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                    ErrorMessage="The Amount is required." ToolTip="The Amount is required." EnableClientScript="false"
                    SetFocusOnError="true" Display="Dynamic" ValidationGroup="NewExpense">*</asp:RequiredFieldValidator>
                <asp:CustomValidator ID="custAmount" runat="server" ControlToValidate="txtAmount"
                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                    ValidationGroup="NewExpense" OnServerValidate="custAmount_ServerValidate"></asp:CustomValidator>
            </td>
            <td>
                <asp:Button ID="btnAddExpense" runat="server" Text="Add Expense" OnClick="btnAddExpense_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:CustomValidator ID="custMonthExpense" runat="server" ControlToValidate="txtExpenseName"
                    ErrorMessage="You must specify at least one monthly expense." ToolTip="You must specify at least one monthly expense."
                    EnableClientScript="false" SetFocusOnError="false" Display="Static" Text="*"
                    ValidationGroup="ExpenseDetail" ValidateEmptyText="true" OnServerValidate="custMonthExpense_ServerValidate"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />&nbsp;
                <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td>
                <asp:Button ID="btnDelete" runat="server" Text="Delete Expense" OnClick="btnDelete_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:ValidationSummary ID="vsumExpenseDetails" runat="server" EnableClientScript="false"
                    ValidationGroup="ExpenseDetail" />
                <asp:ValidationSummary ID="vsumNewExpense" runat="server" EnableClientScript="false"
                    ValidationGroup="NewExpense" />
                <asp:ValidationSummary ID="vsumMonthExpense" runat="server" EnableClientScript="false"
                    ValidationGroup="MonthExpense" />
            </td>
        </tr>
         <tr>
            <td colspan="3">
                <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
            </td>
        </tr>
    </table>
</asp:Content>

