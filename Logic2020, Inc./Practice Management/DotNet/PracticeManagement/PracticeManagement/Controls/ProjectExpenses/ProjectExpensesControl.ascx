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
            <AlternatingRowStyle CssClass="alterrow" />
            <RowStyle CssClass="BackGroundColorWhite" />
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Milestone
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%# ((ProjectExpense)Container.DataItem).Milestone.HtmlEncodedDescription%>
                    </ItemTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <FooterTemplate>
                        Total
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Name
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((ProjectExpense)Container.DataItem).HtmlEncodedName%>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Start Date">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Start Date
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((DateTime) ((ProjectExpense) Container.DataItem).StartDate).ToString("MM/dd/yyyy") %>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="End Date">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            End Date
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((DateTime) ((ProjectExpense) Container.DataItem).EndDate).ToString("MM/dd/yyyy") %>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Expense Type">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Expense Type
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%#  ((ProjectExpense) Container.DataItem).Type.Name %>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:TemplateField>
                 <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                           Estimated Expense, $
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense) Container.DataItem).ExpectedAmount).ToString() %>
                    </ItemTemplate>
                    <FooterStyle CssClass="textCenter" />
                    <FooterTemplate>
                        <asp:Label ID="lblTotalExpAmount" runat="server" Text="$0" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                           Actual Expense, $
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense) Container.DataItem).Amount).ToString() %>
                    </ItemTemplate>
                    <FooterStyle CssClass="textCenter" />
                    <FooterTemplate>
                        <asp:Label ID="lblTotalAmount" runat="server" Text="$0" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, %">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Reimbursed, %
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# (string.Format("{0:0}",((ProjectExpense) Container.DataItem).Reimbursement)) %>%
                    </ItemTemplate>
                    <FooterStyle CssClass="textCenter" />
                    <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursed" runat="server" Text="0%" />
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reimbursed, $">
                    <HeaderTemplate>
                        <div class="ie-bg NoBorder">
                            Reimbursed, $
                        </div>
                    </HeaderTemplate>
                    <ItemStyle CssClass="textCenter" />
                    <ItemTemplate>
                        <%# ((PracticeManagementCurrency) ((ProjectExpense)Container.DataItem).ReimbursementAmount).ToString() %>
                    </ItemTemplate>
                    <FooterStyle CssClass="textCenter" />
                    <FooterTemplate>
                        <asp:Label ID="lblTotalReimbursementAmount" runat="server" Text="$0" />
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="bgColor_e0e0e0" />
        </asp:GridView>
        <asp:ObjectDataSource ID="odsProjectExpenses" runat="server" DataObjectTypeName="DataTransferObjects.ProjectExpense"
            SelectMethod="ProjectExpensesForProject" OnSelecting="odsProjectExpenses_OnSelecting"
            TypeName="PraticeManagement.Controls.ProjectExpenses.ProjectExpenseHelper">
            <SelectParameters>
                <asp:Parameter Name="projectId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>

