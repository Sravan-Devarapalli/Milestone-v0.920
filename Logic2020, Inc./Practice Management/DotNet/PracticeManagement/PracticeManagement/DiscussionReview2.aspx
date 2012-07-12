<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DiscussionReview2.aspx.cs" Inherits="PraticeManagement.DiscussionReview2" %>
    
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Opportunities/OpportunityListControl.ascx" TagName="OpportunityList"
    TagPrefix="uc2" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Summary | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Summary
</asp:Content>
<asp:Content ID="cntHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/datepicker.min.css", this) %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        .ConfirmBoxClassError
        {
            min-height: 60px;
            min-width: 200px;
            max-width: 600px;
            max-height: 500px;
        }
        .strawman tr td
        {
            padding: 4px 0px 4px 0px;
        }
    </style>
   
    <uc2:OpportunityList ID="opportunities" runat="server" FilterMode="GenericFilter"
        AllowAutoRedirectToDetails="true" OnFilterOptionsChanged="ofOpportunityList_OnFilterOptionsChanged" />
</asp:Content>

