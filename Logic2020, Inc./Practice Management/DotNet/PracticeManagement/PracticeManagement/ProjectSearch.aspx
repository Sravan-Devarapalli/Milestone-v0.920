<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectSearch.aspx.cs" Inherits="PraticeManagement.ProjectSearch"
    Title="Practice Management - Project Search Results" %>

<%@ Register TagPrefix="uc" TagName="ProjectNameCellRounded" Src="~/Controls/ProjectNameCellRounded.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ PreviousPageType VirtualPath="~/Projects.aspx" %>
<asp:content id="cntTitle" contentplaceholderid="title" runat="server">
    <title>Practice Management - Project Search Results</title>
</asp:content>
<asp:content id="cntHeader" contentplaceholderid="header" runat="server">
    Project Search Results
</asp:content>
<asp:content id="cntBody" contentplaceholderid="body" runat="server">
<script type="text/javascript" language="javascript">
    function ExpandAll() {
        var lvProjects_table = document.getElementById('<%= lvProjects.ClientID %>'+'_lvProjects_table');
        if (lvProjects_table != null) {
        var tBody = lvProjects_table.children[0];
        for (var i = 1; i < tBody.children.length; i++) {
            var img = tBody.children[i].children[3].getElementsByTagName('IMG');
            if (img != null && img.length > 0) {
                img.fireEvent('onclick');
            }
            }
        }
    }
