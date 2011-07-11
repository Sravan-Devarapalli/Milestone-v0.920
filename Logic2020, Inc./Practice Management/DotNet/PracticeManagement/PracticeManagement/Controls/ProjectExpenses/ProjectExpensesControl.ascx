<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectExpensesControl.ascx.cs"
    Inherits="PraticeManagement.Controls.ProjectExpenses.ProjectExpensesControl" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:UpdateProgress ID="updProgress" runat="server" AssociatedUpdatePanelID="updProjectExpenses">
    <ProgressTemplate>
        <asp:Image ID="img" runat="server" ImageUrl="~/Images/ajax-loader.gif" />
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="updProjectExpenses" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:GridView ID="gvProjectExpenses" runat="server" DataSourceID="odsProjectExpenses"
            EmptyDataText="No project expenses for this Project" ShowFooter="True" AutoGenerateColumns="False"
            AlternatingRowStyle-BackColor="#e0e0e0" DataKeyNames="Id" OnRowDataBound="gvProjectExpenses_OnRowDataBound"
            FooterStyle-Font-Bold="true" FooterStyle-VerticalAlign="Top" CssClass="CompPerfTable WholeWidth"
            GridLines="None" BackColor="White" OnRowUpdating="gvProjectExpenses_OnRowUpdating">
            <AlternatingRowStyle BackColor="#F9FAFF" />
            <RowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            Name
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((ProjectExpense) Container.DataItem).Name %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditName" runat="server" Text='<%# Bind("Name") %>' ValidationGroup="ProjectExpensesEdit" />
                        <asp:RequiredFieldValidator ID="valReqName" runat="server" ControlToValidate="tbEditName"
                            ErrorMessage="Expense name is required" Text="*" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        Total
                        <hr />
                        <asp:TextBox ID="tbEditName" runat="server" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqName" ValidationGroup="ProjectExpensesAdd"
                            runat="server" ControlToValidate="tbEditName" ErrorMessage="Expense name is required"
                            Text="*" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Start Date">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            Start Date
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((DateTime) ((ProjectExpense) Container.DataItem).StartDate).ToString("MM/dd/yyyy") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate", "{0:MM/dd/yyyy}") %>'
                                        ValidationGroup="ProjectExpensesEdit" Width="80px" />
                                </td>
                                <td>
                                    <asp:HyperLink ID="lnkCalendarStartDate" runat="server" ImageUrl="~/Images/calendar.gif"
                                        NavigateUrl="#"></asp:HyperLink>
                                    <ajaxToolkit:CalendarExtender ID="clStartDate" runat="server" BehaviorID="bhclStartDate"
                                        Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>' PopupPosition="BottomLeft"
                                        PopupButtonID="lnkCalendarStartDate" TargetControlID="txtStartDate" />
                                    <asp:RequiredFieldValidator ID="valReqStartDate" ValidationGroup="ProjectExpensesEdit"
                                        runat="server" ControlToValidate="txtStartDate" ErrorMessage="Start Date is required"
                                        Text="*" />
                                    <asp:RegularExpressionValidator ID="rgStartDateFormat" runat="server" ControlToValidate="txtStartDate"
                                        ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ValidationExpression="^([1-9]|0[1-9]|1[012])[/]([1-9]|0[1-9]|[12][0-9]|3[01])[/][0-9]{4}$"
                                        ValidationGroup='ProjectExpensesEdit'>*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="cstStartDateShouldbewithinProjectPeriod" runat="server"
                                        ValidationGroup="ProjectExpensesEdit" ErrorMessage="Start Date must be with in the Project Period range."
                                        Text="*" ToolTip="Start Date must be with in the Project Period range."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <br />
                        <hr />
                        <table style="font-weight: normal;">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtStartDate" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        Width="80px" />
                                </td>
                                <td>
                                    <asp:HyperLink ID="lnkCalendarStartDate" runat="server" ImageUrl="~/Images/calendar.gif"
                                        NavigateUrl="#"></asp:HyperLink>
                                    <ajaxToolkit:CalendarExtender ID="clStartDate" runat="server" BehaviorID="bhclStartDate"
                                        Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>' PopupPosition="BottomLeft"
                                        PopupButtonID="lnkCalendarStartDate" TargetControlID="txtStartDate" />
                                    <asp:RequiredFieldValidator ID="valReqStartDate" ValidationGroup="ProjectExpensesAdd"
                                        runat="server" ControlToValidate="txtStartDate" ErrorMessage="Start Date is required"
                                        Text="*" />
                                    <asp:RegularExpressionValidator ID="rgStartDateFormat" runat="server" ControlToValidate="txtStartDate"
                                        ToolTip="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'." ErrorMessage="The Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ValidationExpression="^([1-9]|0[1-9]|1[012])[/]([1-9]|0[1-9]|[12][0-9]|3[01])[/][0-9]{4}$"
                                        ValidationGroup='ProjectExpensesAdd'>*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="cstStartDateShouldbewithinProjectPeriod" runat="server"
                                        ValidationGroup="ProjectExpensesEdit" ErrorMessage="Start Date must be with in the Project Period range."
                                        Text="*" ToolTip="Start Date must be with in the Project Period range."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            End Date
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((DateTime) ((ProjectExpense) Container.DataItem).EndDate).ToString("MM/dd/yyyy") %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate", "{0:MM/dd/yyyy}") %>'
                                        ValidationGroup="ProjectExpensesEdit" Width="80px" />
                                </td>
                                <td>
                                    <asp:HyperLink ID="lnkCalendarEndDate" runat="server" ImageUrl="~/Images/calendar.gif"
                                        NavigateUrl="#"></asp:HyperLink>
                                    <ajaxToolkit:CalendarExtender ID="clEndDate" runat="server" BehaviorID="bhclEndDate"
                                        Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>' PopupPosition="BottomLeft"
                                        PopupButtonID="lnkCalendarEndDate" TargetControlID="txtEndDate" />
                                    <asp:CustomValidator ID="cstEndShouldBeGreater" runat="server" Text="*" ErrorMessage="End date must be greater than Start date"
                                        ToolTip="End date must be greater than Start date" Display="Dynamic" ValidationGroup="ProjectExpensesEdit"></asp:CustomValidator>
                                    <asp:CustomValidator ID="cstPeriodOverlaps" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        ErrorMessage="Selected Period Overlaps with existing periods." Text="*" Display="Dynamic"></asp:CustomValidator>
                                    <asp:RequiredFieldValidator ID="valReqEndDate" ValidationGroup="ProjectExpensesEdit"
                                        runat="server" ControlToValidate="txtEndDate" ErrorMessage="End Date is required"
                                        Text="*" Display="Dynamic" />
                                    <asp:RegularExpressionValidator ID="rgEndDateFormat" runat="server" ControlToValidate="txtEndDate"
                                        ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ValidationExpression="^([1-9]|0[1-9]|1[012])[/]([1-9]|0[1-9]|[12][0-9]|3[01])[/][0-9]{4}$"
                                        ValidationGroup='ProjectExpensesEdit'>*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="cstEndDateShouldbewithinProjectPeriod" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        ErrorMessage="End Date must be with in the Project Period range." Display="Dynamic"
                                        Text="*" ToolTip="End Date must be with in the Project Period range."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <br />
                        <hr />
                        <table style="font-weight: normal;">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtEndDate" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        Width="80px" />
                                </td>
                                <td>
                                    <asp:HyperLink ID="lnkCalendarEndDate" runat="server" ImageUrl="~/Images/calendar.gif"
                                        NavigateUrl="#"></asp:HyperLink>
                                    <ajaxToolkit:CalendarExtender ID="clEndDate" runat="server" BehaviorID="bhclEndDate"
                                        Format='<%# PraticeManagement.Constants.Formatting.EntryDateFormat %>' PopupPosition="BottomLeft"
                                        PopupButtonID="lnkCalendarEndDate" TargetControlID="txtEndDate" />
                                    <asp:RequiredFieldValidator ID="valReqEndDate" ValidationGroup="ProjectExpensesAdd"
                                        runat="server" ControlToValidate="txtEndDate" ErrorMessage="End Date is required"
                                        Text="*" />
                                    <asp:RegularExpressionValidator ID="rgEndDateFormat" runat="server" ControlToValidate="txtEndDate"
                                        ToolTip="The End Date has an incorrect format. It must be 'MM/dd/yyyy'." ErrorMessage="The End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                        ValidationExpression="^([1-9]|0[1-9]|1[012])[/]([1-9]|0[1-9]|[12][0-9]|3[01])[/][0-9]{4}$"
                                        ValidationGroup='ProjectExpensesAdd'>*</asp:RegularExpressionValidator>
                                    <asp:CustomValidator ID="cstEndShouldBeGreater" runat="server" Text="*" ErrorMessage="End date shuould be greater than Start date"
                                        ToolTip="End date shuould be greater than Start date" ValidationGroup="ProjectExpensesEdit"></asp:CustomValidator>
                                    <asp:CustomValidator ID="cstPeriodOverlaps" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        ErrorMessage="Selected Period Overlaps with existing periods." Text="*"></asp:CustomValidator>
                                    <asp:CustomValidator ID="cstEndDateShouldbewithinProjectPeriod" runat="server" ValidationGroup="ProjectExpensesEdit"
                                        ErrorMessage="End Date must be with in the Project Period range." Text="*" ToolTip="End Date must be with in the Project Period range."></asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg">
                            Expense, $
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense) Container.DataItem).Amount).ToString() %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditAmount" runat="server" Text='<%# Bind("Amount") %>' ValidationGroup="ProjectExpensesEdit"
                            Width="80px" />
                        <asp:RequiredFieldValidator ID="valReqAmount" ValidationGroup="ProjectExpensesEdit"
                            runat="server" ControlToValidate="tbEditAmount" ErrorMessage="Expense amount is required"
                            Text="*" />
                        <asp:RangeValidator ID="valRangeAmout" ValidationGroup="ProjectExpensesEdit" runat="server"
                            ControlToValidate="tbEditAmount" Type="Double" MinimumValue="0.01" MaximumValue="1000000000"
                            ErrorMessage="Amount should be positive real" Text="*" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblTotalAmount" runat="server" Text="$0" />
                        <hr />
                        <asp:TextBox ID="tbEditAmount" runat="server" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqAmount" ValidationGroup="ProjectExpensesAdd"
                            runat="server" ControlToValidate="tbEditAmount" ErrorMessage="Expense amount is required"
                            Text="*" />
                        <asp:RangeValidator ID="valRangeAmout" ValidationGroup="ProjectExpensesAdd" runat="server"
                            ControlToValidate="tbEditAmount" Type="Double" MinimumValue="0.01" MaximumValue="1000000000"
                            ErrorMessage="Amount should be positive real" Text="*" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, %">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            Reimbursed, %
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# (string.Format("{0:0}",((ProjectExpense) Container.DataItem).Reimbursement)) %>%
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbEditReimbursement" runat="server" Text='<%# Bind("Reimbursement") %>'
                            ValidationGroup="ProjectExpensesEdit" Width="80px" />
                        <asp:RequiredFieldValidator ID="valReqReimbursement" ValidationGroup="ProjectExpensesEdit"
                            runat="server" ControlToValidate="tbEditReimbursement" ErrorMessage="Expense reimbursement is required"
                            Text="*" />
                        <asp:RangeValidator ID="valRangeReimbursement" ValidationGroup="ProjectExpensesEdit"
                            runat="server" ControlToValidate="tbEditReimbursement" Type="Double" MinimumValue="0"
                            MaximumValue="500" ErrorMessage="Reimbursement should be positive real between 0.0 and 500.00"
                            Text="*" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursed" runat="server" Text="0%" />
                        <hr />
                        <asp:TextBox ID="tbEditReimbursement" runat="server" Width="80px" ValidationGroup="ProjectExpensesAdd" />
                        <asp:RequiredFieldValidator ID="valReqReimbursement" ValidationGroup="ProjectExpensesAdd"
                            runat="server" ControlToValidate="tbEditReimbursement" ErrorMessage="Expense reimbursement is required"
                            Text="*" />
                        <asp:RangeValidator ID="valRangeReimbursement" ValidationGroup="ProjectExpensesAdd"
                            runat="server" ControlToValidate="tbEditReimbursement" Type="Double" MinimumValue="0"
                            MaximumValue="500" ErrorMessage="Reimbursement should be positive real between 0.0 and 500.00"
                            Text="*" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, $">
                    <HeaderTemplate>
                        <div class="ie-bg">
                            Reimbursed, $
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense)Container.DataItem).ReimbursementAmount).ToString() %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <%# ((PracticeManagementCurrency)((ProjectExpense)Container.DataItem).ReimbursementAmount).ToString()%>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursementAmount" runat="server" Text="$0" />
                        <hr />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False" FooterStyle-VerticalAlign="Bottom" FooterStyle-Font-Bold="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="False" CommandName="Edit"
                            Text="Edit" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" ValidationGroup="ProjectExpensesEdit"
                            CausesValidation="True" CommandName="Update" Text="Update" />
                        &nbsp;
                        <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="Cancel" />
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:ShadowedTextButton ID="btnAddExpense" runat="server" Text="Add Expense" CssClass="add-btn"
                            CommandName="Insert" OnClick="lnkAdd_OnClick" ValidationGroup="ProjectExpensesAdd" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete"
                            Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this expense record?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle BackColor="#E0E0E0" />
        </asp:GridView>
        <asp:ValidationSummary ID="vsProjectExpensesEdit" ValidationGroup="ProjectExpensesEdit"
            runat="server" />
        <asp:ValidationSummary ID="vsProjectExpensesAdd" ValidationGroup="ProjectExpensesAdd"
            runat="server" />
        <asp:ObjectDataSource ID="odsProjectExpenses" runat="server" DataObjectTypeName="DataTransferObjects.ProjectExpense"
            SelectMethod="ProjectExpensesForProject" UpdateMethod="UpdateProjectExpense"
            OnSelecting="odsProjectExpenses_OnSelecting" InsertMethod="AddProjectExpense"
            DeleteMethod="RemoveProjectExpense" TypeName="PraticeManagement.Controls.ProjectExpenses.ProjectExpenseHelper">
            <SelectParameters>
                <asp:Parameter Name="projectId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>

