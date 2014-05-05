<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByBusinessUnit.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ByAccount.ByBusinessUnit" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
                <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Collapse All" UseSubmitBehavior="false"
                    CssClass="Width100Px" ToolTip="Collapse All" />
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
    <asp:Panel ID="pnlFilterBusinessUnit" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repBusinessUnit" runat="server" OnItemDataBound="repBusinessUnit_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblAccountSummaryByBusinessReport" class="tablesorter PersonSummaryReport WholeWidth zebra">
                    <thead>
                        <tr>
                            <th class="TextAlignLeftImp Width320PxImp">
                                Business Unit
                                <img alt="Filter" title="Filter" src="../../../Images/search_filter.png" class="PosAbsolute"
                                    runat="server" id="imgBusinessUnitFilter" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnitFilter" runat="server"
                                    TargetControlID="imgBusinessUnitFilter" BehaviorID="pceBusinessUnitFilter" PopupControlID="pnlFilterBusinessUnit"
                                    Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width170PxImp">
                                Account
                            </th>
                            <th class="Width170PxImp">
                                # of Active Projects
                            </th>
                            <th class="Width170PxImp">
                                # of Completed Projects
                            </th>
                            <th class="Width170PxImp">
                                Projected Hours
                            </th>
                            <th class="Width150pxImp">
                                Billable
                            </th>
                            <th class="Width150pxImp">
                                Non-Billable
                            </th>
                            <th class="Width150pxImp">
                                Actual Hours
                            </th>
                            <th class="Width100Px">
                                BD
                            </th>
                            <th class="Width150pxImp">
                                Total BU Hours
                            </th>
                            <th class="Width170PxImp">
                                Billable Hours Variance
                                <asp:Image alt="Billable Hours Variance Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                    ID="imgBillableHoursVarianceHint" CssClass="CursorPointer" ToolTip="Billable Hours Variance Calculation" />
                                <AjaxControlToolkit:ModalPopupExtender ID="mpeBillableUtilization" runat="server"
                                    TargetControlID="imgBillableHoursVarianceHint" CancelControlID="btnCancel" BehaviorID="pnlBillableUtilization"
                                    BackgroundCssClass="modalBackground" PopupControlID="pnlBillableUtilization"
                                    DropShadow="false" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="padLeft5 textLeft">
                    <%# Eval("BusinessUnit.HtmlEncodedName")%>
                    (<%# ((Boolean)Eval("BusinessUnit.IsActive")) ? "Active" : "Inactive"%>)
                </td>
                <td>
                    <asp:Label ID="lblAccount" runat="server"></asp:Label>
                </td>
                <td>
                    <%# Eval("ActiveProjectsCount")%>
                </td>
                <td>
                    <%# Eval("CompletedProjectsCount")%>
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
                    <%# GetDoubleFormat((double)Eval("ActualHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("BusinessDevelopmentHours"))%>
                </td>
                <td>
                    <%# GetDoubleFormat((double)Eval("TotalHours"))%>
                </td>
                <td sorttable_customkey='<%# Eval("BillableHoursVariance") %>'>
                    <table class="WholeWidth TdLevelNoBorder">
                        <tr>
                            <td class="Width50Percent textRightImp">
                                <%#((double)Eval("BillableHoursVariance") > 0) ? "+" + GetDoubleFormat((double)Eval("BillableHoursVariance")) : GetDoubleFormat((double)Eval("BillableHoursVariance"))%>
                            </td>
                            <td class="Width50Percent t-left">
                                <asp:Label ID="lblExclamationMark" runat="server" Visible='<%# ((double)Eval("BillableHoursVariance") < 0)%>'
                                    Text="!" CssClass="error-message fontSizeLarge" ToolTip="Project Underrun"></asp:Label>
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
        There are no projects with Active or Completed statuses for the report parameters
        selected.
    </div>
    <asp:HiddenField ID="hdncpeExtendersIds" runat="server" />
    <asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
    <asp:Repeater ID="repClientsByBusinessUnit" runat="server" OnItemDataBound="repClientsByBusinessUnit_ItemDataBound">
        <HeaderTemplate>
            <div class="border_black">
                <table class="ConsultingDemandDetails">
                    <thead>
                        <tr class="headerRow">
                            <th class="FirstTD">
                                <%--<asp:LinkButton ID="btnTitleSkill" runat="server" CausesValidation="false" CommandArgument="TitleSkill"
                                    Style="text-decoration: none; color: Black; width: 100%;" OnCommand="btnTitleSkill_Command">--%>
                                Account
                                <%--</asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%--<asp:LinkButton ID="btnSalesStage" runat="server" CausesValidation="false" CommandArgument="SalesStage"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnSalesStage_Command">--%>
                                Business Unit
                                <%--</asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%--  <asp:LinkButton ID="btnOpportunityNumber" runat="server" CausesValidation="false"
                                    CommandArgument="OpportunityNumber" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnOpportunityNumber_Command">--%>
                                # of Active projects
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%-- <asp:LinkButton ID="btnProjectNumber" runat="server" CausesValidation="false" CommandArgument="ProjectNumber"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnProjectNumber_Command">--%>
                                # of Completed projects
                                <%--</asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%-- <asp:LinkButton ID="btnAccountName" runat="server" CausesValidation="false" CommandArgument="AccountName"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnAccountName_Command">--%>
                                Projected Hours
                                <%--</asp:LinkButton>--%>
                            </th>
                            <th class="Width5Percent">
                                <%-- <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" CommandArgument="ProjectName"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnProjectName_Command">--%>
                                Billable
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width5Percent">
                                <%--   <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                    CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnResourceStartDate_Command">--%>
                                Non Billable
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%--   <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                    CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnResourceStartDate_Command">--%>
                                Actual Hours
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width5Percent">
                                <%--   <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                    CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnResourceStartDate_Command">--%>
                                BD
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%--   <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                    CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnResourceStartDate_Command">--%>
                                Total BU Hours
                                <%-- </asp:LinkButton>--%>
                            </th>
                            <th class="Width10Per">
                                <%--   <asp:LinkButton ID="btnResourceStartDate" runat="server" CausesValidation="false"
                                    CommandArgument="ResourceStartDate" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnResourceStartDate_Command">--%>
                                Billable Hours Variance
                                <%-- </asp:LinkButton>--%>
                            </th>
                        </tr>
                    </thead>
                </table>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="ConsultingDemandDetails">
                <tr class="bgColorD4D0C9 textCenter">
                    <td class="textLeft padLeft20Imp no-wrap Width16Percent">
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Account Details"
                            ExpandedText="Collapse Account Details" EnableViewState="true" BehaviorID="cpeDetails"
                            Collapsed="true" TargetControlID="pnlAccountDetails" ImageControlID="imgDetails"
                            CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                            ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                        <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                        <asp:Label ID="lblAccount" CssClass="displayNone" runat="server"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).Account.HtmlEncodedName %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BusinessUnitsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).ActiveProjectsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).CompletedProjectsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectedHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BillableHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).NonBillableHours)%>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalActualHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BusinessDevelopmentHours)%>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectHours)%>
                    </td>
                    <td class="Width10Per">
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlAccountDetails" runat="server">
                <asp:Repeater ID="repAccountDetails" runat="server">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="bgcolorwhite textCenter">
                                <td class="Width16Percent">
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessUnit.HtmlEncodedName%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActiveProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).CompletedProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActualHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessDevelopmentHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).TotalHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHoursVariance)%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="alterrow textCenter">
                                <td class="Width16Percent">
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessUnit.HtmlEncodedName%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActiveProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).CompletedProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActualHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessDevelopmentHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).TotalHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHoursVariance)%>
                                </td>
                            </tr>
                        </table>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <table class="ConsultingDemandDetails">
                <tr class="bgcolor_ECE9D9 textCenter">
                    <td class="textLeft padLeft20Imp no-wrap Width16Percent">
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Account Details"
                            ExpandedText="Collapse Account Details" EnableViewState="true" BehaviorID="cpeDetails"
                            Collapsed="true" TargetControlID="pnlAccountDetails" ImageControlID="imgDetails"
                            CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                            ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                        <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Title/Skill Details" />
                        <asp:Label ID="lblAccount" CssClass="displayNone" runat="server"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).Account.HtmlEncodedName %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BusinessUnitsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).ActiveProjectsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).CompletedProjectsCount %>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectedHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BillableHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).NonBillableHours)%>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalActualHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BusinessDevelopmentHours)%>
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectHours)%>
                    </td>
                    <td class="Width10Per">
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlAccountDetails" runat="server">
                <asp:Repeater ID="repAccountDetails" runat="server">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="bgcolorwhite textCenter">
                                <td class="Width16Percent">
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessUnit.HtmlEncodedName%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActiveProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).CompletedProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActualHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessDevelopmentHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).TotalHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHoursVariance)%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="alterrow textCenter">
                                <td class="Width16Percent">
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessUnit.HtmlEncodedName%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActiveProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).CompletedProjectsCount%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).ActualHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BusinessDevelopmentHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).TotalHours)%>
                                </td>
                                <td class="Width10Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.BusinessUnitLevelGroupedHours)Container.DataItem).BillableHoursVariance)%>
                                </td>
                            </tr>
                        </table>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</div>
<asp:Panel ID="pnlBillableUtilization" runat="server" CssClass="popUpBillableUtilization"
    Style="display: none;">
    <table>
        <tr>
            <td colspan="2" class="textCenter">
                <label class="LabelProject">
                    Billable Hours Variance
                </label>
            </td>
            <td>
                <asp:Button ID="btnCancel" runat="server" CssClass="mini-report-close floatright"
                    ToolTip="Close" Text="X"></asp:Button>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; For a time period that includes
                    today's date, the Billable Hours Variance is calculated as the number of Billable
                    Hours <b>up to and including today</b> minus the number of Projected Hours <b>up to
                        and including today</b>.</p>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <p>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For historical time periods, the
                    system calculates Billable Hours Variance as Projected Hours minus Actual Hours.</p>
            </td>
        </tr>
    </table>
</asp:Panel>

