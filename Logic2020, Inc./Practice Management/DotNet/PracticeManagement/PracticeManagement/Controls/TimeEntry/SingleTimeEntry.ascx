<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SingleTimeEntry.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.SingleTimeEntry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.DirtyState"
    TagPrefix="ext" %>
<table cellpadding="0" cellspacing="0px" style="width:90%">
    <tr>
        <td style="width:60%;">
            <asp:TextBox ID="tbActualHours" runat="server" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnActualHours" runat="server" Value="" />
        </td>
        <td style="padding-left: 10px; width:40%;">
            <asp:ImageButton ID="imgNote" runat="server" OnClientClick='<%# "SetFocus(\"" + modalEx.ClientID + "\",\"" + tbNotes.ClientID + "\"); return false;"%>'
                ImageUrl='<%# string.IsNullOrEmpty(tbNotes.Text) ? PraticeManagement.Constants.ApplicationResources.AddCommentIcon : PraticeManagement.Constants.ApplicationResources.RecentCommentIcon %>' />
            <image src='Images/trash-icon.gif' id='imgClear' style='padding-top: 5px;' title="Clear time and notes entered for this day only."
                onclick='<%# "javaScript:$find(\"" + deActualHours.ClientID + "\").clearData(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\");"%>' />
        </td>
    </tr>
</table>
<AjaxControlToolkit:ModalPopupExtender ID="modalEx" runat="server" TargetControlID="imgNote"
    PopupControlID="pnlTimeEntry" DropShadow="true" BackgroundCssClass="modalBackground"
    CancelControlID="cpp" />
<asp:Panel ID="pnlTimeEntry" runat="server" Style="display: none;" CssClass="pnlTimeEntryCss" Width="375">
    <asp:Label ID="lblEntryDate" runat="server" Text='<%# GetNowDate() %>' Visible="false" />
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="te-modal-header">
                <asp:LinkButton ID="cpp" runat="server" OnClientClick='<%# "javaScript:$find(\"" + deActualHours.ClientID + "\").clearNotes(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); "%>'
                    Text="" CssClass="modal-close" ToolTip="Cancel without saving" />
                Review status:
                <asp:Label ID="lblReview" runat="server" Text="N/A" />
            </td>
        </tr>
        <tr>
            <td class="comment">
                <asp:TextBox ID="tbNotes" runat="server" Columns="50" MaxLength="1000" Rows="5" TextMode="MultiLine"
                    TabIndex="1"  />
                <asp:HiddenField ID="hdnNotes" runat="server" Value="" />
            </td>
        </tr>
        <tr>
            <td class="te-modal-inner">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbIsChargeable" runat="server" Text=""  />
                                        <asp:HiddenField ID="hdnIsChargeable" runat="server" Value="false" />
                                        <asp:HiddenField ID="hdnDefaultIsChargeable" runat="server" Value="false" />
                                    </td>
                                    <td align="left">
                                        <label for="chbIsChargeable" runat="server">
                                            Time entered is billable to client.</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbForDiffProject" runat="server" Checked="False" Text=""  />
                                        <asp:HiddenField ID="hdnForDiffProject" runat="server" Value="false" />
                                    </td>
                                    <td align="left">
                                        <label id="Label1" for="chbForDiffProject" runat="server">
                                            I am not sure this is the correct milestone.</label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="padding-right: 4px;">
                            <asp:Button ID="btnSaveNotes" runat="server" CausesValidation="false" Text="Save Notes"
                                OnClientClick='<%# "assignHiddenValues(\"" + hdnNotes.ClientID + "\",\"" + tbNotes.ClientID + "\",\"" + hdnIsChargeable.ClientID + "\",\"" + chbIsChargeable.ClientID + "\",\"" + hdnForDiffProject.ClientID + "\",\"" + chbForDiffProject.ClientID + "\"); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); $find(\"" + modalEx.ClientID + "\").hide(); $find(\"" + deActualHours.ClientID + "\").makeDirty(); return false;"%>' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <uc:MessageLabel ID="mlMessage" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                                WarningColor="#660099" EnableViewState="false" CssClass="ste-label" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfDirtyHours" runat="server" />
    <asp:HiddenField ID="hfVerticalTotalCalculatorExtender" runat="server" />
    <asp:HiddenField ID="hfHorizontalTotalCalculatorExtender" runat="server" />
    <asp:HiddenField ID="hfSpreadSheetTotalCalculatorExtender" runat="server" />
    <ext:DirtyExtender ID="deActualHours" runat="server" TargetControlID="hfDirtyHours"
        HiddenActualHoursId="hdnActualHours" NoteId="tbNotes" ActualHoursId="tbActualHours"
        IsCorrectId="chbForDiffProject" HiddenNoteId="hdnNotes" HiddenIsChargeableId="hdnIsChargeable"
        HiddenIsCorrectId="hdnForDiffProject" HiddenDefaultIsChargeableIdValue="hdnDefaultIsChargeable"
        HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender" VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender"
        SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender" IsChargeableId="chbIsChargeable" />
</asp:Panel>

