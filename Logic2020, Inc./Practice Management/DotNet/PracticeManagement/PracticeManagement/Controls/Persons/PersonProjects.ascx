<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonProjects.ascx.cs"
    Inherits="PraticeManagement.Controls.Persons.PersonProjects" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/ProjectNameCellRounded.ascx" TagName="ProjectNameCellRounded" TagPrefix="uc" %>
<asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="False" EmptyDataText="There is nothing to be displayed here"
    OnRowDataBound="gvProjects_RowDataBound" ShowFooter="True" DataSourceID="odsProjects"
    CssClass="CompPerfTable WholeWidth" GridLines="None" BackColor="White" HeaderStyle-Width="10px" HeaderStyle-Wrap="True">
    <AlternatingRowStyle BackColor="#F9FAFF" />
    <RowStyle Wrap="true" />    
    <Columns>        
        <asp:TemplateField>    
            <HeaderTemplate>
                <div class="ie-bg"></div>
            </HeaderTemplate>       
            <ItemStyle/>            
            <ItemTemplate>
                <div class="cell-pad">
                    <uc:ProjectNameCellRounded ID="crStatus" runat="server" ToolTipOffsetX="5" ToolTipOffsetY="-25" 
                    ButtonProjectNameToolTip='<%# GetProjectNameCellToolTip((int)Eval("ProjectStatusId"),(object) Eval("FileName"),(string)Eval("ProjectStatus")) %>' 
                    ButtonCssClass='<%# GetProjectNameCellCssClass((int)Eval("ProjectStatusId") , (object) Eval("FileName") )%>' />
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Project#">
            <HeaderTemplate>
                <div class="ie-bg">Project#</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod"/>            
            <ItemTemplate>
                <div class="cell-pad"> <%# Eval("ProjectNumber") %></div>                
            </ItemTemplate>            
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg">Project</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlProject" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("ProjectName")) %>'
                    NavigateUrl='<%# GetProjectRedirectUrl(Eval("ProjectId")) %>'
                    onclick='<%# "javascript:checkDirty(\"" + PROJECT_TARGET + "\", " + Eval("ProjectId") + ")" %>'/>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <div class="ie-bg">Milestone</div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlMilestone" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("MilestoneName")) %>'
                    NavigateUrl='<%# GetMilestoneRedirectUrl(Eval("MilestoneId"), Eval("ProjectId")) %>'
                    onclick='<%# "javascript:checkDirty(\"" + MILESTONE_TARGET + "\", " + string.Format("\"{0}:{1}\"", Eval("MilestoneId"), Eval("ProjectId")) + ")" %>'/>
            </ItemTemplate>
        </asp:TemplateField>        
        <asp:TemplateField HeaderText="Role">
            <HeaderTemplate>
                <div class="ie-bg">Role</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod" />
            <ItemTemplate>
               <div class="cell-pad"> <%# Eval("RoleName") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Start Date">
            <HeaderTemplate>
                <div class="ie-bg">Start Date</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod" />
            <ItemTemplate>
                <div class="cell-pad"><asp:Label ID="Label2" runat="server" 
                    Text='<%# Bind("StartDate", "{0:MM/dd/yyyy}") %>'></asp:Label></div>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("StartDate") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="End Date">
            <HeaderTemplate>
                <div class="ie-bg">End Date</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod" />
            <ItemTemplate>
                <div class="cell-pad"><asp:Label ID="Label3" runat="server" 
                    Text='<%# Bind("EndDate", "{0:MM/dd/yyyy}") %>'></asp:Label></div>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("EndDate") %>'></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblOverallMargin" runat="server" ToolTip="Overall Margin" Font-Bold="true"/>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Revenue" FooterText="Total Projects Revenue">
            <HeaderTemplate>
                <div class="ie-bg">Revenue</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod" />
            <ItemTemplate>
                <div class="cell-pad"><asp:Label ID="lblRevenue" runat="server" Text='<%# ((PracticeManagementCurrency)((decimal)Eval("Revenue"))).ToString() %>' CssClass="Revenue"></asp:Label></div>
            </ItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblTotalProjectsRevenue" runat="server" ToolTip="Total Projects Revenue" Font-Bold="true"/>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Margin" FooterText="Total Projects Margin">
            <HeaderTemplate>
                <div class="ie-bg">Margin</div>
            </HeaderTemplate>
            <ItemStyle CssClass="CompPerfPeriod" />
            <ItemTemplate>
                <div class="cell-pad"><asp:Label ID="lblGrossMargin" runat="server" Text='<%# ((PracticeManagementCurrency)((decimal)Eval("GrossMargin"))).ToString() %>' CssClass="Margin"></asp:Label></div>
            </ItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lblTotalProjectsMargin" runat="server" ToolTip="Total Projects Margin" Font-Bold="true"/>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <AlternatingRowStyle BackColor="#f5f5f5" />
    <RowStyle Wrap="false" />
</asp:GridView>
<asp:ObjectDataSource ID="odsProjects" runat="server" 
    SelectMethod="GetPersonMilestoneWithFinancials" 
    TypeName="PraticeManagement.PersonService.PersonServiceClient">
    <SelectParameters>
        <asp:QueryStringParameter Name="personId" QueryStringField="id" Type="Object" />
    </SelectParameters>
</asp:ObjectDataSource>

