<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentNotes.ascx.cs" Inherits="PraticeManagement.Controls.Generic.RecentNotes" %>
<%@ Import Namespace="DataTransferObjects" %>
<asp:UpdatePanel ID="pnlCanvas" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlRecentNotes" runat="server" Height="100px" ScrollBars="Vertical" BackColor="White">
            <asp:ListView ID="lvRecentNotes" runat="server" class="WholeWidth" DataSourceID="odsActivities" >    
                <LayoutTemplate>
                    <table>
                        <tr runat="server" id="itemPlaceholder" />
                    </table>
                </LayoutTemplate>
                 <ItemTemplate>
                    <tr runat="server">
                        <td>
                            <asp:Label runat="server" Text='<%# GetContent((ActivityLogItem)Container.DataItem) %>' />
                        </td>
                    </tr>
                </ItemTemplate>    
            </asp:ListView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:ObjectDataSource ID="odsActivities" runat="server" TypeName="PraticeManagement.Controls.Generic.RecentNotes"
    SelectCountMethod="GetActivitiesCount" SelectMethod="GetActivities">
    <SelectParameters>
        <asp:ControlParameter ControlID="rnRecentNotest" Name="periodFilter" PropertyName="PeriodFilter"
            Type="String" />
        <asp:ControlParameter ControlID="rnRecentNotest" Name="personId" PropertyName="PersonId"
            Type="String" />
        <asp:ControlParameter ControlID="rnRecentNotest" Name="sourceFilter" PropertyName="SourceFilter"
            Type="String" />
        <asp:ControlParameter ControlID="rnRecentNotest" Name="projectId" Type="String" ConvertEmptyStringToNull="true" 
            PropertyName="ProjectId"/>
        <asp:ControlParameter ControlID="rnRecentNotest" Name="opportunityId" PropertyName="OpportunityId"
            Type="String" ConvertEmptyStringToNull="true" />
        <asp:ControlParameter ControlID="rnRecentNotest" Name="milestoneId" PropertyName="MilestoneId"
            Type="String" ConvertEmptyStringToNull="true" />                 
    </SelectParameters>
</asp:ObjectDataSource>
