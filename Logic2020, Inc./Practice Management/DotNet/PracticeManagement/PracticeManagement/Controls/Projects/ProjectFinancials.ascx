<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectFinancials" %>
<asp:Panel ID="pnlFinancials" runat="server" CssClass="tab-pane">
    <table class="alterrow">
        <tr>
            <td>
                Estimated Revenue
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblEstimatedRevenue" runat="server" CssClass="Revenue">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Reimbursed Expenses, $
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblReimbursedExpenses" runat="server" Font-Bold="true">Unavailable</asp:Label>
            </td>
            <td class="NoBorder">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Client Discount (<asp:Label ID="lblDiscount" runat="server"></asp:Label>%)
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblDiscountAmount" CssClass="Revenue" runat="server">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Revenue net of discounts
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblRevenueNet" CssClass="Revenue" runat="server">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Estimated COGS
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblEstimatedCogs" runat="server" CssClass="Cogs">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Expenses, $
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblExpenses" runat="server" Font-Bold="true" >Unavailable</asp:Label>
            </td>
            <td class="NoBorder">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Contribution margin
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblGrossMargin" CssClass="Margin" runat="server">Unavailable</asp:Label>
            </td>
            <td>
                &nbsp;
            </td>
            <td id="tdTargetMargin" runat="server"  class="textRightImp">
                (<asp:Label ID="lblTargetMargin" runat="server">Unavailable</asp:Label>)
            </td>
        </tr>
        <tr>
            <td>
                Sales Commission
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblSalesCommission" runat="server">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Practice Area Manager Commission
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblPracticeManagerCommission" runat="server">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                Net Margin
            </td>
            <td class="textRightImp">
                <asp:Label ID="lblEstimatedMargin" runat="server" CssClass="Margin">Unavailable</asp:Label>
            </td>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>

