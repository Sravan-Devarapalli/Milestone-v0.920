<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByProject.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessDevelopment.ascx" TagName="GroupByBusinessDevelopment"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ProjectDetailTabByResource.ascx" TagName="ProjectDetailTabByResource"
    TagPrefix="uc" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;">
    <tr>
        <td style="font-size: 16px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; padding-bottom: 10px;">
                        <asp:Literal ID="ltProjectCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; vertical-align: bottom;">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 470px; padding-bottom: 10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 27%;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Total Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                    Avg Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px; text-align: center;">
                                    <asp:Literal ID="ltrlAvgHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 27%; vertical-align: bottom;">
                        <table width="100%">
                            <tr>
                                <td style="text-align: center;">
                                    BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-bottom: 5px; text-align: center;">
                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center;">
                                    NON-BILLABLE
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center;">
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
<div class="tab-pane">
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
    <asp:Panel ID="pnlFilterResource" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblClients" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" Height="125px" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
        <HeaderTemplate>
            <div style="min-height: 250px;">
                <table id="tblProjectSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="t-left padLeft5" style="width: 500px; height: 20px;">
                                Project
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                    id="imgClientFilter" style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceClient" runat="server" TargetControlID="imgClientFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 110px; height: 20px;">
                                Status
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" style="position: absolute; padding-left: 2px;" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 110px; height: 20px;">
                                Billing
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Billable
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Non-Billable
                            </th>
                            <th style="width: 100px; height: 20px;">
                                Total
                            </th>
                            <th style="width: 325px; height: 20px;">
                                Project Variance (in Hours)
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'>
                    <table class="TdLevelNoBorder">
                        <tr>
                            <td style="color: Gray; padding-bottom: 3px; padding-left: 2px; text-align: left;">
                                <%# Eval("Project.Client.Name")%>
                                >
                                <%# Eval("Project.Group.Name")%>
                            </td>
                            <td rowspan="2" style="padding-left: 3px;" valign="middle">
                                <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="display: none;" />
                            </td>
                        </tr>
                        <tr>
                            <td style="font-weight: bold; padding-bottom: 5px; padding-left: 2px; text-align: left;">
                                <asp:LinkButton ID="lnkProject" AccountId='<%# Eval("Project.Client.Id")%>' GroupId='<%# Eval("Project.Group.Id")%>'
                                    Font-Underline="false" ForeColor="Black" ClientName=' <%# Eval("Project.Client.Name")%>'
                                    GroupName=' <%# Eval("Project.Group.Name")%>' ProjectNumber='<%# Eval("Project.ProjectNumber")%>'
                                    runat="server" ToolTip='<%# GetProjectName((string)Eval("Project.ProjectNumber"),(string)Eval("Project.Name"))%>'
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
                    <table class="WholeWidth  TdLevelNoBorder">
                        <tr>
                            <td style="width: 5%;">
                            </td>
                            <td style="width: 70%; text-align: right;">
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
                            <td style="width: 20%;">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="text-align: right; padding-right: 3px;">
                                            <%# Eval("Variance")%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 5%;">
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
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Time Entries towards this range selected.
    </div>
</div>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpeProjectDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelProjectDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlProjectDetailReport"
    DropShadow="false" />
<asp:Panel ID="pnlProjectDetailReport" Style="background-color: rgb(226, 235, 255);
    display: none;" runat="server" BorderColor="Black" BorderWidth="2px" Width="85%">
    <table style="width: 100%; padding: 5px;">
        <tr>
            <td style="width: 100%;">
                <table class="WholeWidthWithHeight">
                    <tr style="background-color: rgb(245, 250, 255);">
                        <td style="width: 90%; font-weight: bold; font-size: 15px; padding: 3px; padding-left: 10px;">
                            <asp:Literal ID="ltrlProject" runat="server"></asp:Literal>
                        </td>
                        <td style="width: 10%; font-weight: bold; text-align: right; font-size: 15px; padding: 3px;">
                            <asp:Literal ID="ltrlProjectDetailTotalhours" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width: 100%;">
                            <div style="max-height: 500px; overflow-y: auto; width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 97%;">
                                            <uc:GroupByBusinessDevelopment ID="ucGroupByBusinessDevelopment" runat="server" Visible="false" />
                                            <uc:ProjectDetailTabByResource ID="ucProjectDetailReport" runat="server" Visible="false" />
                                        </td>
                                        <td style="width: 3%;">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr style="background-color: rgb(245, 250, 255);">
                        <td colspan="2" style="width: 100%; text-align: right; padding: 3px;">
                            <asp:Button ID="btnCancelProjectDetailReport" Text="Close" ToolTip="Close" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Panel>

