<%@ Page Title="Capabilities" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PracticeCapabilities.aspx.cs" Inherits="PraticeManagement.Config.PracticeCapabilities" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>Capabilities | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="header" runat="server">
    Capabilities
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <asp:Repeater ID="repPractices" runat="server">
                <HeaderTemplate>
                    <div class="WholeWidth repCapabilities">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="PracticeHeaderTd">
                        <asp:Label ID="lbPracticeName" runat="server" Text='<%# GetPracticeLable((string)Eval("HtmlEncodedName"),(bool)Eval("IsActive"),(string)Eval("Abbreviation")) %>'></asp:Label>
                    </div>
                    <asp:Repeater ID="repPracticeCapabilities" runat="server" DataSource='<%# Eval("PracticeCapabilities") %>'>
                        <ItemTemplate>
                            <div class="capabilityTd">
                                <asp:HiddenField ID="hdCapabilityId" runat="server" Value='<%# Bind("CapabilityId") %>' />
                                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                    ToolTip="Edit Capability" />
                                <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                    Visible="false" OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                                <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                    Visible="false" ToolTip="Cancel" />
                                <asp:Label ID="lbCapabilityName" runat="server" Text='<%# Bind("HtmlEncodedName") %>'></asp:Label>
                                <asp:TextBox ID="tbEditCapabilityName" runat="server" Text='<%# Bind("Name") %>'
                                    CssClass="width60P" Visible="false" />
                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmeEditCapabilityName" runat="server"
                                    TargetControlID="tbEditCapabilityName" WatermarkCssClass="watermarkedtext width60P"
                                    WatermarkText="Enter capability name here...">
                                </AjaxControlToolkit:TextBoxWatermarkExtender>
                                <asp:RequiredFieldValidator ID="rvEditCapability" runat="server" ControlToValidate="tbEditCapabilityName"
                                    Enabled="false" Display="Dynamic" ErrorMessage="Capability Name is required"
                                    ToolTip="Capability Name is required" ValidationGroup="EditCapability">*</asp:RequiredFieldValidator>
                                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                    CssClass="padRight15 floatright" OnClick="imgDelete_OnClick" ToolTip="Delete Capability"
                                    Visible='<%# !(bool)Eval("InUse") %>' InUse='<%# (bool)Eval("InUse") %>' />
                                <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgDelete" runat="server" TargetControlID="imgDelete" ConfirmText="Are you sure. Do you want to delete the capability?">
                                </AjaxControlToolkit:ConfirmButtonExtender>
                            </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="capabilityTd alterrow">
                                <asp:HiddenField ID="hdCapabilityId" runat="server" Value='<%# Bind("CapabilityId") %>' />
                                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/icon-edit.png" OnClick="imgEdit_OnClick"
                                    ToolTip="Edit Capability" />
                                <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                                    Visible="false" OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                                <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                                    Visible="false" ToolTip="Cancel" />
                                <asp:Label ID="lbCapabilityName" runat="server" Text='<%# Bind("HtmlEncodedName") %>'></asp:Label>
                                <asp:TextBox ID="tbEditCapabilityName" runat="server" Text='<%# Bind("Name") %>'
                                    CssClass="width60P" Visible="false" />
                                <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmeEditCapabilityName" runat="server"
                                    TargetControlID="tbEditCapabilityName" WatermarkCssClass="watermarkedtext width60P"
                                    WatermarkText="Enter capability name here...">
                                </AjaxControlToolkit:TextBoxWatermarkExtender>
                                <asp:RequiredFieldValidator ID="rvEditCapability" runat="server" ControlToValidate="tbEditCapabilityName"
                                    Enabled="false" Display="Dynamic" ErrorMessage="Capability Name is required"
                                    ToolTip="Capability Name is required" ValidationGroup="EditCapability">*</asp:RequiredFieldValidator>
                                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/icon-delete.png"
                                    CssClass="padRight15 floatright" OnClick="imgDelete_OnClick" ToolTip="Delete Capability"
                                    Visible='<%# !(bool)Eval("InUse") %>' InUse='<%# (bool)Eval("InUse") %>' />
                                <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgDelete" runat="server" TargetControlID="imgDelete" ConfirmText="Are you sure. Do you want to delete the capability?">
                                </AjaxControlToolkit:ConfirmButtonExtender>
                            </div>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                    <div class="PlusTd">
                        <asp:HiddenField ID="hdPracticeId" runat="server" Value='<%# Bind("Id") %>' />
                        <asp:ImageButton ID="imgPlus" runat="server" ImageUrl="~/Images/add_16.png" ToolTip="Add Capability"
                            Visible='<%# (bool)Eval("IsActive") %>' OnClick="imgPlus_OnClick" />
                        <asp:ImageButton ID="imgUpdate" runat="server" ImageUrl="~/Images/icon-check.png"
                            IsInsert="True" Visible="false" OnClick="imgUpdate_OnClick" ToolTip="Confirm" />
                        <asp:ImageButton ID="imgCancel" runat="server" ImageUrl="~/Images/no.png" OnClick="imgCancel_OnClick"
                            IsInsert="True" Visible="false" ToolTip="Cancel" />
                        <asp:TextBox ID="tbInsertCapabilityName" runat="server" CssClass="width60P" Visible="false" />
                        <AjaxControlToolkit:TextBoxWatermarkExtender ID="wmeInsertCapabilityName" runat="server"
                            TargetControlID="tbInsertCapabilityName" WatermarkCssClass="watermarkedtext width60P"
                            WatermarkText="Enter capability name here...">
                        </AjaxControlToolkit:TextBoxWatermarkExtender>
                        <asp:RequiredFieldValidator ID="rvInsertCapability" runat="server" ControlToValidate="tbInsertCapabilityName"
                            Enabled="false" Display="Dynamic" ErrorMessage="Capability Name is required"
                            ToolTip="Capability Name is required" ValidationGroup="InsertCapability">*</asp:RequiredFieldValidator>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
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
                            <asp:ValidationSummary ID="valSummaryInsert" ValidationGroup="InsertCapability" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice capability." />
                            <asp:ValidationSummary ID="valSummaryEdit" ValidationGroup="EditCapability" runat="server"
                                DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a practice capability." />
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

