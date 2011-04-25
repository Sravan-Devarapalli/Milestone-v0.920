<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ExpenseList.aspx.cs" Inherits="PraticeManagement.ExpenseList" Title="Practice Management - Expenses" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>
<%@ Register Src="Controls/MonthPicker.ascx" TagName="MonthPicker" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Expenses</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Expenses
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="pnlBody" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="buttons-block">
                 <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server"
                                TargetControlID="pnlFilters" ImageControlID="btnExpandCollapseFilter"
                                CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />                        
                <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />
                <asp:ShadowedTextButton ID="btnAddExpense" runat="server" Text="Add Expense" OnClick="btnAddExpense_Click"
                    CausesValidation="False" CssClass="add-btn" />
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <AjaxControlToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0"
                    CssClass="CustomTabStyle">
                    <ajaxToolkit:TabPanel runat="server" ID="tpMainFilters">
                        <HeaderTemplate>
                            <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span></span>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td>
                                        Select Dates
                                    </td>
                                    <td>
                                        <uc2:MonthPicker ID="mpPeriodStart" runat="server" AutoPostBack="false" />
                                    </td>
                                    <td>
                                        &nbsp;to&nbsp;
                                    </td>
                                    <td>
                                        <uc2:MonthPicker ID="mpPeriodEnd" runat="server" AutoPostBack="false" />
                                    </td>
                                    <td>
                                        <asp:CustomValidator ID="custPeriod" runat="server" ErrorMessage="The Period Start must be greater than or equal to the Period End"
                                            ToolTip="The Period Start must be greater than or equal to the Period End" Text="*"
                                            EnableClientScript="false" OnServerValidate="custPeriod_ServerValidate"></asp:CustomValidator>
                                        <asp:CustomValidator ID="custPeriodLengthLimit" runat="server" EnableViewState="false"
                                            ErrorMessage="The period length must be not more then {0} months." ToolTip="The period length must be not more then {0} months."
                                            Text="*" EnableClientScript="false" OnServerValidate="custPeriodLengthLimit_ServerValidate"></asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update View" OnClick="btnUpdate_Click" />
                                    </td>
                                </tr>
                            </table>
                            <asp:ValidationSummary ID="vsumExpenses" runat="server" EnableClientScript="false" />
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </AjaxControlToolkit:TabContainer>
            </asp:Panel>
            <br />
            <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here"
                DataKeyNames="Name" EnableViewState="False" OnRowDataBound="gvExpenses_RowDataBound"
                CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White">
                <AlternatingRowStyle BackColor="#F9FAFF" />
                <Columns>
                    <asp:TemplateField HeaderText="Category">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Category</div>
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Item</div>
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Basis">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Basis</div>
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Week paid">
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Week paid</div>
                        </HeaderTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

