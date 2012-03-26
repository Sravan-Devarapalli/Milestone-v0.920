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
                        <asp:HyperLink ID="hlByTimePeriod" Width="100%" Height="100%" runat="server" NavigateUrl="~/Reports/TimePeriodSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false"><div class="PaddingTop6">By Time Period</div></asp:HyperLink>
                    </th>
                    <th id="thProject" runat="server">
                        <asp:HyperLink ID="hlByProject" Width="100%" Height="100%" runat="server" NavigateUrl="~/Reports/ProjectSummaryReport.aspx"
                            ForeColor="Black" Font-Underline="false" ><div class="PaddingTop6">By Project</div></asp:HyperLink>
                    </th>
                    <th id="thPerson" runat="server">
                        <asp:HyperLink ID="hlByPerson" Width="100%" Height="100%" runat="server" NavigateUrl="~/Reports/PersonDetailTimeReport.aspx" style="vertical-align:baseline !important;"
                            ForeColor="Black" Font-Underline="false"><div class="PaddingTop6">By Person</div></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td style="width: 5%;">
        </td>
    </tr>
</table>

