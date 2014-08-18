<%@ Page Title="Lock down" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="Lockout.aspx.cs" Inherits="PraticeManagement.Lockout" %>

<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel runat="server" ID="upCommissionAttribution">
        <ContentTemplate>
            <asp:GridView ID="gvLockout" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvLockout_RowDataBound"
                CssClass="CompPerfTable MileStoneDetailPageResourcesTab" EditRowStyle-Wrap="false"
                Width="50%" RowStyle-Wrap="false" HeaderStyle-Wrap="false" EmptyDataText='There are no Lockout details.' RowStyle-Height="40px"
                GridLines="None" BackColor="White">
                <AlternatingRowStyle CssClass="bgcolorF9FAFFImp" />
                <HeaderStyle CssClass="textCenter" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg" style="border-left: #c5c5c5 1px solid;">
                                Page &nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width10PerImp" />
                        <ItemTemplate>
                            <asp:Label ID="lblPageName" runat="server"></asp:Label>
                            <asp:HiddenField ID="hdnPageId" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg">
                                Functionality&nbsp;
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width20Percent textLeft padLeft120" />
                        <ItemTemplate>
                            <%# Eval("HtmlEncodedName")%>
                            <asp:HiddenField ID="hdnFunctionalityName" runat="server" />
                            <asp:HiddenField ID="hdnLockoutId" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Lock down</div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width2Percent textCenter" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chbLockout" runat="server" />
                            <asp:CustomValidator ID="custchbLockout" runat="server" Text="*" OnServerValidate="custchbLockout_ServerValidate"
                                ValidationGroup="UpdateLockout"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <div class="ie-bg no-wrap">
                                Lock down Date
                            </div>
                        </HeaderTemplate>
                        <ItemStyle CssClass="Width10PerImp textCenter" />
                        <ItemTemplate>
                            <uc:DatePicker ID="dpLockoutDate" runat="server" TextBoxWidth="70" Style="width: 100%" />
                            <asp:CompareValidator ID="compCompletionDate" runat="server" ControlToValidate="dpLockoutDate"
                                ValidationGroup="UpdateLockout" ErrorMessage="The Lock down date has an incorrect format. It must be 'MM/dd/yyyy'."
                                ToolTip="The Lock down date has an incorrect format. It must be 'MM/dd/yyyy'."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                            <asp:CustomValidator ID="custLockoutDateNotFuture" runat="server" ValidationGroup="UpdateLockout"
                                ErrorMessage="Lock down date cannot be in future" ToolTip="Lock down date cannot be in future"
                                Text="*" OnServerValidate="custLockoutDateNotFuture_ServerValidate"></asp:CustomValidator>
                            <asp:CustomValidator ID="custLockoutDate" runat="server" Text="*" OnServerValidate="custLockoutDate_ServerValidate"
                                ValidationGroup="UpdateLockout"></asp:CustomValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <table style="width: 100%">
                <tr>
                    <td style="width: 50%; text-align: center;padding-top: 10px;">
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                    </td>
                    <td style="width: 50%">
                        &nbsp;
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" />
            <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                <table class="Width100Per">
                    <tr>
                        <th class="bgcolorGray TextAlignCenterImp vBottom">
                            <b class="BtnClose">Attention!</b>
                            <asp:Button ID="btnCancelErrorPanel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Cancel" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="Padding10Px">
                            <asp:ValidationSummary ID="valSummaryUpdate" ValidationGroup="UpdateLockout" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving lock down details." />
                            <uc:Label ID="mlInsertStatus" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

