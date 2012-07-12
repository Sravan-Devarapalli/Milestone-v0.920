<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.PersonSummaryReport" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" class="Width90Percent">
        </td>
        <td class="Width10Percent padRight5">
            <table class="WholeWidth">
                <tr>
                    <td>
                        Export:
                    </td>
                    <td>
                        <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                            UseSubmitBehavior="false" ToolTip="Export To Excel" />
                    </td>
                    <td>
                        <asp:Button ID="btnExportToPDF" runat="server" Text="PDF" OnClick="btnExportToPDF_OnClick"
                            Enabled="false" UseSubmitBehavior="false" ToolTip="Export To PDF" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<asp:Repeater ID="repSummary" runat="server">
    <HeaderTemplate>
        <table id="tblPersonSummaryReport" class="tablesorter TimePeriodByproject WholeWidth">
            <thead>
                <tr>
                    <th class="textLeft Width550PxImp">
                        Project Name
                    </th>
                    <th class="Width110Px">
                        Status
                    </th>
                    <th class="Width110Px">
                        Billing
                    </th>
                    <th class="Width100Px">
                        Billable
                    </th>
                    <th class="Width100Px">
                        Non-Billable
                    </th>
                    <th class="Width100Px">
                        Total
                    </th>
                    <th class="Width325Px">
                        Percent of Total Hours this Period
                    </th>
                </tr>
            </thead>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="bgcolorwhite">
            <td sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'
                class="textLeft">
                <table class="TdLevelNoBorder PeronSummaryReport">
                    <tr>
                        <td class="FirstTd">
                            <%# Eval("Client.Name") %>
                            >
                            <%# Eval("Project.Group.Name")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="SecondTd">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <%# Eval("Project.Status.Name")%>
            </td>
            <td>
                <%# Eval("BillableType")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td sorttable_customkey='<%# Eval("ProjectTotalHoursPercent")%>'>
                <table class="TdLevelNoBorder UtlizationGraph">
                    <tr>
                        <td class="Width10Percent">
                        </td>
                        <td class="PersonSummaryGraphTd GraphTd">
                            <table>
                                <tr>
                                    <td class="FirstTd" width="<%# Eval("ProjectTotalHoursPercentBillable")%>%" title="<%# Eval("ProjectTotalHoursPercentBillable")%>%">
                                    </td>
                                    <td class="ThirdTd" width="<%# Eval("ProjectTotalHoursPercentNonBillable")%>%" title="<%# Eval("ProjectTotalHoursPercentNonBillable")%>%">
                                    </td>
                                    <td class="SecondTd" width="<%# Eval("TotalHoursPercentExceptThisProject")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="PersonSummaryGraphValueTd">
                            <%# Eval("ProjectTotalHoursPercent")%>%
                        </td>
                        <td class="Width10Percent">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'
                class="textLeft">
                <table class="TdLevelNoBorder PeronSummaryReport">
                    <tr>
                        <td class="FirstTd">
                            <%# Eval("Client.Name") %>
                            >
                            <%# Eval("Project.Group.Name")%>
                        </td>
                    </tr>
                    <tr>
                        <td class="SecondTd">
                            <%# Eval("Project.ProjectNumber")%>
                            -
                            <%# Eval("Project.Name")%>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <%# Eval("Project.Status.Name")%>
            </td>
            <td>
                <%# Eval("BillableType")%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("BillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
            </td>
            <td>
                <%# GetDoubleFormat((double)Eval("TotalHours"))%>
            </td>
            <td sorttable_customkey='<%# Eval("ProjectTotalHoursPercent")%>'>
                <table class="TdLevelNoBorder UtlizationGraph">
                    <tr>
                        <td class="Width10Percent">
                        </td>
                        <td class="PersonSummaryGraphTd GraphTd">
                            <table>
                                <tr>
                                    <td class="FirstTd" width="<%# Eval("ProjectTotalHoursPercentBillable")%>%" title="<%# Eval("ProjectTotalHoursPercentBillable")%>%">
                                    </td>
                                    <td class="ThirdTd" width="<%# Eval("ProjectTotalHoursPercentNonBillable")%>%" title="<%# Eval("ProjectTotalHoursPercentNonBillable")%>%">
                                    </td>
                                    <td class="SecondTd" width="<%# Eval("TotalHoursPercentExceptThisProject")%>%">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="PersonSummaryGraphValueTd">
                            <%# Eval("ProjectTotalHoursPercent")%>%
                        </td>
                        <td class="Width10Percent">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
    This person has not entered Time Entries for the selected period.
</div>

