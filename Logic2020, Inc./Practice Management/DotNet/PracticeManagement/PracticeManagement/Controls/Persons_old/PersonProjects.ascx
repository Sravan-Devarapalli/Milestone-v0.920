<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonProjects.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonProjects" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="false" EmptyDataText="The is nothing to be displayed here."
    OnRowDataBound="gvProjects_RowDataBound" ShowFooter="true">
    <Columns>
        <asp:TemplateField HeaderText="Project">
            <ItemTemplate>
                <asp:LinkButton ID="btnProject" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Milestone.Project.Name")) %>'
                    CommandArgument='<%# Eval("Milestone.Project.Id") %>' OnCommand="btnProject_Command"
                    Visible='<%# UserIsAdministrator %>'></asp:LinkButton>
                <asp:Label ID="lblProject" runat="server" Visible='<%# !UserIsAdministrator %>' Text='<%# HttpUtility.HtmlEncode((string)Eval("Milestone.Project.Name")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Milestone">
            <ItemTemplate>
                <asp:LinkButton ID="btnMilestone" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Milestone.Description")) %>'
                    CommandArgument='<%# string.Concat(Eval("Milestone.Id"), "_", Eval("Milestone.Project.Id")) %>'
                    OnCommand="btnMilestone_Command" Visible='<%# UserIsAdministrator %>'></asp:LinkButton>
                <asp:Label ID="lblMilestone" runat="server" Visible='<%# !UserIsAdministrator %>'
                    Text='<%# HttpUtility.HtmlEncode((string)Eval("Milestone.Description")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="lblProjectStatus" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Role">
            <ItemTemplate>
                <asp:Label ID="lblRoleName" runat="server" Text='<%# Eval("Entries[0].Role.Name") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date">
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# ((DateTime)Eval("Entries[0].StartDate")).ToShortDateString() %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <strong>Totals: </strong>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <ItemTemplate>
                <asp:Label ID="lblProjectedDeliveryDate" runat="server" Text='<%# ((DateTime)Eval("Milestone.ProjectedDeliveryDate")).ToShortDateString() %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblOverallMargin" runat="server" ToolTip="Overall Margin = Total Margin / Total Revenue"
                    Font-Bold="true"></asp:Label>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Revenue" FooterText="Total Projects Revenue">
            <ItemTemplate>
                <asp:Label ID="lblRevenue" runat="server" Text='<%# Eval("ComputedFinancials.Revenue") %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblTotalProjectsRevenue" runat="server" ToolTip="Total Projects Revenue"
                    Font-Bold="true"></asp:Label>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin" FooterText="Total Projects Margin">
            <ItemTemplate>
                <asp:Label ID="lblGrossMargin" runat="server" Text='<%# Eval("ComputedFinancials.GrossMargin") %>'></asp:Label>
            </ItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblTotalProjectsMargin" runat="server" ToolTip="Total Projects Margin"
                    Font-Bold="true"></asp:Label>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <AlternatingRowStyle BackColor="#f5f5f5" />
    <RowStyle Wrap="false" />
</asp:GridView>
<%--<AjaxControlToolkit:TabContainer ID="tabConsReport" runat="server">
    <AjaxControlToolkit:TabPanel runat="server" ID="tabConsReportTable" HeaderText="Table">
        <ContentTemplate>
        </ContentTemplate>
    </AjaxControlToolkit:TabPanel>
    <AjaxControlToolkit:TabPanel runat="server" ID="tabConsReportChart" HeaderText="Chart">
        <ContentTemplate>
            <asp:Chart ID="personDetailsChart" runat="server" 
                Width="1000px"
                Height="500px">                
                <Series>
                    <asp:Series Name="Revenue" Legend="Person Details Legend" 
                        ChartType="SplineArea">
                    </asp:Series>
                    <asp:Series Name="Margin" Legend="Person Details Legend" 
                    ChartType="SplineArea" />
               </Series>
                <Legends>
                    <asp:Legend Name="Person Details Legend" Alignment="Center" Docking="Bottom">
                    </asp:Legend>
                </Legends>
                <ChartAreas>
                    <asp:ChartArea>
                        <AxisX IsLabelAutoFit="false" Interval="1" LabelAutoFitStyle="LabelsAngleStep90">
                            <LabelStyle Angle="60" />
                            <MajorGrid Enabled="true" />
                        </AxisX>
                        <AxisY>
                            <MajorGrid LineColor="Gray" LineDashStyle="Dash" />
                        </AxisY>
                    </asp:ChartArea>
                </ChartAreas>
            </asp:Chart>
        </ContentTemplate>
    </AjaxControlToolkit:TabPanel>
</AjaxControlToolkit:TabContainer>
--%><%--<asp:ObjectDataSource ID="obsProjects" runat="server" 
    SelectMethod="GetMilestonePersonListByPerson" 
    TypeName="PraticeManagement.MilestonePersonService.MilestonePersonServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="personId" QueryStringField="id" Type="Object" />
    </SelectParameters>
</asp:ObjectDataSource>--%>

