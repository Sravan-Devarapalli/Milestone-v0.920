<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="PraticeManagement.Login" Title="Welcome to Practice Management" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Welcome to Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Welcome to Practice Management - Please Log In
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery.blockUI.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function RaiseCustomForgotPasswordClick() {
            Yes();
        }

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
            $.blockUI({ message: '<div class="Padding4Px">Resetting Password...</div>', css: { width: '300px'} });
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
            <div class="DivLogin bgcolorE2EBFF vMiddle">
                <asp:Login ID="login" runat="server" OnLoggedIn="login_LoggedIn" OnLoginError="login_LoginError"
                    OnLoggingIn="login_OnLoggingIn" RememberMeText="Remember me" Font-Bold="true"
                    RememberMeSet="true" />
                <div class="PaddingBottom5">
                    <asp:LinkButton ID="lnkbtnForgotPwd" runat="server" Text="Forgot your Password?"
                        OnClientClick="return ConfirmChangePwd('{0}','{1}');" OnClick="lnkbtnForgotPwd_OnClick">
                    </asp:LinkButton>
                    <br />
                    <br />
                    <asp:Label ID="loginErrorDetails" runat="server" />
                    <asp:Button ID="Button1" runat="server" OnClick="lnkbtnForgotPwd_OnClick" CssClass="displayNone"
                        Text="Reset Password" />
                </div>
            </div>
            <div class="DivLogin vMiddle">
                <uc:MessageLabel ID="msglblForgotPWDErrorDetails" runat="server" ErrorColor="Red"
                    InfoColor="Green" WarningColor="Orange" EnableViewState="false" />
            </div>
            <div class="divConfirmation displayNone" id="divProgress">
                <table class="TableCofirm">
                    <tr>
                        <td class="bgcolorGray height15Px" colspan="2">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="Padding3PX">
                            <br />
                            <p>
                                Please confirm that you would like your Practice Management password to be reset
                                and sent to you in an e-mail within the next few minutes.
                            </p>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="Width50Percent TextAlignCenterImp">
                            <input id="btnYes" type="button" name="btnYes" value="Reset Password" onclick="Yes();" />
                        </td>
                        <td class="TextAlignCenterImp">
                            <input id="btnCancel" type="button" name="btnCancel" value="Cancel" onclick="Cancel();" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

