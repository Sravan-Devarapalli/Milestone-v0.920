<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryTabByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryTabByResource" %>
<table class="WholeWidthWithHeight">
    <tr>
        <td colspan="4" style="width: 90%;">
        </td>
        <td style="text-align: right; width: 10%; padding-right: 5px;">
            <table width="100%" style="text-align: right;">
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
<asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
    <HeaderTemplate>
        <table id="tblProjectSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth">
            <thead>
                <tr>
                    <th class="t-left padLeft5" style="width: 300px; background-color: White;">
                        Resource
                    </th>
                    <th style="width: 140px;">
                    </th>
                    <th style="width: 130px;">
                        Project Role
                    </th>
                    <th style="width: 100px;">
                        Billable
                    </th>
                    <th style="width: 100px;">
                        Non-Billable
                    </th>
                    <th style="width: 100px;">
                        Total
                    </th>
                    <th style="width: 140px;">
                    </th>
                    <th style="width: 325px;">
                        Project Variance (in Hours)
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="ReportItemTemplate">
            <td class="t-left padLeft5">
                <%# Eval("Person.PersonLastFirstName")%>
            </td>
            <td>
            </td>
            <td class="t-center padLeft5">
                <%# Eval("Person.ProjectRoleName")%>
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
            <td>
            </td>
            <td>
                <table class="WholeWidth  TdLevelNoBorder">
                    <tr>
                        <td style="width: 75%;">
                            <table class="WholeWidth">
                                <tr style="border: 1px solid black;">
                                    <td style="width: 50%; border-right: 1px solid black;">
                                        <table width="100%">
                                            <tr>
                                                <td style="<%# Eval("BillableFirstHalfHtmlStyle")%>">
                                                </td>
                                                <td style="<%# Eval("BillableSecondHalfHtmlStyle")%>">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 50%; border-left: 1px solid black;">
                                        <table width="100%">
                                            <tr>
                                                <td style="<%# Eval("ForecastedFirstHalfHtmlStyle")%>">
                                                </td>
                                                <td style="<%# Eval("ForecastedSecondHalfHtmlStyle")%>">
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 25%;">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="text-align: right; padding-right: 3px;">
                                        <%# Eval("Variance")%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>
<br />
<div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
    runat="server">
    There are no Time Entries towards this project.
</div>

