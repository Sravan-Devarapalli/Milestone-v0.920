<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BillableAndNonBillableSingleTimeEntry.ascx.cs"
    Inherits="PraticeManagement.Controls.TimeEntry.BillableAndNonBillableSingleTimeEntry" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.DirtyStateExtender"
    TagPrefix="ext" %>
<table>
    <tr>
        <td title="Billable">
            &nbsp;B
        </td>
        <td>
            <asp:TextBox ID="tbBillableHours" runat="server" MaxLength="5" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnBillableHours" runat="server" Value="" />
            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteActualHours" TargetControlID="tbBillableHours"
                FilterType="Numbers,Custom" FilterMode="ValidChars" ValidChars="." runat="server">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td>
            &nbsp;
        </td>
        <td valign="middle" rowspan="2">
            <table>
                <tr>
                    <td>
                        <asp:ImageButton ID="imgNote" runat="server" OnClientClick='<%# "SetFocus(\"" + modalEx.ClientID + "\",\"" + tbNotes.ClientID + "\",\"" + tbBillableHours.ClientID + "\",\"" + btnSaveNotes.ClientID + "\",\"" + tbNonBillableHours.ClientID + "\"); return false;"%>'
                            ImageUrl='<%# string.IsNullOrEmpty(tbNotes.Text) ? PraticeManagement.Constants.ApplicationResources.AddCommentIcon : PraticeManagement.Constants.ApplicationResources.RecentCommentIcon %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <image src='Images/trash-icon.gif' id='imgClear' class='PaddingTop5' title="Clear time and notes entered for this day only."
                            runat="server" onclick='<%# "javaScript:$find(\"" + deBillableHours.ClientID + "\").clearData(); $find(\"" + deNonBillableHours.ClientID + "\").clearData(); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\");"%>' />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td title="Non Billable">
            &nbsp;N
        </td>
        <td>
            <asp:TextBox ID="tbNonBillableHours" runat="server" MaxLength="5" onchange="setDirty();EnableSaveButton(true);" />
            <asp:HiddenField ID="hdnNonBillableHours" runat="server" Value="" />
            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteNonBillableHours" TargetControlID="tbNonBillableHours"
                FilterType="Numbers,Custom" FilterMode="ValidChars" ValidChars="." runat="server">
            </AjaxControlToolkit:FilteredTextBoxExtender>
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
</table>
<AjaxControlToolkit:ModalPopupExtender ID="modalEx" runat="server" TargetControlID="imgNote"
    PopupControlID="pnlTimeEntry" DropShadow="true" BackgroundCssClass="modalBackground"
    CancelControlID="cpp" />
<asp:Panel ID="pnlTimeEntry" runat="server" Style="display: none;" CssClass="pnlTimeEntryCss">
    <table>
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
                    TabIndex="1" />
                <asp:HiddenField ID="hdnNotes" runat="server" Value="" />
            </td>
        </tr>
        <tr>
            <td class="te-modal-inner">
                <table>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td align="right" class="padRight4">
                            <asp:Button ID="btnSaveNotes" runat="server" CausesValidation="false" Text="Save Notes"
                                OnClientClick='<%# "$find(\"" + deBillableHours.ClientID + "\").checkDirty(); $find(\"" + deNonBillableHours.ClientID + "\").checkDirty(); assignHiddenValues(\"" + hdnNotes.ClientID + "\",\"" + tbNotes.ClientID + "\"); changeIcon(\"" + tbNotes.ClientID + "\",\"" + imgNote.ClientID + "\"); $find(\"" + modalEx.ClientID + "\").hide(); $find(\"" + deBillableHours.ClientID + "\").makeDirty(); $find(\"" + deNonBillableHours.ClientID + "\").makeDirty(); ChangeTooltip(\"" + tbNotes.ClientID + "\"); return false;"%>' />
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
    <asp:HiddenField ID="hdnIsChargeCodeTurnOff" runat="server" />
    <ext:DirtyStateExtender ID="deBillableHours" runat="server" TargetControlID="hfDirtyBillableHours"
        HiddenActualHoursId="hdnBillableHours" NoteId="tbNotes" ActualHoursId="tbBillableHours"
        HiddenNoteId="hdnNotes" HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender"
        VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender" SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender" />
    <ext:DirtyStateExtender ID="deNonBillableHours" runat="server" TargetControlID="hfDirtyNonBillableHours"
        HiddenActualHoursId="hdnNonBillableHours" NoteId="tbNotes" ActualHoursId="tbNonBillableHours"
        HiddenNoteId="hdnNotes" HorizontalTotalCalculatorExtenderId="hfHorizontalTotalCalculatorExtender"
        VerticalTotalCalculatorExtenderId="hfVerticalTotalCalculatorExtender" SpreadSheetExtenderId="hfSpreadSheetTotalCalculatorExtender" />
</asp:Panel>

