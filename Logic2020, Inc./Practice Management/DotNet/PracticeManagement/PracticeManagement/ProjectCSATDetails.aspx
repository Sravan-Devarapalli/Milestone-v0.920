<%@ Page Title="Project CSAT Details" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectCSATDetails.aspx.cs" Inherits="PraticeManagement.ProjectCSATDetails" %>

<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register TagPrefix="cc" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <uc:LoadingProgress ID="loadingProgress" runat="server" />
    <asp:UpdatePanel ID="upd" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                function enterPressed(evn) {
                    if (window.event && window.event.keyCode == 13) {
                        return false;
                    } else if (evn && evn.keyCode == 13) {
                        return false;
                    }
                }
                function pageLoad() {
                    document.onkeypress = enterPressed;
                }

            </script>
            <asp:HiddenField ID="hdCSATId" runat="server" />
            <div class="bg-light-frame WholeWidth">
                <div class="ProjectDetailBlue">
                    <asp:Label ID="lblProjectName" runat="server"></asp:Label>
                </div>
                <table class="Width50Percent Padding5">
                    <tr>
                        <td class="width30P PaddingTop5 padLeft5">
                            Project Review Period
                        </td>
                        <td class="PaddingTop5">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        Start Date
                                    </td>
                                    <td>
                                        <span class="fl-left Width85Percent">
                                            <uc2:DatePicker ID="dpReviewStartDate" ValidationGroup="CSATPopup" runat="server" 
                                                TextBoxWidth="90%" AutoPostBack="false" />
                                        </span><span class="Width15Percent vMiddle">
                                            <asp:RequiredFieldValidator ID="reqStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                                                ValidationGroup="CSATPopup" ErrorMessage="The Review Start Date is required."
                                                ToolTip="The Start Date is required." Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Static"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compStartDate" runat="server" ControlToValidate="dpReviewStartDate"
                                                ValidationGroup="CSATPopup" ErrorMessage="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                ToolTip="The Review Start Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                        </span>
                                    </td>
                                    <td>
                                        End Date
                                    </td>
                                    <td>
                                        <span class="fl-left Width85Percent">
                                            <uc2:DatePicker ID="dpReviewEndDate" ValidationGroup="CSATPopup" runat="server" TextBoxWidth="90%"
                                                AutoPostBack="false" />
                                        </span><span class="Width15Percent vMiddle">
                                            <asp:RequiredFieldValidator ID="reqEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                                                ValidationGroup="CSATPopup" ErrorMessage="The Review End Date is required." ToolTip="The End Date is required."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="compEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                                                ValidationGroup="CSATPopup" ErrorMessage="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                ToolTip="The Review End Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                            <asp:CompareValidator ID="compEndDateGreater" runat="server" ControlToValidate="dpReviewEndDate"
                                                ControlToCompare="dpReviewStartDate" ErrorMessage="The Review Period End Date must be greater or equal to Review Period Start Date."
                                                ToolTip="The Review Period End Date must be greater or equal to Review Period Start Date."
                                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                                Operator="GreaterThanEqual" Type="Date" ValidationGroup="CSATPopup"></asp:CompareValidator>
                                            <asp:CustomValidator ID="custCSATEndDate" runat="server" ControlToValidate="dpReviewEndDate"
                                                ErrorMessage="The Review End Date can not be greater than the date on which project status was set to 'Completed'."
                                                ToolTip="The Review End Date can not be greater than the date on which project status was set to 'Completed'."
                                                ValidationGroup="CSATPopup" Text="*" EnableClientScript="false" SetFocusOnError="true"
                                                Display="Dynamic" OnServerValidate="custCSATEndDate_ServerValidate"></asp:CustomValidator>
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="PaddingTop5 padLeft5">
                            CSAT Completion Date
                        </td>
                        <td class="PaddingTop5">
                            <span class="fl-left Width85Percent">
                                <uc2:DatePicker ID="dpCompletionDate" ValidationGroup="CSATPopup" runat="server"
                                    TextBoxWidth="90%" AutoPostBack="false" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="dpCompletionDate"
                                    ValidationGroup="CSATPopup" ErrorMessage="The Completion Date is required." ToolTip="The Completion Date is required."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="dpCompletionDate"
                                    ValidationGroup="CSATPopup" ErrorMessage="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                    ToolTip="The Completion Date has an incorrect format. It must be 'MM/dd/yyyy'."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                    Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                                <asp:CustomValidator ID="custCSATCompleteDate" runat="server" ValidationGroup="CSATPopup"
                                    ErrorMessage="The CSAT Completion Date must be less than or equal to current date."
                                    ToolTip="The CSAT Completion Date must be less than or equal to current date."
                                    Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Dynamic"
                                    OnServerValidate="custCSATCompleteDate_ServerValidate"></asp:CustomValidator>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="PaddingTop5 padLeft5">
                            CSAT Reviewer
                        </td>
                        <td class="PaddingTop5">
                            <asp:DropDownList ID="ddlReviewer" runat="server" Style="width: 32.5%" onchange="setDirty();" >
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="reqReviewer" runat="server" ControlToValidate="ddlReviewer"
                                ValidationGroup="CSATPopup" ErrorMessage="The Reviewer is required." ToolTip="The Reviewer is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="PaddingTop5 padLeft5">
                            On a scale of 0-10, would you refer<br />
                            Logic20/20 to a friend or colleague?
                        </td>
                        <td class="PaddingTop5">
                            <asp:DropDownList ID="ddlScore" runat="server" Style="width: 32.5%"  onchange="setDirty();" >
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="reqScore" runat="server" ControlToValidate="ddlScore"
                                ValidationGroup="CSATPopup" ErrorMessage="The Referral Score is required." ToolTip="The Referral Score is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="PaddingTop5 padLeft5">
                            Comments
                        </td>
                        <td class="PaddingTop5">
                            <textarea class="ResizeNone" id="taComments" runat="server" rows="5" style="width: 93%" onchange="setDirty();" ></textarea>
                            <asp:RequiredFieldValidator ID="reqComments" runat="server" ControlToValidate="taComments"
                                ValidationGroup="CSATPopup" ErrorMessage="The Comments is required." ToolTip="The Comments is required."
                                Text="*" EnableClientScript="false" SetFocusOnError="true" Display="Static"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <div class="buttons-block PaddingTop10">
                    <cc:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" CssClass="Width150pxImp" />&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" Width="100px" />
                </div>
                <asp:HiddenField ID="hdnTargetErrorPanel" runat="server" />
                <AjaxControlToolkit:ModalPopupExtender ID="mpeErrorPanel" runat="server" BehaviorID="mpeErrorPanelBehaviourId"
                    TargetControlID="hdnTargetErrorPanel" BackgroundCssClass="modalBackground" PopupControlID="pnlErrorPanel"
                    OkControlID="btnOKErrorPanel" CancelControlID="btnOKErrorPanel" DropShadow="false" />
                <asp:Panel ID="pnlErrorPanel" runat="server" Style="display: none;" CssClass="ProjectDetailErrorPanel PanelPerson">
                    <table class="Width100Per">
                        <tr>
                            <th align="center" class="TextAlignCenter BackGroundColorGray vBottom">
                                <b class="BtnClose">Attention!</b>
                            </th>
                        </tr>
                        <tr>
                            <td class="Padding10Px">
                                <uc:Label ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                <asp:ValidationSummary ID="vsumCSAT" runat="server" DisplayMode="BulletList" CssClass="ApplyStyleForDashBoardLists"
                                    ShowMessageBox="false" ShowSummary="true" EnableClientScript="false" HeaderText="Following errors occurred while saving a project."
                                    ValidationGroup="CSATPopup" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Padding10Px TextAlignCenter">
                                <asp:Button ID="btnOKErrorPanel" runat="server" ToolTip="OK" Text="OK" CssClass="Width100PxImp"
                                    OnClientClick="$find('mpeErrorPanelBehaviourId').hide();return false;" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

