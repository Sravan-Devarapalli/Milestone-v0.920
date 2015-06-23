<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.CalendarReport" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width95Per">
            </td>
            <td class="textRight Width5PercentImp padRight5">
                <table class="textRight WholeWidth">
                    <tr>
                        <td class="PaddingBottom5Imp">
                            Export:
                        </td>
                        <td class="PaddingBottom5Imp">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportExcel_Click"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="repSummary" runat="server">
        <HeaderTemplate>
            <table id="tblCSATSummary" class="tablesorter WholeWidth">
                <thead>
                    <tr class="trCSATSummaryHeader">
                        <th class="TextAlignLeftImp">
                            Person ID
                        </th>
                        <th>
                            Person Name
                        </th>
                        <th>
                            Time Off Type
                        </th>
                        <th>
                            Time Off Start Date
                        </th>
                        <th>
                            Time Off End Date
                        </th>
                        <th>
                            Project Number
                        </th>
                        <th>
                            Project Name
                        </th>
                        <th>
                            Project Status
                        </th>
                        <th>
                            Account
                        </th>
                        <th>
                            Business Group
                        </th>
                        <th>
                            Business Unit
                        </th>
                        <th>
                            Practice Area
                        </th>
                        <th>
                            Project Access
                        </th>
                        <th>
                            Engagement Manager
                        </th>
                        <th>
                            Executive in Charge
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplateCSAT">
                <td class="padLeft5 textLeft">
                    <%# Eval("EmployeeNumber")%>
                </td>
                <td>
                    <%# Eval("HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.TimeType.Name")%>
                </td>
                <td>
                  <%# ((DateTime)Eval("TimeOff.TimeOffStartDate")).ToString("MM/dd/yyyy")%>
                </td>
                <td>
                    <%# ((DateTime)Eval("TimeOff.TimeOffEndDate")).ToString("MM/dd/yyyy")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.ProjectNumber")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.Status.Name")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.Client.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.BusinessGroup.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.Group.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.Practice.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.ProjectManagerNames")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.SeniorManagerName")%>
                </td>
                <td>
                    <%# Eval("TimeOff.Project.Director.FormattedName")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no Time Offs for this range selected.
    </div>
</div>

