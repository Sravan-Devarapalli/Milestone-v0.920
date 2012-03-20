<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeEntryReportsHeader.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimeEntryReportsHeader" %>
<table width="100%">
    <tr>
        <td style="width: 5%;">
        </td>
        <td style="width: 90%;">
            <table class="TimeEntruReportHeader">
                <tr>
                    <th id="thTimePeriod" runat="server">
                        <asp:HyperLink ID="hlByTimePeriod" runat="server" Text="Time Period Summary" NavigateUrl="~/Reports/TimePeriodSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                    <th id="thProject" runat="server">
                        <asp:HyperLink ID="hlByProject" runat="server" Text="Project Summary" NavigateUrl="~/Reports/ProjectSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                    <th id="thPerson" runat="server">
                        <asp:HyperLink ID="hlByPerson" runat="server" Text="Person Detail" NavigateUrl="~/Reports/PersonDetailTimeReport.aspx"
                            ForeColor="Black" Font-Underline="false"></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td style="width: 5%;">
        </td>
    </tr>
</table>
<br />

