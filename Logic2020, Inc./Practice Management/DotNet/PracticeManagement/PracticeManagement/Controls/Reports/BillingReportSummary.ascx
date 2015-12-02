<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillingReportSummary.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.BillingReportSummary" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td class="textRight Width10Percent padRight5">
                <table class="textRight WholeWidth">
                    <tr class="WholeWidth">
                        <td style="width: 585px;">
                            &nbsp;
                        </td>
                        <td id="tdLifetoDate" runat="server" style="width: 310px; font-weight: bold; text-align: center;
                            font-size: 16px;">
                            Life to date(Prior to Projected Range)
                        </td>
                        <td>
                            Export:<asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                                UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlFilterAccount" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblAccountFilter" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterPractice" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblPracticeFilter" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlSalesperson" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblSalespersonFilter" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectManager" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectManagers" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterSeniorManager" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblSeniorManager" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterDirector" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblDirectorFilter" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repBillingReport" runat="server" OnItemDataBound="repBillingReport_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblBillingReport" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th class="TextAlignLeftImp Width8Per">
                                Project Number
                            </th>
                            <th class="Width5Percent">
                                Account
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgAccountFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceAccountFilter" runat="server" TargetControlID="imgAccountFilter"
                                    BehaviorID="pceAccountFilter" PopupControlID="pnlFilterAccount" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10PerImp">
                                Project Name
                            </th>
                            <th class="Width10PerImp">
                                Practice Area
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgPracticeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePracticeFilter" runat="server" TargetControlID="imgPracticeFilter"
                                    BehaviorID="pcePracticeFilter" PopupControlID="pnlFilterPractice" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width6PercentImp bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateProjected" runat="server"></asp:Label>
                            </th>
                            <th class="Width5Percent bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateActual" runat="server"></asp:Label>
                            </th>
                            <th class="Width5Percent bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateRemaining" runat="server"></asp:Label>
                            </th>
                            <th class="Width8Per bgColorE0DAECImp">
                                Range Projected
                            </th>
                            <th class="Width5Percent bgColorE0DAECImp">
                                Range Actual
                            </th>
                            <th class="Width5Percent bgColorE0DAECImp">
                                Difference
                            </th>
                            <th class="Width6PercentImp">
                                SalesPerson
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgSalespersonFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSalespersonFilter" runat="server"
                                    TargetControlID="imgSalespersonFilter" BehaviorID="pceSalespersonFilter" PopupControlID="pnlSalesperson"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10PerImp">
                                Project Access
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgProjectManagerFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceProjectManagerFilter" runat="server"
                                    TargetControlID="imgProjectManagerFilter" BehaviorID="pceProjectManagerFilter"
                                    PopupControlID="pnlFilterProjectManager" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width9Percent">
                                Engagement Manager
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgSeniorManagerFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSeniorManagerFilter" runat="server"
                                    TargetControlID="imgSeniorManagerFilter" BehaviorID="pceSeniorManagerFilter"
                                    PopupControlID="pnlFilterSeniorManager" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10PerImp">
                                Executive in Charge
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgDirectorFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceDirectorFilter" runat="server" TargetControlID="imgDirectorFilter"
                                    BehaviorID="pceDirectorFilter" PopupControlID="pnlFilterDirector" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th>
                                PONumber
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5 textLeft">
                    <%# Eval("Project.ProjectNumber")%>
                </td>
                <td>
                    <%# Eval("Project.Client.HtmlEncodedName")%>
                </td>
                <td>
                    <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                        Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                    </asp:HyperLink>
                </td>
                <td>
                    <%# Eval("Project.Practice.HtmlEncodedName")%>
                </td>
                <td class="bgcolorE2EBFFImp" sorttable_customkey='<%# IsHoursData()?Eval("ForecastedHours"): Eval("SOWBudget.Value") %>'>
                    <asp:Label ID="lblLifetoDateProjectedValue" runat="server"></asp:Label>
                </td>
                <td class="bgcolorE2EBFFImp" sorttable_customkey='<%# IsHoursData() ? Eval("ActualHours"): Eval("ActualToDate.Value") %>'>
                    <asp:Label ID="lblLifetoDateActualValue" runat="server"></asp:Label>
                </td>
                <td class="bgcolorE2EBFFImp" sorttable_customkey='<%# IsHoursData() ? Eval("RemainingHours"):Eval("Remaining.Value") %>'>
                    <asp:Label ID="lblLifetoDateRemainingValue" runat="server"></asp:Label>
                </td>
                <td class="bgColorE0DAECImp" sorttable_customkey='<%# IsHoursData() ?Eval("ForecastedHoursInRange"):Eval("RangeProjected.Value")  %>'>
                    <asp:Label ID="lblRangeProjectedValue" runat="server"></asp:Label>
                </td>
                <td class="bgColorE0DAECImp" sorttable_customkey='<%# IsHoursData() ?Eval("ActualHoursInRange") :Eval("RangeActual.Value")%>'>
                    <asp:Label ID="lblRangeActual" runat="server"></asp:Label>
                </td>
                <td class="bgColorE0DAECImp" sorttable_customkey='<%# IsHoursData() ?Eval("DifferenceInHours") : Eval("DifferenceInCurrency.Value")%>'>
                    <asp:Label ID="lblRangeDifference" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Project.SalesPersonName")%>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Project.SeniorManagerName")%>
                </td>
                <td>
                    <asp:Label ID="lblDirector" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("Project.PONumber")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <thead>
                <tr>
                    <th class="TextAlignLeftImp fontBold">
                        Total
                    </th>
                    <th colspan="3">
                    </th>
                    <th class="bgcolorE2EBFFImp">
                        <asp:Label ID="lblTotalLifetoDateProjectedValue" runat="server"></asp:Label>
                    </th>
                    <th class="bgcolorE2EBFFImp">
                        <asp:Label ID="lblTotalLifetoDateActualValue" runat="server"></asp:Label>
                    </th>
                    <th class="bgcolorE2EBFFImp">
                        <asp:Label ID="lblTotalLifetoDateRemainingValue" runat="server"></asp:Label>
                    </th>
                    <th class="bgColorE0DAECImp">
                        <asp:Label ID="lblTotalRangeProjectedValue" runat="server"></asp:Label>
                    </th>
                    <th class="bgColorE0DAECImp">
                        <asp:Label ID="lblTotalRangeActual" runat="server"></asp:Label>
                    </th>
                    <th class="bgColorE0DAECImp">
                        <asp:Label ID="lblTotalRangeDifference" runat="server"></asp:Label>
                    </th>
                    <th colspan="5">
                    </th>
                </tr>
            </thead>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no projects with Active or Projected or Proposed statuses for the report
        parameters selected.
    </div>
</div>

