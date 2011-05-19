<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ExpenseCategories.aspx.cs" Inherits="PraticeManagement.Config.ExpenseCategories" %>

<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Expense Categories</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Expense Categories
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table>
        <tr>
            <td colspan="4">
                <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" Width="400px"
                    DataKeyNames="Id" DataSourceID="odsCategories" OnRowUpdating="gvCategories_RowUpdating"
                    CssClass="CompPerfTable" GridLines="None" BackColor="White">
                    <AlternatingRowStyle BackColor="#F9FAFF" />
                    <EmptyDataTemplate>
                        <table id="tblfirst" class="CompPerfTable" width="400px">
                            <tr>
                                <th>
                                </th>
                                <th style="width: 350px;">
                                    <span>Category Name</span>
                                </th>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-left: 30px; padding-top: 10px; padding-bottom: 10px;"
                                    class="GridViewPadding">
                                    <span>There are no Expense Categories.</span>
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle Width="20%" HorizontalAlign="Center" Height="25px" />
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnEdit" CommandName="edit" runat="server" ToolTip="Edit"
                                    ImageUrl="~/Images/icon-edit.png" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgbtnUpdate" CommandName="update" runat="server" ToolTip="Update"
                                    ImageUrl="~/Images/icon-check.png" />
                                <asp:ImageButton ID="imgbtnCancel" CommandName="cancel" runat="server" ToolTip="Cancel"
                                    ImageUrl="~/Images/no.png" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Category Name">
                            <ItemStyle Width="80%" />
                            <ItemTemplate>
                                <asp:Literal ID="litCategoryName" runat="Server" Text='<%# Server.HtmlEncode( (string) Eval("Name")) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditCategoryName" runat="Server" Text='<%# Bind("Name") %>' Width="90%"
                                    MaxLength="25" /><asp:RequiredFieldValidator ID="rfvEditCategoryName" runat="server"
                                        ErrorMessage="The Name is required." ToolTip="The Name is required." SetFocusOnError="true"
                                        ControlToValidate="txtEditCategoryName" Display="dynamic" ValidationGroup="UpdateCategory">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ButtonType="Image" DeleteImageUrl="~/Images/icon-delete.png" ShowInsertButton="false"
                            ShowSelectButton="false" CausesValidation="True" ShowDeleteButton="True" ShowEditButton="false" />
                    </Columns>
                </asp:GridView>
                <table width="400px">
                    <tr style="background-color: #F9FAFF; height: 25px;">
                        <td style="width: 20%; text-align: center;">
                            <asp:ImageButton ID="ibtnInsertCategory" runat="server" Visible="true" ToolTip="Add Expense Category"
                                ImageUrl="~/Images/add_16.png" OnClick="ibtnInsertCategory_Click" />
                            <asp:ImageButton ID="ibtnInsert" runat="server" Visible="false" OnClick="ibtnInsert_Clicked"
                                ToolTip="Save" ImageUrl="~/Images/icon-check.png" />
                            <asp:ImageButton ID="ibtnCancel" CommandName="cancel" runat="server" Visible="false"
                                ToolTip="Cancel" ImageUrl="~/Images/no.png" OnClick="ibtnCancel_Clicked" />
                        </td>
                        <td style="width: 80%;">
                            <asp:TextBox ID="txtNewCategoryName" runat="server" Width="90%" Visible="false" ValidationGroup="NewCategory" />
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="tbwmextender" runat="server" TargetControlID="txtNewCategoryName"
                                WatermarkText="Add Expense Category" WatermarkCssClass="watermarkedtext">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                            <asp:RequiredFieldValidator ID="reqNewCategoryName" runat="server" ControlToValidate="txtNewCategoryName"
                                ErrorMessage="The Name is required." ToolTip="The Name is required." EnableClientScript="false"
                                SetFocusOnError="true" Text="*" ValidationGroup="NewCategory"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <asp:ObjectDataSource ID="odsCategories" runat="server" SelectMethod="GetCategoryList"
                    DeleteMethod="DeleteCategory" UpdateMethod="SaveCategory" OnDeleted="odsCategories_Change"
                    OnUpdated="odsCategories_Change"></asp:ObjectDataSource>
                <asp:TextBox ID="txtDummy" runat="server" Style="display: none"></asp:TextBox>
                <asp:CustomValidator ID="custCategoriyDelete" runat="server" ControlToValidate="txtDummy"
                    ErrorMessage="Cannot delete the Category because it is in use." ToolTip="Cannot delete the Category because it is in use."
                    EnableClientScript="false" SetFocusOnError="false" Display="None" ValidateEmptyText="true"
                    OnServerValidate="custCategoriyDelete_ServerValidate" ValidationGroup="Processing"></asp:CustomValidator>
                <asp:CustomValidator ID="custDataAccessError" runat="server" ControlToValidate="txtDummy"
                    EnableClientScript="false" SetFocusOnError="false" Display="None" ValidateEmptyText="true"
                    OnServerValidate="custDataAccessError_ServerValidate" ValidationGroup="Processing"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="msgLabel" runat="server" ForeColor="Red"></asp:Label>
                <asp:ValidationSummary ID="vsumNewCategory" runat="server" EnableClientScript="false"
                    ValidationGroup="NewCategory" />
                <asp:ValidationSummary ID="vsumProcessing" runat="server" EnableClientScript="false"
                    ValidationGroup="Processing" />
                <asp:ValidationSummary ID="vsumUpdate" runat="server" EnableClientScript="false"
                    ValidationGroup="UpdateCategory" />
            </td>
        </tr>
    </table>
</asp:Content>

