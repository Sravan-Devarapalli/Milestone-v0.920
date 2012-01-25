<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" Title="Time Entry | Practice Management"
    AutoEventWireup="true" CodeBehind="TimeEntry_New.aspx.cs" Inherits="PraticeManagement.TimeEntry_New" %>

<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.DuplicateOptionsRemove"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox"
    TagPrefix="ext" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagPrefix="uc" TagName="MessageLabel" %>
<%@ Register Src="~/Controls/TimeEntry/WeekSelector_New.ascx" TagName="WeekSelector"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Persons/PersonChooser.ascx" TagName="PersonChooser"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/TimeEntry/NonBillableTimeEntryBar.ascx" TagName="NonBillableTimeEntryBar"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/TimeEntry/AdministrativeTimeEntryBar.ascx" TagName="AdministrativeTimeEntryBar"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/TimeEntry/BillableAndNonBillableTimeEntryBar.ascx" TagName="BillableAndNonBillableTimeEntryBar"
    TagPrefix="uc" %>
<%@ Register Src="Controls/Generic/LoadingProgress.ascx" TagName="LoadingProgress"
    TagPrefix="uc" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic"
    TagPrefix="pcg" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<asp:Content ID="cntTitle" ContentPlaceHolderID="title" runat="server">
    <title>Time Entry | Practice Management</title>
