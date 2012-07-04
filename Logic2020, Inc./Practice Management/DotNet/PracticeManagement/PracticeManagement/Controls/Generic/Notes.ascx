<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Notes.ascx.cs" Inherits="PraticeManagement.Controls.Generic.Notes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<uc:LoadingProgress ID="lpNotes" runat="server" />
<asp:UpdatePanel ID="updNotes" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="WholeWidth">
            <tr>
                <td>
                    <asp:TextBox ID="tbNote" runat="server" Rows="5" Style="width: 100%;" TextMode="MultiLine"
                        ValidationGroup="Notes" />
                    <ajax:TextBoxWatermarkExtender ID="twNote" runat="server" TargetControlID="tbNote"
                        WatermarkText="Add note here." WatermarkCssClass="watermarked-text" />
                </td>
                <td class = "Width100Px textCenter">
                    <asp:Button ID="btnAddNote" runat="server" ValidationGroup="Notes" OnClick="btnAddNote_Click"
                        Text="Add Note" CssClass="Width80Px" />
                </td>
            </tr>
            <tr class="height15Px">
                <td colspan="2">
                    <asp:RequiredFieldValidator ID="rvNotes" runat="server" ValidationGroup="Notes" ControlToValidate="tbNote"
                        ErrorMessage="Note text is empty." Display="Dynamic" />
                    <asp:CustomValidator ID="cvLen" runat="server" ErrorMessage="Maximum length of the Note is 2000 characters."
                        ClientValidationFunction="javascript:len=args.Value.length;args.IsValid=(len>0 && len<=2000);"
                        OnServerValidate="cvLen_OnServerValidate" EnableClientScript="true" ControlToValidate="tbNote"
                        ValidationGroup="Notes" Display="Dynamic" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnAddNote" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

