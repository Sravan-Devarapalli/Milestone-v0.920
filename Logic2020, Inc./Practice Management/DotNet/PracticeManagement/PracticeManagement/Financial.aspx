<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Financial.aspx.cs" Inherits="PraticeManagement.Financial" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/ExpenseCategoryListControl.ascx" TagName="ExpenseCategoryListControl"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/PersonOverheadCalculationControl.ascx" TagName="PersonOverheadCalculation"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/ExpenseListControl.ascx" TagName="ExpenseListControl"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/BudgetEntries.ascx" TagName="Budget"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Financial</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Financial
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript">
        function selfCheckDirtyWithRedirect(arg) {
            var result = showDialodWhenRequired();
            if (result == true) {
                __doPostBack('__Page', arg);
                return true;
            }
            else if (result == false) {
                __doPostBack('__Page', arg);
            }
            return false;
        }

    </script>
    <ajaxtoolkit:TabContainer ID="tabSettings" runat="server" CssClass="CustomTabStyle">
        <ajaxtoolkit:TabPanel runat="server" ID="tpnlCommissions">
            <HeaderTemplate>
                <span class="bg"><a href="#?0" onclick="return selfCheckDirtyWithRedirect(this.href);"><span>Commissions</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                Page not exist .
            </ContentTemplate>
        </ajaxtoolkit:TabPanel>
        <ajaxtoolkit:TabPanel runat="server" ID="tpnlExpenses">
            <HeaderTemplate>
                <span class="bg"><a href="#?1" onclick="return selfCheckDirtyWithRedirect(this.href);"><span>Expenses</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlExpenseListControl" runat="server">
                    <ContentTemplate>
                        <uc2:ExpenseListControl ID="expenseListControl" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxtoolkit:TabPanel>
        <ajaxtoolkit:TabPanel runat="server" ID="pnlExpenseCategoryList">
            <HeaderTemplate>
                <span class="bg"><a href="#?2" onclick="return selfCheckDirtyWithRedirect(this.href);"><span>Expense Categories</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlExpenseCategoryList" runat="server">
                    <ContentTemplate>
                        <uc2:ExpenseCategoryListControl ID="expenseCategoryList" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxtoolkit:TabPanel>
        <ajaxtoolkit:TabPanel runat="server" ID="tpnlOverheads">
            <HeaderTemplate>
                <span class="bg"><a href="#?3" onclick="return selfCheckDirtyWithRedirect(this.href);"><span>Overheads</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlOverheads" runat="server">
                    <ContentTemplate>
                        <uc2:PersonOverheadCalculation ID="personOverheadCalculation" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxtoolkit:TabPanel>
        <ajaxtoolkit:TabPanel runat="server" ID="tpnlBudget">
            <HeaderTemplate>
                <span class="bg"><a href="#?4" onclick="return selfCheckDirtyWithRedirect(this.href);"><span>Revenue Goals</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlBudget" runat="server">
                    <ContentTemplate>
                        <uc2:Budget ID="ucBudget" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxtoolkit:TabPanel>
    </ajaxtoolkit:TabContainer>
</asp:Content>

