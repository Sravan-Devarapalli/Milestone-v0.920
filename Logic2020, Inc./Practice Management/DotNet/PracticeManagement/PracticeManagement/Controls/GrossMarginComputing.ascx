<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GrossMarginComputing.ascx.cs"
    Inherits="PraticeManagement.Controls.GrossMarginComputing" %>
<table>
    <tr>
        <td class="PaddingTop3 textLeft">
            <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlTermsAndCalculations"
                ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                ToolTip="Expand Filters and Sort Options" />
        </td>
        <td class="padLeft5">
            Defined Terms and Calculations
        </td>
    </tr>
</table>
<asp:Panel ID="pnlTermsAndCalculations" runat="server">
    <table class="GrossMarginComputingTable">
        <tr>
            <th colspan="3">
                Defined Terms
            </th>
        </tr>
        <tr>
            <td>
                BHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Bonus Hourly Expense
            </td>
        </tr>
        <tr>
            <td>
                FCOGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Full-COGS (based on FLHR)
            </td>
        </tr>
        <tr>
            <td>
                FLHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Fully-Loaded Hourly Rate
            </td>
        </tr>
        <tr>
            <td>
                HBR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Hourly Bill Rate
            </td>
        </tr>
        <tr>
            <td>
                HPW
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Hours per Week (Working)
            </td>
        </tr>
        <tr>
            <td>
                HPY
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Hours per Year (Working)
            </td>
        </tr>
        <tr>
            <td>
                MCM
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Monthly Contribution Margin
            </td>
        </tr>
        <tr>
            <td>
                MGR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Monthly Gross Revenue
            </td>
        </tr>
        <tr>
            <td>
                ML%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Minimun Load Percentage
            </td>
        </tr>
        <tr>
            <td>
                MLF
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Minimum Load Factor
            </td>
        </tr>
        <tr>
            <td>
                RHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Raw Hourly Rate
            </td>
        </tr>
        <tr>
            <td>
                PTO
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                PTO Accrual (per Year)
            </td>
        </tr>
        <tr>
            <td>
                PTOHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                PTO Accrual Hourly Expense
            </td>
        </tr>
        <tr>
            <td>
                AD%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                Account Discount Percentage
            </td>
        </tr>
        <tr>
            <th colspan="3">
                Defined Calculations
            </th>
        </tr>
        <tr>
            <td>
                BHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                [Bonus] / HPY
            </td>
        </tr>
        <tr>
            <td>
                FCOGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                FLHR * HPW * 4.2
            </td>
        </tr>
        <tr>
            <td>
                HPY
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                (52 * HPW)
            </td>
        </tr>
        <tr>
            <td>
                MCM
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                (MGR - [AD%]) - FCOGS
            </td>
        </tr>
        <tr>
            <td>
                MGR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                HBR * HPW * 4.2
            </td>
        </tr>
        <tr>
            <td>
                ML%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                <a target="_blank" href="Config/Overheads.aspx">Defined in Overheads</a>
            </td>
        </tr>
        <tr>
            <td>
                MLF
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                RHR * [ML%]
            </td>
        </tr>
        <tr>
            <td>
                Monthly COGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                FCOGS
            </td>
        </tr>
        <tr>
            <td class="vTop">
                RHR
            </td>
            <td class="vTop">
                &nbsp;=&nbsp;
            </td>
            <td class="WS-Normal">
                ([W2-Salary]/ HPY) OR [W2-Hourly Rate] OR [1099 Hourly Rate] OR ([1099/POR] * HBR/100)
            </td>
        </tr>
        <tr>
            <td>
                FLHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                RHR + [Applicable Overheads] OR [MLF]
            </td>
        </tr>
        <tr>
            <td>
                PTOHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td>
                RHR * (PTO * (HPW / 5)) / HPY
            </td>
        </tr>
    </table>
</asp:Panel>