</asp:Content>
<asp:Content ID="cntHead" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
      
        function SetFocus(modalExId, tbNotesId, tbBillableHoursId, chbIsChargeableId, chbForDiffProjectId, btnSaveNotesId, tbNonBillableHoursId) {

            var tbActualHours = $find(tbBillableHours);

            var modalEx = $find(modalExId);
            modalEx.show();
            var tbNotes = $get(tbNotesId);
            var chbIsChargeable = $get(chbIsChargeableId);
            var chbForDiffProject = $get(chbForDiffProjectId);
            var btnSaveNotes = $get(btnSaveNotesId);
            var tbBillableHours = $get(tbBillableHoursId);
            var tbNonBillableHours = $get(tbNonBillableHoursId);

            if (tbBillableHours.disabled && tbNonBillableHours.disabled) {
                tbNotes.disabled = 'disabled';
                chbIsChargeable.disabled = 'disabled';
                chbForDiffProject.disabled = 'disabled';
                btnSaveNotes.disabled = 'disabled';
            }
            else {
                tbNotes.disabled = '';
                chbIsChargeable.disabled = '';
                chbForDiffProject.disabled = '';
                btnSaveNotes.disabled = '';

            }

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

        function ddlChild_onchange(ddl) {

            var btnAdd = document.getElementById(ddl.attributes["add"].value);
            if (ddl.options.length > 0) {
                var optionList = ddl.getElementsByTagName('option');
                if (optionList[0].value == ddl.value) {
                    btnAdd.disabled = true;
                }
                else {
                    btnAdd.disabled = false;
                }
            }
            else {
                btnAdd.disabled = true;
            }
        }

        function ddlParent_onchange(ddl) {
            var btnAdd = document.getElementById(ddl.attributes["add"].value);
            btnAdd.disabled = true;
        }

        function EnableSaveButton(enable) {

        }

        function ChangeTooltip(tbnote) {
            var imgNoteClientId = document.getElementById(tbnote.attributes["imgNoteClientId"].value);
            imgNoteClientId.title = tbnote.value;
            changeIcon(tbnote.id, imgNoteClientId.id);
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
                <div>
                    <div class="tem-persons">
                        <uc:PersonChooser ID="pcPersons" runat="server" OnPersonChanged="pcPersons_PersonChanged" />
                    </div>
                    <div class="tem-week-of">
                        <uc:WeekSelector ID="wsChoose" runat="server" OnWeekChanged="wsChoose_WeekChanged"
                            OnDatePickerChanged="dpChoose_OnSelectionChanged" />
                    </div>
                    <div class="clear0">
                    </div>
                    <div style="text-align: center;">
                        <span style="color: Red;">ALERT</span><asp:Label ID="lblAlertNote" runat="server"></asp:Label></div>
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
        <asp:UpdatePanel ID="updTimeEntries" runat="server">
            <ContentTemplate>
                <script type="text/javascript">
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

                    function endRequestHandle(sender, args) {
                        var cpeProjectSection = $find('cpeProjectSection');
                        cpeProjectSection._collapsed = false;
                        cpeProjectSection._autoExpand = true;
                    }
                </script>
                <uc:MessageLabel ID="mlErrors" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                    WarningColor="Orange" EnableViewState="false" />
                <asp:Panel ID="pnlShowTimeEntries" Visible="false" runat="server">
                    <div class="buttons-block">
                        <table cellpadding="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeProjectSection" runat="Server"
                                        EnableViewState="false" TargetControlID="pnlProjectSection" ImageControlID="btnExpandCollapseFilter"
                                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                        ExpandControlID="btnExpandCollapseFilter" TextLabelID="lblFilter" BehaviorID="cpeProjectSection" />
                                    <asp:Label ID="lblFilter" runat="server"></asp:Label>
                                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                        ToolTip="Expand Filters" />&nbsp;<b>Project</b>
                                </td>
                                <td align="center">
                                </td>
                                <td style="text-align: right">
                                    <asp:Button ID="btnAddProject" runat="server" Text="Add Project" CssClass="mrg0" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlProjectSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repProjectSections" runat="server" OnItemDataBound="repProjectSections_OnItemDataBound">
                            <ItemTemplate>
                                <table cellpadding="0" class="WholeWidth">
                                    <tr>
                                        <td>
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("AccountName")).Value + "-" + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("ProjectName")).Value + "(" + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("ProjectNumber")).Value + ")"%>
                                        </td>
                                        <td align="center">
                                        </td>
                                        <td style="text-align: right">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecursiveProjectSection" runat="server"
                                                TargetControlID="imgBtnRecursiveProjectSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecursiveProjectSection" runat="server" ImageUrl="~/Images/Recursive.png"
                                                OnClick="imgBtnRecursiveSection_OnClick" CssClass="mrg0" />
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeimgBtnDeleteProjectSection" runat="server"
                                                TargetControlID="imgBtnDeleteProjectSection" ConfirmText="Are you sure to delete the section?">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnDeleteProjectSection" runat="server" ImageUrl="~/Images/close_16.png"
                                                OnClick="imgBtnDeleteSection_OnClick" CssClass="mrg0" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repProjectTes" runat="server" OnItemDataBound="repProjectTes_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="time-entry-bar-time-types">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repProjectTesHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-te">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hours">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td style="width: 3%">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:BillableAndNonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:ImageButton ID="imgPlusProjectSection" OnClick="imgPlusProjectSection_OnClick"
                                            runat="server" ImageUrl="~/Images/add_16.png" ToolTip="Add Additional Work type" />
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeBusinessDevelopmentSection" runat="Server"
                                        EnableViewState="false" TargetControlID="pnlBusinessDevelopmentSection" ImageControlID="Image1"
                                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="Image1"
                                        ExpandControlID="Image1" TextLabelID="Label1" />
                                    <asp:Label ID="Label1" runat="server"></asp:Label>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />&nbsp;<b>Business
                                        Development</b>
                                </td>
                                <td align="center">
                                </td>
                                <td style="text-align: right">
                                    <asp:Button ID="btnAddAccount" runat="server" Text="Add Account" CssClass="mrg0" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlBusinessDevelopmentSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repBusinessDevelopmentSections" OnItemDataBound="repBusinessDevelopmentSections_OnItemDataBound"
                            runat="server">
                            <ItemTemplate>
                                <table cellpadding="0" class="WholeWidth">
                                    <tr>
                                        <td>
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("AccountName")).Value + "-" + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("BusinessUnitName")).Value %>
                                        </td>
                                        <td align="center">
                                        </td>
                                        <td style="text-align: right">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecurrenceBusinessDevelopmentSection"
                                                runat="server" TargetControlID="imgBtnRecurrenceBusinessDevelopmentSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecurrenceBusinessDevelopmentSection" runat="server" OnClick="imgBtnRecursiveSection_OnClick"
                                                CssClass="mrg0" />
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnDeleteBusinessDevelopmentSection"
                                                runat="server" TargetControlID="imgBtnDeleteBusinessDevelopmentSection" ConfirmText="Are you sure to delete the section?">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnDeleteBusinessDevelopmentSection" runat="server" ImageUrl="~/Images/close_16.png"
                                                OnClick="imgBtnDeleteSection_OnClick" CssClass="mrg0" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repBusinessDevelopmentTes" OnItemDataBound="repBusinessDevelopmentTes_ItemDataBound"
                                    runat="server">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="time-entry-bar-time-types">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repBusinessDevelopmentHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-te">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hours">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td style="width: 3%">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:NonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:ImageButton ID="imgPlusBusinessDevelopmentSection" OnClick="imgPlusBusinessDevelopmentSection_OnClick"
                                            runat="server" ImageUrl="~/Images/add_16.png" ToolTip="Add Additional Work type" />
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeInternalSection" runat="Server"
                                        EnableViewState="false" TargetControlID="pnlInternalSection" ImageControlID="Image2"
                                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="Image2"
                                        ExpandControlID="Image2" TextLabelID="Label2" />
                                    <asp:Label ID="Label2" runat="server"></asp:Label>
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Filters" />&nbsp;<b>Internal</b>
                                </td>
                                <td align="center">
                                </td>
                                <td style="text-align: right">
                                    <asp:Button ID="btnAddInternalProject" runat="server" Text="Add Project" CssClass="mrg0" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlInternalSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repInternalSections" OnItemDataBound="repInternalSections_ItemDataBound"
                            runat="server">
                            <ItemTemplate>
                                <table cellpadding="0" class="WholeWidth">
                                    <tr>
                                        <td>
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("BusinessUnitName")).Value + "-" + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get("ProjectName")).Value%>
                                        </td>
                                        <td align="center">
                                        </td>
                                        <td style="text-align: right">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecurrenceInternalSection"
                                                runat="server" TargetControlID="imgBtnRecurrenceInternalSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecurrenceInternalSection" runat="server" OnClick="imgBtnRecursiveSection_OnClick"
                                                CssClass="mrg0" />
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnDeleteInternalSection" runat="server"
                                                TargetControlID="imgBtnDeleteInternalSection" ConfirmText="Are you sure to delete the section?">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnDeleteInternalSection" runat="server" ImageUrl="~/Images/close_16.png"
                                                OnClick="imgBtnDeleteSection_OnClick" CssClass="mrg0" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repInternalTes" OnItemDataBound="repInternalTes_ItemDataBound"
                                    runat="server">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="time-entry-bar-time-types">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repInternalTesHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-te">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hours">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td style="width: 3%">
                                                    &nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:NonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:ImageButton ID="imgPlusInternalSection" OnClick="imgPlusInternalSection_OnClick"
                                            runat="server" ImageUrl="~/Images/add_16.png" ToolTip="Add Additional Work type" />
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" class="WholeWidth">
                            <tr>
                                <td>
                                    &nbsp;<b>Administrative</b>
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="Panel3" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repAdministrativeTes" OnItemDataBound="repAdministrativeTes_ItemDataBound"
                            runat="server">
                            <HeaderTemplate>
                                <table class="CompPerfTable WholeWidth">
                                    <tr class="CompPerfHeader WholeWidth">
                                        <td class="time-entry-bar-time-types">
                                            <div class="ie-bg">
                                                Work Type</div>
                                        </td>
                                        <asp:Repeater ID="repAdministrativeTesHeader" runat="server">
                                            <ItemTemplate>
                                                <td class="time-entry-bar-single-te">
                                                    <div class="ie-bg">
                                                        <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                </td>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <td class="time-entry-bar-total-hours">
                                            <div class="ie-bg">
                                                TOTAL</div>
                                        </td>
                                        <td style="width: 3%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <uc:AdministrativeTimeEntryBar runat="server" ID="bar" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <asp:Panel ID="pnlTotalSection" runat="server" Style="padding: 10px;" CssClass="cp bg-white">
                        <table class="WholeWidth">
                            <tr class="time-entry-bar">
                                <td style="text-align: right; font-weight: bold; padding: 10px;" class="time-entry-bar-time-typesNew">
                                    TOTAL HOURS:
                                </td>
                                <asp:Repeater ID="repDayTotalHours" OnItemDataBound="repDayTotalHours_OnItemDataBound"
                                    runat="server">
                                    <ItemTemplate>
                                        <td style="border: 1px solid black;" class="time-entry-bar-single-teNew">
                                            <asp:Label ID="lblDayTotal" TotalHours="" runat="server"></asp:Label>
                                            <ext:TotalCalculatorExtender ID="extDayTotal" runat="server" TargetControlID="lblDayTotal" />
                                            <asp:HiddenField ID="hdnDayTotal" runat="server"></asp:HiddenField>
                                            <ext:MaxValueAllowedForTextBoxExtender ID="extMaxValueAllowedForTextBoxExtender"
                                                runat="server" TargetControlID="lblDayTotal">
                                            </ext:MaxValueAllowedForTextBoxExtender>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <td class="time-entry-total-hoursNew">
                                </td>
                                <td style="width: 3%;">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" style="width: 70%; text-align: right; font-weight: bold;">
                                    BILLABLE TOTAL :
                                </td>
                                <td class="time-entry-total-hoursNew">
                                    <label id="lblBillableGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extBillableGrandTotal" runat="server" TargetControlID="lblBillableGrandTotal" />
                                </td>
                                <td style="width: 3%;">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" style="width: 70%; text-align: right; font-weight: bold;">
                                    NON BILLABLE TOTAL :
                                </td>
                                <td class="time-entry-total-hoursNew">
                                    <label id="lblNonBillableGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extNonBillableGrandTotal" runat="server" TargetControlID="lblNonBillableGrandTotal" />
                                </td>
                                <td style="width: 3%;">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" style="width: 70%; text-align: right; font-weight: bold; padding-top: 15px;">
                                    TIME PERIOD GRAND TOTAL:
                                </td>
                                <td style="padding-top: 15px;" class="time-entry-total-hoursNew">
                                    <label id="lbltimePeriodGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lbltimePeriodGrandTotal" />
                                </td>
                                <td style="width: 3%;">
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" class="WholeWidth">
                            <tr>
                                <td colspan="2">
                                    <asp:CustomValidator ID="custWorkType" runat="server" ErrorMessage="Please select Work Type."
                                        OnServerValidate="custWorkType_ServerValidate" EnableClientScript="false" ToolTip="Please select Work Type."
                                        SetFocusOnError="true" Text="*" ValidationGroup="TE" Display="None" />
                                    <asp:CustomValidator ID="custActualHours" runat="server" ToolTip="Hours should be real and 0.01-24.00. Invalid entries are highlighted in red."
                                        ErrorMessage="Hours should be real and 0.01-24.00. Invalid entries are highlighted in red."
                                        SetFocusOnError="true" OnServerValidate="custActualHours_ServerValidate" EnableClientScript="false"
                                        Display="None" Text="*" ValidationGroup="TE" />
                                    <asp:CustomValidator ID="custNote" runat="server" ErrorMessage="Note should be 3-1000 characters long. Invalid entries are highlighted in red."
                                        OnServerValidate="custNote_ServerValidate" EnableClientScript="false" Text="*"
                                        Display="None" SetFocusOnError="true" ValidationGroup="TE" ToolTip="Note should be 3-1000 characters long. Invalid entries are highlighted in red." />
                                    <asp:CustomValidator ID="cvDayTotal" runat="server" ErrorMessage="Day Total hours must be lessthan or equals to 24."
                                        OnServerValidate="cvDayTotal_ServerValidate" EnableClientScript="false" Text="*"
                                        Display="None" SetFocusOnError="true" ValidationGroup="TE" ToolTip="Day Total hours must be lessthan or equals to 24." />
                                      <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green" WarningColor="Orange" />
                                    <asp:ValidationSummary ID="valSumSaveTimeEntries" runat="server" ValidationGroup="TE" />
                                </td>
                                <td valign="middle">
                                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_OnClick" Text="Save All"
                                        ToolTip="Save All" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
                <AjaxControlToolkit:ModalPopupExtender ID="mpePopup" runat="server" TargetControlID="btnAddProject"
                    CancelControlID="btnCancelProjectSection" BehaviorID="mpeProjectSectionPopup"
                    BackgroundCssClass="modalBackground" PopupControlID="pnlProjectSectionPopup"
                    DropShadow="false" />
                <asp:Panel ID="pnlProjectSectionPopup" runat="server" BackColor="White" BorderColor="Black"
                    CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                    <table width="100%">
                        <tr>
                            <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                                valign="bottom">
                                <b style="font-size: 14px; padding-top: 2px;">Add Project</b>
                                <asp:Button ID="btnCloseProjectSection" runat="server" CssClass="mini-report-close"
                                    ToolTip="Cancel" OnClientClick="$find('mpeProjectSectionPopup').hide(); return false;"
                                    Style="float: right;" Text="X"></asp:Button>
                            </th>
                        </tr>
                        <tr>
                            <td style="padding: 10px;" colspan="2">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Account :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlAccountProjectSection" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Project :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlProjectProjectSection" onchange="ddlChild_onchange(this);"
                                                Width="250px" runat="server">
                                            </asp:DropDownList>
                                            <AjaxControlToolkit:CascadingDropDown ID="cddClientProjects" runat="server" ParentControlID="ddlAccountProjectSection"
                                                TargetControlID="ddlProjectProjectSection" Category="Group" LoadingText="Loading Projects..."
                                                EmptyText="No Projects found" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                                                PromptText="Please Select a Project" PromptValue="-1" ServicePath="~/CompanyPerfomanceServ.asmx"
                                                ServiceMethod="GetProjectsList" UseContextKey="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" style="padding: 6px 6px 15px 6px;">
                                <table>
                                    <tr>
                                        <td style="padding-right: 3px;">
                                            <asp:Button ID="btnAddProjectSection" OnClick="btnAddProjectSection_OnClick" runat="server"
                                                Enabled="false" Text="ADD" ToolTip="ADD" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelProjectSection" runat="server" Text="Cancel" ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <AjaxControlToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="btnAddAccount"
                    CancelControlID="btnCancelBusinessDevelopmentSection" BehaviorID="mpeBusinessDevelopmentSectionPopup"
                    BackgroundCssClass="modalBackground" PopupControlID="pnlBusinessDevelopmentSectionPopup"
                    DropShadow="false" />
                <asp:Panel ID="pnlBusinessDevelopmentSectionPopup" runat="server" BackColor="White"
                    BorderColor="Black" CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                    <table width="100%">
                        <tr>
                            <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                                valign="bottom">
                                <b style="font-size: 14px; padding-top: 2px;">Add Account</b>
                                <asp:Button ID="btnCloseBusinessDevelopmentSection" runat="server" CssClass="mini-report-close"
                                    ToolTip="Cancel" OnClientClick="$find('mpeBusinessDevelopmentSectionPopup').hide(); return false;"
                                    Style="float: right;" Text="X"></asp:Button>
                            </th>
                        </tr>
                        <tr>
                            <td style="padding: 10px;" colspan="2">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Account :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlAccountBusinessDevlopmentSection" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Business Unit :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlBusinessUnitBusinessDevlopmentSection" onchange="ddlChild_onchange(this);"
                                                Width="250px" runat="server">
                                            </asp:DropDownList>
                                            <AjaxControlToolkit:CascadingDropDown ID="cddBusinessUnitBDSection" runat="server"
                                                ParentControlID="ddlAccountBusinessDevlopmentSection" TargetControlID="ddlBusinessUnitBusinessDevlopmentSection"
                                                Category="Group" LoadingText="Loading Projects..." EmptyText="No Projects found"
                                                ScriptPath="~/Scripts/CascadingDropDownBehavior.js" ServicePath="~/CompanyPerfomanceServ.asmx"
                                                PromptText="Please Select a Business Unit" PromptValue="-1" ServiceMethod="GetDdlProjectGroupContents"
                                                UseContextKey="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" style="padding: 6px 6px 15px 6px;">
                                <table>
                                    <tr>
                                        <td style="padding-right: 3px;">
                                            <asp:Button ID="btnAddBusinessDevelopmentSection" runat="server" Enabled="false"
                                                OnClick="btnAddBusinessDevelopmentSection_OnClick" Text="ADD" ToolTip="ADD" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelBusinessDevelopmentSection" runat="server" Text="Cancel"
                                                ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <AjaxControlToolkit:ModalPopupExtender ID="ModalPopupExtender2" runat="server" TargetControlID="btnAddInternalProject"
                    CancelControlID="btnCancelInternalProjectSection" BehaviorID="mpeInternalProjectSectionPopup"
                    BackgroundCssClass="modalBackground" PopupControlID="pnlInternalProjectSectionPopup"
                    DropShadow="false" />
                <asp:Panel ID="pnlInternalProjectSectionPopup" runat="server" BackColor="White" BorderColor="Black"
                    CssClass="ConfirmBoxClass" Style="display: none" BorderWidth="2px">
                    <table width="100%">
                        <tr>
                            <th align="center" style="text-align: center; background-color: Gray;" colspan="2"
                                valign="bottom">
                                <b style="font-size: 14px; padding-top: 2px;">Add Project</b>
                                <asp:Button ID="btnCloseInternalProjectSection" runat="server" CssClass="mini-report-close"
                                    ToolTip="Cancel" OnClientClick="$find('mpeInternalProjectSectionPopup').hide(); return false;"
                                    Style="float: right;" Text="X"></asp:Button>
                            </th>
                        </tr>
                        <tr>
                            <td style="padding: 10px;" colspan="2">
                                <table class="WholeWidth">
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Business Unit :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlBusinessUnitInternal" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%; font-weight: bold;">
                                            Project :
                                        </td>
                                        <td style="width: 80%;">
                                            <asp:DropDownList ID="ddlProjectInternal" onchange="ddlChild_onchange(this);" Width="250px"
                                                runat="server" />
                                            <AjaxControlToolkit:CascadingDropDown ID="cddProjectsInternal" runat="server" ParentControlID="ddlBusinessUnitInternal"
                                                TargetControlID="ddlProjectInternal" Category="Group" LoadingText="Loading Projects..."
                                                EmptyText="No Projects found" PromptText="Please Select a Project" PromptValue="-1"
                                                ScriptPath="~/Scripts/CascadingDropDownBehavior.js" ServicePath="~/CompanyPerfomanceServ.asmx"
                                                ServiceMethod="GetProjectsListByProjectGroupId" UseContextKey="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" style="padding: 6px 6px 15px 6px;">
                                <table>
                                    <tr>
                                        <td style="padding-right: 3px;">
                                            <asp:Button ID="btnAddInternalProjectSection" runat="server" Enabled="false" Text="ADD"
                                                OnClick="btnAddInternalProjectSection_OnClick" ToolTip="ADD" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelInternalProjectSection" runat="server" Text="Cancel" ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="pcPersons" />
                <asp:AsyncPostBackTrigger ControlID="wsChoose" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>

