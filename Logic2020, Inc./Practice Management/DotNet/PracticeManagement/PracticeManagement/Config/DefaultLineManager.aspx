<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="DefaultLineManager.aspx.cs" Inherits="PraticeManagement.Config.DefaultLineManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Default Line manager</title>
</asp:Content>
<%@ Register Src="~/Controls/Configuration/DefaultUser.ascx" TagPrefix="uc" TagName="DefaultManager" %>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    Default Line manager
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:DefaultManager ID="defaultManager" runat="server" AllowChange="true" PersonsRole="Practice Area Manager" />
</asp:Content>

