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
                        <td id="tdLifetoDate" runat="server" style="width:310px; font-weight: bold;text-align:center;font-size:16px;">
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
                            <th class="TextAlignLeftImp Width180Px">
                                Project Number
                            </th>
                            <th class="Width170PxImp">
                                Account
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgAccountFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceAccountFilter" runat="server" TargetControlID="imgAccountFilter"
                                    BehaviorID="pceAccountFilter" PopupControlID="pnlFilterAccount" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width170PxImp">
                                Project Name
                            </th>
                            <th class="Width170PxImp">
                                Practice Area
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgPracticeFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pcePracticeFilter" runat="server" TargetControlID="imgPracticeFilter"
                                    BehaviorID="pcePracticeFilter" PopupControlID="pnlFilterPractice" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width170PxImp">
                                Type
                            </th>
                            <th class="Width150pxImp bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateProjected" runat="server"></asp:Label>
                            </th>
                            <th class="Width150pxImp bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateActual" runat="server"></asp:Label>
                            </th>
                            <th class="Width150pxImp bgcolorE2EBFFImp">
                                <asp:Label ID="lblLifetoDateRemaining" runat="server"></asp:Label>
                            </th>
                            <th class="Width100Px">
                                Range Projected
                            </th>
                            <th class="Width150pxImp">
                                Range Actual
                            </th>
                            <th class="Width170PxImp">
                                Difference
                            </th>
                            <th class="Width170PxImp">
                                SalesPerson
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgSalespersonFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSalespersonFilter" runat="server"
                                    TargetControlID="imgSalespersonFilter" BehaviorID="pceSalespersonFilter" PopupControlID="pnlSalesperson"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width170PxImp">
                                Project Manager(s)
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgProjectManagerFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceProjectManagerFilter" runat="server"
                                    TargetControlID="imgProjectManagerFilter" BehaviorID="pceProjectManagerFilter"
                                    PopupControlID="pnlFilterProjectManager" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width180Px">
                                Senior Manager
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgSeniorManagerFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceSeniorManagerFilter" runat="server"
                                    TargetControlID="imgSeniorManagerFilter" BehaviorID="pceSeniorManagerFilter"
                                    PopupControlID="pnlFilterSeniorManager" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width170PxImp">
                                Director
                                <img alt="Filter" title="Filter" src="../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgDirectorFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceDirectorFilter" runat="server" TargetControlID="imgDirectorFilter"
                                    BehaviorID="pceDirectorFilter" PopupControlID="pnlFilterDirector" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width150pxImp">
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
                <td>
                    Revenue
                </td>
                <td class="bgcolorE2EBFFImp">
                    <asp:Label ID="lblLifetoDateProjectedValue" runat="server"></asp:Label>
                </td>
                <td class="bgcolorE2EBFFImp">
                    <asp:Label ID="lblLifetoDateActualValue" runat="server"></asp:Label>
                </td>
                <td class="bgcolorE2EBFFImp">
                    <asp:Label ID="lblLifetoDateRemainingValue" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("RangeProjected")%>
                </td>
                <td>
                    <%# Eval("RangeActual")%>
                </td>
                <td>
                    <%# Eval("Difference")%>
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
                    <%# Eval("Project.Director.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Project.PONumber")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody></table></div>
        </FooterTemplate>
    </asp:Repeater>
    <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
        There are no projects with Active or Projected or Proposed statuses for the report
        parameters selected.
    </div>
</div>

