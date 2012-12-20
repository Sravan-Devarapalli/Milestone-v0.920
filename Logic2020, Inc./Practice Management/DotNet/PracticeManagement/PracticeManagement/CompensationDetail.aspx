<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true"
    CodeBehind="CompensationDetail.aspx.cs" Inherits="PraticeManagement.CompensationDetail"
    Title="Compensation Details | Practice Management" %>

<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="Controls/PersonnelCompensation.ascx" TagName="PersonnelCompensation"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/PersonInfo.ascx" TagName="PersonInfo" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ PreviousPageType TypeName="PraticeManagement.Controls.PracticeManagementPersonDetailPageBase" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Compensation Details | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Compensation Details
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <cc1:StyledUpdatePanel ID="pnlBody" runat="server" CssClass="bg-light-frame">
        <ContentTemplate>
            <uc1:PersonInfo ID="personInfo" runat="server" />
            <table>
                <tr>
                    <td>
                        <uc1:PersonnelCompensation ID="personnelCompensation" runat="server" OnCompensationMethodChanged="personnelCompensation_CompensationMethodChanged"
                            ValidationGroup="CompensationDetail" OnSaveDetails="personnelCompensation_SaveDetails"
                            OnPeriodChanged="personnelCompensation_PeriodChanged" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hndEmployeePayTypeChange" runat="server" Value="change" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeEmployeePayTypeChange" runat="server"
                TargetControlID="hndEmployeePayTypeChange" PopupControlID="pnlEmployeePayTypeChange"
                BackgroundCssClass="modalBackground" DropShadow="false">
            </AjaxControlToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlEmployeePayTypeChange" runat="server" Style="display: none;" CssClass="popUpAttrition">
                <table>
                    <tr>
                        <td>
                            <asp:CustomValidator ID="cvEmployeePayTypeChangeViolation" runat="server" Text="*"
                                ForeColor="Black" ToolTip="" OnServerValidate="cvEmployeePayTypeChangeViolation_ServerValidate"
                                ValidationGroup="EmployeePayTypeChangeViolation" SetFocusOnError="true" EnableClientScript="false"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding: 4px;">
                            <asp:Button ID="btnEmployeePayTypeChangeViolationOk" runat="server" Text="Ok" OnClick="btnEmployeePayTypeChangeViolationOk_Click"
                                UseSubmitBehavior="false" />
                            &nbsp;
                            <asp:Button ID="btnEmployeePayTypeChangeViolationCancel" runat="server" Text="Cancel"
                                OnClick="btnEmployeePayTypeChangeViolationCancel_Click" UseSubmitBehavior="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                CancelControlID="btnCancelErrorPanel" DropShadow="false" OkControlID="btnOKErrorPanel" />
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
                            <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                            <asp:CustomValidator ID="custUserName" runat="server" ValidationGroup="PersonDetailsSave"
                                OnServerValidate="custUserName_ServerValidate"></asp:CustomValidator>
                            <asp:ValidationSummary ID="vsumCompensation" runat="server" EnableClientScript="false"
                                ValidationGroup="CompensationDetail" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10Px TextAlignCenterImp">
                            <asp:Button ID="btnOKErrorPanel" runat="server" Text="OK" Width="100" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hdRehireConfirmation" runat="server" Value="change" />
            <AjaxControlToolkit:ModalPopupExtender ID="mpeRehireConfirmation" runat="server"
                TargetControlID="hdRehireConfirmation" PopupControlID="pnlRehireConfirmation"
                BackgroundCssClass="modalBackground" DropShadow="false">
            </AjaxControlToolkit:ModalPopupExtender>
            <asp:Panel ID="pnlRehireConfirmation" runat="server" Style="display: none;" CssClass="popUpAttrition">
                <table>
                    <tr>
                        <td>
                            <asp:CustomValidator ID="cvRehireConfirmation" runat="server" Text="*" ErrorMessage=""
                                ForeColor="Black" ToolTip="" OnServerValidate="cvRehireConfirmation_ServerValidate"
                                ValidationGroup="RehireConfirmation" SetFocusOnError="true" EnableClientScript="false"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding: 4px;">
                            <asp:Button ID="btnRehireConfirmationOk" runat="server" Text="Ok" OnClick="btnRehireConfirmationOk_Click"
                                UseSubmitBehavior="false" />
                            &nbsp;
                            <asp:Button ID="btnRehireConfirmationCancel" runat="server" Text="Cancel" OnClick="btnRehireConfirmationCancel_Click"
                                UseSubmitBehavior="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="buttons-block">
                <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" CssClass="Width150pxImp" />&nbsp;
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="Width150pxImp" />
                <div class="clear0">
                </div>
            </div>
        </ContentTemplate>
    </cc1:StyledUpdatePanel>
</asp:Content>

