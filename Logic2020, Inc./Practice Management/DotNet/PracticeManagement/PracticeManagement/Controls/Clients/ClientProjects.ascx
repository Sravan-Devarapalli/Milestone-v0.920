<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientProjects.ascx.cs"
    EnableViewState="true" Inherits="PraticeManagement.Controls.Clients.ClientProjects" %>
<asp:Repeater ID="repProjects" runat="server" >
    <HeaderTemplate>
        <div class="minheight250Px">
            <table id="tblAccountSummaryByProject" class="CompPerfTable WholeWidth BackGroundColorWhite">
                <thead>
                    <tr>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Project Name
                            </div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Business Unit</div>
                        </th>
                        
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Start Date</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                End Date</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Division</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Practice Area</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Status</div>
                        </th>
                        <th class="CursorPointer color0898E6 fontUnderline">
                            <div class='ie-bg'>
                                Billable</div>
                        </th>
                    </tr>
                </thead>
                <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <asp:HyperLink ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                    Target="_blank" NavigateUrl='<%#GetProjectLinkURL((DataTransferObjects.Project) Container.DataItem) %>'
                    Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:HyperLink>
            </td>
            <td>
                <%# Eval("Group.HtmlEncodedName")%>
            </td>
      
            <td class="textCenter">
                <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td class="textCenter">
                <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
                <%# Eval("Division") != null ? Eval("Division.Name") : string.Empty%>
            </td>
            <td>
                <%# Eval("Practice.HtmlEncodedName")%>
            </td>
            <td class="textCenter">
                <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
            </td>
            <td class="textCenter" sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="alterrow">
            <td>
                <asp:HyperLink ID="HyperLink1" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                    Target="_blank" NavigateUrl='<%#GetProjectLinkURL((DataTransferObjects.Project) Container.DataItem) %>'
                    Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:HyperLink>
            </td>
            <td>
                <%# Eval("Group.HtmlEncodedName")%>
            </td>
           
            <td class="textCenter"> 
                <%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td class="textCenter">
                <%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>
            </td>
            <td>
                <%# Eval("Division") != null ? Eval("Division.Name") : string.Empty%>
            </td>
            <td>
                <%# Eval("Practice.HtmlEncodedName")%>
            </td>
            <td class="textCenter">
                <%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>
            </td>
            <td class="textCenter" sorttable_customkey='<%# Eval("IsChargeable") %><%#Eval("HtmlEncodedName")%>'>
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

