<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="EmailTemplates.aspx.cs" Inherits="PraticeManagement.EmailTemplates" %>

<%@ Register Src="~/Controls/Configuration/EmailTemplates.ascx" TagName="EmailTemplates"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Configuration/SmtpPopSetUpControl.ascx" TagName="Smtp"
    TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="act" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Email Templates</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Email Templates
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <act:TabContainer ID="tabSettings" runat="server" CssClass="CustomTabStyle">
        <act:TabPanel runat="server" ID="tpnlSmtpPopSetup">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>SMTP Setup</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
               <uc:Smtp ID="SmtpSetUp" runat="server" />
            </ContentTemplate>
        </act:TabPanel>
        <act:TabPanel runat="server" ID="tpnlEmailTemplates">
            <HeaderTemplate>
                <span class="bg"><a href="#"><span>E-Mail Templates</span></a> </span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upnlEmailTemplates" runat="server">
                    <contenttemplate>
                        <uc:EmailTemplates ID="EmailTemplates1" runat="server" />
                    </contenttemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </act:TabPanel>
    </act:TabContainer>
</asp:Content>

