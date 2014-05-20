<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ByProject.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ByAccount.ByProject" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
<div class="tab-pane">
    <table class="WholeWidthWithHeight">
        <tr>
            <td colspan="4" class="Width90Percent">
                <asp:Button ID="btnExpandOrCollapseAll" runat="server" Text="Expand All" UseSubmitBehavior="false"
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
    <asp:Panel ID="pnlFilterResource" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBusinessUnits" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlBilling" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblBilling" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repProject" runat="server" OnItemDataBound="repProject_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblAccountSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="ProjectColoum">
                                Project
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBusinessUnitFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnit" runat="server" TargetControlID="imgBusinessUnitFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th>
                                Account
                            </th>
                            <th class="Width140pxImp">
                                Status
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width140pxImp">
                                Billing Type
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBilling" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBilling" runat="server" TargetControlID="imgBilling"
                                    PopupControlID="pnlBilling" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width140pxImp">
                                Projected Hours
                            </th>
                            <th class="Width140pxImp">
                                Billable
                            </th>
                            <th class="Width130pxImp">
                                Non-Billable
                            </th>
                            <th class="Width140pxImp">
                                Actual Hours
                            </th>
                            <th class="Width170PxImp">
                                Total Estimated Billings
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
                <td class="t-left padLeft5" sorttable_customkey='<%# Eval("Project.TimeEntrySectionId")%><%# Eval("Project.ProjectNumber")%>'>
                    <table class="TdLevelNoBorder PeronSummaryReport">
                        <tr>
                            <td class="FirstTd">
                                <%# Eval("Project.Client.HtmlEncodedName")%>
                                >
                                <%# Eval("Project.Group.HtmlEncodedName")%>
                            </td>
                        </tr>
                        <tr>
                            <td class="SecondTd">
                                <%# Eval("Project.ProjectNumber")%>
                                -
                                <asp:Label ID="lblProjectName" runat="server" Visible="false" Text=' <%# Eval("Project.HtmlEncodedName")%> '></asp:Label>
                                <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                                    Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                </asp:HyperLink>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <%# Eval("Project.Client.HtmlEncodedName")%>
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
                    <asp:Label ID="lblActualHours" runat="server" Visible="false" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '></asp:Label>
                    <asp:HyperLink ID="hlActualHours" runat="server" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '
                        Target="_blank" NavigateUrl='<%# GetReportByProjectLink((string)Eval("Project.ProjectNumber"))%>'>
                    </asp:HyperLink>
                </td>
                <td>
                    <asp:Label ID="lblEstimatedBillings" runat="server"></asp:Label>
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
    <div id="divEmptyMessage" style="display: none;" class="EmptyMessagediv" runat="server">
        There are no projects with Active or Completed statuses for the report parameters
        selected.
    </div>
    <asp:HiddenField ID="hdncpeExtendersIds" runat="server" />
    <asp:HiddenField ID="hdnCollapsed" runat="server" Value="true" />
    <asp:Repeater ID="repClientsByProject" runat="server" OnItemDataBound="repClientsByProject_ItemDataBound">
        <HeaderTemplate>
            <div class="border_black">
                <table class="ConsultingDemandDetails">
                    <thead>
                        <tr class="headerRow">
                            <th class="Width13Percent">
                                <asp:LinkButton ID="btnAccount" runat="server" CausesValidation="false" CommandArgument="Account"
                                    Style="text-decoration: none; color: Black; width: 100%;" OnCommand="btnAccount_Command">
                                Account
                                </asp:LinkButton>
                            </th>
                            <th class="Width17Per">
                                <asp:LinkButton ID="btnProjectNumber" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnProjectNumber_Command">
                                Project
                                </asp:LinkButton>
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBusinessUnitFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBusinessUnit" runat="server" TargetControlID="imgBusinessUnitFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10Per">
                                <asp:LinkButton ID="btnStatus" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnStatus_Command">
                                Status
                                </asp:LinkButton>
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectStatusFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgProjectStatusFilter"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width7Percent">
                                <asp:LinkButton ID="btnBillingType" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnBillingType_Command">
                                BillingType
                                </asp:LinkButton>
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgBilling" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceBilling" runat="server" TargetControlID="imgBilling"
                                    PopupControlID="pnlBilling" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width13Percent">
                                <asp:LinkButton ID="btnProjectedHours" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnProjectedHours_Command">
                                Projected Hours
                                </asp:LinkButton>
                            </th>
                            <th class="Width5Percent">
                                <asp:LinkButton ID="btnBillableHours" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnBillableHours_Command">
                                Billable
                                </asp:LinkButton>
                            </th>
                            <th class="Width8Per">
                                <asp:LinkButton ID="btnNonBillableHours" runat="server" CausesValidation="false"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnNonBillableHours_Command">
                                Non Billable
                                </asp:LinkButton>
                            </th>
                            <th class="Width6Percent">
                                <asp:LinkButton ID="btnActualHours" runat="server" CausesValidation="false" Style="text-decoration: none;
                                    color: Black;" OnCommand="btnActualHours_Command">
                                Actual Hours
                                </asp:LinkButton>
                            </th>
                            <th class="Width11Percent">
                                <asp:LinkButton ID="btnTotalEstBillings" runat="server" CausesValidation="false"
                                    Style="text-decoration: none; color: Black;" OnCommand="btnTotalEstBillings_Command">
                                Total Estimated Billings
                                </asp:LinkButton>
                            </th>
                            <th class="Width10Per">
                                <asp:LinkButton ID="btnBillableHoursVariance" runat="server" CausesValidation="false"
                                    CommandArgument="btnBillableHoursVariance" Style="text-decoration: none; color: Black;"
                                    OnCommand="btnBillableHoursVariance_Command">
                                Billable Hours Variance
                                </asp:LinkButton>
                                <asp:Image alt="Billable Hours Variance Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                    ID="imgBillableHoursVarianceHint" CssClass="CursorPointer" ToolTip="Billable Hours Variance Calculation" />
                                <AjaxControlToolkit:ModalPopupExtender ID="mpeBillableUtilization" runat="server"
                                    TargetControlID="imgBillableHoursVarianceHint" CancelControlID="btnCancel" BehaviorID="pnlBillableUtilization"
                                    BackgroundCssClass="modalBackground" PopupControlID="pnlBillableUtilization"
                                    DropShadow="false" />
                            </th>
                        </tr>
                    </thead>
                </table>
        </HeaderTemplate>
        <ItemTemplate>
            <table class="ConsultingDemandDetails">
                <tr class="bgColorD4D0C9 textCenter">
                    <td class="textLeft Width3Per padLeft20Imp WhiteSpaceNormal">
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Account Details"
                            ExpandedText="Collapse Account Details" EnableViewState="true" BehaviorID="cpeDetails"
                            Collapsed="true" TargetControlID="pnlAccountDetails" ImageControlID="imgDetails"
                            CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                            ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                        <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Account Details" />
                    </td>
                    <td class="Width10Per textLeft">
                        <asp:Label ID="lblAccount" CssClass="displayNone" runat="server"></asp:Label>
                        <span class="Width10Px"></span>
                        <asp:Label ID="lblAccountName" runat="server" Text='<%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).Account.HtmlEncodedName %>'></asp:Label>
                    </td>
                    <td class="Width17Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).ProjectsCount%>
                    </td>
                    <td colspan="2" class="Width17Per">
                    </td>
                    <td class="Width13Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectedHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BillableHours)%>
                    </td>
                    <td class="Width8Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).NonBillableHours)%>
                    </td>
                    <td class="Width6Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalActualHours)%>
                    </td>
                    <td class="Width11Percent">
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalBillableHoursVariance)%>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlAccountDetails" runat="server">
                <asp:Repeater ID="repAccountDetails" runat="server" OnItemDataBound="repAccountDetails_ItemDataBound">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="bgcolorwhite textCenter">
                                <td class="Width13Percent">
                                </td>
                                <td class="Width17Per">
                                    <table>
                                        <tr>
                                            <td class="padLeft30 t-left">
                                                <%# Eval("Project.ProjectNumber")%>
                                                -
                                                <asp:Label ID="lblProjectName" runat="server" Visible="false" Text=' <%# Eval("Project.HtmlEncodedName")%> '></asp:Label>
                                                <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                                                    Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                                </asp:HyperLink>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).Project.Status.StatusType.ToString()%>
                                </td>
                                <td class="Width7Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillingType%>
                                </td>
                                <td class="Width13Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width8Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width6Percent">
                                    <asp:Label ID="lblActualHours" runat="server" Visible="false" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '></asp:Label>
                                    <asp:HyperLink ID="hlActualHours" runat="server" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '
                                        Target="_blank" NavigateUrl='<%# GetReportByProjectLink((string)Eval("Project.ProjectNumber"))%>'>
                                    </asp:HyperLink>
                                </td>
                                <td class="Width11Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).EstimatedBillingsWithFormat%>
                                </td>
                                <td class="Width10Per">
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
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="alterrow textCenter">
                                <td class="Width13Percent">
                                </td>
                                <td class="Width17Per">
                                    <table>
                                        <tr>
                                            <td class="padLeft30 t-left">
                                                <%# Eval("Project.ProjectNumber")%>
                                                -
                                                <asp:Label ID="lblProjectName" runat="server" Visible="false" Text=' <%# Eval("Project.HtmlEncodedName")%> '></asp:Label>
                                                <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                                                    Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                                </asp:HyperLink>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).Project.Status.StatusType.ToString()%>
                                </td>
                                <td class="Width7Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillingType%>
                                </td>
                                <td class="Width13Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width8Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width6Percent">
                                    <asp:Label ID="lblActualHours" runat="server" Visible="false" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '></asp:Label>
                                    <asp:HyperLink ID="hlActualHours" runat="server" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '
                                        Target="_blank" NavigateUrl='<%# GetReportByProjectLink((string)Eval("Project.ProjectNumber"))%>'>
                                    </asp:HyperLink>
                                </td>
                                <td class="Width11Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).EstimatedBillingsWithFormat%>
                                </td>
                                <td class="Width10Per">
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
                    <td class="textLeft Width3Per padLeft20Imp WhiteSpaceNormal">
                        <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeDetails" runat="Server" CollapsedText="Expand Account Details"
                            ExpandedText="Collapse Account Details" EnableViewState="true" BehaviorID="cpeDetails"
                            Collapsed="true" TargetControlID="pnlAccountDetails" ImageControlID="imgDetails"
                            CollapsedImage="~/Images/expand.jpg" ExpandedImage="~/Images/collapse.jpg" CollapseControlID="imgDetails"
                            ExpandControlID="imgDetails" TextLabelID="lbTitleSkill" />
                        <asp:Image ID="imgDetails" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Account Details" />
                    </td>
                    <td class="Width10Per textLeft">
                        <asp:Label ID="lblAccount" CssClass="displayNone" runat="server"></asp:Label>
                        <span class="Width10Px"></span>
                        <asp:Label ID="lblAccountName" runat="server" Text='<%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).Account.HtmlEncodedName %>'></asp:Label>
                    </td>
                    <td class="Width17Per">
                        <%# ((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).ProjectsCount%>
                    </td>
                    <td colspan="2" class="Width17Per">
                    </td>
                    <td class="Width13Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalProjectedHours)%>
                    </td>
                    <td class="Width5Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).BillableHours)%>
                    </td>
                    <td class="Width8Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).NonBillableHours)%>
                    </td>
                    <td class="Width6Percent">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalActualHours)%>
                    </td>
                    <td class="Width11Percent">
                    </td>
                    <td class="Width10Per">
                        <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ByAccount.GroupByAccount)Container.DataItem).TotalBillableHoursVariance)%>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlAccountDetails" runat="server">
                <asp:Repeater ID="repAccountDetails" runat="server" OnItemDataBound="repAccountDetails_ItemDataBound">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="bgcolorwhite textCenter">
                                <td class="Width13Percent">
                                </td>
                                <td class="Width17Per">
                                    <table>
                                        <tr>
                                            <td class="padLeft30 t-left">
                                                <%# Eval("Project.ProjectNumber")%>
                                                -
                                                <asp:Label ID="lblProjectName" runat="server" Visible="false" Text=' <%# Eval("Project.HtmlEncodedName")%> '></asp:Label>
                                                <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                                                    Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                                </asp:HyperLink>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).Project.Status.StatusType.ToString()%>
                                </td>
                                <td class="Width7Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillingType%>
                                </td>
                                <td class="Width13Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width8Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width6Percent">
                                    <asp:Label ID="lblActualHours" runat="server" Visible="false" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '></asp:Label>
                                    <asp:HyperLink ID="hlActualHours" runat="server" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '
                                        Target="_blank" NavigateUrl='<%# GetReportByProjectLink((string)Eval("Project.ProjectNumber"))%>'>
                                    </asp:HyperLink>
                                </td>
                                <td class="Width11Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).EstimatedBillingsWithFormat%>
                                </td>
                                <td class="Width10Per">
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
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="ConsultingDemandDetails">
                            <tr class="alterrow textCenter">
                                <td class="Width13Percent">
                                </td>
                                <td class="Width17Per">
                                    <table>
                                        <tr>
                                            <td class="padLeft30 t-left">
                                                <%# Eval("Project.ProjectNumber")%>
                                                -
                                                <asp:Label ID="lblProjectName" runat="server" Visible="false" Text=' <%# Eval("Project.HtmlEncodedName")%> '></asp:Label>
                                                <asp:HyperLink ID="hlProjectName" runat="server" CssClass="HyperlinkByProjectReport"
                                                    Text=' <%# Eval("Project.HtmlEncodedName")%> ' Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id"))) %>'>
                                                </asp:HyperLink>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="Width10Per">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).Project.Status.StatusType.ToString()%>
                                </td>
                                <td class="Width7Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillingType%>
                                </td>
                                <td class="Width13Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).ForecastedHours)%>
                                </td>
                                <td class="Width5Percent">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).BillableHours)%>
                                </td>
                                <td class="Width8Per">
                                    <%# GetDoubleFormat((double)((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).NonBillableHours)%>
                                </td>
                                <td class="Width6Percent">
                                    <asp:Label ID="lblActualHours" runat="server" Visible="false" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '></asp:Label>
                                    <asp:HyperLink ID="hlActualHours" runat="server" Text=' <%# GetDoubleFormat((double)Eval("TotalHours"))%> '
                                        Target="_blank" NavigateUrl='<%# GetReportByProjectLink((string)Eval("Project.ProjectNumber"))%>'>
                                    </asp:HyperLink>
                                </td>
                                <td class="Width11Percent">
                                    <%# ((DataTransferObjects.Reports.ProjectLevelGroupedHours)Container.DataItem).EstimatedBillingsWithFormat%>
                                </td>
                                <td class="Width10Per">
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

