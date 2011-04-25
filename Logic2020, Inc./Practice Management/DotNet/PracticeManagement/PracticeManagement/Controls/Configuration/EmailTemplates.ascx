<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTemplates.ascx.cs"
    Inherits="PraticeManagement.Controls.Configuration.EmailTemplates" %>
<%@ Register Src="../Generic/LoadingProgress.ascx" TagName="LoadingProgress" TagPrefix="uc1" %>
<script type="text/javascript">
    function ClientValidate(source, arguments) {
        if (arguments.Value.length <= 2000) {
            arguments.IsValid = true;
        } else {
            arguments.IsValid = false;
        }
    }
</script>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <table>
            <tr valign="top">
                <td style="overflow: auto; border: 1px solid #000000:width:200px">
                    <table>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lblEmptyTemplate" runat="server" Text="Sample Template" OnClick="lblEmptyTemplate_Click" />
                            </td>
                        </tr>
                        <asp:ListView ID="lvEmailTemplates" runat="server" DataKeyNames="Name" OnSelectedIndexChanged="lvEmailTemplates_SelectedIndexChanged"
                            OnSelectedIndexChanging="lvEmailTemplates_SelectedIndexChanging" OnLayoutCreated="lvEmailTemplates_LayoutCreated">
                            <LayoutTemplate>
                                <tr runat="server" id="itemPlaceholder" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr runat="server">
                                    <td>
                                        <asp:LinkButton ID="lnkEmailTitle" runat="server" Text='<%# Eval("Name") %>' CommandName="Select" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr runat="server" class="rowEven">
                                    <td>
                                        <asp:LinkButton ID="lnkEmailTitle" runat="server" Text='<%# Eval("Name") %>' CommandName="Select" />
                                    </td>
                                </tr>
                            </AlternatingItemTemplate>
                        </asp:ListView>
                    </table>
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                                <strong>Name</strong>
                            </td>
                            <td>
                                <asp:TextBox ID="txTemplateTitle" runat="server" Width="395px" />
                            </td>
                            <td valign="top">
                                <asp:RequiredFieldValidator runat="server" ID="TemplateTitleRequiredFieldValidator"
                                    ControlToValidate="txTemplateTitle" Text="*" ErrorMessage="Template name is required"
                                    Display="Dynamic" ValidationGroup="TemplateFields" />
                                <asp:CustomValidator ID="TemplateNameCustomValidator" runat="server" Display="Dynamic"
                                    ErrorMessage="Template with specified name already exists!" ValidationGroup="TemplateFields"
                                    Text="*"></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>To</strong>
                            </td>
                            <td colspan="2" align="left">
                                <asp:TextBox ID="txTemplateTo" runat="server" Width="395px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Cc</strong>
                            </td>
                            <td colspan="2" align="left">
                                <asp:TextBox ID="txTemplateCc" runat="server" Width="395px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Subject</strong>
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmailSubject" runat="server" Width="395px" />
                            </td>
                            <td valign="top">
                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtEmailSubject"
                                    Text="*" ErrorMessage="Template subject is required" Display="Dynamic" ValidationGroup="TemplateFields" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="txtEmailBody" runat="server" Height="217px" TextMode="MultiLine"
                                    Width="450px" Style="margin-bottom: 0px" />
                            </td>
                            <td valign="top">
                                <asp:RequiredFieldValidator runat="server" ID="TemplateBodyRequiredFieldValidator"
                                    ControlToValidate="txtEmailBody" Text="*" ErrorMessage="Template body is required"
                                    Display="Dynamic" ValidationGroup="TemplateFields" />
                                <asp:CustomValidator ID="maxBodyLengthCustomValidator" runat="server" ErrorMessage="*Template body cannot be more than 2000 symbols"
                                    ControlToValidate="txtEmailBody" ClientValidationFunction="ClientValidate" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" valign="top">
                                <asp:ValidationSummary ID="TemplateFieldsValidationSummary" runat="server" ValidationGroup="TemplateFields"
                                    DisplayMode="BulletList" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2">
                    <asp:Button ID="btnAddAsNew" runat="server" Text="Add As New Template" OnClick="btnAddAsNew_Click" />
                    <asp:Button ID="btnRemove" runat="server" Text="Remove" OnClick="btnRemove_Click"
                        OnClientClick="return confirm('Are you sure you want to delete this template?');" />
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpate_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<uc1:LoadingProgress ID="LoadingProgress1" runat="server" />

