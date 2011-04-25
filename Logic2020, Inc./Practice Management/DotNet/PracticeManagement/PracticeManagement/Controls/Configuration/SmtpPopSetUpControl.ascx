<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmtpPopSetUpControl.ascx.cs"
    Inherits="PraticeManagement.Controls.Configuration.SmtpPopSetUpControl" %>
    <%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<script type="text/javascript" language="javascript">
    function DisablSaveButton() {
        var button = document.getElementById('<%= btnSave.ClientID %>');
        button.disabled = "disabled";
    }
    function AssigntxtPasswordSinleLineValue(password) {
        var txtBox = document.getElementById('<%= txtPasswordSingleLine.ClientID %>');
        txtBox.value = password;
    }

    function AssigntxtPasswordValue(password) {
        var txtBox = document.getElementById('<%= txtPassword.ClientID %>');
        txtBox.value = password;
    }
</script>
<uc:LoadingProgress ID="LoadingProgress1" runat="server" />
<asp:UpdatePanel ID="updSmtpSetup" runat="server">
    <ContentTemplate>
        <table width="80%" class="PaddingClass">
            <tr>
                <td width="43%">
                    <table width="100%" class="PaddingClass">
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                Mail Server
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <div style="padding-left: 2px;">
                                    <asp:TextBox ID="txtMailServer" Width="100%" onchange="DisablSaveButton();" runat="server"></asp:TextBox>
                                    <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="txtMailServer"
                                        WatermarkText="smtp.domain.com" EnableViewState="false" WatermarkCssClass="waterMarkItalicSmtp" />
                                    <asp:RequiredFieldValidator ID="reqMailServer" runat="server" ControlToValidate="txtMailServer"
                                        ErrorMessage="The Mail Server Name is required." ToolTip="The Mail Server Name is required."
                                        Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                Port Number
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <div style="padding-left: 2px;">
                                    <asp:TextBox ID="txtPortNumber" Width="72%" onchange="DisablSaveButton();" runat="server"></asp:TextBox>
                                    (0-65535)
                                    <asp:RequiredFieldValidator ID="reqPortNumber" runat="server" ControlToValidate="txtPortNumber"
                                        ErrorMessage="The Port Number is required." ToolTip="The Port Number is required."
                                        Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                    <asp:RangeValidator ID="rangPortNumber" runat="server" ControlToValidate="txtPortNumber"
                                        ErrorMessage="The Port Number must be  between 0 and 65535." ToolTip="The Port Number must be  between 0 and 65535."
                                        Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                        MinimumValue="0" MaximumValue="15000" Type="Integer" ValidationGroup="SmtpSetUp"></asp:RangeValidator>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                &nbsp;
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <asp:CheckBox ID="chbEnableSSl" AutoPostBack="true" onclick="DisablSaveButton();"
                                    OnCheckedChanged="chbEnableSSl_CheckedChanged" runat="server" />
                                Enable SSL
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="12%">
                </td>
                <td valign="middle" width="45%">
                    <div style="border: 1px solid black; background-color: White; padding: 5px;">
                        <span>Enter your Mail Server's SMTP address here.<br />
                            <br />
                            By default, PM will attempt to use port 25 for SMTP traffic unless the "Enable SSL"
                            box is checked, in which case it will use port 465. If you need to use any other
                            port for SMTP access, simply replace the default port number entered with one specific
                            to your requirements.</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td width="100%" colspan="3">
                    <br />
                </td>
            </tr>
            <tr>
                <td width="100%" colspan="3">
                    SMTP Authentication
                </td>
            </tr>
            <tr>
                <td width="43%">
                    <table width="100%" class="PaddingClass">
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                User Name:
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <div style="padding-left: 2px;">
                                    <asp:TextBox ID="txtUserName" Width="100%" onchange="DisablSaveButton();" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqtxtUserName" runat="server" ControlToValidate="txtUserName"
                                        ErrorMessage="The User Name is required." ToolTip="The User Name is required."
                                        Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                    <asp:RegularExpressionValidator ID="regUserName" runat="server" ControlToValidate="txtUserName"
                                        Display="Dynamic" ErrorMessage="The User Name is not valid." ValidationGroup="SmtpSetUp"
                                        ToolTip="The User Name is not valid." Text="*" EnableClientScript="False" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                Password:
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <div style="padding-left: 2px;">
                                    <asp:TextBox ID="txtPasswordSingleLine" Style="display: none;" Width="100%" TextMode="SingleLine"
                                        onchange="AssigntxtPasswordValue(this.value);DisablSaveButton();" runat="server"></asp:TextBox>
                                    <asp:TextBox ID="txtPassword" Width="100%" TextMode="Password" onchange="AssigntxtPasswordSinleLineValue(this.value);DisablSaveButton();"
                                        runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqtxtPassword" runat="server" ControlToValidate="txtPasswordSingleLine"
                                        ErrorMessage="Password is required." ToolTip="Password is required." Text="*"
                                        SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                            </td>
                            <td style="white-space: nowrap; width: 66%;">
                                <asp:CheckBox ID="chbShowCharacters" AutoPostBack="true" Checked="false" OnCheckedChanged="chbShowCharacters_CheckedChanged"
                                    runat="server" />
                                Show Characters
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="12%">
                </td>
                <td width="45%">
                    <div style="border: 1px solid black; background-color: White; padding: 5px;">
                        <span>Enter valid account credentials used to authenticate to your SMTP server.<br />
                            <br />
                            If you need to change the username or password in the future, simply replace the
                            credentials and click the "Save Changes" button below.</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td width="100%" colspan="3">
                    <br />
                </td>
            </tr>
            <tr>
                <td width="100%" colspan="3">
                    PM Support Email Address
                </td>
            </tr>
            <tr>
                <td width="40%">
                    <table width="100%" class="PaddingClass">
                        <tr>
                            <td style="width: 34%;">
                                &nbsp;
                            </td>
                            <td style="width: 66%;">
                                <div style="padding-left: 2px;">
                                    <asp:TextBox ID="txtPMSupportEmail" Width="100%" onchange="DisablSaveButton();" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="reqtxtEmailAddress" runat="server" ControlToValidate="txtPMSupportEmail"
                                        ErrorMessage="The PM Support Email Address is required." ToolTip="The PM Support Email Address is required."
                                        Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                    <asp:RegularExpressionValidator ID="regtxtEmailAddress" runat="server" ControlToValidate="txtPMSupportEmail"
                                        Display="Dynamic" ErrorMessage="The PM Support Email Address is not valid." ValidationGroup="SmtpSetUp"
                                        ToolTip="The PM Support Email Address is not valid." Text="*" EnableClientScript="False"
                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="10%">
                </td>
                <td width="50%">
                    <div style="border: 1px solid black; background-color: White; padding: 5px;">
                        <span>Enter a valid e-mail address here that will be populated throughout PM as a means
                            of allowing users to contact your company's support staff or help desk.</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="white-space: nowrap; padding-right: 0px;" width="43%">
                    <table width="100%" class="PaddingClass">
                        <tr>
                            <td style="white-space: nowrap; width: 34%;">
                                Connection Test
                            </td>
                            <td style="width: 66%;">
                            </td>
                        </tr>
                        <tr>
                            <td style="white-space: nowrap; padding-right: 0px; width: 34%;">
                            </td>
                            <td align="right" style="white-space: nowrap; padding-right: 0px; width: 66%;">
                                <asp:Button ID="btnTestSettings" OnClick="btnTestSettings_Click" runat="server" Text="Test Settings"
                                    ValidationGroup="SmtpSetUp" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 100%; padding-right: 0px;">
                                <table width="100%">
                                    <tr>
                                        <td style="border: 1px solid black; width: 100%; height: 63px; background-color: White;
                                            padding: 5px;">
                                            <asp:Label ID="lbloutputBox" Style="white-space: normal" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="width: 100%;">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 34%;">
                            </td>
                            <td align="right" style="width: 66%;">
                                <asp:Button ID="btnSave" runat="server" Enabled="false" OnClick="btnSave_Click" ValidationGroup="SmtpSetUp"
                                    Text="Save Changes" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="12%">
                </td>
                <td width="45%">
                    <div style="border: 1px solid black; background-color: White; padding: 5px;">
                        <span>Once you've entered valid SMTP settings and a PM Support Email Address in the
                            boxes above, you can click the "Test Settings " button to make sure everything works
                            as expected. Results of the test will display in the box below.</span>
                    </div>
                </td>
            </tr>
        </table>
        <asp:Label ID="lblMessage" ForeColor="Green" runat="server"></asp:Label>
        <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="SmtpSetUp" />
    </ContentTemplate>
</asp:UpdatePanel>

