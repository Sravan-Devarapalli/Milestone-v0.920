<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="DiscussionReview2.aspx.cs" Inherits="PraticeManagement.DiscussionReview2" %>
<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons" Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register assembly="PraticeManagement" namespace="PraticeManagement.Controls.Generic" tagprefix="cc1" %>
<%@ Register src="Controls/Generic/LoadingProgress.ascx" tagname="LoadingProgress" tagprefix="uc1" %>
<%@ Register src="~/Controls/Opportunities/OpportunityListControl.ascx" tagname="OpportunityList" tagprefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/OpportunityFilter.ascx" TagName="OpportunityFilter" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Opportunities List</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunities List
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td>
                             <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server"
                                            TargetControlID="pnlFilters" ImageControlID="btnExpandCollapseFilter"
                                            CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                            ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />                        
                             <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                             <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />
                       </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                             <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
                                Text="Export" CssClass="pm-button" />
                            <asp:GridView ID="excelGrid" runat="server" Visible="false" />
                            <asp:Button ID="btnResetSort" runat="server" Text="Reset Filter" CssClass="pm-button"
                                OnClick="btnResetSort_OnClick" Width="80px" />
                       </td>
                        <td>
                             <asp:ShadowedHyperlink runat="server" Text="Add Opportunity" ID="lnkAddOpportunity" CssClass="add-btn" NavigateUrl="~/OpportunityDetail.aspx?returnTo=OpportunityList.aspx"/>
                       </td>
                    </tr>
                </table>
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <uc:OpportunityFilter ID="ofOpportunityList" runat="server" OnFilterOptionsChanged="ofOpportunityList_OnFilterOptionsChanged" />
            </asp:Panel>
            <br />
            <uc2:OpportunityList id="opportunities" runat="server" FilterMode="GenericFilter"
                AllowAutoRedirectToDetails="true" OnFilterOptionsChanged="ofOpportunityList_OnFilterOptionsChanged" />            
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc1:LoadingProgress ID="loadingProgress" runat="server" />    
</asp:Content>

