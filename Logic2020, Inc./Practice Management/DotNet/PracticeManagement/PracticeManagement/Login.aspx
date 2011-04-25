<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="PraticeManagement.Login" Title="Welcome to Practice Management" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Welcome to Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Welcome to Practice Management - Please Log In
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <style type="text/css">
        input#ctl00_body_login_Password
        {
            width: 180px;
        }
        input#ctl00_body_login_UserName
        {
            width: 180px;
        }
        table#ctl00_body_login td, tr
        {
            padding: 3px;
        }
        table#ctl00_body_login tr td label
        {
            float: left;
        }
    </style>
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script type="text/javascript">

        function ConfirmChangePwd(textboxId, messageLabel) {
            var textbox = document.getElementById(textboxId);
            if (textbox != null && textbox.value != '') {
                var span = document.getElementById(messageLabel);
                if (span != null) {
                    span.lastChild.nodeValue = '';

                }
                ShowConfirmChangePwdModal();
                return false;
            }
        }

        function ShowConfirmChangePwdModal() {
            $.blockUI({ message: $('#divProgress'), css: { width: '385px'} });
            return false;
        }

        function Cancel() {
            $.unblockUI();
            return false;
        }
        function Yes() {

            var button = document.getElementById('<%= Button1.ClientID%>');
            button.click();
            $.blockUI({ message: '<div style="Padding:4px;">Resetting Password...</div>', css: { width: '300px'} });
            return true;
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandle);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function beginRequestHandle(sender, Args) {

        }
        function endRequestHandle(sender, Args) {
            $.unblockUI();
        }

    </script>
    <asp:UpdatePanel ID="updPanel" runat="server">
        <ContentTemplate>
            <div style="vertical-align: middle; margin-left: auto; margin-right: auto; width: 300px;
                padding: 5px; background: #E2EBFF;">
                <asp:Login ID="login" runat="server" OnLoggedIn="login_LoggedIn" OnLoginError="login_LoginError"
                    RememberMeText="Remember me" Font-Bold="true" RememberMeSet="true" />
                <div style="padding-bottom: 5px;">
                    <asp:LinkButton ID="lnkbtnForgotPwd" runat="server" Text="Forgot your Password?"
                        OnClientClick="return ConfirmChangePwd('{0}','{1}');" OnClick="lnkbtnForgotPwd_OnClick">
                    </asp:LinkButton>
                    <br />
                    <br />
                    <asp:Label ID="loginErrorDetails" runat="server" />
                    <asp:Button ID="Button1" runat="server" OnClick="lnkbtnForgotPwd_OnClick" Style="display: none;"
                        Text="Reset Password" />
                </div>
            </div>
            <div style="vertical-align: middle; margin-left: auto; margin-right: auto; width: 300px;
                padding: 5px;">
                <uc:MessageLabel ID="msglblForgotPWDErrorDetails" runat="server" ErrorColor="Red"
                    InfoColor="Green" WarningColor="Orange" EnableViewState="false" />
            </div>
            <div class="divConfirmation" id="divProgress" style="display: none;">
                <table style="width: 385px; margin-bottom: 8px;">
                    <tr>
                        <td style="height: 15px; background-color: Gray;"  colspan="2" >
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-left: 3px; padding: 3px;">
                            <br />
                            <p>
                                Please confirm that you would like your Practice Management password to be reset
                                and sent to you in an e-mail within the next few minutes.
                            </p>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="width: 50%">
                            <input id="btnYes" type="button" name="btnYes" value="Reset Password" onclick="Yes();" />
                        </td>
                        <td align="center">
                            <input id="btnCancel" type="button" name="btnCancel" value="Cancel" onclick="Cancel();" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

