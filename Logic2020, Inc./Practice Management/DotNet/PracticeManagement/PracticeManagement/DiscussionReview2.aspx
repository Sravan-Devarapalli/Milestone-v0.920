<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DiscussionReview2.aspx.cs" Inherits="PraticeManagement.DiscussionReview2" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register Src="~/Controls/Opportunities/OpportunityListControl.ascx" TagName="OpportunityList"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/OpportunityFilter.ascx" TagName="OpportunityFilter"
    TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Opportunity Summary | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Opportunity Summary
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <link href="Css/datepicker.css" rel="stylesheet" type="text/css" />
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

