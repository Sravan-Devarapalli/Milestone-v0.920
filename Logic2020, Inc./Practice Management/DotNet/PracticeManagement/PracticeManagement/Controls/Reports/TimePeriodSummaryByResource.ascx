<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimePeriodSummaryByResource.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.TimePeriodSummaryByResource" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Reports/ByPerson/GroupByProject.ascx" TagName="GroupByProject"
    TagPrefix="uc" %>
<table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;
    height: 90px;">
    <tr>
        <td style="font-size: 16px; font-weight: bold;">
            <table>
                <tr>
                    <td style="vertical-align: top; padding-bottom: 10px;">
                        <asp:Literal ID="ltPersonCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="padding-top: 10px; vertical-align: bottom;">
                        <asp:Literal ID="lbRange" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </td>
        <td style="text-align: right; width: 610px; padding-bottom: 10px;">
            <table style="table-layout: fixed; width: 100%;">
                <tr>
                    <td style="width: 21%; text-align: center;">
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
                    <td style="width: 21%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Avg Hours
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlAvgHours" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 21%; text-align: center;">
                        <table width="100%">
                            <tr>
                                <td style="font-size: 15px; padding-bottom: 3px;">
                                    Avg Utilization
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 25px;">
                                    <asp:Literal ID="ltrlAvgUtilization" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 23%; vertical-align: bottom; text-align: center;">
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
                    <td style="vertical-align: bottom; width: 6%; padding: 0px !important;">
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
                    <td style="vertical-align: bottom; width: 6%; padding: 0px;">
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
        <uc:FilteredCheckBoxList ID="cblPayTypes" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterSeniority" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblSeniorities" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterOffshore" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblOffShore" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlDivision" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblDivision" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterPersonStatusType" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblPersonStatusType" runat="server" Height="155px" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repResource" runat="server" OnItemDataBound="repResource_ItemDataBound">
        <HeaderTemplate>
            <div style="min-height: 250px;">
                <table id="tblTimePeriodSummaryByResource" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th style="width: 210px; text-align: left;" class="padLeft5">
                                Resource
                                <img alt="Filter" src="../../Images/Terminated.png" style="padding-left: 2px;" runat="server"
                                    title="Person Status" id="imgPersonStatusTypeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePersonStatusTypeFilter" runat="server"
                                    TargetControlID="imgPersonStatusTypeFilter" BehaviorID="pcePersonStatusTypeFilter"
                                    PopupControlID="pnlFilterPersonStatusType" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                                <img alt="Filter" src="../../Images/Offshore_Icon.png" style="padding-left: 2px;"
                                    title="Location" runat="server" id="imgOffShoreFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceOffshoreFilter" runat="server" TargetControlID="imgOffShoreFilter"
                                    BehaviorID="pceOffshoreFilter" PopupControlID="pnlFilterOffshore" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                                <img alt="Filter" src="../../Images/divisions_16x16.png" style="padding-left: 2px;"
                                    title="Division" runat="server" id="imgDivisionFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceDivision" runat="server" TargetControlID="imgDivisionFilter"
                                    BehaviorID="pceDivision" PopupControlID="pnlDivision" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 130px;">
                                Seniority
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" style="position: absolute;
                                    padding-left: 2px;" runat="server" id="imgSeniorityFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSeniorityFilter" runat="server" TargetControlID="imgSeniorityFilter"
                                    BehaviorID="pceSeniorityFilter" PopupControlID="pnlFilterSeniority" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 110px;">
                                Pay Type
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" style="position: absolute;
                                    padding-left: 2px;" runat="server" id="imgPayTypeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePayTypeFilter" runat="server" TargetControlID="imgPayTypeFilter"
                                    BehaviorID="pcePayTypeFilter" PopupControlID="pnlFilterPayType" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th style="width: 100px">
                                Billable
                            </th>
                            <th style="width: 100px;">
                                Non-Billable
                            </th>
                            <th style="width: 100px;">
                                BD
                            </th>
                            <th style="width: 100px;">
                                Internal
                            </th>
                            <th style="width: 100px;">
                                Time-Off
                            </th>
                            <th style="width: 100px;">
                                Total
                            </th>
                            <th style="width: 295px;">
                                Utilization Percent this Period
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td valign="middle" sorttable_customkey='<%# Eval("Person.PersonLastFirstName")%>'>
                    <table class="TdLevelNoBorder">
                        <tr>
                            <td style="text-align: left; padding-left: 5px;">
                                <asp:LinkButton ID="lnkPerson" PersonId='<%# Eval("Person.Id")%>' runat="server"
                                    Font-Underline="false" ForeColor="Black" ToolTip='<%# Eval("Person.PersonLastFirstName")%>'
                                    OnClick="lnkPerson_OnClick" Text='<%# Eval("Person.PersonLastFirstName")%>'></asp:LinkButton>
                            </td>
                            <td style="text-align: left; padding-left: 5px;">
                                <asp:Image ID="imgIspersonTerminated" runat="server" ImageUrl="~/Images/Terminated.png"
                                    ToolTip="Resource is an Terminated employee." Visible='<%# (bool)IsPersonTerminated((int)Eval("Person.Status.Id"))%>' />
                                <asp:Image ID="imgOffshore" runat="server" ImageUrl="~/Images/Offshore_Icon.png"
                                    ToolTip="Resource is an offshore employee." Visible='<%# (bool)Eval("Person.IsOffshore")%>' />
                                <img id="imgZoomIn" runat="server" src="~/Images/Zoom-In-icon.png" style="visibility: hidden;" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td sorttable_customkey='<%# Eval("Person.Seniority.Name") %> <%#Eval("Person.PersonLastFirstName")%>'>
                    <%# Eval("Person.Seniority.Name")%>
                </td>
                <td sorttable_customkey='<%# GetPayTypeSortValue((string)Eval("Person.CurrentPay.TimescaleName"),(string)Eval("Person.PersonLastFirstName"))%>'>
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
                    <table class="TdLevelNoBorder" width="100%">
                        <tr>
                            <td style="width: 5%">
                            </td>
                            <td style="text-align: right; width: 70%">
                                <table style="border: 1px solid black; width: 100%; height: 18px; padding-left: 5px;">
                                    <tr>
                                        <td style="background-color: #7FD13B; height: 18px;" width="<%# Eval("Person.UtlizationPercent")%>%">
                                        </td>
                                        <td style="background-color: White; height: 18px;" width="<%# 100 - ((double)Eval("Person.UtlizationPercent") )%>%">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 20%; text-align: left; padding-left: 10px;">
                                <%# Eval("Person.UtlizationPercent")%>%
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
    <div id="divEmptyMessage" style="text-align: center; font-size: 15px; display: none;"
        runat="server">
        There are no Time Entries by any Employee for the selected range.
    </div>
</div>
<asp:HiddenField ID="hdnTempField" runat="server" />
<AjaxControlToolkit:ModalPopupExtender ID="mpePersonDetailReport" runat="server"
    TargetControlID="hdnTempField" CancelControlID="btnCancelPersonDetailReport"
    BackgroundCssClass="modalBackground" PopupControlID="pnlPersonDetailReport" DropShadow="false" />
<asp:Panel ID="pnlPersonDetailReport" Style="background-color: rgb(226, 235, 255);
    display: none;" runat="server" BorderColor="Black" BorderWidth="2px" Width="85%">
    <uc:GroupByProject ID="ucPersonDetailReport" runat="server" />
    <table style="width: 100%; padding: 5px;">
        <tr style="background-color: rgb(245, 250, 255);">
            <td style="width: 100%; text-align: right; padding: 3px;">
                <asp:Button ID="btnCancelPersonDetailReport" Text="Close" ToolTip="Close" runat="server" />
            </td>
        </tr>
    </table>
</asp:Panel>

