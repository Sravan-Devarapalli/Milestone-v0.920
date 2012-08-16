<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HumanCapitalReportsHeader.ascx.cs" Inherits="PraticeManagement.Controls.Reports.HumanCapital.HumanCapitalReportsHeader" %>
<table class="WholeWidth">
    <tr>
        <td colspan="3" class="height30P vTop fontBold">
            1.&nbsp;Select a report:
        </td>
    </tr>
    <tr>
        <td id="tdFirst" runat="server" class="Width10Percent">
        </td>
        <td id="tdSecond" runat="server" class="Width80Percent">
            <table class="TimeEntruReportHeader">
                <tr>
                    <th id="thNewHireReport" runat="server">
                        <asp:HyperLink ID="hlNewHireReport" runat="server" NavigateUrl="~/Reports/NewHireReport.aspx"><div class="PaddingTop6">New Hire Report</div></asp:HyperLink>
                    </th>
                    <th id="thTerminationReport" runat="server">
                        <asp:HyperLink ID="hlTerminationReport" runat="server" NavigateUrl="~/Reports/TerminationReport.aspx"><div class="PaddingTop6">Termination Report</div></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td id="tdThird" runat="server" class="Width10Percent">
        </td>
    </tr>
</table>

