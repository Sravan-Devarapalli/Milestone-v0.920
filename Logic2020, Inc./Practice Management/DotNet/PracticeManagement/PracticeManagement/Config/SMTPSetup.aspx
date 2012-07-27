<%@ Page Title="" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="SMTPSetup.aspx.cs" Inherits="PraticeManagement.Config.SMTPSetup" %>

<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>SMTP Setup | Practice Management</title>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    SMTP Setup
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
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
            <table class="PaddingClass Width80Percent">
                <tr>
                    <td class="Width43Percent">
                        <table class="PaddingClass SMTPTable">
                            <tr>
                                <td class="Width34Percent">
                                    Mail Server
                                </td>
                                <td class="Width66Percent">
                                    <div>
                                        <asp:TextBox ID="txtMailServer" CssClass="WholeWidth" onchange="DisablSaveButton();"
                                            runat="server"></asp:TextBox>
                                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="watermarker" runat="server" TargetControlID="txtMailServer"
                                            WatermarkText="smtp.domain.com" EnableViewState="false" WatermarkCssClass="waterMarkItalicSmtp WholeWidth" />
                                        <asp:RequiredFieldValidator ID="reqMailServer" runat="server" ControlToValidate="txtMailServer"
                                            ErrorMessage="The Mail Server Name is required." ToolTip="The Mail Server Name is required."
                                            Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Port Number
                                </td>
                                <td>
                                    <div>
                                        <asp:TextBox ID="txtPortNumber" CssClass="Width72Percent" onchange="DisablSaveButton();"
                                            runat="server"></asp:TextBox>
                                        (0-65535)
                                        <asp:RequiredFieldValidator ID="reqPortNumber" runat="server" ControlToValidate="txtPortNumber"
                                            ErrorMessage="The Port Number is required." ToolTip="The Port Number is required."
                                            Text="*" SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                        <asp:RangeValidator ID="rangPortNumber" runat="server" ControlToValidate="txtPortNumber"
                                            ErrorMessage="The Port Number must be  between 0 and 65535." ToolTip="The Port Number must be  between 0 and 65535."
                                            Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                            MinimumValue="0" MaximumValue="65535" Type="Integer" ValidationGroup="SmtpSetUp"></asp:RangeValidator>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td>
                                    <asp:CheckBox ID="chbEnableSSl" AutoPostBack="true" onclick="DisablSaveButton();"
                                        OnCheckedChanged="chbEnableSSl_CheckedChanged" runat="server" />
                                    Enable SSL
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width12Percent">
                    </td>
                    <td class="Width45Percent vMiddle">
                        <div class="SMTPAddressDesc">
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
                    <td class="WholeWidth" colspan="3">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="WholeWidth" colspan="3">
                        SMTP Authentication
                    </td>
                </tr>
                <tr>
                    <td class="Width43Percent">
                        <table class="PaddingClass SMTPTable">
                            <tr>
                                <td class="Width34Percent">
                                    User Name:
                                </td>
                                <td class="Width66Percent">
                                    <div>
                                        <asp:TextBox ID="txtUserName" CssClass="WholeWidth" onchange="DisablSaveButton();"
                                            runat="server"></asp:TextBox>
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
                                <td>
                                    Password:
                                </td>
                                <td>
                                    <div>
                                        <asp:TextBox ID="txtPasswordSingleLine" Style="display: none;" CssClass="WholeWidth"
                                            TextMode="SingleLine" onchange="AssigntxtPasswordValue(this.value);DisablSaveButton();"
                                            runat="server"></asp:TextBox>
                                        <asp:TextBox ID="txtPassword" CssClass="WholeWidth" TextMode="Password" onchange="AssigntxtPasswordSinleLineValue(this.value);DisablSaveButton();"
                                            runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="reqtxtPassword" runat="server" ControlToValidate="txtPasswordSingleLine"
                                            ErrorMessage="Password is required." ToolTip="Password is required." Text="*"
                                            SetFocusOnError="True" EnableClientScript="False" ValidationGroup="SmtpSetUp" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chbShowCharacters" AutoPostBack="true" Checked="false" OnCheckedChanged="chbShowCharacters_CheckedChanged"
                                        runat="server" />
                                    Show Characters
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width12Percent">
                    </td>
                    <td class="Width45Percent">
                        <div class="SMTPAddressDesc">
                            <span>Enter valid account credentials used to authenticate to your SMTP server.<br />
                                <br />
                                If you need to change the username or password in the future, simply replace the
                                credentials and click the "Save Changes" button below.</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="WholeWidth" colspan="3">
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="WholeWidth" colspan="3">
                        PM Support Email Address
                    </td>
                </tr>
                <tr>
                    <td class="Width40P vTop">
                        <table class="PaddingClass SMTPTable">
                            <tr>
                                <td class="Width34Percent">
                                    &nbsp;
                                </td>
                                <td class="Width66Percent">
                                    <div>
                                        <asp:TextBox ID="txtPMSupportEmail" CssClass="WholeWidth" onchange="DisablSaveButton();"
                                            runat="server"></asp:TextBox>
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
                    <td class="Width10Percent">
                    </td>
                    <td class="Width50Percent">
                        <div class="SMTPAddressDesc">
                            <span>Enter a valid e-mail address here that will be populated throughout PM as a means
                                of allowing users to contact your company's support staff or help desk.</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="Width43Percent no-wrap padRight0">
                        <table class="PaddingClass SMTPTable padRight0Table">
                            <tr>
                                <td class="Width34Percent">
                                    Connection Test
                                </td>
                                <td class="Width66Percent">
                                </td>
                            </tr>
                            <tr>
                                <td class="Width34Percent">
                                </td>
                                <td align="right" class="Width66Percent">
                                    <asp:Button ID="btnTestSettings" OnClick="btnTestSettings_Click" runat="server" Text="Test Settings"
                                        ValidationGroup="SmtpSetUp" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="WholeWidth">
                                    <table class="WholeWidth">
                                        <tr>
                                            <td class="SMTPAddressDesc1">
                                                <asp:Label ID="lbloutputBox" CssClass="WS-Normal" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="WholeWidth">
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnSave" runat="server" Enabled="false" OnClick="btnSave_Click" ValidationGroup="SmtpSetUp"
                                        Text="Save Changes" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="Width12Percent">
                    </td>
                    <td class="Width45Percent">
                        <div class="SMTPAddressDesc">
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
</asp:Content>

