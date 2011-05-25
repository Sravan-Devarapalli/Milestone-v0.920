<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectMilestonesFinancials.ascx.cs"
    Inherits="PraticeManagement.Controls.Projects.ProjectMilestonesFinancials" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:gridview id="gvRevenueMilestones" runat="server" autogeneratecolumns="false"
    allowsorting="true" emptydatatext="No milestones have been created for this project."
    onrowdatabound="gvRevenueMilestones_RowDataBound" cssclass="CompPerfTable WholeWidth"
    gridlines="None" backcolor="White" datasourceid="odsMilestones" onsorting="gvRevenueMilestones_Sorting">
    <alternatingrowstyle backcolor="#F9FAFF" />
    <columns>
        <asp:TemplateField HeaderText="Milestone Name">
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbMilestoneName" runat="server" Text="Milestone Name" CommandName="Sort" CommandArgument="MilestoneName"/>
                    </div>
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
                    <asp:LinkButton ID="lbStartDate" runat="server" Text="Start Date" CommandName="Sort" CommandArgument="StartDate" /></div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                    <asp:LinkButton ID="lbEndDate" runat="server" Text="End Date" CommandName="Sort" CommandArgument="ProjectedDeliveryDate" /></div>
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
                <asp:LinkButton ID="lbRevenue" runat="server" Text="Revenue" CommandName="Sort" CommandArgument="Revenue"/>
                    </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("Revenue")).ToString() %>' CssClass="Revenue"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Gross Margin">
            <HeaderTemplate>
                <div class="ie-bg">
                <asp:LinkButton ID="lbGrossMargin" runat="server" Text="Gross Margin" CommandName="Sort" CommandArgument="GrossMargin" />
                    </div>
            </HeaderTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
                <asp:Label ID="lblEstimatedMargin" runat="server" Text='<%# ((PracticeManagementCurrency) (decimal) Eval("GrossMargin")).ToString() %>' CssClass="Margin" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin %">
            <HeaderTemplate>
                <div class="ie-bg">
                <asp:LinkButton ID="lbMargin" runat="server" Text="Margin %" CommandName="Sort" CommandArgument="TargetMargin" />
                    </div>
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
                <asp:LinkButton ID="lbBillable" runat="server" Text="Billable" CommandName="Sort" CommandArgument="IsChargeable" />
                    </div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool) Eval("IsChargeable")) ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Consultants can adjust">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderTemplate>
                <div class="ie-bg">
                <asp:LinkButton ID="lbConsultantsCanAdjust" runat="server" Text="Consultants can adjust" CommandName="Sort" CommandArgument="ConsultantsCanAdjust" />
                    </div>
            </HeaderTemplate>
            <ItemTemplate>
                <%# ((bool)Eval("ConsultantsCanAdjust")) ? "Yes" : "No"%>
            </ItemTemplate>
        </asp:TemplateField>
    </columns>
</asp:gridview>
<asp:objectdatasource id="odsMilestones" runat="server" selectmethod="GetProjectMilestonesFinancials"
    typename="PraticeManagement.ProjectService.ProjectServiceClient">
    <selectparameters>
        <asp:QueryStringParameter Name="projectId" QueryStringField="id" Type="Int32" />
    </selectparameters>
</asp:objectdatasource>

