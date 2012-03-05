﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdministrativeSingleTimeEntry.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.AdministrativeSingleTimeEntry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.DirtyStateExtender"
    TagPrefix="ext" %>
<table cellpadding="0" cellspacing="0px">
    <tr>
        <td>
            &nbsp;N
        </td>
        <td>
            <asp:TextBox ID="tbActualHours" runat="server" MaxLength="5" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnActualHours" runat="server" Value="" />
            <ajaxToolkit:FilteredTextBoxExtender ID="fteActualHours" TargetControlID="tbActualHours"
                FilterType="Numbers,Custom" FilterMode="ValidChars" ValidChars="." runat="server">
            </ajaxToolkit:FilteredTextBoxExtender>
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            <table>
                <tr>
                    <td>
                        <asp:ImageButton ID="imgNote" runat="server" OnClientClick='<%# "SetFocus(\"" + modalEx.ClientID + "\",\"" + tbNotes.ClientID + "\",\"" + tbActualHours.ClientID + "\",\"" + btnSaveNotes.ClientID + "\",\"" + tbActualHours.ClientID + "\"); return false;"%>'
                            ImageUrl='<%# string.IsNullOrEmpty(tbNotes.Text) ? PraticeManagement.Constants.ApplicationResources.AddCommentIcon : PraticeManagement.Constants.ApplicationResources.RecentCommentIcon %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <image src='Images/trash-icon.gif' runat="server" id='imgClear' style='padding-top: 5px;'
                            runat="server" title="Clear time and notes entered for this day only." onclick='<%# "javaScript:$find(\"" + deActualHours.ClientID + "\").clearData(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\");"%>' />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<AjaxControlToolkit:ModalPopupExtender ID="modalEx" runat="server" TargetControlID="imgNote"
    PopupControlID="pnlTimeEntry" DropShadow="true" BackgroundCssClass="modalBackground"
    CancelControlID="cpp" />
<asp:Panel ID="pnlTimeEntry" runat="server" Style="display: none;" CssClass="pnlTimeEntryCss"
    Width="375">
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
                    onchange="ChangeTooltip(this);" Style="resize: none; overflow-y: auto;" TabIndex="1" />
                <asp:HiddenField ID="hdnNotes" runat="server" Value="" />
            </td>
        </tr>
        <tr>
            <td class="te-modal-inner">
                <table>
                    <tr>
                        <td>
                            <table id="tblApprovedby" runat="server">
                                <tr>
                                    <td>
                                        <b>Approved by:</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlApprovedManagers" OnDataBound="ddlApprovedManagers_OnDataBound"
                                            AppendDataBoundItems="false" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="padding-right: 4px;">
                            <asp:Button ID="btnSaveNotes" runat="server" CausesValidation="false" Text="Save Notes"
                                OnClientClick='<%# "$find(\"" + deActualHours.ClientID + "\").checkDirty(); assignHiddenValues(\"" + hdnNotes.ClientID + "\",\"" + tbNotes.ClientID + "\"); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); $find(\"" + modalEx.ClientID + "\").hide(); $find(\"" + deActualHours.ClientID + "\").makeDirty(); return false;"%>' />
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
    <asp:HiddenField ID="hdnIsNoteRequired" runat="server" />
    <asp:HiddenField ID="hdnIsChargeCodeTurnOff" runat="server" />
    <asp:HiddenField ID="hdnIsPTOTimeType" runat="server" />
    <ext:DirtyStateExtender ID="deActualHours" runat="server" TargetControlID="hfDirtyHours"
        HiddenActualHoursId="hdnActualHours" NoteId="tbNotes" ActualHoursId="tbActualHours"
        HiddenNoteId="hdnNotes" HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender"
        VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender" SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender" />
</asp:Panel>

