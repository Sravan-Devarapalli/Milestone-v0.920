<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillableAndNonBillableSingleTimeEntry.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.BillableAndNonBillableSingleTimeEntry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.DirtyState"
    TagPrefix="ext" %>
<table cellpadding="0" cellspacing="0px" class="WholeWidth">
    <tr>
        <td style="width: 2%;">
            B
        </td>
        <td style="padding: 6px 3px 6px 0px; width: 58%;">
            <asp:TextBox ID="tbBillableHours" runat="server" MaxLength="5" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnBillableHours" runat="server" Value="" />
            <ajaxToolkit:FilteredTextBoxExtender ID="fteActualHours" TargetControlID="tbBillableHours"
                FilterType="Numbers,Custom" FilterMode="ValidChars" ValidChars="." runat="server">
            </ajaxToolkit:FilteredTextBoxExtender>
        </td>
        <td valign="middle" rowspan="2" style="padding: 6px 4px 6px 10px; width: 40%;">
            <asp:ImageButton ID="imgNote" runat="server" OnClientClick='<%# "SetFocus(\"" + modalEx.ClientID + "\",\"" + tbNotes.ClientID + "\",\"" + tbBillableHours.ClientID + "\",\"" + chbIsChargeable.ClientID + "\",\"" + chbForDiffProject.ClientID + "\",\"" + btnSaveNotes.ClientID + "\",\"" + tbNonBillableHours.ClientID + "\"); return false;"%>'
                ImageUrl='<%# string.IsNullOrEmpty(tbNotes.Text) ? PraticeManagement.Constants.ApplicationResources.AddCommentIcon : PraticeManagement.Constants.ApplicationResources.RecentCommentIcon %>' />
            <image src='Images/trash-icon.gif' id='imgClear' style='padding-top: 5px;' title="Clear time and notes entered for this day only."
                onclick='<%# "javaScript:$find(\"" + deBillableHours.ClientID + "\").clearData(); $find(\"" + deNonBillableHours.ClientID + "\").clearData(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\");"%>' />
        </td>
    </tr>
    <tr>
        <td style="width: 2%;">
            N
        </td>
        <td style="padding: 6px 3px 6px 0px;  width: 58%;">
            <asp:TextBox ID="tbNonBillableHours" runat="server" MaxLength="5" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnNonBillableHours" runat="server" Value="" />
            <ajaxToolkit:FilteredTextBoxExtender ID="fteNonBillableHours" TargetControlID="tbNonBillableHours"
                FilterType="Numbers,Custom" FilterMode="ValidChars" ValidChars="." runat="server">
            </ajaxToolkit:FilteredTextBoxExtender>
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
                <asp:LinkButton ID="cpp" runat="server" OnClientClick='<%# "javaScript:$find(\"" + deBillableHours.ClientID + "\").clearNotes(); $find(\"" + deNonBillableHours.ClientID + "\").clearNotes(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); "%>'
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
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbIsChargeable" Enabled="false" runat="server" Text="" />
                                        <asp:HiddenField ID="hdnIsChargeable" runat="server" Value="false" />
                                        <asp:HiddenField ID="hdnDefaultIsChargeable" runat="server" Value="false" />
                                    </td>
                                    <td align="left">
                                        <label id="Label1" for="chbIsChargeable" runat="server">
                                            Time entered is billable to account.</label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chbForDiffProject" runat="server" Checked="False" Text="" />
                                        <asp:HiddenField ID="hdnForDiffProject" runat="server" Value="false" />
                                    </td>
                                    <td align="left">
                                        <label id="Label2" for="chbForDiffProject" runat="server">
                                            I am not sure this is the correct milestone.</label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="padding-right: 4px;">
                            <asp:Button ID="btnSaveNotes" runat="server" CausesValidation="false" Text="Save Notes"
                                OnClientClick='<%# "$find(\"" + deBillableHours.ClientID + "\").checkDirty(); $find(\"" + deNonBillableHours.ClientID + "\").checkDirty(); assignHiddenValues(\"" + hdnNotes.ClientID + "\",\"" + tbNotes.ClientID + "\",\"" + hdnIsChargeable.ClientID + "\",\"" + chbIsChargeable.ClientID + "\",\"" + hdnForDiffProject.ClientID + "\",\"" + chbForDiffProject.ClientID + "\"); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); $find(\"" + modalEx.ClientID + "\").hide(); $find(\"" + deBillableHours.ClientID + "\").makeDirty(); $find(\"" + deNonBillableHours.ClientID + "\").makeDirty(); return false;"%>' />
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
    <asp:HiddenField ID="hfDirtyBillableHours" runat="server" />
    <asp:HiddenField ID="hfDirtyNonBillableHours" runat="server" />
    <asp:HiddenField ID="hfVerticalTotalCalculatorExtender" runat="server" />
    <asp:HiddenField ID="hfHorizontalTotalCalculatorExtender" runat="server" />
    <asp:HiddenField ID="hfSpreadSheetTotalCalculatorExtender" runat="server" />
    <asp:HiddenField ID="hdnIsNoteRequired" runat="server" />
    <asp:HiddenField ID="hdnIsHourlyRevenue" runat="server" />
    <asp:HiddenField ID="hdnIsPTOTimeType" runat="server" />
    <ext:DirtyExtender ID="deBillableHours" runat="server" TargetControlID="hfDirtyBillableHours"
        HiddenActualHoursId="hdnBillableHours" NoteId="tbNotes" ActualHoursId="tbBillableHours"
        IsCorrectId="chbForDiffProject" HiddenNoteId="hdnNotes" HiddenIsChargeableId="hdnIsChargeable"
        HiddenIsCorrectId="hdnForDiffProject" HiddenDefaultIsChargeableIdValue="hdnDefaultIsChargeable"
        HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender" VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender"
        IsNoteRequired="hdnIsNoteRequired" SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender"
        IsChargeableId="chbIsChargeable" IsPTOTimeType="hdnIsPTOTimeType" />
    <ext:DirtyExtender ID="deNonBillableHours" runat="server" TargetControlID="hfDirtyNonBillableHours"
        HiddenActualHoursId="hdnNonBillableHours" NoteId="tbNotes" ActualHoursId="tbNonBillableHours"
        IsCorrectId="chbForDiffProject" HiddenNoteId="hdnNotes" HiddenIsChargeableId="hdnIsChargeable"
        HiddenIsCorrectId="hdnForDiffProject" HiddenDefaultIsChargeableIdValue="hdnDefaultIsChargeable"
        HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender" VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender"
        IsNoteRequired="hdnIsNoteRequired" SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender"
        IsChargeableId="chbIsChargeable" IsPTOTimeType="hdnIsPTOTimeType" />
</asp:Panel>

