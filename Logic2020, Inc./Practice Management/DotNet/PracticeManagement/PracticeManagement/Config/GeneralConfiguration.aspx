<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralConfiguration.aspx.cs"
    Inherits="PraticeManagement.Config.GeneralConfiguration" MasterPageFile="~/PracticeManagementMain.Master"
    Title="Practice Management - Time Entry Configuration" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<%@ Register Src="~/Controls/TimeEntry/TimeEntryManagement.ascx" TagPrefix="uc" TagName="TeManage" %>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<%@ Register Src="~/Controls/BrandingSettingsControl.ascx" TagPrefix="uc" TagName="Branding" %>
<%@ Register Src="~/Controls/ClientListControl.ascx" TagPrefix="uc" TagName="ClientList" %>
<%@ Register Src="~/Controls/DefaultMilestoneSettingControl.ascx" TagPrefix="uc"
    TagName="DefaultMilestoneSetting" %>
<%@ Register Src="~/Controls/PersonListControl.ascx" TagPrefix="uc" TagName="PersonList" %>
<%@ Register Src="~/Controls/Configuration/Practices.ascx" TagPrefix="uc" TagName="Practices" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - General Configuration</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    General Configuration
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <act:TabContainer ID="tabSettings" runat="server" CssClass="CustomTabStyle">
        <act:TabPanel runat="server" ID="tpnlBranding" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Branding/Logo</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="updBranding" runat="server">
                    <contenttemplate>
                        <uc:Branding ID="ucBranding" runat="server" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlClientList" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Clients</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <contenttemplate>
                        <uc:ClientList ID="ucClientList" runat="server" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlDefaultManager" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Default Line Manager</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="updDefaultManager" runat="server">
                    <contenttemplate>
                        <uc:DefaultManager ID="defaultManager" runat="server" AllowChange="true" PersonsRole="Practice Area Manager" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlOpportunityPriorities" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Opportunity Priorities</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="udpOpportunityPriorities" runat="server">
                    <contenttemplate>
                        Page does not exist yet.
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlPersons" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Persons</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="updPersonList" runat="server">
                    <contenttemplate>
                        <uc:PersonList ID="ucPersonList" runat="server" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlPractices" Visible="false">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>Practice Areas</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlPractices" runat="server">
                    <contenttemplate>
                        <uc:Practices ID="ucPractices" runat="server" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
    </act:TabContainer>
</asp:Content>

