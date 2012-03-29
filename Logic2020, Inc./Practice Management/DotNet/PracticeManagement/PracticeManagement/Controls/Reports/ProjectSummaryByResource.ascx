<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryByResource" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;
    height: 90px;" id="tbHeader" runat="server">
    <tr>
        <td style="font-size: 14px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; color: Gray; padding-bottom: 5px;">
                        <asp:Literal ID="ltrlAccount" runat="server"></asp:Literal>
                        >
                        <asp:Literal ID="ltrlBusinessUnit" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectNumber" runat="server"></asp:Literal>-
                        <asp:Literal ID="ltrlProjectName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectStatus" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 470px; padding-bottom: 10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 27%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Projected Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlProjectedHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Total Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%; vertical-align: bottom; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td>
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px;">
                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    NON-BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 8%; padding: 0px !important;">
                        <table width="100%">
                            <tr>
                                <td style="padding: 0px !important;">
                                    <table width="100%" style="table-layout: fixed;">
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                            <td style="background-color: #7FD13B; border: 1px solid Gray;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: bottom; width: 8%; padding: 0px;">
                        <table width="100%">
                            <tr>
                                <td style="padding: 0px !important;">
                                    <table width="100%" style="table-layout: fixed;">
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%">
                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                            <td style="background-color: #BEBEBE; border: 1px solid Gray;">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 2%;">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div class="tab-pane WholeWidth">
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
                                UseSubmitBehavior="false" ToolTip="Export To PDF" />
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
                        <th class="t-left padLeft5" style="width: 300px;">
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
</div>

