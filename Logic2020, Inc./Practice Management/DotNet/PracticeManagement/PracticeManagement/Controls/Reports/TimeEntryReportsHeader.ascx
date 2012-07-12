<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryReportsHeader.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimeEntryReportsHeader" %>
<table class="WholeWidth">
    <tr>
        <td colspan="3" class="height30P vTop fontBold">
            1.&nbsp;Select a mode of reporting time:
        </td>
    </tr>
    <tr>
        <td id="tdFirst" runat="server" class="Width1Percent">
        </td>
        <td id="tdSecond" runat="server" class="Width98Percent">
            <table class="TimeEntruReportHeader">
                <tr>
                    <th id="thTimePeriod" runat="server">
                        <asp:HyperLink ID="hlByTimePeriod" runat="server" NavigateUrl="~/Reports/TimePeriodSummaryReport.aspx"><div class="PaddingTop6">By Time Period</div></asp:HyperLink>
                    </th>
                    <th id="thProject" runat="server">
                        <asp:HyperLink ID="hlByProject" runat="server" NavigateUrl="~/Reports/ProjectSummaryReport.aspx"><div class="PaddingTop6">By Project</div></asp:HyperLink>
                    </th>
                    <th id="thPerson" runat="server">
                        <asp:HyperLink ID="hlByPerson" runat="server" NavigateUrl="~/Reports/PersonDetailTimeReport.aspx"><div class="PaddingTop6">By Person</div></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td id="tdThird" runat="server" class="Width1Percent">
        </td>
    </tr>
</table>

