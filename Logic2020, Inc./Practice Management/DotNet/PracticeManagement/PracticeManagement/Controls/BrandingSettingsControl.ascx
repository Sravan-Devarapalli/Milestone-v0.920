<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BrandingSettingsControl.ascx.cs"
    Inherits="PraticeManagement.Controls.BrandingSettingsControl" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
    <script language="javascript" type="text/javascript">
        function EnableSaveButton(enable) {
            var button = document.getElementById("<%=btnSave.ClientID %>");
            var message = document.getElementById("<%=mlConfirmation.ClientID %>" + "_lblMessage");
            if (message != null) {
                message.style.display = "none";
            }
            if (button != null) {
                button.disabled = !enable;
            }
        }
    </script>
    <asp:UpdatePanel ID="updpnlbody" runat="server">
        <ContentTemplate>
            <table class="Width700Px">
                <tr>
                </tr>
                <tr>
                    <td>
                        <label class="no-wrap" for="<%=tbTitle.ClientID%>">
                            Company Name</label>
                    </td>
                    <td class="padRight5">
                        <asp:TextBox ID="tbTitle" onchange="EnableSaveButton(true);" runat="server" CssClass="Width300Px" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="no-wrap" for="<%=fuImagePath.ClientID%>">
                            Company Logo</label>
                    </td>
                    <td class="padRight5">
                        <asp:FileUpload ID="fuImagePath" BackColor="White" onchange="EnableSaveButton(true);"
                            runat="server" CssClass="Width375Px" Size="56" />
                        <asp:HiddenField ID="hdnImagePath" runat="server" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <span class="watermarked">* recommended logo size is 175 x 55 px</span>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="PaddingTop4">
                        <span class="watermarked">* maximum logo size is 500 x 60 px</span>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <span class="watermarked">* file type must be .jpg, .gif,or .png </span>
                        <asp:Button ID="btnSave" runat="server" Text="Save All" CssClass="Margin-Left Width67Px" OnClick="btnSave_Click" 
                            ValidationGroup="BrandingLogo" />
                        <ext:ElementDisablerExtender ID="edeSave" runat="server" TargetControlID="btnSave"
                            ControlToDisableID="btnSave" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CustomValidator ID="cvImagePath" runat="server" ControlToValidate="fuImagePath"
                            Display="None" OnServerValidate="cvImagePath_OnServerValidate" ValidationGroup="BrandingLogo"
                            Text="*" ErrorMessage="Error occured. Can't save the settings: Invalid File Format."></asp:CustomValidator>
                        <asp:CustomValidator ID="cvalidatorImagePath" runat="server" ControlToValidate="fuImagePath"
                            Display="None" OnServerValidate="cvalidatorImagePath_OnServerValidate" ValidationGroup="BrandingLogo"
                            Text="*" ErrorMessage="Error occured. Can't save the settings: Logo size cannot be more than 500 x 60 px."></asp:CustomValidator>
                        <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green"
                            WarningColor="Orange" />
                        <asp:ValidationSummary ID="valsumBrandingLogo" ValidationGroup="BrandingLogo" runat="server" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <div>
            <table>
                <tr>
                    <td class="fontBold PaddingBottom8" colspan="2">Current Logo</td>
                </tr>
                <tr>
                    <td class="border1Px">
                        <asp:Image ID="imgCurrentLogo" runat="server" CssClass="Padding2" ImageUrl="~/Controls/CompanyLogoImage.ashx" />
                    </td>
                </tr>
                <tr>
                    <td class="PaddingTop8 vBottom">
                        <asp:HyperLink ID="hplnkDownloadCurrentImage" runat="server" Text="Download image" NavigateUrl="~/Controls/CompanyLogoImage.ashx?Type=download"></asp:HyperLink>
                    </td>
                </tr>
            </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSave" />
        </Triggers>
    </asp:UpdatePanel>

