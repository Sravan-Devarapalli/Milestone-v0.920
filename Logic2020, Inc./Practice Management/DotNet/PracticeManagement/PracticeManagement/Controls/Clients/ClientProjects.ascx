<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClientProjects.ascx.cs"
    Inherits="PraticeManagement.Controls.Clients.ClientProjects" %>
<asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="False" EmptyDataText="No projects."
    CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White" AllowSorting="true"
    DataSourceID="odsProjects">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <Columns>
        <asp:TemplateField HeaderText= "<div class='ie-bg' > Project Name </div>" SortExpression="Name">
            <ItemTemplate>
                <asp:LinkButton ID="btnProjectName" runat="server" CausesValidation="false" Text='<%# HttpUtility.HtmlEncode((string)Eval("Name")) %>'
                    CommandArgument='<%# Eval("Id") %>' OnCommand="btnProjectName_Command" Enabled='<%# !CheckIfDefaultProject(Eval("Id")) %>'></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Group</div>" SortExpression="GroupName">
            <ItemTemplate>
                <asp:Label ID="lblGroup" runat="server" Text='<%# Eval("Group.Name") != null ? Eval("Group.Name").ToString() : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Manager</div>" SortExpression="ProjectManagerLastName">
            <ItemTemplate>
                <%# Eval("ProjectManager.PersonLastFirstName")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Start Date</div>" SortExpression="StartDate">
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# Eval("StartDate") != null ? ((DateTime)Eval("StartDate")).ToString("d") : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >End Date</div>" SortExpression="EndDate">
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate") != null ? ((DateTime)Eval("EndDate")).ToString("d") : string.Empty %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<div class='ie-bg' >Practice Area</div>" SortExpression="PracticeName">
            <ItemTemplate>
                <asp:Label ID="lblProjectPractice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Practice.Name") %>'></asp:Label>
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

