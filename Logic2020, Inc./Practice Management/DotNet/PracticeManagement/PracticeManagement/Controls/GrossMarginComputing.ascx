﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GrossMarginComputing.ascx.cs"
    Inherits="PraticeManagement.Controls.GrossMarginComputing" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<table>
    <tr>
        <td align="left" style="padding-top: 3px;">
            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlTermsAndCalculations"
                ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                ToolTip="Expand Filters and Sort Options" />
        </td>
        <td style="padding-left: 5px;">
            Defined Terms and Calculations
        </td>
    </tr>
</table>
<asp:Panel ID="pnlTermsAndCalculations" runat="server">
    <table style="margin-left: 8px !important;">
        <tr>
            <th colspan="3" align="left" style="padding: 5px 0px 5px 0px;">
                Defined Terms
            </th>
        </tr>
        <tr>
            <td   nowrap="nowrap">
                BHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Bonus Hourly Expense
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                FCOGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Full-COGS (based on FLHR)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                FLHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Fully-Loaded Hourly Rate
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                HBR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Hourly Bill Rate
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                HPW
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Hours per Week (Working)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                HPY
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Hours per Year (Working)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                MGM
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Monthly Gross Margin
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                MGR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Monthly Gross Revenue
            </td>
        </tr> 
        <tr>
            <td  nowrap="nowrap">
                ML%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Minimun Load Percentage
            </td>
        </tr>       
        <tr>
            <td  nowrap="nowrap">
                MLF
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Minimum Load Factor
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                RHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Raw Hourly Rate
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                VD
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Vacation Days (per Year)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                VHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Vacation Hourly Expense
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                AD%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                Account Discount Percentage
            </td>
        </tr>
        <tr>
            <th colspan="3" align="left" style="padding: 5px 0px 5px 0px">
                Defined Calculations
            </th>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                BHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                [Bonus] / HPY
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                FCOGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                FLHR * HPW * 4.2
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                HPY
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                (52 * HPW)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                MGM
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                (MGR - [AD%]) - FCOGS
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                MGR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                HBR * HPW * 4.2
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                ML%
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                <a target="_blank" href="Config/Overheads.aspx">Defined in Overheads</a>
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                MLF
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                RHR * [ML%]
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                Monthly COGS
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                FCOGS
            </td>
        </tr>
        <tr>
            <td style="no-wrap;" nowrap="nowrap" valign="top">
                RHR
            </td>
            <td valign="top">
                &nbsp;=&nbsp;
            </td>
            <td>
                ([W2-Salary]/ HPY) OR [W2-Hourly Rate] OR [1099 Hourly Rate] OR ([1099/POR] * HBR/100)
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                FLHR
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
               RHR + [Applicable Overheads] OR [MLF]
            </td>
        </tr>
        <tr>
            <td  nowrap="nowrap">
                VHE
            </td>
            <td>
                &nbsp;=&nbsp;
            </td>
            <td  nowrap="nowrap">
                RHR * (VD * (HPW / 5)) / HPY
            </td>
        </tr>
    </table>
</asp:Panel>

