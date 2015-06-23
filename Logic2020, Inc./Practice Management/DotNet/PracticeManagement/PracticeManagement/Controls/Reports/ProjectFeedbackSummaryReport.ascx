<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectFeedbackSummaryReport.ascx.cs"
    Inherits="PraticeManagement.Controls.Reports.ProjectFeedbackSummaryReport" %>
<%@ Register Src="~/Controls/FilteredCheckBoxList.ascx" TagName="FilteredCheckBoxList"
    TagPrefix="uc" %>
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
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlFilterResource" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblResource" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterTitle" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblTitle" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblStatus" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProject" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProject" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterStartDate" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblStartDateMonths" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterEndDate" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblEndDateMonths" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectManagers" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectManagers" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Panel ID="pnlFilterProjectStatus" Style="display: none;" runat="server">
        <uc:FilteredCheckBoxList ID="cblProjectStatus" runat="server" CssClass="Height125PxImp" />
    </asp:Panel>
    <asp:Button ID="btnFilterOK" runat="server" OnClick="btnFilterOK_OnClick" Style="display: none;" />
    <asp:Repeater ID="repFeedback" runat="server" OnItemDataBound="repFeedback_ItemDataBound">
        <HeaderTemplate>
            <div class="minheight250Px">
                <table id="tblAccountSummaryByProject" class="tablesorter TimePeriodByproject WholeWidth">
                    <thead>
                        <tr>
                            <th class="Width15Per">
                                Resource Name
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgResourceNameFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceResource" runat="server" TargetControlID="imgResourceNameFilter"
                                    PopupControlID="pnlFilterResource" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width9Per">
                                Title
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgTitleFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceTitle" runat="server" TargetControlID="imgTitleFilter"
                                    PopupControlID="pnlFilterTitle" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width15Per">
                                Project
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceProject" runat="server" TargetControlID="imgProjectFilter"
                                    PopupControlID="pnlFilterProject" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10Per">
                                Project Status
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectStatus" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceProjectStatus" runat="server" TargetControlID="imgProjectStatus"
                                    PopupControlID="pnlFilterProjectStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width12Per">
                                Review Period Start Date
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgReveiwStartDateFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceReviewStartDate" runat="server" TargetControlID="imgReveiwStartDateFilter"
                                    PopupControlID="pnlFilterStartDate" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width12Per">
                                Review Period End Date
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgReviewEndDateFilter" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceReviewEndDate" runat="server" TargetControlID="imgReviewEndDateFilter"
                                    PopupControlID="pnlFilterEndDate" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width15Per">
                                Project Access
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgProjectManagers" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceProjectManagers" runat="server" TargetControlID="imgProjectManagers"
                                    PopupControlID="pnlFilterProjectManagers" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                            <th class="Width10Per">
                                Status
                                <img alt="Filter" title="Filter" src="~/Images/search_filter.png" runat="server"
                                    id="imgStatus" class="PosAbsolute padLeft2" />
                                <AjaxControlToolkit:PopupControlExtender ID="pceStatus" runat="server" TargetControlID="imgStatus"
                                    PopupControlID="pnlFilterStatus" Position="Bottom">
                                </AjaxControlToolkit:PopupControlExtender>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="ReportItemTemplate">
                <td class="t-left padLeft40Imp">
                    <%# Eval("Person.HtmlEncodedName")%>
                </td>
                <td>
                    <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                </td>
                <td sorttable_customkey='<%# Eval("Project.HtmlEncodedName") %><%#((DateTime)Eval("ReviewStartDate")).ToString("YYYYMMDDHHMMSS")%>' class="t-left padLeft15Imp">
                    <asp:HyperLink ID="hlProjectNumber" runat="server"
                        Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id")),false) %>'>
                        <%# Eval("Project.ProjectNumber")%>-<%# Eval("Project.HtmlEncodedName")%>
                    </asp:HyperLink>
                </td>
                <td>
                    <%# Eval("Project.Status.Name")%>
                </td>
                <td>
                    <%# ((DateTime)Eval("ReviewStartDate")).ToString("MM/dd/yyyy")%>
                </td>
                <td>
                    <%# ((DateTime)Eval("ReviewEndDate")).ToString("MM/dd/yyyy")%>
                </td>
                <td>
                    <asp:Label ID="lblProjectManagers" runat="server"></asp:Label>
                </td>
                <td sorttable_customkey='<%# Eval("Status.Name") %><%#Eval("Person.Name")%>'>
                    <asp:HyperLink ID="hlProjectStatus" runat="server" Text='<%# Eval("Status.HtmlEncodedName")%> '
                        Target="_blank" NavigateUrl='<%# GetProjectDetailsLink((int?)(Eval("Project.Id")),true) %>'>
                    </asp:HyperLink>
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
</div>

