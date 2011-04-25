<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeEntry.aspx.cs" Inherits="PraticeManagement.TimeEntry"
    MasterPageFile="~/PracticeManagementMain.Master" Title="Practice Management - Time Entry" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ElementDisabler" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/TimeEntry/WeekSelector.ascx" TagName="WeekSelector"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Persons/PersonChooser.ascx" TagName="PersonChooser"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/TimeEntry/TimeEntries.ascx" TagName="TimeEntries" TagPrefix="uc" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagPrefix="uc" TagName="MessageLabel" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="pcg" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Practice Management - Time Entry</title>
</asp:Content>
<asp:Content ID="cntHead" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function SetFocus(modalExId, tbNotesId) {
            var modalEx = $find(modalExId);
            modalEx.show();
            var tbNotes = $get(tbNotesId);
            if (tbNotes && !tbNotes.disabled) tbNotes.focus();
        }

        function changeIcon(tbNotesId, imgNoteId) {
            var tbNotes = $get(tbNotesId);
            var imgNote = $get(imgNoteId);
            if (tbNotes && imgNote) {
                if (tbNotes.value && tbNotes.value != '') {
                    imgNote.src = 'Images/balloon-ellipsis.png';
                }
                else {
                    imgNote.src = 'Images/balloon-plus.png';
                    imgNote.title = '';
                }
            }
        }
        function assignHiddenValues(hiddenNoteId, noteId, hiddenIsChargeableId, IsChargeableId, hiddenIsCorrectId, IsCorrectId) {
            var hiddenNote = $get(hiddenNoteId);
            var note = $get(noteId);
            var hiddenIsChargeable = $get(hiddenIsChargeableId);
            var IsChargeable = $get(IsChargeableId);
            var hiddenIsCorrect = $get(hiddenIsCorrectId);
            var IsCorrect = $get(IsCorrectId);
            hiddenNote.value = note.value;
            hiddenIsCorrect.value = IsCorrect.checked;
            hiddenIsChargeable.value = IsChargeable.checked;
        }
        function hideSuccessMessage() {
            message = document.getElementById("<%=mlConfirmation.ClientID %>" + "_lblMessage");
            if (message != null) {
                message.style.display = "none";
            }
            return true;
        }
        function EnableSaveButton(enable) {
            var button = document.getElementById("<%=btnSave.ClientID %>");
            if (button != null) {
                button.disabled = !enable;
            }
        }

        function EnableAddRowButton(enable) {
            var button = document.getElementById("<%=btnAddRow.ClientID %>");
            if (button != null) {
                button.disabled = !enable;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Time Entry
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="time-entry-bg-light-frame">
        <pcg:StyledUpdatePanel ID="updNavigation" runat="server" CssClass="tem-person-time"
            UpdateMode="Conditional">
            <ContentTemplate>
                <div class="tem-persons">
                    <uc:PersonChooser ID="pcPersons" runat="server" OnPersonChanged="pcPersons_PersonChanged" />
                </div>
                <div class="tem-week-of">
                    <uc:WeekSelector ID="wsChoose" runat="server" OnWeekChanged="wsChoose_WeekChanged" />
                </div>
                <div class="clear0">
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="pcPersons" />
                <asp:AsyncPostBackTrigger ControlID="wsChoose" />
            </Triggers>
        </pcg:StyledUpdatePanel>
    </div>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <div id="updateContainer" class="time-entry-grid">
        <asp:UpdatePanel ID="updTimeEntries" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc:MessageLabel ID="mlErrors" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                    WarningColor="Orange" EnableViewState="false" />
                <uc:TimeEntries ID="teList" runat="server" />
                <div class="buttons-block">
                    <table cellpadding="0" class="WholeWidth">
                        <tr>
                            <td colspan="3">
                                <p>
                                    <asp:Label ID="lblProjMile" EnableViewState="False" ForeColor="Red" Visible="False"
                                        runat="server">When milestone-project or time type are changed, please update individual cells in the row to save that changes.</asp:Label>
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnAddRow" runat="server" ValidationGroup="TE" OnClick="btnAddRow_Click"
                                    Text="Add Row" CssClass="fl-left" />
                            </td>
                            <td align="center">
                                <div style="font-size: 14px;">
                                    <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green"
                                        WarningColor="Orange" style="font-size: larger" />
                                </div>
                            </td>
                            <td style="text-align: right">
                                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save All" ValidationGroup="TE"
                                    CssClass="mrg0" OnClientClick="hideSuccessMessage();" />
                                <asp:CustomValidator ID="valMileProjTimeTypeDropdown" runat="server" ClientValidationFunction="ValidateTEBarDropdowns"
                                    ErrorMessage="Please select Project - Milestone(s)/Time Type(s) highlighted in red."
                                    Text="*" ValidationGroup="TE" CssClass="fl-right" Display="None" />
                                <asp:CustomValidator ID="valTeHours" runat="server" ClientValidationFunction="ValidateAllHours"
                                    ErrorMessage="Hours should be real and 0.00-24.00. Invalid entries are highlighted in red."
                                    Text="*" ValidationGroup="TE" CssClass="fl-right" Display="None" />
                                <asp:CustomValidator ID="valTeNote" runat="server" ClientValidationFunction="ValidateAllNotes"
                                    ErrorMessage="Note should be 3-1000 characters long. Invalid entries are highlighted in red."
                                    Text="*" ValidationGroup="TE" CssClass="fl-right" Display="None" />
                            </td>
                        </tr>
                    </table>
                </div>
                <uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="pcPersons" />
                <asp:AsyncPostBackTrigger ControlID="wsChoose" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>

