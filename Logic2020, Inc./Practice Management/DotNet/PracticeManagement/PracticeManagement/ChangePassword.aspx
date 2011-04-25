<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="PraticeManagement.ChangePassword"
    Title="Practice Management - Change User Password" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Change User Password</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Change User Password
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            hdnAreCredentialssaved = document.getElementById('<%= hdnAreCredentialssaved.ClientID %>');
            if (hdnAreCredentialssaved.value == 'true') {
                $.blockUI({ message: '<div style="Padding:5px;">New password successfully saved.  Redirecting to landing page...</div>', css: { width: '380px'} });
                setTimeout('__doPostBack("__Page", "")', 2000);
            }
        }
    );
    </script>
    <div style="width: 300px; margin-left: auto; margin-right: auto; padding: 5px;">
        <asp:ChangePassword ID="changePassword" runat="server" MembershipProvider="PracticeManagementMembershipProvider"
            ContinueDestinationPageUrl="~/" OnChangingPassword="changePassword_OnChangingPassword"
            ChangePasswordFailureText="New Password should be should be at least {0} characters and should contain at least {1} non-alphanumeric characters (e.g. !, @, #, $ etc).">
        </asp:ChangePassword>
    </div>
    <div style="width: 300px; margin-left: auto; margin-right: auto; padding: 5px;">
        <uc:MessageLabel ID="msglblchangePasswordDetails" runat="server" ErrorColor="Red"
            InfoColor="Green" WarningColor="Orange" EnableViewState="false" />
    </div>
    <asp:HiddenField ID="hdnAreCredentialssaved" runat="server" Value="false" />
</asp:Content>

