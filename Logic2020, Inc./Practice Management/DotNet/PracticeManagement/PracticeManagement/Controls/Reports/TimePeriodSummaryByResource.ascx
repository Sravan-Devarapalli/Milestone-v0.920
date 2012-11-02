<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ByPerson/GroupByProject.ascx" TagName="GroupByProject"
    TagPrefix="uc" %>
<script type="text/javascript">
    function ShowPanel(object, displaypnl) {

        var obj = $("#" + object);
        var displayPanel = $("#" + displaypnl);
        iptop = obj.offset().top + obj[0].offsetHeight;
        ipleft = obj.offset().left;
        displayPanel.offset({ top: iptop, left: ipleft });
        displayPanel.show();
        displayPanel.offset({ top: iptop, left: ipleft });
    }

    function HidePanel(hiddenpnl) {

        var displayPanel = $("#" + hiddenpnl);
        displayPanel.hide();
    }
   
</script>
<table class="PaddingTenPx TimePeriodSummaryReportHeader">
    <tr>
        <td class="font16Px fontBold">
            <table>
                <tr>
                    <td class="vtop PaddingBottom10Imp">
                        <asp:Literal ID="ltPersonCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="PaddingTop10Imp vBottom">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td class="TimePeriodTotals Width740px">
            <table class="tableFixed WholeWidth">
                <tr>
                    <td class="Width17Percent">
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
                    <td class="Width17Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Total Time-Off
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Label ID="ltrlTotalTimeoffHours" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width17Percent">
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
                    <td class="Width17Percent">
                        <table class="ReportHeaderTotalsTable">
                            <tr>
                                <td class="FirstTd">
                                    Avg Utilization
                                </td>
                            </tr>
                            <tr>
                                <td class="SecondTd">
                                    <asp:Literal ID="ltrlAvgUtilization" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width20Percent vBottom">
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
                    <td class="ReportHeaderBandNBGraph Width5PercentImp">
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
                    <td class="ReportHeaderBandNBGraph Width5PercentImp">
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
            <td class=" Width10Percent padRight5">
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
                        <td>
                            <asp:Button ID="btnPayCheckExport" runat="server" Text="PayChex" OnClick="btnPayCheckExport_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export PayChex" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlFilterPayType" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblPayTypes" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterSeniority" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblSeniorities" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterOffshore" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblOffShore" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlDivision" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblDivision" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterPersonStatusType" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblPersonStatusType" runat="server" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblTimePeriodSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr class="TimeperiodSummaryReportTr">
                            <th class="ResourceColum">
                                Resource
                                <img alt="Filter" src="../../Images/Terminated.png" runat="server" title="Person Status"
                                    id="imgPersonStatusTypeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePersonStatusTypeFilter" runat="server"
                                    TargetControlID="imgPersonStatusTypeFilter" BehaviorID="pcePersonStatusTypeFilter"
                                    PopupControlID="pnlFilterPersonStatusType" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                                <img alt="Filter" src="../../Images/Offshore_Icon.png" title="Location" runat="server"
                                    id="imgOffShoreFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceOffshoreFilter" runat="server" TargetControlID="imgOffShoreFilter"
                                    BehaviorID="pceOffshoreFilter" PopupControlID="pnlFilterOffshore" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                                <img alt="Filter" src="../../Images/divisions_16x16.png" title="Division" runat="server"
                                    id="imgDivisionFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceDivision" runat="server" TargetControlID="imgDivisionFilter"
                                    BehaviorID="pceDivision" PopupControlID="pnlDivision" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width130pxImp">
                                Seniority
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="FilterImg"
                                    runat="server" id="imgSeniorityFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSeniorityFilter" runat="server" TargetControlID="imgSeniorityFilter"
                                    BehaviorID="pceSeniorityFilter" PopupControlID="pnlFilterSeniority" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width110PxImp">
                                Pay Type
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="FilterImg"
                                    runat="server" id="imgPayTypeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePayTypeFilter" runat="server" TargetControlID="imgPayTypeFilter"
                                    BehaviorID="pcePayTypeFilter" PopupControlID="pnlFilterPayType" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th>
                                <asp:Label ID="lblTotalBillable" runat="server" Text="Billable"></asp:Label>
                            </th>
                            <th class="no-wrap">
                                <asp:Label ID="lblTotalNonBillable" runat="server" Text="Non-Billable"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="lblTotalBD" runat="server" Text="BD"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="lblTotalInternal" runat="server" Text="Internal"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="lblTotalTimeOff" runat="server" Text="Time-Off"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="lblTotalHours" runat="server" Text="Total"></asp:Label>
                            </th>
                            <th class="Width295PxImp">
                                Utilization Percent this Period
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td sorttable_customkey='<%# Eval("Person.HtmlEncodedName")%>'>
                    <table class="TdLevelNoBorder ResourceColumTable">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkPerson" PersonId='<%# Eval("Person.Id")%>' runat="server"
                                    ToolTip='<%# Eval("Person.HtmlEncodedName")%>' OnClick="lnkPerson_OnClick" Text='<%# Eval("Person.HtmlEncodedName")%>'></asp:LinkButton>
                            </td>
                            <td>
                                <asp:Image ID="imgIspersonTerminated" runat="server" ImageUrl="~/Images/Terminated.png"
                                    ToolTip="Resource is an Terminated employee." Visible='<%# (bool)IsPersonTerminated((int)Eval("Person.Status.Id"))%>' />
                                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                                    ToolTip="Resource is an offshore employee." Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
                                <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="visibility: hidden;" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td sorttable_customkey='<%# Eval("Person.Seniority.Name") %> <%#Eval("Person.HtmlEncodedName")%>'>
                    <%# Eval("Person.Seniority.Name")%>
                </td>
                <td sorttable_customkey='<%# GetPayTypeSortValue((string)Eval("Person.CurrentPay.TimescaleName"),(string)Eval("Person.HtmlEncodedName"))%>'>
                    <%# Eval("Person.CurrentPay.TimescaleName")%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("ProjectNonBillableHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BusinessDevelopmentHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("InternalHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("AdminstrativeHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td sorttable_customkey='<%# Eval("Person.UtlizationPercent")%>'>
                    <table class="TdLevelNoBorder UtlizationGraph">
                        <tr>
                            <td class="Width5Percent">
                            </td>
                            <td class="GraphTd">
                                <table>
                                    <tr>
                                        <td class="FirstTd" width="<%# Eval("Person.UtlizationPercent")%>%">
                                        </td>
                                        <td class="SecondTd" width="<%# 100 - ((double)Eval("Person.UtlizationPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="GraphValueTd">
                                <%# Eval("Person.UtlizationPercent")%>%
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
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no Time Entries by any Employee for the selected range.
    </div>
