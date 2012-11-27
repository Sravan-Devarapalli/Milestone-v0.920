<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectSearch.aspx.cs" Inherits="PraticeManagement.ProjectSearch"
    Title="Project Search Results | Practice Management" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ PreviousPageType TypeName="PraticeManagement.Controls.PracticeManagementSearchPageBase" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Project Search Results | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Search Results
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" language="javascript">

        function setPosition(item, ytop, xleft) {
            item.offset({ top: ytop, left: xleft });
        }

        function SetTooltipText(descriptionText, hlinkObj) {
            var hlinkObjct = $(hlinkObj);
            var displayPanel = $('#<%= pnlProjectToolTipHolder.ClientID %>');
            iptop = hlinkObjct.offset().top;
            ipleft = hlinkObjct.offset().left + hlinkObjct[0].offsetWidth + 5;
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();
            setPosition(displayPanel, iptop - 20, ipleft);
            displayPanel.show();

            var lblProjectTooltip = document.getElementById('<%= lblProjectTooltip.ClientID %>');
            lblProjectTooltip.innerHTML = descriptionText.toString();
        }


        function HidePanel() {
            var displayPanel = $('#<%= pnlProjectToolTipHolder.ClientID %>');
            displayPanel.hide();
        }


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
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSearch">
        <div class="project-filter DivProjectFilter">
            <table class="WholeWidth">
                <tbody>
                    <tr>
                        <td class="padRight8">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" MaxLength="255">
                            </asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                ErrorMessage="The Search Text is required." ToolTip="The Search Text is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td class="width55Px">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                CssClass="width55Px" />
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
                                Account</div>
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
                <tr runat="server" id="boundingRow" class="MinHeight25Px vTop">
                    <td class="PaddingTop4Px">
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.HtmlEncodedName")) %>'
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
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("HtmlEncodedName")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" CssClass="padLeft30">
                            <asp:DataList ID="dtlProposedPersons" runat="server">
                                <ItemTemplate>
                                    <%-- Eval("PersonLastFirstName") --%>
                                    <div class="DivMileStoneNames">
                                        <asp:LinkButton ID="btnMilestoneNames" runat="server" CssClass="height10Px" Text='<%# HighlightFound(Eval("HtmlEncodedDescription")) %>'
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
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr runat="server" id="boundingRow" class="rowEven MinHeight20Px vTop">
                    <td class="PaddingTop4Px">
                    </td>
                    <td class="CompPerfProjectState AddLeftPadding">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber AddLeftPadding">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.HtmlEncodedName")) %>'
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
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("HtmlEncodedName")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" CssClass="padLeft30">
                            <asp:DataList ID="dtlProposedPersons" runat="server">
                                <ItemTemplate>
                                    <div class="DivMileStoneNames">
                                        <asp:LinkButton ID="btnMilestoneNames" runat="server" CssClass="height10Px" Text='<%# HighlightFound(Eval("HtmlEncodedDescription")) %>'
                                            CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Project.Id")) %>' OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:DataList>
                        </asp:Panel>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod AddLeftPadding">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'
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
    <asp:Panel ID="pnlProjectToolTipHolder" runat="server" CssClass="ToolTip WordWrap PanelOppNameToolTipHolder">
        <table>
            <tr class="top">
                <td class="lt">
                    <div class="tail">
                    </div>
                </td>
                <td class="tbor">
                </td>
                <td class="rt">
                </td>
            </tr>
            <tr class="middle">
                <td class="lbor">
                </td>
                <td class="content WordWrap">
                    <asp:Label ID="lblProjectTooltip" CssClass="WordWrap" runat="server"></asp:Label>
                </td>
                <td class="rbor">
                </td>
            </tr>
            <tr class="bottom">
                <td class="lb">
                </td>
                <td class="bbor">
                </td>
                <td class="rb">
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

