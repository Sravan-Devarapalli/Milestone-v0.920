<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectExpensesControl.ascx.cs"
    Inherits="PraticeManagement.Controls.ProjectExpenses.ProjectExpensesControl" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>

<asp:UpdateProgress ID="updProgress" runat="server" AssociatedUpdatePanelID="updProjectExpenses">
    <ProgressTemplate>
        <asp:Image ID="img" runat="server" ImageUrl="~/Images/ajax-loader.gif" />
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="updProjectExpenses" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:GridView ID="gvProjectExpenses" runat="server" DataSourceID="odsProjectExpenses"
            EmptyDataText="No project expenses for this milestone" ShowFooter="True" AutoGenerateColumns="False"
            AlternatingRowStyle-BackColor="#e0e0e0" DataKeyNames="Id" OnRowDataBound="gvProjectExpenses_OnRowDataBound"
            FooterStyle-Font-Bold="true" FooterStyle-VerticalAlign="Top"
            CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White">
            <AlternatingRowStyle BackColor="#F9FAFF" />
            <RowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <%# ((ProjectExpense) Container.DataItem).Name %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditName" runat="server" Text='<%# Bind("Name") %>' ValidationGroup="ProjectExpensesEdit" />
                        <asp:RequiredFieldValidator ID="valReqName" runat="server" 
                                ControlToValidate="tbEditName" ErrorMessage="Expense name is required" Text="*"/>
                    </EditItemTemplate>
                    <FooterTemplate>
                        Total
                        <hr />
                        <asp:TextBox ID="tbEditName" runat="server" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqName" ValidationGroup="ProjectExpensesAdd" runat="server" 
                                ControlToValidate="tbEditName" ErrorMessage="Expense name is required" Text="*"/>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Expense, $">
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense) Container.DataItem).Amount).ToString() %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditAmount" runat="server" Text='<%# Bind("Amount") %>' ValidationGroup="ProjectExpensesEdit" />
                        <asp:RequiredFieldValidator ID="valReqAmount" ValidationGroup="ProjectExpensesEdit" runat="server" 
                                ControlToValidate="tbEditAmount" ErrorMessage="Expense amount is required" Text="*"/>
                        <asp:RangeValidator ID="valRangeAmout" ValidationGroup="ProjectExpensesEdit" runat="server" 
                            ControlToValidate="tbEditAmount" Type="Double" MinimumValue="0.01" MaximumValue="1000000000" 
                            ErrorMessage="Amount should be positive real" Text="*"/>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblTotalAmount" runat="server" Text="$0"/>
                        <hr />
                        <asp:TextBox ID="tbEditAmount" runat="server" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqAmount" ValidationGroup="ProjectExpensesAdd" runat="server" 
                                ControlToValidate="tbEditAmount" ErrorMessage="Expense amount is required" Text="*"/>
                        <asp:RangeValidator ID="valRangeAmout" ValidationGroup="ProjectExpensesAdd" runat="server" 
                            ControlToValidate="tbEditAmount" Type="Double" MinimumValue="0.01" MaximumValue="1000000000"
                            ErrorMessage="Amount should be positive real" Text="*"/>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, %">
                    <ItemTemplate>
                        <%# ((ProjectExpense) Container.DataItem).Reimbursement %>%
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditReimbursement" runat="server" Text='<%# Bind("Reimbursement") %>' ValidationGroup="ProjectExpensesEdit" />
                        <asp:RequiredFieldValidator ID="valReqReimbursement" ValidationGroup="ProjectExpensesEdit" runat="server" 
                            ControlToValidate="tbEditReimbursement" ErrorMessage="Expense reimbursement is required" Text="*"/>
                        <asp:RangeValidator ID="valRangeReimbursement" ValidationGroup="ProjectExpensesEdit" runat="server" 
                            ControlToValidate="tbEditReimbursement" Type="Double" MinimumValue="0" MaximumValue="500"
                            ErrorMessage="Reimbursement should be positive real between 0.0 and 500.00" Text="*"/>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursed" runat="server" Text="0%"/>
                        <hr />
                        <asp:TextBox ID="tbEditReimbursement" runat="server" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqReimbursement" ValidationGroup="ProjectExpensesAdd" runat="server" 
                            ControlToValidate="tbEditReimbursement" ErrorMessage="Expense reimbursement is required" Text="*"/>
                         <asp:RangeValidator ID="valRangeReimbursement" ValidationGroup="ProjectExpensesAdd" runat="server" 
                            ControlToValidate="tbEditReimbursement" Type="Double" MinimumValue="0" MaximumValue="500"
                            ErrorMessage="Reimbursement should be positive real between 0.0 and 500.00" Text="*"/>
                   </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, $">
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense)Container.DataItem).ReimbursementAmount).ToString() %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <%# ((PracticeManagementCurrency)((ProjectExpense)Container.DataItem).ReimbursementAmount).ToString()%>
                    </EditItemTemplate>
                     <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursementAmount" runat="server" Text="$0"/>
                        <hr />
                     </FooterTemplate>
               </asp:TemplateField>
                <asp:TemplateField ShowHeader="False" FooterStyle-VerticalAlign="Bottom" FooterStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit"/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" ValidationGroup="ProjectExpensesEdit" CausesValidation="True" CommandName="Update" Text="Update"/>
                        &nbsp;
                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"/>
                    </EditItemTemplate>
                    <FooterTemplate>                        
                        <asp:ShadowedTextButton ID="btnAddExpense" runat="server" Text="Add Expense" CssClass="add-btn" 
                            CommandName="Insert" OnClick="lnkAdd_OnClick" ValidationGroup="ProjectExpensesAdd"/>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" 
                            CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this expense record?');"/>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle BackColor="#E0E0E0"/>
        </asp:GridView>
        <asp:ValidationSummary ID="vsProjectExpensesEdit" ValidationGroup="ProjectExpensesEdit" runat="server" />
        <asp:ValidationSummary ID="vsProjectExpensesAdd" ValidationGroup="ProjectExpensesAdd" runat="server" />
       <asp:ObjectDataSource ID="odsProjectExpenses" runat="server" DataObjectTypeName="DataTransferObjects.ProjectExpense"
            SelectMethod="ProjectExpensesForMilestone" UpdateMethod="UpdateProjectExpense" InsertMethod="AddProjectExpense"
            DeleteMethod="RemoveProjectExpense" TypeName="PraticeManagement.Controls.ProjectExpenses.ProjectExpenseHelper">
            <SelectParameters>
                <asp:QueryStringParameter QueryStringField="id" Name="milestoneId" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>

