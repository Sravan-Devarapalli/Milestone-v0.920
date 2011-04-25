<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="ExpenseCategoryList.aspx.cs" Inherits="PraticeManagement.ExpenseCategoryList" Title="Practice Management - Expense Categories" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
	<title>Practice Management - Expense Categories</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
	Expense Categories
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
	<table>
		<tr>
			<td colspan="4">
				<asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" 
					EmptyDataText="There is nothing to be displayed here" DataKeyNames="Id" 
					DataSourceID="odsCategories" OnRowUpdating="gvCategories_RowUpdating"
                    CssClass="CompPerfTable" GridLines="None" BackColor="White">
                    <AlternatingRowStyle BackColor="#F9FAFF" />
                        <Columns>
					    <asp:TemplateField HeaderText="Category Name">
					        <ItemStyle Width="250px" />
					        <ItemTemplate> 
                              <asp:Literal ID="litCategoryName"  Runat="Server" Text='<%# Server.HtmlEncode( (string) Eval("Name")) %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                              <asp:TextBox ID="txtEditCategoryName" Runat="Server" Text='<%# Bind("Name") %>' Width="90%"  /><asp:RequiredFieldValidator 
                                ID="rfvEditCategoryName" 
                                runat="server" 
                                ErrorMessage="The Name is required." 
                                ToolTip="The Name is required." 
                                SetFocusOnError="true"
                                ControlToValidate="txtEditCategoryName" 
                                Display="dynamic" ValidationGroup="UpdateCategory">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
					    </asp:TemplateField>
						<asp:CommandField ButtonType="Link" ShowInsertButton="false" 
							ShowSelectButton="false" CausesValidation="True" ShowDeleteButton="True" 
							ShowEditButton="True" />
					</Columns>
				</asp:GridView>
				<asp:ObjectDataSource ID="odsCategories" runat="server" 
					SelectMethod="GetCategoryList" DeleteMethod="DeleteCategory" UpdateMethod="SaveCategory" 
					ondeleted="odsCategories_Change" onupdated="odsCategories_Change">
				</asp:ObjectDataSource>
				<asp:TextBox ID="txtDummy" runat="server" style="display: none"></asp:TextBox>
				<asp:CustomValidator ID="custCategoriyDelete" runat="server" ControlToValidate="txtDummy"
					ErrorMessage="Cannot delete the Category because it is in use."
					ToolTip="Cannot delete the Category because it is in use."
					EnableClientScript="false" SetFocusOnError="false" Display="None" ValidateEmptyText="true"
					onservervalidate="custCategoriyDelete_ServerValidate" ValidationGroup="Processing"></asp:CustomValidator>
				<asp:CustomValidator ID="custDataAccessError" runat="server" ControlToValidate="txtDummy"
					EnableClientScript="false" SetFocusOnError="false" Display="None" 
					ValidateEmptyText="true" onservervalidate="custDataAccessError_ServerValidate" 
					ValidationGroup="Processing"></asp:CustomValidator>
			</td>
		</tr>
		<tr>
		    <td>&nbsp;</td>
		</tr>
		<tr>
			<td>Enter a Name for a new Category</td>
		</tr>
		<tr>
			<td>
				<asp:TextBox ID="txtNewCategoryName" runat="server" Width="250px"/>
				<asp:RequiredFieldValidator ID="reqNewCategoryName" runat="server" ControlToValidate="txtNewCategoryName"
					ErrorMessage="The Name is required."
					ToolTip="The Name is required."
					EnableClientScript="false" SetFocusOnError="true" Text="*" ValidationGroup="NewCategory"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td>
				<asp:ShadowedTextButton ID="btnAddCategory" runat="server" Text="Add Expense Category" 
					onclick="btnAddCategory_Click" ValidationGroup="NewCategory" CssClass="add-btn"/>
			</td>
		</tr>
		<tr>
			<td colspan="4">
				<asp:ValidationSummary ID="vsumNewCategory" runat="server" 
					EnableClientScript="false" ValidationGroup="NewCategory" />
				<asp:ValidationSummary ID="vsumProcessing" runat="server" 
					EnableClientScript="false" ValidationGroup="Processing" />
					<asp:ValidationSummary ID="vsumUpdate" runat="server" 
					EnableClientScript="false" ValidationGroup="UpdateCategory" />
			</td>
		</tr>
	</table>
</asp:Content>

