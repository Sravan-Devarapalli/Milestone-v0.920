<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectSearch.aspx.cs" Inherits="PraticeManagement.ProjectSearch"
    Title="Practice Management - Project Search Results" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="uc" TagName="ProjectNameCellRounded" Src="~/Controls/ProjectNameCellRounded.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ PreviousPageType VirtualPath="~/Projects.aspx" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Project Search Results</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Search Results
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function ExpandAll(btn) {
            var hdnExpandCollapseExtendersIds = document.getElementById('<%= hdnExpandCollapseExtendersIds.ClientID %>');
            var isExpand = false;
            if (btn.value == "Expand All") {
                isExpand = true;
                btn.value = "Collapse All";
            }
            else {
                btn.value = "Expand All";
            }
            if (hdnExpandCollapseExtendersIds != null) {
                var ids = hdnExpandCollapseExtendersIds.value.split(",");
                for (var i = 0; i < ids.length; i++) {
                    var cpe = $find(ids[i]);
                    if (cpe != null) {
                        if (isExpand)
                            cpe._doOpen();
                        else
                            cpe._doClose();
                    }
                }
            }
        }
        $(document).ready(
        function () {
            var hdnExpandCollapseExtendersIds = document.getElementById('<%= hdnExpandCollapseExtendersIds.ClientID %>');
            var btn = document.getElementById('<%= ExpandAll.ClientID %>');
            var ids = hdnExpandCollapseExtendersIds.value.split(",");
            if (ids == null || ids.length < 2) {
                btn.style.display = "none";
            }
        });
    </script>
    <style type="text/css">
        .AddLeftPadding
        {
            padding-left: 4px;
        }
    </style>
    <asp:Panel runat="server" DefaultButton="btnSearch">
        <div class="project-filter" style="background: #E2EBFF; margin-bottom: 10px; padding: 5px;">
            <table class="WholeWidth">
                <tbody>
                    <tr>
                        <td style="padding-right: 8px;">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" MaxLength="255">
                            </asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                ErrorMessage="The Search Text is required." ToolTip="The Search Text is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 55px;">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                Width="55" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:ValidationSummary ID="vsumProjectSearch" runat="server" EnableClientScript="false" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <asp:Button ID="ExpandAll" runat="server" Text="Expand All" OnClientClick="ExpandAll(this);return false;">
        </asp:Button>
        <br />
        <br />
        <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id" OnItemDataBound="lvProjects_ItemDataBound">
            <LayoutTemplate>
                <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth AddLeftPadding">
                    <tr runat="server" id="lvHeader" class="CompPerfHeader">
                        <td class="CompPerfProjectState">
                            <div>
                            </div>
                        </td>
                        <td class="CompPerfProjectState AddLeftPadding">
                            <div class="ie-bg">
                                Project #</div>
                        </td>
                        <td class="CompPerfProjectNumber AddLeftPadding">
                            <div class="ie-bg">
                                Client</div>
                        </td>
                        <td class="CompPerfClient AddLeftPadding">
                            <div class="ie-bg">
                                Project Name
                            </div>
                        </td>
                        <%--<td class="CompPerfProject">
                            <div class="ie-bg">
                                Milestone Name
                            </div>
                        </td>--%>
                        <td class="CompPerfPeriod AddLeftPadding">
                            <div class="ie-bg">
                                Project Start Date
                            </div>
                        </td>
                        <td class="CompPerfPeriod AddLeftPadding">
                            <div class="ie-bg">
                                Project End Date
                            </div>
                        </td>
                    </tr>
                    <tbody>
                        <tr runat="server" id="itemPlaceholder" />
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server" id="boundingRow" valign="top" style="min-height: 25px;">
                    <td style="padding-top:4px;">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%# GetProjectNameCellCssClass(((Project) Container.DataItem))%>' />
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.Name")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient AddLeftPadding">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlMilestones"
                            ImageControlID="btnExpandCollapseMilestones" CollapsedImage="Images/expand.jpg"
                            ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseMilestones"
                            ExpandControlID="btnExpandCollapseMilestones" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseMilestones" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Project Milestones" />
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" Style="padding-left: 30px;">
                            <asp:DataList ID="dtlProposedPersons" runat="server">
                                <ItemTemplate>
                                    <%-- Eval("PersonLastFirstName") --%>
                                    <div style="padding: 2px 0px 2px 0px;">
                                        <asp:LinkButton ID="btnMilestoneNames" runat="server" Style="height: 10px;" Text='<%# HighlightFound(Eval("Description")) %>'
                                            CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Project.Id")) %>' OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                        </asp:Panel>
                    </td>
                    <%--<td class="CompPerfProject">
                        <asp:LinkButton ID="btnMilestoneName" runat="server" Text='<%# HighlightFound(Eval("Milestones[0].Description")) %>'
                            CommandArgument='<%# string.Concat(Eval("Milestones[0].Id"), "_", Eval("Id")) %>'
                            OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                    </td>--%>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr runat="server" id="boundingRow" class="rowEven" valign="top" style="min-height: 20px;">
                    <td style="padding-top:4px;">
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25"
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' ButtonCssClass='<%#PraticeManagement.Utils.ProjectHelper.GetIndicatorClassByStatusId((int)Eval("Status.Id")) %>' />
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.Name")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient AddLeftPadding">
                        <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlMilestones"
                            ImageControlID="btnExpandCollapseMilestones" CollapsedImage="Images/expand.jpg"
                            ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseMilestones"
                            ExpandControlID="btnExpandCollapseMilestones" Collapsed="True" TextLabelID="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseMilestones" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Project Milestones" />
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" Style="padding-left: 30px;">
                            <asp:DataList ID="dtlProposedPersons" runat="server">
                                <ItemTemplate>
                                    <div style="padding: 2px 0px 2px 0px;">
                                        <asp:LinkButton ID="btnMilestoneNames" runat="server" Text='<%# HighlightFound(Eval("Description")) %>'
                                            CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Id")) %>' OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                        </asp:Panel>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <EmptyDataTemplate>
                <tr runat="server" id="EmptyDataRow">
                    <td>
                        No projects found.
                    </td>
                </tr>
            </EmptyDataTemplate>
        </asp:ListView>
        <asp:HiddenField ID="hdnExpandCollapseExtendersIds" runat="server" Value="" />
    </asp:Panel>
</asp:Content>

