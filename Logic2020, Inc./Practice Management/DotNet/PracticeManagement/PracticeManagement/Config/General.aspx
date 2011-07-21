<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="PraticeManagement.Config.General" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - General</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    General
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <table width="100%" class="PaddingClass">
        <tr>
            <td valign="top" width="60%">
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td colspan="2">
                            <b>Password Policy :</b>
                        </td>
                    </tr>
                </table>
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td>
                            How many previous passwords should be remembered?
                        </td>
                        <td>
                            <asp:DropDownList ID="ddloldPassWordCheckCount" onchange="setDirty();" runat="server">
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Selected="True" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            How often can the password be changed?
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlChangePasswordTimeSpanLimit" onchange="setDirty();" runat="server">
                                <asp:ListItem Selected="True" Text="no limit" Value="*"></asp:ListItem>
                                <asp:ListItem Text="not more than once in 24 hours" Value="1"></asp:ListItem>
                                <asp:ListItem Text="not more than once in 1 week" Value="7"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" width="40%">
                <div style="border: 1px solid black; background-color: White; padding: 5px;">
                    <p>
                        Practice Management requires a secure password of at least 7 characters. A secure
                        password is one consisting of a mix of upper and lowercase letters, as well as numbers
                        and/or special characters.</p>
                    <br />
                    <p>
                        By default, the last 3 passwords cannot be re-used, and the password cannot be changed
                        more than once in 24 hours. These two values, if changed, will apply to all users.</p>
                    <br />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td colspan="2" style="padding: 10px;">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top" width="60%">
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td valign="middle" colspan="2" style="text-align: left;">
                            <b>Lockout Policy :</b>
                            <asp:CheckBox ID="chbLockOutPolicy" AutoPostBack="true" TextAlign="Left" onclick="setDirty();"
                                Checked="true" runat="server" OnCheckedChanged="chbLockOutPolicy_OnCheckedChanged" />
                        </td>
                    </tr>
                </table>
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td colspan="2">
                            If the incorrect password is entered
                            <asp:DropDownList ID="ddlFailedPasswordAttemptCount" onchange="setDirty();" runat="server">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Selected="True" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="reqFailedPasswordAttemptCount" runat="server" ControlToValidate="ddlFailedPasswordAttemptCount"
                                ErrorMessage="Incorrect Password Attempt Count is required." ToolTip="Incorrect Password Attempt Count is required."
                                ValidationGroup="General" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                            time(s) within
                            <asp:DropDownList ID="ddlPasswordAttemptWindow" onchange="setDirty();" runat="server">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Selected="True" Text="15 minutes" Value="15"></asp:ListItem>
                                <asp:ListItem Text="30 minutes" Value="30"></asp:ListItem>
                                <asp:ListItem Text="45 minutes" Value="45"></asp:ListItem>
                                <asp:ListItem Text="1 hour" Value="60"></asp:ListItem>
                                <asp:ListItem Text="1 day" Value="1440"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="reqPasswordAttemptWindow" runat="server" ControlToValidate="ddlPasswordAttemptWindow"
                                ErrorMessage="Password Attempt Window is required." ToolTip="Password Attempt Window is required."
                                ValidationGroup="General" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                            , lock the user account for
                            <asp:DropDownList ID="ddlUnlockUser" onchange="setDirty();" runat="server">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="15 minutes" Value="15"></asp:ListItem>
                                <asp:ListItem Text="30 minutes" Selected="True" Value="30"></asp:ListItem>
                                <asp:ListItem Text="45 minutes" Value="45"></asp:ListItem>
                                <asp:ListItem Text="1 hour" Value="60"></asp:ListItem>
                                <asp:ListItem Text="1 day" Value="1440"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="reqUnlockUser" runat="server" ControlToValidate="ddlUnlockUser"
                                ErrorMessage="lockOut Period is required." ToolTip="lockOut Period is required."
                                ValidationGroup="General" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                            .
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" width="40%">
                <div style="border: 1px solid black; background-color: White; padding: 5px;">
                    <p>
                        If enabled, the Lockout Policy determines how many incorrect attempts to login to
                        Practice Management a person can make before their account is disabled temporarily.</p>
                    <br />
                    <p>
                        By default, if an incorrect password is entered 3 times within 15 minutes, the account
                        will be disabled for 30 minutes, and the user will receive an e-mail notification.</p>
                    <br />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <b>Inactivity Logoff Policy :</b>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td colspan="2">
                            Log off the user if inactive for &nbsp;
                            <asp:TextBox ID="txtFormsAuthenticationTimeOutMin" runat="server" Width="80px"></asp:TextBox>
                            &nbsp;
                            <asp:RequiredFieldValidator ID="rqFormsAuthenticationTimeOutMin" runat="server" Text="*"
                                ControlToValidate="txtFormsAuthenticationTimeOutMin" ErrorMessage="Inactivity period is required."
                                ToolTip="Inactivity period is required." ValidationGroup="General" Display="Dynamic">
                            </asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="valDatatypeCheck" runat="server" ControlToValidate="txtFormsAuthenticationTimeOutMin"
                                Operator="DataTypeCheck" Type="Integer" ErrorMessage="Inactivity period must be an integer."
                                ToolTip="Inactivity period must be an integer." Text="*" ValidationGroup="General"
                                Display="Dynamic">
                            </asp:CompareValidator>
                            <asp:RangeValidator ID="valRanage" runat="server" ControlToValidate="txtFormsAuthenticationTimeOutMin"
                                MinimumValue="2" MaximumValue="35790" ErrorMessage="Inactivity period must be between 2 and 35790"
                                Type="Integer" ToolTip="Inactivity period must be between 2 and 35790." Text="*"
                                ValidationGroup="General" Display="Dynamic">
                            </asp:RangeValidator>
                            minutes. (2 - 35790)
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td valign="top" width="59%">
                <table width="100%" class="PaddingClass">
                    <tr>
                        <td style="padding-top: 30px;">
                            <asp:ValidationSummary ID="vsumSave" runat="server" ValidationGroup="General" />
                            <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        </td>
                        <td align="right" style="padding-top: 30px; padding-right: 35px;">
                            <asp:Button ID="btnSave" Text="Save" ToolTip="Save" ValidationGroup="General" runat="server"
                                OnClick="btnSave_OnClick" />
                        </td>
                    </tr>
                </table>
            </td>
            <td width="40%">
            </td>
        </tr>
    </table>
</asp:Content>

