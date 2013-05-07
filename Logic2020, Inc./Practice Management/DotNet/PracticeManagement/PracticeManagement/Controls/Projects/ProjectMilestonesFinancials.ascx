<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectMilestonesFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectMilestonesFinancials" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:GridView ID="gvRevenueMilestones" runat="server" AutoGenerateColumns="false"
    AllowSorting="true" EmptyDataText="No milestones have been created for this project."
    OnRowDataBound="gvRevenueMilestones_RowDataBound" CssClass="CompPerfTable WholeWidth"
    GridLines="None" BackColor="White" DataSourceID="odsMilestones" OnSorting="gvRevenueMilestones_Sorting">
    <AlternatingRowStyle CssClass="alterrow" />
    <Columns>
        <asp:TemplateField>
            <ItemStyle CssClass="Width3Percent" />
            <ItemTemplate>
                <asp:ImageButton ID="imgbtnEdit" CommandName="edit" runat="server" ToolTip="Edit Milestone Name"
                    ImageUrl="~/Images/icon-edit.png" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imgbtnUpdate" runat="server" ToolTip="Save" OnClick="imgbtnUpdate_OnClick"
                    ImageUrl="~/Images/icon-check.png" />
                <asp:ImageButton ID="imgbtnCancel"  runat="server" ToolTip="Cancel" OnClick="imgbtnCancel_OnClick"
                    ImageUrl="~/Images/no.png" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Milestone Name">
            <ItemStyle CssClass="wrapMilestoneName Width35Percent" />
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbMilestoneName" runat="server" Text="Milestone Name" CommandName="Sort"
                        CommandArgument="MilestoneName" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlMilestoneName" runat="server" NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId")) %>'
                    Text='<%# GetWrappedTest(HttpUtility.HtmlEncode((string)Eval("MilestoneName"))) %>'
                    onclick='<%# "return checkDirty(\"" + MILESTONE_TARGET + "\", " + Eval("MilestoneId") + ")" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="tbMilestoneName" runat="server" MilestoneId='<%# Eval("MilestoneId") %>'
                    Text='<%# Bind("MilestoneName") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMilestoneName" runat="server" ControlToValidate="tbMilestoneName" EnableClientScript="false" ValidationGroup="MilestoneTab"
                    Text="*" ErrorMessage="Milestone Name Required" ToolTip="Milestone Name Required"></asp:RequiredFieldValidator>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date">
            <ItemStyle HorizontalAlign="Center" CssClass="Width9Percent" />
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbStartDate" runat="server" Text="Start Date" CommandName="Sort"
                        CommandArgument="StartDate" /></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <ItemStyle HorizontalAlign="Center" CssClass="Width9Percent" />
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbEndDate" runat="server" Text="End Date" CommandName="Sort"
                        CommandArgument="ProjectedDeliveryDate" /></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime)Eval("ProjectedDeliveryDate")).ToString("MM/dd/yyyy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Revenue">
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbRevenue" runat="server" Text="Revenue" CommandName="Sort" CommandArgument="Revenue" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("Revenue")).ToString() %>'
                    CssClass="Revenue"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contribution Margin">
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbGrossMargin" runat="server" Text="Contribution Margin" CommandName="Sort"
                        CommandArgument="GrossMargin" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" CssClass="Width11Percent" />
            <ItemTemplate>
                <asp:Label ID="lblEstimatedMargin" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("GrossMargin")).ToString() %>'
                    NegativeValue='<%# (decimal) Eval("GrossMargin") < 0 %>' CssClass="Margin" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin %">
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbMargin" runat="server" Text="Margin %" CommandName="Sort" CommandArgument="TargetMargin" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" CssClass="Width7Percent" />
            <ItemTemplate>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(PraticeManagement.Constants.Formatting.PercentageFormat, Eval("TargetMargin") ?? 0) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Billable">
            <ItemStyle HorizontalAlign="Center" CssClass="Width7Percent" />
            <HeaderTemplate>
                <div class="ie-bg NoBorder MilestoneHeaderText">
                    <asp:LinkButton ID="lbBillable" runat="server" Text="Billable" CommandName="Sort"
                        CommandArgument="IsChargeable" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsMilestones" runat="server" SelectMethod="GetProjectMilestonesFinancials"
    TypeName="PraticeManagement.ProjectService.ProjectServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="projectId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

