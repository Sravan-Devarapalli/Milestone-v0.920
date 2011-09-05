<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="OpportunityList.aspx.cs" Inherits="PraticeManagement.OpportunityList"
    Title="Practice Management - Opportunity List" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/Generic/OpportunityList.ascx" TagName="OpportunityList"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/OpportunityFilter.ascx" TagName="OpportunityFilter"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity List | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity List
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/ScrollinDropDown.js" type="text/javascript"></script>
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr>
                        <td>
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                ImageControlID="btnExpandCollapseFilter" CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg"
                                CollapseControlID="btnExpandCollapseFilter" ExpandControlID="btnExpandCollapseFilter"
                                Collapsed="True" TextLabelID="lblFilter" />
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                ToolTip="Expand Filters" />
                        </td>
                        <td>
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpeSummary" runat="Server" TargetControlID="pnlSummary"
                                ImageControlID="btnExpandCollapseSummary" CollapsedImage="Images/expand.jpg"
                                ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseSummary"
                                ExpandControlID="btnExpandCollapseSummary" Collapsed="True" TextLabelID="lblSummary" />
                            <asp:Label ID="lblSummary" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseSummary" runat="server" ImageUrl="~/Images/collapse.jpg"
                                ToolTip="Expand Summary Details" />
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:ShadowedHyperlink runat="server" Text="Add Opportunity" ID="lnkAddOpportunity"
                                CssClass="add-btn" NavigateUrl="~/OpportunityDetail.aspx?returnTo=OpportunityList.aspx" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel CssClass="filters" ID="pnlFilters" runat="server">
                <uc:OpportunityFilter ID="ofOpportunityList" runat="server" />
            </asp:Panel>
            <asp:Panel CssClass="summary" Style="white-space: nowrap; overflow-x: auto;" ID="pnlSummary"
                runat="server">
            </asp:Panel>
            <br />
            <uc2:OpportunityList ID="opportunities" runat="server" FilterMode="GenericFilter"
                AllowAutoRedirectToDetails="true" OnFilterOptionsChanged="ofOpportunityList_OnFilterOptionsChanged" />
            <table width="100%" >
                <tr>
                    <td style="padding:5px;" align="right">
                        <asp:Button ID="btnExportToExcel" runat="server" OnClick="btnExportToExcel_Click"
                            Text="Export" CssClass="pm-button" />
                        <asp:GridView ID="excelGrid" runat="server" Visible="false" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc1:LoadingProgress ID="loadingProgress" runat="server" />
</asp:Content>

