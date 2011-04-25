<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ChangePasswordError.aspx.cs" Inherits="PraticeManagement.GuestPages.ChangePasswordError" %>

<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Change Password Error</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Invalid Request.
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <h2>
        <img src="../Images/alert_Icon.png" />
        Invalid Link.</h2>
    <h4>
        We apologize, but we are unable to verify the link you used to access Change Password
        page. Please click Continue to return to the Login page and click forgot password.</h4>
    <br />
    <br />
    <div style="text-align: center;">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" OnClick="btnContinue_OnClick" />
    </div>
</asp:Content>

