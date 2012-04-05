<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectSummaryByResource" %>
    <%@ Register Src="~/Controls/Reports/ProjectDetailTabByResource.ascx" TagPrefix="uc" TagName="ProjectDetailReport" %>
     <%@ Register Src="~/Controls/Reports/ProjectSummaryTabByResource.ascx" TagPrefix="uc" TagName="ProjectSummaryReport" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;
    height: 115px;" id="tbHeader" runat="server">
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
                 <tr>
                    <td style="padding-top: 5px; vertical-align: bottom;">
                        <asp:Literal ID="ltrlProjectRange" runat="server"></asp:Literal>
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
                                    <asp:Literal ID="ltrlTotalHours" runat="server" ></asp:Literal>
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
<div class="WholeWidth">
    <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CustomTabStyle">
        <asp:TableRow ID="rowSwitcher" runat="server">
            <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                <span class="bg"><span>
                    <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                        OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                </span>
            </asp:TableCell>
            <asp:TableCell ID="cellDetail" runat="server">
                <span class="bg"><span>
                    <asp:LinkButton ID="lnkbtnDetail" runat="server" Text="Detail" CausesValidation="false"
                        OnCommand="btnView_Command" CommandArgument="1" ToolTip="Detail"></asp:LinkButton></span>
                </span>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:MultiView ID="mvProjectReport" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwProjectSummaryReport" runat="server">
            <asp:Panel ID="pnlProjectSummaryReport" runat="server" CssClass="tab-pane">
              <uc:ProjectSummaryReport id="ucProjectSummaryReport" runat="server" />
            </asp:Panel>
        </asp:View>
        <asp:View ID="vwProjectDetailReport" runat="server">
            <asp:Panel ID="pnlProjectDetailReport" runat="server" CssClass="tab-pane">
                <uc:ProjectDetailReport id="ucProjectDetailReport" runat="server" />
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</div>

