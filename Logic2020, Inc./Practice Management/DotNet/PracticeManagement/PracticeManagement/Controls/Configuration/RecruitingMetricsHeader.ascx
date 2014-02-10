<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecruitingMetricsHeader.ascx.cs" Inherits="PraticeManagement.Controls.Configuration.RecruitingMetricsHeader" %>
<table class="WholeWidth">
    <tr>
        <td id="tdFirst" runat="server" class="Width10Percent">
        </td>
        <td id="tdSecond" runat="server" class="Width80Percent">
            <table class="TimeEntruReportHeader">
                <tr>
                    <th id="thSourceRecruitingMetrics" runat="server">
                        <asp:HyperLink ID="hlSourceRecruitingMetrics" runat="server" NavigateUrl="~/SourceRecruitingMetrics.aspx"><div class="PaddingTop6">Candidate Source Field Customization</div></asp:HyperLink>
                    </th>
                    <th id="thTargetCompanyRecruitingMetrics" runat="server">
                        <asp:HyperLink ID="hlTargetCompanyRecruitingMetrics" runat="server" NavigateUrl="~/TargetedCompanyRecruitingMetrics.aspx"><div class="PaddingTop6">Targeted Company Field Customization</div></asp:HyperLink>
                    </th>
                </tr>
            </table>
        </td>
        <td id="tdThird" runat="server" class="Width10Percent">
        </td>
    </tr>
</table>
