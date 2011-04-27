<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="ConsultantUtilTimelineReport.aspx.cs" Inherits="PraticeManagement.Config.ConsultantUtilTimelineReport" %>
<%@ Register Src="~/Controls/Reports/UTilTimelineFilter.ascx" TagName="UtilTimelineFilter"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc3" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Reports</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Reports
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="Scripts/ScrollinDropDown.js"></script>
    <asp:UpdatePanel ID="updFilters" runat="server">
        <ContentTemplate>
            <uc:UtilTimelineFilter ID="utf" BtnSaveReportVisible="true" BtnUpdateViewVisible="false"
                IsSampleReport="true" BtnResetFilterVisible="false" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc3:LoadingProgress ID="progress" runat="server" />
    <br />
    <asp:HyperLink ID="hlnkConsultantUtilTimeLineReport" Target="_blank" runat="server"
        NavigateUrl="~/Reports/ConsultantUtilTimeLineReport.aspx?refresh=10" Text="Consultant Util Timeline report"></asp:HyperLink>
    <br />
    <asp:Label ID="lblSaveMessage" runat="server" EnableViewState="false" />
</asp:Content>

