﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientProjects.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientProjects" %>
<asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="False" EmptyDataText="No projects." OnRowDataBound="gvProjects_OnRowDataBound"
    CssClass="CompPerfTable WholeWidth BackGroundColorWhite" GridLines="None" AllowSorting="true"
    DataSourceID="odsProjects">
    <AlternatingRowStyle CssClass="alterrow" />
    <Columns>
        <asp:TemplateField HeaderText="<div class='ie-bg' > Project Name </div>" SortExpression="Name">
            <ItemTemplate>
                <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# (string)Eval("HtmlEncodedName") %>'
                    CommandArgument='<%# Eval("Id") %>' OnCommand="btnProjectName_Command" Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Business Unit</div>" SortExpression="GroupName">
            <ItemTemplate>
                <asp:Label ID="lblGroup" runat="server" Text='<%# Eval("Group.HtmlEncodedName") != null ? Eval("Group.HtmlEncodedName").ToString() : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Project Manager(s)</div>" >
            <ItemTemplate>
                <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlProjectManagers"
                    ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                    ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                    ExpandControlID="btnExpandCollapseFilter" Collapsed="True"  />
                
                <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                    ToolTip="" />
                <asp:Panel ID="pnlProjectManagers" runat="server"  >
                    <asp:Repeater ID="repProjectManagers" runat="server" DataSource='<%# Eval("ProjectManagers")%>' >
                        <ItemTemplate>
                            <div>
                                <%# Eval("PersonLastFirstName")%>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Start Date</div>" SortExpression="StartDate">
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >End Date</div>" SortExpression="EndDate">
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Practice Area</div>" SortExpression="PracticeName">
            <ItemTemplate>
                <asp:Label ID="lblProjectPractice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Practice.HtmlEncodedName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Status</div>" SortExpression="ProjectStatusName">
            <ItemTemplate>
                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") != null ? Eval("Status.Name") : string.Empty %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Billable</div>" SortExpression="ProjectIsChargeable">
            <ItemTemplate>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsProjects" runat="server" SortParameterName="sortBy"
    SelectMethod="ListProjectsByClient" TypeName="PraticeManagement.Controls.DataHelper">
    <SelectParameters>
        <asp:QueryStringParameter Name="clientId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

