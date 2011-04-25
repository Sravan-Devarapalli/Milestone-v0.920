<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Notes.ascx.cs" Inherits="PraticeManagement.Controls.Generic.Notes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register src="~/Controls/Generic/LoadingProgress.ascx" tagname="LoadingProgress" tagprefix="uc" %>

<uc:LoadingProgress ID="lpNotes" runat="server" />
<asp:UpdatePanel ID="updNotes" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="WholeWidth">
            <tr>
                <td colspan="2">
                    <asp:Panel ID="pnlNotes" runat="server" CssClass="opportunity-panel">
                        <asp:GridView ID="gvNotes" runat="server" DataSourceID="odsNotes" 
                            AutoGenerateColumns="False" CssClass="CompPerfTable WholeWidth"
                            GridLines="None" BackColor="White" EmptyDataText="No notes.">
                            <AlternatingRowStyle BackColor="#F9FAFF" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">Created</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("CreateDate", "{0:MM/dd/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">By</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblPerson" runat="server" 
                                            Text='<%# Eval("Author.LastName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="100" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <div class="ie-bg">Note</div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblNote" runat="server" Text='<%# Eval("NoteText") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <asp:ObjectDataSource ID="odsNotes" runat="server" SelectMethod="NoteListByTargetId"
                        TypeName="PraticeManagement.MilestoneService.MilestoneServiceClient">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="targetId" QueryStringField="id" Type="Int32" />
                            <asp:Parameter Name="noteTargetId" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="tbNote" runat="server" CssClass="WholeWidth" MaxLength="2000" Rows="5"
                        TextMode="MultiLine" ValidationGroup="Notes" />
                    <ajax:TextBoxWatermarkExtender ID="twNote" runat="server" 
                        TargetControlID="tbNote" WatermarkText="Add note here." WatermarkCssClass="watermarked-text" />
                </td>
                <td style="text-align:center; width:100px;">
                    <asp:Button ID="btnAddNote" runat="server" ValidationGroup="Notes"
                        OnClick="btnAddNote_Click" Text="Add Note" Width="80"/>
                </td>
            </tr>
            <tr style="height:15px">
                <td colspan="2">
                    <asp:RequiredFieldValidator ID="rvNotes" runat="server" ValidationGroup="Notes" ControlToValidate="tbNote"
                        ErrorMessage="Note text is empty." Display="Dynamic" />
                    <asp:CustomValidator ID="cvLen" runat="server" ErrorMessage="Maximum length of the Note is 2000 characters." 
                        ClientValidationFunction="javascript:len=args.Value.length;args.IsValid=(len>0 && len<=2000);" OnServerValidate="cvLen_OnServerValidate"
                        EnableClientScript="true" ControlToValidate="tbNote" ValidationGroup="Notes" Display="Dynamic"/>
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnAddNote" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

