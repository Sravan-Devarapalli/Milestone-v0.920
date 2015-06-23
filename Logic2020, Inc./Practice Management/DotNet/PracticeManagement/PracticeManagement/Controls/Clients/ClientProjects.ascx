<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientProjects.ascx.cs" EnableViewState="false"
    Inherits="PraticeManagement.Controls.Clients.ClientProjects" %>
<asp:Repeater ID="repProjects" runat="server" OnItemDataBound="repProjects_ItemDataBound">
    <HeaderTemplate>
        <div class="minheight250Px">
            <table id="tblAccountSummaryByProject" class="CompPerfTable WholeWidth BackGroundColorWhite">
                <thead>
                    <tr>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' >  Project Name </div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' > Business Unit</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                             <div class='ie-bg' > Project Access</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                             <div class='ie-bg' > Start Date</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' >  End Date</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' >  Practice Area</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' >  Status</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg' >  Billable</div>
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                  <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                    CommandArgument='<%# Eval("Id") %>' OnCommand="btnProjectName_Command" Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:LinkButton>
            </td>
            <td>
                <%# Eval("Group.HtmlEncodedName")%>
            </td>
            <td>
                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlProjectManagers"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True" />
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="" />
                <asp:Panel ID="pnlProjectManagers" runat="server">
                    <asp:Repeater ID="repProjectManagers" runat="server" DataSource='<%# Eval("ProjectManagers")%>'>
                        <ItemTemplate>
                            <div>
                                <%# Eval("PersonLastFirstName")%>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </td>
            <td>
                <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
               <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
              <%# Eval("Practice.HtmlEncodedName")%> 
            </td>
            <td>
                <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
            </td>
            <td sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
                 <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td>
                 <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                    CommandArgument='<%# Eval("Id") %>' OnCommand="btnProjectName_Command" Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:LinkButton>
            </td>
            <td>
                <%# Eval("Group.HtmlEncodedName")%>
            </td>
            <td>
                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlProjectManagers"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True" />
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="" />
                <asp:Panel ID="pnlProjectManagers" runat="server">
                    <asp:Repeater ID="repProjectManagers" runat="server" DataSource='<%# Eval("ProjectManagers")%>'>
                        <ItemTemplate>
                            <div>
                                <%# Eval("PersonLastFirstName")%>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </td>
            <td>
                <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
               <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
              <%# Eval("Practice.HtmlEncodedName")%> 
            </td>
            <td>
                <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
            </td>
            <td sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
                 <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
        </tbody></table></div>
    </FooterTemplate>
</asp:Repeater>
 <div id="divEmptyMessage" style="display: none;" runat="server">
        No projects.
    </div>