</div>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpePersonDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelPersonDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlPersonDetailReport" DropShadow="false" />
<asp:Panel ID="pnlPersonDetailReport" CssClass="TimePeriodSummaryPersonDetailReportPopUp"
    Style="display: none;" runat="server">
    <uc:GroupByProject ID="ucPersonDetailReport" runat="server" />
    <table class="CloseButtonTable">
        <tr>
            <td>
                <asp:Button ID="btnCancelPersonDetailReport" Text="Close" ToolTip="Close" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTotalTimeOff" Style="display: none;" runat="server" CssClass="pnlTotal zindex2001Imp">
    <table>
        <tr>
            <td class="fontBold">
                PTO:
            </td>
            <td>
                <asp:Label ID="lblPtoHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Holiday:
            </td>
            <td>
                <asp:Label ID="lblHolidayHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Bereavement:
            </td>
            <td>
                <asp:Label ID="lblBereavementHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                JuryDuty:
            </td>
            <td>
                <asp:Label ID="lblJuryDutyHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                ORT:
            </td>
            <td>
                <asp:Label ID="lblOrtHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Unpaid:
            </td>
            <td>
                <asp:Label ID="lblUnpaidHours" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold padRight15">
                Sick/Safe Leave:
            </td>
            <td>
                <asp:Label ID="lblSickOrSafeLeaveHours" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlTotalBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Billable:
    </label>
    <asp:Label ID="lblBillable" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalNonBillableHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Non-Billable:
    </label>
    <asp:Label ID="lblNonBillable" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalBD" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total BD:
    </label>
    <asp:Label ID="lblBD" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalInternalHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Internal:
    </label>
    <asp:Label ID="lblInternal" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalTimeOffHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <label class="fontBold">
        Total Time-Off:
    </label>
    <asp:Label ID="lblTimeOff" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlTotalHours" Style="display: none;" runat="server" CssClass="pnlTotal">
    <table>
        <tr>
            <td class="fontBold">
                Total Billable:
            </td>
            <td>
                <asp:Label ID="pthLblBillable" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold padRight15">
                Total Non-Billable:
            </td>
            <td>
                <asp:Label ID="pthLblNonBillable" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total BD:
            </td>
            <td>
                <asp:Label ID="pthLblBD" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total Internal:
            </td>
            <td>
                <asp:Label ID="pthLblInternal" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Total Time-Off:
            </td>
            <td>
                <asp:Label ID="pthLblTimeOff" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="fontBold">
                Grand Total:
            </td>
            <td>
                <asp:Label ID="pthLblGrandTotal" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>

