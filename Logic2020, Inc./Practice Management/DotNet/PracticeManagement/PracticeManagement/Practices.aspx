<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
 CodeBehind="Practices.aspx.cs" Inherits="PraticeManagement.Practices" %>
<%@ Register Src="~/Controls/Configuration/Practices.ascx" TagPrefix="uc" TagName="Practices" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Practice Areas</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Practice Areas
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <uc:Practices ID="ucPractices" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
