<%@ Page Title="Time Entry By Person | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.aspx.cs" Inherits="PraticeManagement.Sandbox.TimeEntriesByPerson" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/TimeEntry/WeekSelector.ascx" TagName="WeekSelector"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="Content3" ContentPlaceHolderID="title" runat="server">
    <title>Time Entry By Person | Practice Management</title>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <script language="javascript" type="text/javascript">
        function saveReportExcel() {
            var hlnkExportToExcel = document.getElementById('<%= hlnkExportToExcel.ClientID %>');
            if (navigator.userAgent.indexOf(' Chrome/') > -1) {
                var evObj = document.createEvent('MouseEvents');
                evObj.initMouseEvent('click', true, true, window, 0, 0, 0, 0, 0, false, false, true, false, 0, null);
                hlnkExportToExcel.dispatchEvent(evObj)
            }
            else {
                hlnkExportToExcel.click()
            }
        }
        function saveReport() {
            var hdnGuid = document.getElementById('<%= hdnGuid.ClientID %>');
            var divPersonListSummary = $("div[id$='divPersonListSummary']");
            var hdnSaveReportText = document.getElementById('<%= hdnSaveReportText.ClientID %>');
            var html = ""; if (divPersonListSummary != null && divPersonListSummary.length > 0) {
                for (var i = 0; i < divPersonListSummary.length; i++) {
                    html += divPersonListSummary[i].innerHTML + hdnGuid.value
                }
            }
            hdnSaveReportText.value = html
        }
        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>");
            if (button != null) {
                button.disabled = false; hiddenField.value = "true"
            }
        } function CheckIsPostBackRequired(sender) {
            var defaultDate = (new Date(sender.defaultValue)).format('M/d/yyyy');
            if (sender.value == defaultDate) {
                return false
            } return true
        } Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function MakeAsynchronousCalls() {
            var hdnPersonIds = document.getElementById("<%= hdnPersonIds.ClientID%>");
            var hdnStartDate = document.getElementById("<%= hdnStartDate.ClientID%>");
            var hdnEndDate = document.getElementById("<%= hdnEndDate.ClientID%>");
            var hdnPayScaleIds = document.getElementById("<%= hdnPayScaleIds.ClientID%>");
            var hdnPracticeIds = document.getElementById("<%= hdnPracticeIds.ClientID%>");
            var displayPanel = $("#" + "<%= pnlList.ClientID%>"); var hmlData = "";
            if (hdnPersonIds.value != "") {
                var array = hdnPersonIds.value.split(',');
                var temp = 0; for (var i = 0; i < array.length; i++) {
                    if (array[i] != "" && array[i] != "undefined") {
                        if ($get("<%= LoadingProgress1.ClientID%>" + '_upTimeEntries').style.display == "none")
                        { $get("<%= LoadingProgress1.ClientID%>" + '_upTimeEntries').style.display = 'block' }
                        var urlVal = "../Controls/Reports/TimeEntriesGetByPersonHandler.ashx?PersonID=" + array[i].toString() + "&StartDate=" + hdnStartDate.value + "&EndDate=" + hdnEndDate.value + "&PayScaleIds=" + hdnPayScaleIds.value + "&PracticeIds=" + hdnPracticeIds.value;
                        $.post(urlVal, function (data) { $get("<%= LoadingProgress1.ClientID%>" + '_upTimeEntries').style.display = 'block'; temp++; displayPanel.append(data); if (temp == array.length - 1) { $get("<%= LoadingProgress1.ClientID%>" + '_upTimeEntries').style.display = 'none' } })
                    }
                }
            }
        }
        function endRequestHandle(sender, Args) {
            var hdnUpdateClicked = document.getElementById("<%= hdnUpdateClicked.ClientID%>");
            if (hdnUpdateClicked.value == "true") {
                MakeAsynchronousCalls();
            }
        }
    </script>
    <style type="text/css">
        .AlignRight
        {
            text-align: right;
        }
        .AlignCentre
        {
            text-align: center;
            padding-right: -1px;
        }
        
        .TextAlignCenter
        {
            text-align: center;
        }
        
        .NotVisible
        {
            display: none;
            visibility: hidden;
        }
        table.time-entry-person-projects th
        {
            border-bottom: 1px solid gray;
            border-top: 1px solid gray;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: middle;
            text-align: center;
            font-weight: 700;
        }
        table.time-entry-person-projects tr
        {
            border-bottom: 1px solid lightgray;
            border-top: 1px solid lightgray;
            vertical-align: middle;
        }
        table.time-entry-person-projects td
        {
            vertical-align: middle;
            border-bottom: 1px solid lightgray;
            border-top: 1px solid lightgray;
            padding-left: 5px;
            text-align: center;
            padding-right: 5px;
        }
        
        .divPersonName
        {
            padding-bottom: 5px;
            font-size: 20px;
            font-weight: bold;
        }
        
        .divTeTable
        {
            overflow-x: auto;
            overflow-y: display;
        }
        .TimeEntrySummary
        {
            font-style: italic;
            font-size: 16px;
            font-weight: bold;
        }
        .divProjects
        {
            overflow-x: auto;
            padding-top: 10px;
        }
        .TimeEntryDetail
        {
            font-style: italic;
            font-size: 16px;
            font-weight: bold;
        }
        .ClientAndProjectName
        {
            padding-top: 3px;
            font-size: 14px;
            font-weight: bold;
        }
        .HeaderDiv
        {
            text-align: center;
            font-weight: bold;
        }
        .divHrClass
        {
            padding-top: 10px;
            padding-bottom: 5px;
            padding-left: 5px;
            padding-right: 5px;
        }
        .hrClass
        {
            margin-left: 5px;
            margin-right: 5px;
            width: 100%;
            color: #888888;
        }
        .ClientProjectTimeType
        {
            text-align: left !important;
            width: 20%;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr valign="top">
                        <td align="left" style="width: 20px; padding-top: 3px;">
                            <ajaxToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/expand.jpg"
                                ToolTip="Expand Filters" />
                        </td>
                        <td style="width: 13%; padding-top: 3px; white-space: nowrap;" align="left">
                            &nbsp;&nbsp;Show Time Entered
                        </td>
                        <td style="width: 20%" align="left">
                            <uc:DateInterval ID="diRange" runat="server" FromToDateFieldWidth="70" IsFromDateRequired="true"
                                IsToDateRequired="true" />
                            <asp:ValidationSummary ID="valSum" runat="server" />
                        </td>
                        <td style="width: 3%; padding-top: 3px; white-space: nowrap;" align="left">
                            &nbsp;&nbsp;for&nbsp;&nbsp;
                        </td>
                        <td style="width: 28%; padding-top: 3px;" align="left">
                            <div style="margin-top: -2px;">
                                <cc2:ScrollingDropDown ID="cblPersons" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPersons','Person')" BackColor="White"
                                    CellPadding="3" NoItemsType="All" SetDirty="False" Width="350px" DropDownListType="Person"
                                    BorderWidth="0" />
                                <ext:ScrollableDropdownExtender ID="sdePersons" runat="server" TargetControlID="cblPersons"
                                    Width="250px" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </div>
                        </td>
                        <td style="padding-top: 3px;" align="right">
                            <div style="margin-top: -2px;">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" Width="100px" OnClick="btnUpdate_OnClick"
                                                EnableViewState="False" />
                                            <asp:HiddenField ID="hdnUpdateClicked" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" Width="100px"
                                                OnClick="btnResetFilter_OnClick" CausesValidation="false" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                        </td>
                        <td align="right">
                        </td>
                        <td style="padding-top: 5px;">
                            <input type="button" runat="server" id="btnExportToXL" value="Export To Excel" disabled="disabled"
                                enableviewstate="false" style="width: 100px" onclick="saveReportExcel();" title="Export To Excel" />
                            <asp:HyperLink ID="hlnkExportToExcel" runat="server" Style="display: none;" Text="Export To Excel"
                                ToolTip="Export To Excel"></asp:HyperLink>
                            <asp:Button ID="btnExportToPDF" runat="server" Text="Export To PDF" OnClientClick="saveReport();"
                                Enabled="false" Width="100px" OnClick="ExportToPDF" EnableViewState="False" /><asp:HiddenField
                                    ID="hdnSaveReportText" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <asp:Panel ID="pnlFilters" runat="server">
                    <ajaxToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0" CssClass="CustomTabStyle">
                        <ajaxToolkit:TabPanel runat="server" ID="tpFilters">
                            <HeaderTemplate>
                                <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span> </span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table>
                                    <tr align="center">
                                        <td style="width: 200px; border-bottom: 1px solid black;" valign="top" colspan="2">
                                            Person Status
                                        </td>
                                        <td style="width: 30px;">
                                        </td>
                                        <td style="width: 150px; border-bottom: 1px solid black;" valign="top">
                                            Pay Type
                                        </td>
                                        <td style="width: 30px;">
                                        </td>
                                        <td style="width: 250px; border-bottom: 1px solid black;">
                                            Practice Area
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chbActivePersons" runat="server" Text="Active" ToolTip="Include active persons into report"
                                                AutoPostBack="true" OnCheckedChanged="PersonStatus_OnCheckedChanged" Checked="True"
                                                onclick="EnableResetButton();" />
                                        </td>
                                        <td align="right">
                                            <asp:CheckBox ID="chbTerminatedPersons" runat="server" Text="Terminated" ToolTip="Include terminated persons into report"
                                                AutoPostBack="true" Checked="false" onclick="EnableResetButton();" OnCheckedChanged="PersonStatus_OnCheckedChanged" />
                                        </td>
                                        <td>
                                        </td>
                                        <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                            <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="Null"
                                                onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')" BackColor="White"
                                                CellPadding="3" NoItemsType="All" SetDirty="False" Width="200px" DropDownListType="Pay Type"
                                                Height="100px" BorderWidth="0" />
                                            <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="200px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td class="floatRight" style="padding-top: 5px; padding-left: 3px;">
                                            <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="Null"
                                                onclick="scrollingDropdown_onclick('cblPractices','Practice Area')" BackColor="White"
                                                CellPadding="3" Height="250px" NoItemsType="All" SetDirty="False" DropDownListType="Practice Area"
                                                Width="260px" BorderWidth="0" />
                                            <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                                UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chbInactivePersons" runat="server" Text="Inactive" ToolTip="Include projected persons into report"
                                                AutoPostBack="true" Checked="false" onclick="EnableResetButton();" OnCheckedChanged="PersonStatus_OnCheckedChanged" />
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </asp:Panel>
                <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="updReport" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlList" runat="server">
                <uc2:CalendarLegend ID="CalendarLegend" runat="server" />
            </asp:Panel>
            <asp:HiddenField ID="hdnGuid" runat="server" />
            <asp:HiddenField ID="hdnPersonIds" runat="server" />
            <asp:HiddenField ID="hdnStartDate" runat="server" />
            <asp:HiddenField ID="hdnEndDate" runat="server" />
            <asp:HiddenField ID="hdnPayScaleIds" runat="server" />
            <asp:HiddenField ID="hdnPracticeIds" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

