<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectPersons.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectPersons" %>
<asp:Panel ID="pnlTabPersons" runat="server" CssClass="tab-pane">
    <asp:GridView ID="gvPeople" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here."
        CssClass="CompPerfTable WholeWidth" GridLines="None"
        BackColor="White" DataSourceID="odsMilestonePersons">
        <AlternatingRowStyle CssClass="alterrow"/>
        <Columns>
            <asp:TemplateField HeaderText="Person Name">
                <HeaderStyle CssClass="Width20Percent"/>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Person Name</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="hlPersonName" runat="server" NavigateUrl='<%# GetMilestonePersonRedirectUrl(Eval("Milestone.Id"), Eval("Id")) %>'
                        Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("Person.LastName"), Eval("Person.FirstName"))) %>'
                        onclick='<%# "javascript:checkDirty(\"" + PERSON_TARGET + "\", " + string.Format("\"{0}:{1}\"", Eval("Milestone.Id"), Eval("Id")) + ")" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Role">
                <HeaderStyle CssClass="Width10Percent"/>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Role</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblPersonRole" runat="server" Text='<%# Eval("Entries[0].Role.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderStyle CssClass="Width32Percent"/>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Milestone Name</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblMilestoneName" runat="server" Text=' <%# Eval("Milestone.HtmlEncodedDescription") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start">
            <ItemStyle CssClass="alignCenter" />
                <HeaderStyle CssClass="Width8Percent"/>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Start</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblStartDate0" runat="server" Text='<%# ((DateTime)Eval("Entries[0].StartDate")).ToString("MM/dd/yyyy") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End">
            <ItemStyle CssClass="alignCenter" />
                <HeaderStyle CssClass="Width8Percent" />
                <HeaderTemplate>
                    <div class="ie-bg">
                        End</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblStartDate1" runat="server" Text='<%# ((DateTime?)Eval("Entries[0].EndDate")).HasValue ? ((DateTime)Eval("Entries[0].EndDate")).ToString("MM/dd/yyyy") : string.Empty %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
            <ItemStyle CssClass="alignCenter" />
                <HeaderStyle CssClass="Width11Percent TextAlignCenterImp"/>
                <HeaderTemplate>
                    <div class="ie-bg">
                        Assigned Hours</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblHoursAssigned" runat="server" Text='<%# (Eval("Entries[0].ProjectedWorkload")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
            <ItemStyle CssClass="alignCenter" />
                <HeaderStyle CssClass="Width11Percent TextAlignCenterImp" />
                <HeaderTemplate>
                    <div class="ie-bg">
                        Assigned Per Day</div>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblHoursPerMonth" runat="server" Text='<%# Eval("Entries[0].HoursPerDay") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<asp:ObjectDataSource ID="odsMilestonePersons" runat="server" 
    SelectMethod="GetMilestonePersonListByProjectWithoutPay" 
    TypeName="PraticeManagement.Controls.DataHelper" >
    <SelectParameters>
        <asp:QueryStringParameter Name="projectId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

