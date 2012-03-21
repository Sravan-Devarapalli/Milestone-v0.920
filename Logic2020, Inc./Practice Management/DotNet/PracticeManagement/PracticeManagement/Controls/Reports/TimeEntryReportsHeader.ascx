<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryReportsHeader.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimeEntryReportsHeader" %>
<table width="100%">
    <tr>
        <td colspan="3" class="height30P vTop fontBold">
            1.&nbsp;Select a mode of reporting time:
        </td>
    </tr>
    <tr>
        <td style="width: 5%;">
        </td>
        <td style="width: 90%;">
            <table class="TimeEntruReportHeader">
                <tr>
                    <th id="thTimePeriod" runat="server">
                        <asp:HyperLink ID="hlByTimePeriod" runat="server" Text="By Time Period" NavigateUrl="~/Reports/TimePeriodSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                    <th id="thProject" runat="server">
                        <asp:HyperLink ID="hlByProject" runat="server" Text="By Project" NavigateUrl="~/Reports/ProjectSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                    <th id="thPerson" runat="server">
                        <asp:HyperLink ID="hlByPerson" runat="server" Text="By Person" NavigateUrl="~/Reports/PersonDetailTimeReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td style="width: 5%;">
        </td>
    </tr>
</table>

