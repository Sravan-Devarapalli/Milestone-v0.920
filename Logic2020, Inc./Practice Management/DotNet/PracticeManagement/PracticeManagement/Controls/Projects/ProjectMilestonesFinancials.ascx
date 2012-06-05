<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectMilestonesFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectMilestonesFinancials" %>
<%@ Import Namespace="DataTransferObjects" %>
<style>
    .wrapMilestoneName
    {
        word-wrap: break-word !important; /* Internet Explorer 5.5+ */
        word-break: break-all;
        white-space: normal;
    }
</style>
<asp:GridView ID="gvRevenueMilestones" runat="server" AutoGenerateColumns="false"
    AllowSorting="true" EmptyDataText="No milestones have been created for this project."
    OnRowDataBound="gvRevenueMilestones_RowDataBound" CssClass="CompPerfTable WholeWidth"
    GridLines="None" BackColor="White" DataSourceID="odsMilestones" OnSorting="gvRevenueMilestones_Sorting">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <Columns>
        <asp:TemplateField>
            <ItemStyle Width="3%" />
            <ItemTemplate>
                <asp:ImageButton ID="imgbtnEdit" CommandName="edit" runat="server" ToolTip="Edit Milestone Name"
                    ImageUrl="~/Images/icon-edit.png" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imgbtnUpdate" runat="server" ToolTip="Save" OnClick="imgbtnUpdate_OnClick"
                    ImageUrl="~/Images/icon-check.png" />
                <asp:ImageButton ID="imgbtnCancel" CommandName="cancel" runat="server" ToolTip="Cancel"
                    ImageUrl="~/Images/no.png" />
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Milestone Name">
            <ItemStyle Width="35%" CssClass="wrapMilestoneName" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbMilestoneName" runat="server" Text="Milestone Name" CommandName="Sort"
                        CommandArgument="MilestoneName" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlMilestoneName" runat="server" NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId")) %>'
                    Text='<%# GetWrappedTest(HttpUtility.HtmlEncode((string)Eval("MilestoneName"))) %>'
                    onclick='<%# "javascript:checkDirty(\"" + MILESTONE_TARGET + "\", " + Eval("MilestoneId") + ")" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="tbMilestoneName" runat="server" MilestoneId='<%# Eval("MilestoneId") %>' Text='<%# Bind("MilestoneName") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMilestoneName" runat="server" ControlToValidate="tbMilestoneName" Text="*" ErrorMessage="Milestone Name Required"
                    ToolTip="Milestone Name Required"></asp:RequiredFieldValidator>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date">
            <ItemStyle HorizontalAlign="Center" Width="9%" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbStartDate" runat="server" Text="Start Date" CommandName="Sort"
                        CommandArgument="StartDate" /></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <ItemStyle HorizontalAlign="Center" Width="9%" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbEndDate" runat="server" Text="End Date" CommandName="Sort"
                        CommandArgument="ProjectedDeliveryDate" /></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime)Eval("ProjectedDeliveryDate")).ToString("MM/dd/yyyy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <%--<asp:TemplateField HeaderText="Expected Hours">
            <HeaderTemplate>
                <div class="ie-bg">
                    Expected Hrs</div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblExpectedHours" runat="server" Text='<%# Eval("ExpectedHours") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Actual Hours">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    Actual Hrs</div>
            </HeaderTemplate>
            <ItemTemplate>
                &nbsp;</ItemTemplate>
        </asp:TemplateField>--%>
        <asp:TemplateField HeaderText="Revenue">
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbRevenue" runat="server" Text="Revenue" CommandName="Sort" CommandArgument="Revenue" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" Width="7%" />
            <ItemTemplate>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("Revenue")).ToString() %>'
                    CssClass="Revenue"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Gross Margin">
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbGrossMargin" runat="server" Text="Gross Margin" CommandName="Sort"
                        CommandArgument="GrossMargin" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" Width="11%" />
            <ItemTemplate>
                <asp:Label ID="lblEstimatedMargin" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("GrossMargin")).ToString() %>'
                    NegativeValue='<%# (decimal) Eval("GrossMargin") < 0 %>' CssClass="Margin" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin %">
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbMargin" runat="server" Text="Margin %" CommandName="Sort" CommandArgument="TargetMargin" />
                </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" Width="7%" />
            <ItemTemplate>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(PraticeManagement.Constants.Formatting.PercentageFormat, Eval("TargetMargin") ?? 0) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Billable">
            <ItemStyle HorizontalAlign="Center" Width="7%" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbBillable" runat="server" Text="Billable" CommandName="Sort"
                        CommandArgument="IsChargeable" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>
        <%--<asp:TemplateField HeaderText="Consultants can adjust">
            <ItemStyle HorizontalAlign="Center" Width="16%" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbConsultantsCanAdjust" runat="server" Text="Consultants can adjust"
                        CommandName="Sort" CommandArgument="ConsultantsCanAdjust" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool)Eval("ConsultantsCanAdjust")) ? "Yes" : "No"%>
            </ItemTemplate>
        </asp:TemplateField>--%>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsMilestones" runat="server" SelectMethod="GetProjectMilestonesFinancials"
    TypeName="PraticeManagement.ProjectService.ProjectServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="projectId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

