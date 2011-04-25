<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="DefaultRecruitingCommissionDetail.aspx.cs" Inherits="PraticeManagement.DefaultRecruitingCommissionDetail"
    Title="Practice Management - Default Recruiting Commission Details" %>

<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/PersonInfo.ascx" TagName="PersonInfo" TagPrefix="uc1" %>
<%@ Register Src="Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Default Recruiting Commission Details</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Default Recruiting Commission Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="bg-light-frame">
        <uc1:PersonInfo ID="personInfo" runat="server" />
        <table>
            <tr>
                <td>
                    Start Date
                </td>
                <td>
                    <uc2:DatePicker ID="dpStartDate" runat="server" />
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="The Start Date is required." ToolTip="The Start Date is required."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="CommissionHeader" />
                    <asp:CompareValidator ID="compStartDateType" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                        ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                        Type="Date" ValidationGroup="CommissionHeader" />
                </td>
                <td>
                    End Date
                </td>
                <td>
                    <uc2:DatePicker ID="dpEndDate" runat="server" />
                </td>
                <td>
                    <asp:CompareValidator ID="compEndDateType" runat="server" ControlToValidate="dpEndDate"
                        ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                        ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." Text="*"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" Operator="DataTypeCheck"
                        Type="Date" ValidationGroup="CommissionHeader"></asp:CompareValidator>
                    <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpEndDate"
                        ControlToCompare="dpStartDate" ErrorMessage="The End Date must be greater than or equal to the Start Date."
                        ToolTip="The End Date must be greater or equal to the Start Date." Text="*" EnableClientScript="false"
                        SetFocusOnError="true" Display="Dynamic" Operator="GreaterThanEqual" Type="Date"
                        ValidationGroup="CommissionHeader"></asp:CompareValidator>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:CustomValidator ID="custStartDate" runat="server" ControlToValidate="dpStartDate"
                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="CommissionHeader"
                        OnServerValidate="custStartDate_ServerValidate" Text="*" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    Commissions
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:GridView ID="gvRecruitingCommissions" runat="server" EmptyDataText="There is nothing to be displayed here."
                        AutoGenerateColumns="False" DataKeyNames="HoursToCollect" ShowFooter="True" OnRowCancelingEdit="gvRecruitingCommissions_RowCancelingEdit"
                        OnRowDeleting="gvRecruitingCommissions_RowDeleting" OnRowEditing="gvRecruitingCommissions_RowEditing"
                        OnRowUpdating="gvRecruitingCommissions_RowUpdating" OnRowDataBound="gvRecruitingCommissions_RowDataBound"
                        CssClass="CompPerfTable" GridLines="None">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div class="ie-bg no-wrap">
                                        After days</div>
                                </HeaderTemplate>
                                <HeaderStyle Width="150px" />
                                <ItemStyle Width="150px" />
                                <FooterStyle Width="150px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="lblHoursToCollect" runat="server" Text='<%# (int)Eval("HoursToCollect") / 8 %>'></asp:Label>
                                    <asp:HiddenField ID="hidHoursToCollect" runat="server" Value='<%# Bind("HoursToCollect") %>' />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtHoursToCollect" runat="server" Width="95%"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <div class="ie-bg no-wrap">
                                        Amount</div>
                                </HeaderTemplate>
                                <HeaderStyle Width="200px" />
                                <FooterStyle Width="200px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("Amount") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAmount" runat="server" Text='<%# Eval("Amount.Value") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAmount" runat="server" Width="95%"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                                        ErrorMessage="The Amount is required." ToolTip="The Amount is required." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="CommissionItem"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compAmount" runat="server" ControlToValidate="txtAmount"
                                        ErrorMessage="A number with 2 decimal digits is allowed for the Amount." ToolTip="A number with 2 decimal digits is allowed for the Amount."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="CommissionItem"></asp:CompareValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:RequiredFieldValidator ID="reqHoursToCollect" runat="server" ControlToValidate="txtHoursToCollect"
                                        ErrorMessage="The After days is required." ToolTip="The After days is required."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        ValidationGroup="CommissionItem"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compHoursToCollect" runat="server" ControlToValidate="txtHoursToCollect"
                                        ErrorMessage="The After days must be an integer number." ToolTip="The After days must be an integer number."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="DataTypeCheck" Type="Integer" ValidationGroup="CommissionItem"></asp:CompareValidator>
                                    <asp:RequiredFieldValidator ID="reqAmount" runat="server" ControlToValidate="txtAmount"
                                        ErrorMessage="The Amount is required." ToolTip="The Amount is required." Text="*"
                                        EnableClientScript="false" SetFocusOnError="true" Display="Dynamic" ValidationGroup="CommissionItem"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="compAmount" runat="server" ControlToValidate="txtAmount"
                                        ErrorMessage="A number with 2 decimal digits is allowed for the Amount." ToolTip="A number with 2 decimal digits is allowed for the Amount."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        Operator="DataTypeCheck" Type="Currency" ValidationGroup="CommissionItem"></asp:CompareValidator>
                                    <asp:CustomValidator ID="custHoursToCollectDuplicate" runat="server" ControlToValidate="txtHoursToCollect"
                                        ErrorMessage="The commission for the same period alredy exists." ToolTip="The commission for the same period alredy exists."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        ValidateEmptyText="false" ValidationGroup="CommissionItem" OnServerValidate="custHoursToCollectDuplicate_ServerValidate"></asp:CustomValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:ImageButton ID="btnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                        Text="" ImageUrl="~/Images/icon-check.png" />
                                    &nbsp;<asp:ImageButton ID="btnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="" ImageUrl="~/Images/no.png" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="" ImageUrl="~/Images/icon-edit.png" />
                                    &nbsp;<asp:ImageButton ID="btnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="" ImageUrl="~/Images/icon-delete.png" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:ImageButton ID="btnInsert" runat="server" CausesValidation="true" Text="Insert"
                                        ValidationGroup="CommissionItem" OnClick="btnInsert_Click" ImageUrl="~/Images/button-add.png" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:HiddenField ID="hdDeleted" runat="server" Value="False" />
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:CustomValidator ID="custCommissionItems" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="At least one commission must be specified." ToolTip="At least one commission must be specified."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="CommissionHeader" ValidateEmptyText="true" OnServerValidate="custCommissionItems_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custdateRangeBegining" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="The Start Date is incorrect. There are several other recruiting commission records for the specified period. Please edit them first."
                        ToolTip="The Start Date is incorrect. There are several other compensation recruiting commission for the specified period. Please edit them first."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="CommissionHeader" ValidateEmptyText="false" OnServerValidate="custdateRangeBegining_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custdateRangeEnding" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="The End Date is incorrect. There are several other recruiting commission records for the specified period. Please edit them first."
                        ToolTip="The End Date is incorrect. There are several other compensation recruiting commission for the specified period. Please edit them first."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="CommissionHeader" ValidateEmptyText="false" OnServerValidate="custdateRangeEnding_ServerValidate"></asp:CustomValidator>
                    <asp:CustomValidator ID="custdateRangePeriod" runat="server" ControlToValidate="dpStartDate"
                        ErrorMessage="The period is incorrect. There are several other recruiting commission records for the specified period. Please edit them first."
                        ToolTip="The period is incorrect. There are several other compensation recruiting commission for the specified period. Please edit them first."
                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                        ValidationGroup="CommissionHeader" ValidateEmptyText="false" OnServerValidate="custdateRangePeriod_ServerValidate"></asp:CustomValidator>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:ValidationSummary ID="vsumDefaultRecruitingCommissionHeader" runat="server"
                        EnableClientScript="false" ValidationGroup="CommissionHeader" />
                    <asp:ValidationSummary ID="vsumDefaultRecruitingCommissionItem" runat="server" EnableClientScript="false"
                        ValidationGroup="CommissionItem" />
                    <asp:ValidationSummary ID="valSum" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <div class="buttons-block" style="margin-bottom: 10px;">
        <asp:HiddenField ID="hdnRecruiterCommissionId" runat="server" />
        <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
        <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" />&nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <div class="clear0">
        </div>
    </div>
</asp:Content>

