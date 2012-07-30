<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="ConsultantUtilTimelineReport.aspx.cs" Inherits="PraticeManagement.Config.ConsultantUtilTimelineReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/UTilTimelineFilter.ascx" TagName="UtilTimelineFilter"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Consulting Utilization Report | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Reports
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="updFilters" runat="server">
        <ContentTemplate>
            <uc:UtilTimelineFilter ID="utf" BtnSaveReportVisible="true" BtnUpdateViewVisible="false"
                IsSampleReport="true" BtnResetFilterVisible="false" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc3:LoadingProgress ID="progress" runat="server" />
    <br />
    <asp:HyperLink ID="hlnkConsultantUtilTimeLineReport" Target="_blank" runat="server"
        NavigateUrl="~/Reports/ConsultantUtilTimeLineReport.aspx?refresh=10" Text="Consulting Utilization report"></asp:HyperLink>
    <br />
    <asp:Label ID="lblSaveMessage" runat="server" EnableViewState="false" />
</asp:Content>

