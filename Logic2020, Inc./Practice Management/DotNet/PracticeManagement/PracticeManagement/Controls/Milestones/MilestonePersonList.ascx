<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MilestonePersonList.ascx.cs"
    Inherits="PraticeManagement.Controls.Milestones.MilestonePersonList" %>
<asp:GridView ID="gvPeople" runat="server" AutoGenerateColumns="False" BorderStyle="None"
    BorderWidth="1" EmptyDataText="There is nothing to be displayed." OnRowDataBound="gvPeople_RowDataBound"
    ShowFooter="true" CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <RowStyle BackColor="White" />
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Person Name</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="btnPersonLink" runat="server" NavigateUrl='<%# GetMpeRedirectUrl(Eval("Id")) %>'
                    Text='<%# HttpUtility.HtmlEncode(string.Format("{0}, {1}", Eval("Person.LastName"), Eval("Person.FirstName"))) %>'
                    onclick='<%# "javascript:checkDirty(" + Eval("Id") + ")" %>' />
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Role</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblPersonRole" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].Role.Name") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap" style="white-space: nowrap;">
                    Start Date</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" CssClass="spacing" Text='<%# ((DateTime)Eval("Entries[0].StartDate")).ToString("d") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End">
            <HeaderTemplate>
                <div class="ie-bg no-wrap" style="white-space: nowrap;">
                    End Date</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" CssClass="spacing" Text='<%# ((DateTime?)Eval("Entries[0].EndDate")).HasValue ? ((DateTime)Eval("Entries[0].EndDate")).ToString("d") : string.Empty %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle HorizontalAlign="Right" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Eff. Bill Rate</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblEffectiveBillRate" runat="server" CssClass="spacing" Text='<%# Eval("Entries[0].ComputedFinancials.BillRateMinusDiscount")%>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                Totals by months
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total Hours">
            <ItemStyle HorizontalAlign="Right" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    &#931 Hours</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblTotalHours" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.HoursBilled") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle HorizontalAlign="Right" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Revenue</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblRevenueContribution" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    Margin</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblMarginContribution" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.GrossMargin") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle BorderStyle="None" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    &#931 Revenue</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblTotalRevenue" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.Revenue") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle VerticalAlign="Top" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg no-wrap">
                    &#931 Gross Margin</div>
                </th>
            </HeaderTemplate>
            <ItemStyle Font-Bold="true" HorizontalAlign="Right" />
            <ItemTemplate>
                <asp:Label ID="lblTotalMargin" runat="server" Text='<%# Eval("Entries[0].ComputedFinancials.GrossMargin") %>'></asp:Label>
            </ItemTemplate>
            <FooterStyle HorizontalAlign="Right" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>

