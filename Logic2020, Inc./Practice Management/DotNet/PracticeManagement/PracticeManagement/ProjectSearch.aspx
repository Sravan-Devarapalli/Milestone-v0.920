<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ProjectSearch.aspx.cs" Inherits="PraticeManagement.ProjectSearch"
    Title="Practice Management - Project Search Results" %>
<%@ Register TagPrefix="uc" TagName="ProjectNameCellRounded" Src="~/Controls/ProjectNameCellRounded.ascx" %>

<%@ PreviousPageType VirtualPath="~/Projects.aspx" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Project Search Results</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Project Search Results
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:Panel runat="server" DefaultButton="btnSearch">
        <div class="project-filter" style="background: #E2EBFF; margin-bottom: 10px; padding:5px;">
            <table class="WholeWidth">
                <tbody>
                    <tr>
                        <td style="padding-right: 8px;">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="WholeWidth" MaxLength="255"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator ID="reqSearchText" runat="server" ControlToValidate="txtSearchText"
                                ErrorMessage="The Search Text is required." ToolTip="The Search Text is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 55px;">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" Width="55" />
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
        <asp:ListView ID="lvProjects" runat="server" DataKeyNames="Id">
            <LayoutTemplate>
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
                        <td class="CompPerfProject">
                            <div class="ie-bg">
                                Milestone Name
                            </div>
                        </td>
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
            </LayoutTemplate>
            <ItemTemplate>
                <tr runat="server" id="boundingRow">
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
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProject">
                        <asp:LinkButton ID="btnMilestoneName" runat="server" Text='<%# HighlightFound(Eval("Milestones[0].Description")) %>'
                            CommandArgument='<%# string.Concat(Eval("Milestones[0].Id"), "_", Eval("Id")) %>'
                            OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>               
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr runat="server" id="boundingRow" class="rowEven">
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
                        <asp:LinkButton ID="btnProjectName" runat="server" Text='<%# HighlightFound(Eval("Name")) %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfProject">
                        <asp:LinkButton ID="btnMilestoneName" runat="server" Text='<%# HighlightFound(Eval("Milestones[0].Description")) %>'
                            CommandArgument='<%# string.Concat(Eval("Milestones[0].Id"), "_", Eval("Id")) %>'
                            OnCommand="btnMilestoneName_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
                        <asp:LinkButton ID="btnProjectStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToShortDateString() : string.Empty %>'
                            CommandArgument='<%# Eval("Id") %>' OnCommand="Project_Command"></asp:LinkButton>
                    </td>
                    <td class="CompPerfPeriod">
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
    </asp:Panel>
</asp:Content>

