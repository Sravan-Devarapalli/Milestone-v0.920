<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConsultantMilestones.ascx.cs" Inherits="PraticeManagement.Controls.Reports.ConsultantMilestones" %>
<h4><asp:Label ID="lblTitle" runat="server" /></h4>
<asp:GridView ID="dvConsultantMilestones" runat="server" AllowSorting="True" 
    DataSourceID="odsConsultantMilestones" AutoGenerateColumns="False" 
    onrowdatabound="dvConsultantMilestones_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="Project Name" SortExpression="ProjectName">
            <ItemTemplate>
                <%# Eval("ProjectName") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Milestone" SortExpression="Description">
            <ItemTemplate>
                <%# Eval("Description")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
            <ItemTemplate> 
                <%# ((DateTime)Eval("StartDate")).ToShortDateString()%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
            <ItemTemplate>
                <%# ((DateTime)Eval("EndDate")).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="H/D" SortExpression="HoursPerDay">
            <ItemTemplate>
                <%# Eval("HoursPerDay")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
            <ItemTemplate>
                <%# Eval("Amount")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Util." SortExpression="wutil">
            <ItemTemplate>
                <%# Eval("wutil")%>%
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsConsultantMilestones" runat="server" 
    SelectMethod="GetConsultantMilestones" 
    TypeName="PraticeManagement.MilestonePersonService.MilestonePersonServiceClient">
    <SelectParameters>
        <asp:Parameter Name="personId" Type="Int32" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="startDate" Type="DateTime" ConvertEmptyStringToNull="true" />
        <asp:Parameter Name="endDate" Type="DateTime" ConvertEmptyStringToNull="true" />
    </SelectParameters>
</asp:ObjectDataSource>


