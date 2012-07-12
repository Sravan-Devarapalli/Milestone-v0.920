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
                                    Total Hours
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
                                Project Variance (in Hours)
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
                                <%# Eval("Project.Client.Name")%>
                                >
                                <%# Eval("Project.Group.Name")%>
                            </td>
                            <td rowspan="2" class="ThirdTd">
                                <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="display: none;" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SecondTd">
                                <asp:LinkButton ID="lnkProject" AccountId='<%# Eval("Project.Client.Id")%>' GroupId='<%# Eval("Project.Group.Id")%>'
                                    ClientName=' <%# Eval("Project.Client.Name")%>' GroupName=' <%# Eval("Project.Group.Name")%>'
                                    ProjectNumber='<%# Eval("Project.ProjectNumber")%>' runat="server" ToolTip='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.Name"))%>'
                                    OnClick="lnkProject_OnClick" Text='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.Name"))%>'></asp:LinkButton>
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
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("NonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td sorttable_customkey='<%# GetVarianceSortValue((string)Eval("Variance"))%>'>
                    <table class="WholeWidth TdLevelNoBorder">
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="Width70Per textRight">
                                <table class="WholeWidth">
                                    <tr class="border1Px">
                                        <td class="Width50Percent borderRightImp">
                                            <table class="WholeWidth">
                                                <tr>
                                                    <td style="<%# Eval("BillableFirstHalfHtmlStyle")%>">
                                                    </td>
                                                    <td style="<%# Eval("BillableSecondHalfHtmlStyle")%>">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="Width50Percent borderLeft">
                                            <table class="WholeWidth">
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
                            <td class="Width20Percent">
                                <table class="WholeWidth">
                                    <tr>
                                        <td class="TimePeriodByProjectVariance">
                                            <%# Eval("Variance")%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="Width5Percent">
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

