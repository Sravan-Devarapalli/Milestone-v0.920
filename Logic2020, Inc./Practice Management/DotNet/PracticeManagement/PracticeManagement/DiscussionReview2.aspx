<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DiscussionReview2.aspx.cs" Inherits="PraticeManagement.DiscussionReview2" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controls/Opportunities/OpportunityListControl.ascx" TagName="OpportunityList"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/OpportunityFilter.ascx" TagName="OpportunityFilter"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Opportunities List</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunities List
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="pnlBody" runat="server">
        <ContentTemplate>
            <uc2:OpportunityList ID="opportunities" runat="server" FilterMode="GenericFilter"
                AllowAutoRedirectToDetails="true" OnFilterOptionsChanged="ofOpportunityList_OnFilterOptionsChanged" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

