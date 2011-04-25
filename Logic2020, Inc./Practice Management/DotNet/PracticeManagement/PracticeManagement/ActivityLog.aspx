<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ActivityLog.aspx.cs" Inherits="PraticeManagement.ActivityLog" Title="Practice Management - User Activity Log" %>

<%@ Register Src="~/Controls/ActivityLogControl.ascx" TagPrefix="uc" TagName="ActivityLogControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Src="~/Controls/Generic/BecomeUser.ascx" TagPrefix="uc" TagName="BecomeUser" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Troubleshooting</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Troubleshooting
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <act:TabContainer ID="tabSettings" runat="server" CssClass="CustomTabStyle">
        <act:TabPanel runat="server" ID="tpnlActivityLog">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Activity Log</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlActivityLog" runat="server">
                    <contenttemplate>
                    <uc:ActivityLogControl runat="server" ID="activityLog" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlBecomeUser">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Become User</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlBecomeUser" runat="server">
                    <contenttemplate>
                    <uc:BecomeUser runat="server" ID="becomeUser" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlHelp">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Help</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlHelp" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlSiteMap">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Site Map</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlSiteMap" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlSupportLink">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Support Link</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlSupportLink" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
    </act:TabContainer>
</asp:Content>

