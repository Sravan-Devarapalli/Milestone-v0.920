<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectMilestonesFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectMilestonesFinancials" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:GridView ID="gvRevenueMilestones" runat="server" AutoGenerateColumns="false"
    EmptyDataText="No milestones have been created for this project." OnRowDataBound="gvRevenueMilestones_RowDataBound"
    CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White" DataSourceID="odsMilestones">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <Columns>
        <asp:TemplateField HeaderText="Milestone Name">
            <HeaderTemplate>
                <div class="ie-bg">
                    Milestone Name</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlMilestoneName" runat="server" NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId")) %>'
                    Text='<%# HttpUtility.HtmlEncode((string)Eval("MilestoneName")) %>' onclick='<%# "javascript:checkDirty(\"" + MILESTONE_TARGET + "\", " + Eval("MilestoneId") + ")" %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    Start Date</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("d") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    End Date</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# ((DateTime)Eval("ProjectedDeliveryDate")).ToString("d") %>' />
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
                    Revenue</div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("Revenue")).ToString() %>' CssClass="Revenue"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Gross Margin">
            <HeaderTemplate>
                <div class="ie-bg">
                    Gross Margin</div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblEstimatedMargin" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("GrossMargin")).ToString() %>' CssClass="Margin" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin %">
            <HeaderTemplate>
                <div class="ie-bg">
                    Margin %</div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblTargetMargin" runat="server" Text='<%# string.Format(PraticeManagement.Constants.Formatting.PercentageFormat, Eval("TargetMargin") ?? 0) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Billable">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    Billable</div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Consultants can adjust">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    Consultants can adjust</div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool)Eval("ConsultantsCanAdjust")) ? "Yes" : "No"%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsMilestones" runat="server" 
    SelectMethod="GetProjectMilestonesFinancials" 
    TypeName="PraticeManagement.ProjectService.ProjectServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="projectId" QueryStringField="id" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

