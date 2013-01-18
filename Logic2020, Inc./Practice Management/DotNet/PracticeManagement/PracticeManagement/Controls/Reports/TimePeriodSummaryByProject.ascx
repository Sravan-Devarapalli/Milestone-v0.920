<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessDevelopment.ascx" TagName="GroupByBusinessDevelopment"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ProjectDetailTabByResource.ascx" TagName="ProjectDetailTabByResource"
    TagPrefix="uc" %>
<table class="PaddingTenPx TimePeriodSummaryReportHeader">
    <tr>
        <td class="font16Px fontBold">
            <table>
                <tr>
                    <td class="vtop PaddingBottom10Imp">
                        <asp:Literal ID="ltProjectCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="PaddingTop10Imp vBottom">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td class="TimePeriodTotals ByProject">
            <table class="tableFixed WholeWidth">
                <tr>
                    <td class="Width27Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Total Actual Hours
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width27Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Avg Hours
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Literal ID="ltrlAvgHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width27Percent vBottom">
                        <table class="ReportHeaderBillAndNonBillTable">
                            <tr>
                                <td>
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td class="billingHours">
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
                    <td class="ReportHeaderBandNBGraph">
                        <table>
                            <tr>
                                <td>
                                    <table class="tableFixed">
                                        <tr>
                                            <td>
                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                            <td class="billingGraph">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="ReportHeaderBandNBGraph">
                        <table>
                            <tr>
                                <td>
                                    <table class="tableFixed">
                                        <tr>
                                            <td>
                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                            </td>
                                        </tr>
                                    </table>
                                    <table>
                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                            <td class="nonBillingGraph">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width2Percent">
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
            </td>
            <td class="textRight Width10Percent padRight5">
                <table class="textRight WholeWidth">
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
    <asp:Panel ID="pnlFilterResource" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblClients" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblProjectSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="ProjectColoum">
                                Project
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                    id="imgClientFilter" class="FilterImg" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceClient" runat="server" TargetControlID="imgClientFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width110Px">
                                Status
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" class="FilterImg" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width110Px">
                                Billing
                            </th>
                            <th class="Width125Px">
                                <asp:Label ID="lblProjectedHours" runat="server" Text="Projected Hours"></asp:Label>
                            </th>
                            <th class="Width100Px">
                                <asp:Label ID="lblBillable" runat="server" Text="Billable"></asp:Label>
                            </th>
                            <th class="Width100Px">
                                <asp:Label ID="lblNonBillable" runat="server" Text="Non-Billable"></asp:Label>
                            </th>
                            <th class="Width100Px">
                                <asp:Label ID="lblActualHours" runat="server" Text="Actual Hours"></asp:Label>
                            </th>
                            <th class="Width200Px">
                                <asp:Label ID="lblBillableHoursVariance" runat="server" Text="Billable Hours Variance"></asp:Label>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'>
                    <table class="TdLevelNoBorder PeronSummaryReport">
                        <tr>
                            <td class="FirstTd">
                                <%# Eval("Project.Client.HtmlEncodedName")%>
                                >
                                <%# Eval("Project.Group.HtmlEncodedName")%>
                            </td>
                            <td rowspan="2" class="ThirdTd">
                                <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="display: none;" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SecondTd">
                                <asp:LinkButton ID="lnkProject" AccountId='<%# Eval("Project.Client.Id")%>' GroupId='<%# Eval("Project.Group.Id")%>'
                                    ClientName=' <%# Eval("Project.Client.HtmlEncodedName")%>' GroupName=' <%# Eval("Project.Group.HtmlEncodedName")%>'
                                    ProjectNumber='<%# Eval("Project.ProjectNumber")%>' runat="server" ToolTip='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'
                                    OnClick="lnkProject_OnClick" Text='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.HtmlEncodedName"))%>'></asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="textCenter" sorttable_customkey='<%# Eval("Project.Status.Name") %><%#Eval("Project.ProjectNumber")%>'>
                    <%# Eval("Project.Status.Name")%>
                </td>
                <td>
                    <%# Eval("BillingType")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("ForecastedHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                </td>
                <td>
                    <asp:LinkButton ID="lnkActualHours" runat="server" ToolTip='<%# GetDoubleFormat((double)Eval("TotalHours"))%>'
                        OnClientClick="OpenUrlInNewWindow(this);return false;" Text='<%# GetDoubleFormat((double)Eval("TotalHours"))%>'></asp:LinkButton>
                </td>
                <td sorttable_customkey='<%# Eval("BillableHoursVariance") %>'>
                    <table class="WholeWidth TdLevelNoBorder">
                        <tr>
                            <td class="Width50Percent textRightImp">
                                <%# ((double)Eval("BillableHoursVariance") > 0) ? "+" + GetDoubleFormat((double)Eval("BillableHoursVariance")) : GetDoubleFormat((double)Eval("BillableHoursVariance")) %>
                            </td>
                            <td class="Width50Percent t-left">
                                <asp:Label ID="lblExclamationMark" runat="server" Visible='<%# ((double)Eval("BillableHoursVariance") < 0) %>'
                                    Text="!" CssClass="error-message fontSizeLarge t-left" ToolTip="Project Underrun"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
    <br />
    <div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeProjectDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelProjectDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlProjectDetailReport"
    DropShadow="false" />
<asp:Panel ID="pnlProjectDetailReport" class="" Style="display: none;" runat="server"
    CssClass="TimePeriodByProject_ProjectDetailReport">
    <table class="WholeWidth Padding5">
        <tr>
            <td class="WholeWidth">
                <table class="WholeWidthWithHeight">
                    <tr class="bgColor_F5FAFF">
                        <td class="TimePeriodByProject_ProjectName">
                            <asp:Literal ID="ltrlProject" runat="server"></asp:Literal>
                        </td>
                        <td class="TimePeriodByProject_ProjectTotalHours">
                            <asp:Literal ID="ltrlProjectDetailTotalhours" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="WholeWidth">
                            <div class="TimePeriodByProject_ProjectDetailsDiv">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="Width97Percent">
                                            <uc:GroupByBusinessDevelopment ID="ucGroupByBusinessDevelopment" runat="server" Visible="false" />
                                            <uc:ProjectDetailTabByResource ID="ucProjectDetailReport" runat="server" Visible="false" />
                                        </td>
                                        <td class="Width3Percent">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr class="bgColor_F5FAFF">
                        <td colspan="2" class="WholeWidth textRight Padding3PX">
                            <asp:Button ID="btnCancelProjectDetailReport" Text="Close" ToolTip="Close" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTotalProjectedHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Projected Hours:
    </label>
    <asp:Label ID="lblTotalProjectedHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Billable:
    </label>
    <asp:Label ID="lblTotalBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalNonBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Non-Billable:
    </label>
    <asp:Label ID="lblTotalNonBillableHours" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalActualHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <table>
        <tr>
            <td class="fontBold">
                Total Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total Non-Billable:
            </td>
            <td>
                <asp:Label ID="lblTotalNonBillablePanlActual" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold padRight15">
                Total Actual Hours:
            </td>
            <td>
                <asp:Label ID="lblTotalActualHours" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlBillableHoursVariance" Style="display: none;" runat="server" CssClass="pnlTotal Width170PxImp">
    <label class="fontBold">
        Total Billable Hours Variance:
    </label>
    <br />
    <asp:Label ID="lblTotalBillableHoursVariance" runat="server"></asp:Label>
    <asp:Label ID="lblExclamationMarkPanl" runat="server" Visible="false" Text="!" CssClass="error-message fontSizeLarge t-left"
        ToolTip="Project Underrun"></asp:Label>
</asp:Panel>

