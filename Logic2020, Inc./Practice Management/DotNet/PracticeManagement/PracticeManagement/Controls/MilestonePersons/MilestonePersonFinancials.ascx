<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.MilestonePersons.MilestonePersonFinancials" %>
<style type="text/css">
    .align-center
    {
        text-align: center;
    }
    
    table.financials
    {
        background: white;
    }
    
    table.financials td
    {
        padding: 1px;
    }
</style>
<table class="financials">
    <tr>
        <td class="align-center">
            <strong>Quotient</strong></td>
        <td class="align-center">
            <strong>Value</strong></td>
        <td>
            <strong>Acronym</strong></td>
        <td class="align-center">
            <strong>Formula</strong></td>
    </tr>
    <tr>
        <td>
            Client Discount</td>
        <td>
            <asp:Label ID="lblClientDiscount" runat="server" Text="Unavailable" 
                CssClass="Revenue"></asp:Label>
        </td>
        <td class="align-center">
            CD</td>
        <td>
            Predefined discount for this project</td>
    </tr>
    <tr>
        <td>
            Gross Hourly Bill Rate
        </td>
        <td>
            <asp:Label ID="lblGrossHourlyBillRate" runat="server" Text="Unavailable" CssClass="Revenue"></asp:Label>
        </td>
        <td class="align-center">
            GHBR</td>
        <td>
            &#931; [Net Revenue] / [Billed Hours]</td>
    </tr>
    <tr>
        <td>
            Loaded Hourly Pay Rate
        </td>
        <td>
            <asp:Label ID="lblLoadedHourlyPay" runat="server" Text="Unavailable"></asp:Label>
        </td>
        <td class="align-center">
            LHPR</td>
        <td>
            &#931; 
            [COGS] / [Billed Hours]</td>
    </tr>
    <tr>
        <td>
            Hours in Period</td>
        <td>
            <asp:Label ID="lblHoursInPeriod" runat="server" Text="Unavailable"></asp:Label>
        </td>
        <td class="align-center">
            HIP</td>
        <td>
            &#931;
            [Hours per day] x [Business days]</td>
    </tr>
    <tr>
        <td>
            Projected Milestone Revenue Contribution
        </td>
        <td>
            <asp:Label ID="lblProjectedRevenueContribution" runat="server" Text="Unavalable"
                CssClass="Revenue"></asp:Label>
        </td>
        <td class="align-center">
            PMRC</td>
        <td>
            HIP x GHBR - CD</td>
    </tr>
    <tr>
        <td>
            Projected Milestone COGS
        </td>
        <td>
            <asp:Label ID="lblProjectedCogs" runat="server" Text="Unavailable"></asp:Label>
        </td>
        <td class="align-center">
            COGS</td>
        <td>
            HIP x LHPR</td>
    </tr>
    <tr>
        <td>
            Projected Milestone Margin Contribution
        </td>
        <td>
            <asp:Label ID="lblProjectedMarginContribution" runat="server" Text="Unavailable"
                CssClass="Margin"></asp:Label>
        </td>
        <td class="align-center">
            PMMC</td>
        <td>
            PMRC - COGS</td>
    </tr>
</table>

