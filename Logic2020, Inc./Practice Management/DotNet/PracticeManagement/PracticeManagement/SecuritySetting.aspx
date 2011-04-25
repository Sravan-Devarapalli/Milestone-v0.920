<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SecuritySetting.aspx.cs" Inherits="PraticeManagement.SecuritySetting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Security Setting</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Security Setting
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <act:TabContainer ID="tabSettings" runat="server" CssClass="CustomTabStyle">
        <act:TabPanel runat="server" ID="tpnlGeneral">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>General</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlGeneral" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlSecurityRoles">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Security Roles</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlSecurityRoles" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlSeniorityLevels">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Seniority Levels</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlSeniorityLevels" runat="server">
                    <contenttemplate>
                    Page Not Exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
    </act:TabContainer>
</asp:Content>