</script>
    <asp:panel runat="server" defaultbutton="btnSearch">
        <div class="project-filter" style="background: #E2EBFF; margin-bottom: 10px; padding: 5px;">
            <table class="WholeWidth">
                <tbody>
                    <tr>
                        <td style="padding-right: 8px;">
                            <asp:textbox id="txtSearchText" runat="server" cssclass="WholeWidth" maxlength="255">
                            </asp:textbox>
                        </td>
                        <td>
                            <asp:requiredfieldvalidator id="reqSearchText" runat="server" controltovalidate="txtSearchText"
                                errormessage="The Search Text is required." tooltip="The Search Text is required."
                                text="*" enableclientscript="false" setfocusonerror="true" display="Dynamic">
                            </asp:requiredfieldvalidator>
                        </td>
                        <td style="width: 55px;">
                            <asp:button id="btnSearch" runat="server" text="Search" onclick="btnSearch_Click"
                                width="55" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:validationsummary id="vsumProjectSearch" runat="server" enableclientscript="false" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <asp:Button id="ExpandAll" runat="server" Text="Expand All" OnClientClick="ExpandAll();return false;"  ></asp:Button>
        <asp:listview id="lvProjects" runat="server" datakeynames="Id" onitemdatabound="lvProjects_ItemDataBound">
            <layouttemplate>
                <table id="lvProjects_table" runat="server" class="CompPerfTable WholeWidth">
                    <tr runat="server" id="lvHeader" class="CompPerfHeader">
                        <td class="CompPerfProjectState">
                            <div>                            
                            </div>
                        </td>
                        <td class="CompPerfProjectState">
                            <div class="ie-bg">
                                Project #</div>
                        </td>
                        <td class="CompPerfProjectNumber">
                            <div class="ie-bg">
                                Client</div>
                        </td>
                        <td class="CompPerfClient">
                            <div class="ie-bg">
                                Project Name
                            </div>
                        </td>
                        <%--<td class="CompPerfProject">
                            <div class="ie-bg">
                                Milestone Name
                            </div>
                        </td>--%>
                        <td class="CompPerfPeriod">
                            <div class="ie-bg">
                                Project Start Date
                            </div>
                        </td>
                        <td class="CompPerfPeriod">
                            <div class="ie-bg">
                                Project End Date
                            </div>
                        </td>                    
                    </tr>
                    <tbody>
                        <tr runat="server" id="itemPlaceholder" />
                    </tbody>
                </table>
            </layouttemplate>
            <itemtemplate>
                <tr runat="server" id="boundingRow" valign="top">
                     <td>                    
                        <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" 
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' 
                            ButtonCssClass='<%#PraticeManagement.Utils.ProjectHelper.GetIndicatorClassByStatusId((int)Eval("Status.Id")) %>' />
                    </td>
                    <td class="CompPerfProjectState">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.Name")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient">
                   
                        <ajaxtoolkit:collapsiblepanelextender id="cpe" runat="Server" targetcontrolid="pnlMilestones"
                            imagecontrolid="btnExpandCollapseMilestones" collapsedimage="Images/expand.jpg" expandedimage="Images/collapse.jpg"
                            collapsecontrolid="btnExpandCollapseMilestones" expandcontrolid="btnExpandCollapseMilestones"
                            collapsed="True" textlabelid="lblFilter" />
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseMilestones" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Project Milestones" />
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" style="padding-left:30px;">
                       <asp:DataList ID="dtlProposedPersons" runat="server">
                            <ItemTemplate>
                            <%-- Eval("PersonLastFirstName") --%>
                                <asp:LinkButton ID="btnMilestoneNames" runat="server" Text='<%# HighlightFound(Eval("Description")) %>'
                                CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Project.Id")) %>'
                                OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:DataList>
                         </asp:Panel>
                    </td>
                    <%--<td class="CompPerfProject">
                        <asp:LinkButton ID="btnMilestoneName" runat="server" Text='<%# HighlightFound(Eval("Milestones[0].Description")) %>'
                            CommandArgument='<%# string.Concat(Eval("Milestones[0].Id"), "_", Eval("Id")) %>'
                            OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                    </td>--%>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>               
                </tr>
            </itemtemplate>
            <alternatingitemtemplate>
                <tr runat="server" id="boundingRow" class="rowEven" valign="top">
                    <td>
                         <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" 
                            ButtonProjectNameToolTip='<%# Eval("Status.Name") %>' 
                            ButtonCssClass='<%#PraticeManagement.Utils.ProjectHelper.GetIndicatorClassByStatusId((int)Eval("Status.Id")) %>' />
                    </td>
                    <td class="CompPerfProjectState">
                        <asp:LinkButton ID="btnProjectNumber" runat="server" Text='<%# HighlightFound(Eval("ProjectNumber")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProjectNumber">
                        <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# HighlightFound(Eval("Client.Name")) %>'
                            CommandArgument='<%# Eval("Client.Id") %>' OnCommand="btnClientName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfClient">
                        <ajaxtoolkit:collapsiblepanelextender id="cpe" runat="Server" targetcontrolid="pnlMilestones"
                            imagecontrolid="btnExpandCollapseMilestones" collapsedimage="Images/expand.jpg" expandedimage="Images/collapse.jpg"
                            collapsecontrolid="btnExpandCollapseMilestones" expandcontrolid="btnExpandCollapseMilestones"
                            collapsed="True" textlabelid="lblFilter" />
                           
                        <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                        <asp:Image ID="btnExpandCollapseMilestones" runat="server" ImageUrl="~/Images/collapse.jpg"
                            ToolTip="Project Milestones" />
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                        <asp:Panel ID="pnlMilestones" runat="server" style="padding-left:30px;">
                       <asp:DataList ID="dtlProposedPersons" runat="server">
                            <ItemTemplate>
                            <%-- Eval("PersonLastFirstName") --%>
                                <asp:LinkButton ID="btnMilestoneNames" runat="server" Text='<%# HighlightFound(Eval("Description")) %>'
                                CommandArgument='<%# string.Concat(Eval("Id"), "_", Eval("Id")) %>'
                                OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:DataList>
                         </asp:Panel>
                    </td>
                    <%--<td class="CompPerfProject">
                        <asp:LinkButton ID="btnMilestoneName" runat="server" Text='<%# HighlightFound(Eval("Milestones[0].Description")) %>'
                            CommandArgument='<%# string.Concat(Eval("Milestones[0].Id"), "_", Eval("Id")) %>'
                            OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                    </td>--%>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>                
                </tr>
            </alternatingitemtemplate>
            <emptydatatemplate>
                <tr runat="server" id="EmptyDataRow">
                    <td>
                        No projects found.
                    </td>
                </tr>
            </emptydatatemplate>
        </asp:listview>
    </asp:panel>
</asp:content>

