<%@ Page Language="C#" MasterPageFile="~/PracticeManagementMain.Master" Title="Time Entry | Practice Management"
    AutoEventWireup="true" CodeBehind="TimeEntry_New.aspx.cs" Inherits="PraticeManagement.TimeEntry_New" %>

<%@ Register TagPrefix="ext" Namespace="PraticeManagement.Controls.Generic.TotalCalculator"
    Assembly="PraticeManagement" %>
<%@ Register TagPrefix="ext2" Namespace="PraticeManagement.Controls.Generic.DuplicateOptionsRemove"
    Assembly="PraticeManagement" %>
<%@ Register Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.MaxValueAllowedForTextBox"
    TagPrefix="ext3" %>
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

        function pageLoad() {
            SetTooltipsForallDropDowns();
        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

        function SetFocus(modalExId, tbNotesId, tbBillableHoursId, btnSaveNotesId, tbNonBillableHoursId) {

            var tbActualHours = $find(tbBillableHours);

            var modalEx = $find(modalExId);
            modalEx.show();
            var tbNotes = $get(tbNotesId);
            var btnSaveNotes = $get(btnSaveNotesId);
            var tbBillableHours = $get(tbBillableHoursId);
            var tbNonBillableHours = $get(tbNonBillableHoursId);

            if (tbBillableHours.disabled && tbNonBillableHours.disabled) {
                tbNotes.disabled = 'disabled';
                btnSaveNotes.disabled = 'disabled';
            }
            else {
                if (!(tbBillableHours.getAttribute('IsPTO') != null && tbBillableHours.getAttribute('IsPTO').toString().toLowerCase() == "true")) {
                    tbNotes.disabled = '';
                    btnSaveNotes.disabled = '';
                }
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

        function assignHiddenValues(hiddenNoteId, noteId) {
            var hiddenNote = $get(hiddenNoteId);
            var note = $get(noteId);
            hiddenNote.value = note.value;
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
        function btnClose_OnClientClick() {
            $find("mpeTimetypeAlertMessage").hide();
            return false;
        }
        
    </script>
</asp:Content>
<asp:Content ID="cntHeader" ContentPlaceHolderID="header" runat="server">
    Time Entry
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="body" runat="server">
    <div class="time-entry-new-bg-light-frame">
        <pcg:StyledUpdatePanel ID="updNavigation" runat="server" CssClass="tem-person-timeNew"
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
    <div id="updateContainer" class="time-entry-new-grid">
        <asp:UpdatePanel ID="updTimeEntries" runat="server">
            <ContentTemplate>
                <script type="text/javascript">
                    function ExpandPanel(name) {
                        var isCollapsed = $find(name).get_Collapsed();
                        if (isCollapsed) {
                            $find(name).expandPanel();
                        }
                    }

                    function CollapsePanel(name, rowsCount) {
                        if (rowsCount == 1) {
                            var isCollapsed = $find(name).get_Collapsed();
                            if (!isCollapsed) {
                                $find(name).togglePanel();
                            }
                        }
                    }

                    function DeleteSection(name, rowsCount, controlClientId) {
                        var imgControl = document.getElementById(controlClientId);
                        var confirmMessageFormat = "This will remove the {0} / {1} and any associated time entries!  If this {0} / {1} has been selected as recurring, continuing will result in the {0} / {1} being removed for only this time entry period.  Do you want to continue?";

                        for (var i = 0; i < 3; i++) {
                            confirmMessageFormat = confirmMessageFormat.replace('{0}', imgControl.getAttribute('FormatValueOne'));
                            confirmMessageFormat = confirmMessageFormat.replace('{1}', imgControl.getAttribute('FormatValueTwo'));
                        }

                        if (confirm(confirmMessageFormat)) {
                            CollapsePanel(name, rowsCount);
                            return true;
                        }
                        else {
                            return false;
                        }

                    }

                    function SelectDefaultValues(cddID) {
                        var dd = $find(cddID);
                        var parentElement = $get(dd._parentControlID);
                        if (parentElement) {
                            parentElement.selectedIndex = 0;
                            ddlParent_onchange(parentElement);
                        }

                        dd.set_SelectedValue('');
                        dd._onParentChange(null, true);

                    }

                    function expandCollapseSections(SectionName, lblSection) {
                        var cpeSection = $find(SectionName);
                        var isCollapsed = cpeSection.get_Collapsed();
                        var cpeSectionCount = lblSection.getAttribute('rowsCount');
                        if (cpeSectionCount != '0') {
                            if (isCollapsed) {
                                cpeSection.togglePanel();
                            }
                        }
                        else {
                            if (!isCollapsed) {
                                cpeSection.togglePanel();
                            }
                        }
                    }

                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
                    function endRequestHandle(sender, Args) {
                        SetTooltipsForallDropDowns();
                        var hdIsWeekOrPersonChanged = document.getElementById('<%= hdIsWeekOrPersonChanged.ClientID %>');
                        if (hdIsWeekOrPersonChanged.value.toLowerCase() == 'true') {
                            var lbProjectSection = document.getElementById('<%=lbProjectSection.ClientID %>');
                            var lbBusinessDevelopmentSection = document.getElementById('<%=lbBusinessDevelopmentSection.ClientID %>');
                            var lbInternalSection = document.getElementById('<%=lbInternalSection.ClientID %>');
                            expandCollapseSections('cpeProjectSection', lbProjectSection);
                            expandCollapseSections('cpeBusinessDevelopmentSection', lbBusinessDevelopmentSection);
                            expandCollapseSections('cpeInternalSection', lbInternalSection);
                            hdIsWeekOrPersonChanged.value = 'false';
                        }
                    }

                </script>
                <uc:MessageLabel ID="mlErrors" runat="server" ErrorColor="Red" InfoColor="DarkGreen"
                    WarningColor="Orange" EnableViewState="false" />
                <asp:Panel ID="pnlShowTimeEntries" Visible="false" runat="server">
                    <div class="buttons-block">
                        <table cellpadding="0" cellspacing="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeProjectSection" runat="Server"
                                        CollapsedText="Expand Section" ExpandedText="Collapse Section" EnableViewState="false"
                                        TargetControlID="pnlProjectSection" ImageControlID="btnExpandCollapseFilter"
                                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                        ExpandControlID="btnExpandCollapseFilter" TextLabelID="lbProjectSection" BehaviorID="cpeProjectSection" />
                                    <asp:Label ID="lbProjectSection" Style="display: none;" runat="server"></asp:Label>
                                    <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/collapse.jpg"
                                        ToolTip="Expand Section" />&nbsp;<b>PROJECT</b>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="btnAddProject" runat="server" OnClientClick="SelectDefaultValues('cddClientProjects');"
                                        Text="Add Project" CssClass="mrg0" ToolTip="Add Project" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlProjectSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repProjectSections" runat="server" OnItemDataBound="repProjectSections_OnItemDataBound">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="Section WholeWidth">
                                    <tr>
                                        <td class="SectionFirstTD" colspan="5">
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.AccountNameXname)).Value + " - " + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.ProjectNumberXname)).Value + " - " + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.ProjectNameXname)).Value%>
                                        </td>
                                        <td class="SectionSecondTD" colspan="3">
                                        </td>
                                        <td class="DeleteWidth">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecursiveProjectSection" runat="server"
                                                TargetControlID="imgBtnRecursiveProjectSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecursiveProjectSection" runat="server" ImageUrl="~/Images/Recursive.png"
                                                OnClick="imgBtnRecursiveSection_OnClick" />
                                        </td>
                                        <td class="DeleteWidth">
                                            <asp:ImageButton ID="imgBtnDeleteProjectSection" runat="server" ImageUrl="~/Images/close_24.png"
                                                OnClick="imgBtnDeleteSection_OnClick" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repProjectTes" runat="server" OnItemDataBound="repProjectTes_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="DeleteWidth">
                                                    <div class="ie-bg">
                                                    </div>
                                                </td>
                                                <td class="time-entry-bar-time-typesNewHeader">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repProjectTesHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-teNew">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hoursNew">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td class="DeleteWidth">
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:BillableAndNonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <table cellpadding="0" cellspacing="0" class="PaddingClass WholeWidth">
                                            <tr>
                                                <td class="DeleteWidth">
                                                    <asp:ImageButton ID="imgPlusProjectSection" OnClick="imgPlusProjectSection_OnClick"
                                                        runat="server" ImageUrl="~/Images/add_24.png" />
                                                </td>
                                                <td colspan="10">
                                                </td>
                                            </tr>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" cellspacing="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeBusinessDevelopmentSection" runat="Server"
                                        CollapsedText="Expand Section" ExpandedText="Collapse Section" EnableViewState="false"
                                        BehaviorID="cpeBusinessDevelopmentSection" TargetControlID="pnlBusinessDevelopmentSection"
                                        ImageControlID="Image1" CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg"
                                        CollapseControlID="Image1" ExpandControlID="Image1" TextLabelID="Label1" />
                                    <asp:Label ID="lbBusinessDevelopmentSection" Style="display: none;" runat="server"></asp:Label>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Section" />&nbsp;<b>BUSINESS
                                        DEVELOPMENT</b>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="btnAddAccount" runat="server" OnClientClick="SelectDefaultValues('cddBusinessUnitBDSection');"
                                        Text="Add Account" CssClass="mrg0" ToolTip="Add Account" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlBusinessDevelopmentSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repBusinessDevelopmentSections" OnItemDataBound="repBusinessDevelopmentSections_OnItemDataBound"
                            runat="server">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="Section WholeWidth">
                                    <tr>
                                        <td class="SectionFirstTD" colspan="5">
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.AccountNameXname)).Value + " - " + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.BusinessUnitNameXname)).Value %>
                                        </td>
                                        <td class="SectionSecondTD" colspan="3">
                                        </td>
                                        <td class="DeleteWidth">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecurrenceBusinessDevelopmentSection"
                                                runat="server" TargetControlID="imgBtnRecurrenceBusinessDevelopmentSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecurrenceBusinessDevelopmentSection" runat="server" OnClick="imgBtnRecursiveSection_OnClick" />
                                        </td>
                                        <td class="DeleteWidth">
                                            <asp:ImageButton ID="imgBtnDeleteBusinessDevelopmentSection" runat="server" ImageUrl="~/Images/close_24.png"
                                                OnClick="imgBtnDeleteSection_OnClick" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repBusinessDevelopmentTes" OnItemDataBound="repBusinessDevelopmentTes_ItemDataBound"
                                    runat="server">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="DeleteWidth">
                                                    <div class="ie-bg">
                                                    </div>
                                                </td>
                                                <td class="time-entry-bar-time-typesNewHeader">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repBusinessDevelopmentHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-teNew">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hoursNew">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td class="DeleteWidth">
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:NonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <table cellpadding="0" cellspacing="0" class="PaddingClass WholeWidth">
                                            <tr>
                                                <td class="DeleteWidth">
                                                    <asp:ImageButton ID="imgPlusBusinessDevelopmentSection" OnClick="imgPlusBusinessDevelopmentSection_OnClick"
                                                        runat="server" ImageUrl="~/Images/add_24.png" />
                                                </td>
                                                <td colspan="10">
                                                </td>
                                            </tr>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" cellspacing="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <AjaxControlToolkit:CollapsiblePanelExtender ID="cpeInternalSection" runat="Server"
                                        BehaviorID="cpeInternalSection" CollapsedText="Expand Section" ExpandedText="Collapse Section"
                                        EnableViewState="false" TargetControlID="pnlInternalSection" ImageControlID="Image2"
                                        CollapsedImage="Images/expand.jpg" ExpandedImage="Images/collapse.jpg" CollapseControlID="Image2"
                                        ExpandControlID="Image2" TextLabelID="lbInternalSection" />
                                    <asp:Label ID="lbInternalSection" Style="display: none;" runat="server"></asp:Label>
                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/collapse.jpg" ToolTip="Expand Section" />&nbsp;<b>INTERNAL</b>
                                </td>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="btnAddInternalProject" runat="server" OnClientClick="SelectDefaultValues('cddProjectsInternal');"
                                        Text="Add Project" CssClass="mrg0" ToolTip="Add Project" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlInternalSection" runat="server" CssClass="cp bg-white">
                        <asp:Repeater ID="repInternalSections" OnItemDataBound="repInternalSections_ItemDataBound"
                            runat="server">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="Section WholeWidth">
                                    <tr>
                                        <td class="SectionFirstTD" colspan="5">
                                            <%#((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.BusinessUnitNameXname)).Value + " - " + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.ProjectNumberXname)).Value + " - " + ((System.Xml.Linq.XElement)Container.DataItem).Attribute(System.Xml.Linq.XName.Get(PraticeManagement.TimeEntry_New.ProjectNameXname)).Value%>
                                        </td>
                                        <td class="SectionSecondTD" colspan="3">
                                        </td>
                                        <td class="DeleteWidth">
                                            <AjaxControlToolkit:ConfirmButtonExtender ID="cbeImgBtnRecurrenceInternalSection"
                                                runat="server" TargetControlID="imgBtnRecurrenceInternalSection">
                                            </AjaxControlToolkit:ConfirmButtonExtender>
                                            <asp:ImageButton ID="imgBtnRecurrenceInternalSection" runat="server" OnClick="imgBtnRecursiveSection_OnClick" />
                                        </td>
                                        <td class="DeleteWidth">
                                            <asp:ImageButton ID="imgBtnDeleteInternalSection" runat="server" ImageUrl="~/Images/close_24.png"
                                                OnClick="imgBtnDeleteSection_OnClick" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repInternalTes" OnItemDataBound="repInternalTes_ItemDataBound"
                                    runat="server">
                                    <HeaderTemplate>
                                        <table class="CompPerfTable WholeWidth">
                                            <tr class="CompPerfHeader WholeWidth">
                                                <td class="DeleteWidth">
                                                    <div class="ie-bg">
                                                    </div>
                                                </td>
                                                <td class="time-entry-bar-time-typesNewHeader">
                                                    <div class="ie-bg">
                                                        Work Type</div>
                                                </td>
                                                <asp:Repeater ID="repInternalTesHeader" runat="server">
                                                    <ItemTemplate>
                                                        <td class="time-entry-bar-single-teNew">
                                                            <div class="ie-bg">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td class="time-entry-bar-total-hoursNew">
                                                    <div class="ie-bg">
                                                        TOTAL</div>
                                                </td>
                                                <td class="DeleteWidth">
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <uc:NonBillableTimeEntryBar runat="server" ID="bar" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <table cellpadding="0" cellspacing="0" class="PaddingClass WholeWidth">
                                            <tr>
                                                <td class="DeleteWidth">
                                                    <asp:ImageButton ID="imgPlusInternalSection" OnClick="imgPlusInternalSection_OnClick"
                                                        runat="server" ImageUrl="~/Images/add_24.png" />
                                                </td>
                                                <td colspan="10">
                                                </td>
                                            </tr>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <ext2:DupilcateOptionsRemoveExtender ID="extDupilcateOptionsRemoveExtender" runat="server"
                                    TargetControlID="lblDupilcateOptionsRemoveExtender" />
                                <label id="lblDupilcateOptionsRemoveExtender" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" cellspacing="0" class="WholeWidth">
                            <tr>
                                <td>
                                    <img src="Images/collapse.jpg" style="visibility: hidden;" />
                                    &nbsp;<b>ADMINISTRATIVE</b>
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
                                        <td class="DeleteWidth">
                                            <div class="ie-bg">
                                            </div>
                                        </td>
                                        <td class="time-entry-bar-time-typesNewHeader">
                                            <div class="ie-bg">
                                                Work Type</div>
                                        </td>
                                        <asp:Repeater ID="repAdministrativeTesHeader" runat="server">
                                            <ItemTemplate>
                                                <td class="time-entry-bar-single-teNew">
                                                    <div class="ie-bg">
                                                        <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd MMM d}")%></div>
                                                </td>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <td class="time-entry-bar-total-hoursNew">
                                            <div class="ie-bg">
                                                TOTAL</div>
                                        </td>
                                        <td class="DeleteWidth">
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <uc:AdministrativeTimeEntryBar runat="server" ID="bar" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <asp:Panel ID="pnlTotalSection" runat="server" Style="padding: 10px 0px 10px 0px"
                        CssClass="cp bg-white">
                        <table class="CompPerfTable WholeWidth">
                            <tr class="time-entry-bar">
                                <td class="DeleteWidth">
                                </td>
                                <td class="time-entry-bar-time-typesNew TOTALHOURSTD">
                                    TOTAL HOURS:
                                </td>
                                <asp:Repeater ID="repDayTotalHours" OnItemDataBound="repDayTotalHours_OnItemDataBound"
                                    runat="server">
                                    <ItemTemplate>
                                        <td class="time-entry-bar-single-teNew DayTotalHours">
                                            <asp:Label ID="lblDayTotal" CssClass="alignRight" Font-Bold="true" TotalHours=""
                                                runat="server"></asp:Label>
                                            <ext:TotalCalculatorExtender ID="extDayTotal" runat="server" TargetControlID="lblDayTotal" />
                                            <asp:HiddenField ID="hdnDayTotal" runat="server"></asp:HiddenField>
                                            <ext3:MaxValueAllowedForTextBoxExtender ID="extMaxValueAllowedForTextBoxExtender"
                                                runat="server" TargetControlID="lblDayTotal">
                                            </ext3:MaxValueAllowedForTextBoxExtender>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <td class="time-entry-total-hoursNew">
                                </td>
                                <td class="DeleteWidth">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="DeleteWidth ">
                                </td>
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" class="TOTALTD PaddingTop6">
                                    BILLABLE TOTAL :
                                </td>
                                <td class="time-entry-total-hoursNew PaddingLeft4">
                                    <label id="lblBillableGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extBillableGrandTotal" runat="server" TargetControlID="lblBillableGrandTotal" />
                                </td>
                                <td class="DeleteWidth">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="DeleteWidth">
                                </td>
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" class="TOTALTD">
                                    NON BILLABLE TOTAL :
                                </td>
                                <td class="time-entry-total-hoursNew PaddingLeft4">
                                    <label id="lblNonBillableGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extNonBillableGrandTotal" runat="server" TargetControlID="lblNonBillableGrandTotal" />
                                </td>
                                <td class="DeleteWidth">
                                </td>
                            </tr>
                            <tr class="time-entry-bar">
                                <td class="DeleteWidth">
                                </td>
                                <td class="time-entry-bar-time-typesNew">
                                </td>
                                <td colspan="7" class="TOTALTD" style="padding-top: 15px;">
                                    TIME PERIOD GRAND TOTAL:
                                </td>
                                <td style="padding-top: 15px;" class="time-entry-total-hoursNew PaddingLeft4">
                                    <label id="lbltimePeriodGrandTotal" runat="server" />
                                    <ext:TotalCalculatorExtender ID="extTotalHours" runat="server" TargetControlID="lbltimePeriodGrandTotal" />
                                </td>
                                <td class="DeleteWidth">
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <div class="buttons-block">
                        <table cellpadding="0" cellspacing="0" class="WholeWidth">
                            <tr>
                                <td colspan="2">
                                    <asp:CustomValidator ID="custWorkType" runat="server" ErrorMessage="Please select Work Type."
                                        OnServerValidate="custWorkType_ServerValidate" EnableClientScript="false" ToolTip="Please select Work Type."
                                        SetFocusOnError="true" Text="*" ValidationGroup="TE" Display="None" />
                                    <asp:CustomValidator ID="custActualHours" runat="server" ToolTip="Hours should be real and 0.01-24.00. Invalid entries are highlighted in red."
                                        ErrorMessage="Hours should be real and 0.01-24.00. Invalid entries are highlighted in red."
                                        SetFocusOnError="true" OnServerValidate="custActualHours_ServerValidate" EnableClientScript="false"
                                        Display="None" Text="*" ValidationGroup="TE" />
                                    <asp:CustomValidator ID="cvPTOHours" runat="server" ToolTip="PTO Hours should be real and less than 8. Invalid entries are highlighted in red."
                                        ErrorMessage="PTO Hours should be real and less than 8. Invalid entries are highlighted in red."
                                        SetFocusOnError="true" OnServerValidate="custPTOHours_ServerValidate" EnableClientScript="false"
                                        Display="None" Text="*" ValidationGroup="TE" />
                                    <asp:CustomValidator ID="custNote" runat="server" ErrorMessage="Note should be 3-1000 characters long. Invalid entries are highlighted in red."
                                        OnServerValidate="custNote_ServerValidate" EnableClientScript="false" Text="*"
                                        Display="None" SetFocusOnError="true" ValidationGroup="TE" ToolTip="Note should be 3-1000 characters long. Invalid entries are highlighted in red." />
                                    <asp:CustomValidator ID="cvDayTotal" runat="server" ErrorMessage="Day Total hours must be lessthan or equals to 24."
                                        OnServerValidate="cvDayTotal_ServerValidate" EnableClientScript="false" Text="*"
                                        Display="None" SetFocusOnError="true" ValidationGroup="TE" ToolTip="Day Total hours must be lessthan or equals to 24." />
                                    <uc:MessageLabel ID="mlConfirmation" runat="server" ErrorColor="Red" InfoColor="Green"
                                        WarningColor="Orange" />
                                    <asp:ValidationSummary ID="valSumSaveTimeEntries" runat="server" ValidationGroup="TE" />
                                </td>
                                <td>
                                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_OnClick" Text="Save All"
                                        CssClass="mrg0" ToolTip="Save All" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
                <uc2:CalendarLegend ID="CalendarLegend" runat="server" disableChevron="true" />
                <AjaxControlToolkit:ModalPopupExtender ID="mpeProjectSectionPopup" runat="server"
                    TargetControlID="btnAddProject" CancelControlID="btnCancelProjectSection" BehaviorID="mpeProjectSectionPopup"
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
                                        <td class="ModalPopUpSideHeading">
                                            Account :
                                        </td>
                                        <td class="Width80Percent">
                                            <asp:DropDownList ID="ddlAccountProjectSection" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ModalPopUpSideHeading ModalPopUpDropDownPadding">
                                            Project :
                                        </td>
                                        <td class="Width80Percent ModalPopUpDropDownPadding">
                                            <asp:DropDownList ID="ddlProjectProjectSection" onchange="ddlChild_onchange(this);"
                                                Width="250px" runat="server">
                                            </asp:DropDownList>
                                            <AjaxControlToolkit:CascadingDropDown ID="cddClientProjects" runat="server" ParentControlID="ddlAccountProjectSection"
                                                TargetControlID="ddlProjectProjectSection" Category="Group" LoadingText="Loading Projects..."
                                                EmptyText="No Projects found" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                                                BehaviorID="cddClientProjects" PromptText="-- Select Project --" PromptValue="-1"
                                                ServicePath="~/CompanyPerfomanceServ.asmx" ServiceMethod="GetProjectsList" UseContextKey="true" />
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
                                                Enabled="false" Text="Add" ToolTip="Add" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelProjectSection" runat="server" Text="Cancel" ToolTip="Cancel"
                                                OnClientClick="$find('mpeProjectSectionPopup').hide(); return false;" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <AjaxControlToolkit:ModalPopupExtender ID="mpeBusinessDevelopmentSectionPopup" runat="server"
                    TargetControlID="btnAddAccount" CancelControlID="btnCancelBusinessDevelopmentSection"
                    BehaviorID="mpeBusinessDevelopmentSectionPopup" BackgroundCssClass="modalBackground"
                    PopupControlID="pnlBusinessDevelopmentSectionPopup" DropShadow="false" />
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
                                        <td class="ModalPopUpSideHeading">
                                            Account :
                                        </td>
                                        <td class="Width80Percent">
                                            <asp:DropDownList ID="ddlAccountBusinessDevlopmentSection" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ModalPopUpSideHeading ModalPopUpDropDownPadding">
                                            Business Unit :
                                        </td>
                                        <td class="Width80Percent ModalPopUpDropDownPadding">
                                            <asp:DropDownList ID="ddlBusinessUnitBusinessDevlopmentSection" onchange="ddlChild_onchange(this);"
                                                Width="250px" runat="server">
                                            </asp:DropDownList>
                                            <AjaxControlToolkit:CascadingDropDown ID="cddBusinessUnitBDSection" runat="server"
                                                BehaviorID="cddBusinessUnitBDSection" ParentControlID="ddlAccountBusinessDevlopmentSection"
                                                TargetControlID="ddlBusinessUnitBusinessDevlopmentSection" Category="Group" LoadingText="Loading BusinessUnits..."
                                                EmptyText="No Business Units found" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                                                ServicePath="~/CompanyPerfomanceServ.asmx" PromptText="-- Select Business Unit --"
                                                PromptValue="-1" ServiceMethod="GetDdlProjectGroupContents" UseContextKey="true" />
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
                                                OnClick="btnAddBusinessDevelopmentSection_OnClick" Text="Add" ToolTip="Add" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelBusinessDevelopmentSection" runat="server" Text="Cancel"
                                                OnClientClick="$find('mpeBusinessDevelopmentSectionPopup').hide(); return false;"
                                                ToolTip="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <AjaxControlToolkit:ModalPopupExtender ID="mpeInternalProjectSectionPopup" runat="server"
                    TargetControlID="btnAddInternalProject" CancelControlID="btnCancelInternalProjectSection"
                    BehaviorID="mpeInternalProjectSectionPopup" BackgroundCssClass="modalBackground"
                    PopupControlID="pnlInternalProjectSectionPopup" DropShadow="false" />
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
                                        <td class="ModalPopUpSideHeading">
                                            Business Unit :
                                        </td>
                                        <td class="Width80Percent">
                                            <asp:DropDownList ID="ddlBusinessUnitInternal" onchange="ddlParent_onchange(this);"
                                                Width="250px" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="ModalPopUpSideHeading ModalPopUpDropDownPadding">
                                            Project :
                                        </td>
                                        <td class="Width80Percent ModalPopUpDropDownPadding">
                                            <asp:DropDownList ID="ddlProjectInternal" onchange="ddlChild_onchange(this);" Width="250px"
                                                runat="server" />
                                            <AjaxControlToolkit:CascadingDropDown ID="cddProjectsInternal" runat="server" ParentControlID="ddlBusinessUnitInternal"
                                                TargetControlID="ddlProjectInternal" Category="Group" LoadingText="Loading Projects..."
                                                EmptyText="No Projects found" PromptText="-- Select Project --" PromptValue="-1"
                                                BehaviorID="cddProjectsInternal" ScriptPath="~/Scripts/CascadingDropDownBehavior.js"
                                                ServicePath="~/CompanyPerfomanceServ.asmx" ServiceMethod="GetProjectsListByProjectGroupId"
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
                                            <asp:Button ID="btnAddInternalProjectSection" runat="server" Enabled="false" Text="Add"
                                                OnClick="btnAddInternalProjectSection_OnClick" ToolTip="Add" />
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <asp:Button ID="btnCancelInternalProjectSection" runat="server" Text="Cancel" ToolTip="Cancel"
                                                OnClientClick="$find('mpeInternalProjectSectionPopup').hide(); return false;" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:HiddenField ID="hdTimetypeAlertMessage" runat="server" />
                <asp:HiddenField ID="hdIsWeekOrPersonChanged" runat="server" />
                <AjaxControlToolkit:ModalPopupExtender ID="mpeTimetypeAlertMessage" runat="server"
                    BehaviorID="mpeTimetypeAlertMessage" TargetControlID="hdTimetypeAlertMessage"
                    BackgroundCssClass="modalBackground" PopupControlID="pnlTimetypeAlertMessage"
                    DropShadow="false" CancelControlID="btnClose" />
                <asp:Panel ID="pnlTimetypeAlertMessage" runat="server" BackColor="White" BorderColor="Black"
                    Style="display: none" BorderWidth="2px" Width="380px">
                    <table width="100%" style="padding: 5px;">
                        <tr>
                            <th align="center" style="text-align: center; background-color: Gray;" valign="bottom">
                                <b style="font-size: 14px; padding-top: 2px;">Attention!</b>
                                <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                                    Style="float: right;" OnClientClick="return btnClose_OnClientClick();" Text="X">
                                </asp:Button>
                            </th>
                        </tr>
                        <tr>
                            <td style="font-weight: bold; padding: 8px;">
                                There is a time entry for a date on which the selected ChargeCode is turned off.Please
                                reassign the time entry to other ChargeCode or delete the time entry before changing.
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center; padding: 8px;">
                                <asp:Button ID="btnOk" runat="server" Text="OK" OnClientClick="return btnClose_OnClientClick();" />
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

