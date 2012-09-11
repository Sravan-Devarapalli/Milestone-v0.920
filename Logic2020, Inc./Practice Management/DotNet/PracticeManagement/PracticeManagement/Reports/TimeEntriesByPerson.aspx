<%@ Page Title="Time Entry By Person | Practice Management" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.aspx.cs" Inherits="PraticeManagement.Sandbox.TimeEntriesByPerson" %>

<%@ Import Namespace="DataTransferObjects" %>
<%@ Import Namespace="DataTransferObjects.TimeEntry" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/CalendarLegend.ascx" TagName="CalendarLegend" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="Content3" ContentPlaceHolderID="title" runat="server">
    <title>Time Entry By Person | Practice Management</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript">

        function Excel_Click() {
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
                    html += divPersonListSummary[i].innerHTML + hdnGuid.value;
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
        }
        function CheckIsPostBackRequired(sender) {
            var defaultDate = (new Date(sender.defaultValue)).format('M/d/yyyy');
            if (sender.value == defaultDate) {
                return false
            } return true
        }
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function removeByIndex(arr, index) {
            arr.splice(index, 1);
        }

        function getUrlVars(urlValue) {
            var vars = [], hash;
            var hashes = urlValue.slice(urlValue.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

        function MakeAsynchronousCalls() {
            var hdnPersonIds = document.getElementById("<%= hdnPersonIds.ClientID%>");
            var hdnStartDate = document.getElementById("<%= hdnStartDate.ClientID%>");
            var hdnEndDate = document.getElementById("<%= hdnEndDate.ClientID%>");
            var hdnPayScaleIds = document.getElementById("<%= hdnPayScaleIds.ClientID%>");
            var hdnPracticeIds = document.getElementById("<%= hdnPracticeIds.ClientID%>");
            var ddlView = document.getElementById("<%= ddlView.ClientID%>");
            var loadingProgress = $get("<%= LoadingProgress1.ClientID%>" + '_upTimeEntries');

            if (hdnPersonIds.value != "") {
                var array = hdnPersonIds.value.split(',');

                for (var i = 0; i < array.length; i++) {
                    if (array[i] == "undefined") {
                        removeByIndex(array, i);
                    }
                }

                var temp = 0; for (var i = 0; i < array.length; i++) {

                    if (loadingProgress.style.display == "none") {
                        loadingProgress.style.display = 'block'
                    }

                    var urlVal = "../Controls/Reports/TimeEntriesGetByPersonHandler.ashx?PersonID=" + array[i].toString() + "&StartDate=" + hdnStartDate.value + "&EndDate=" + hdnEndDate.value + "&PayScaleIds=" + hdnPayScaleIds.value + "&PracticeIds=" + hdnPracticeIds.value + "&view=" + ddlView.value;
                    $.post(urlVal,
                        function (data) {
                            var controlId = getUrlVars(this.url)["ControlId"];

                            loadingProgress.style.display = 'block';

                            document.getElementById(controlId).innerHTML = data;
                            temp++;
                            if (temp == array.length) {
                                loadingProgress.style.display = 'none'
                            }
                        }
                           );
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
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="buttons-block">
                <table class="WholeWidth">
                    <tr class="vTop Padding3PxTd">
                        <td class="Width20Px">
                            <AjaxControlToolkit:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="pnlFilters"
                                ImageControlID="btnExpandCollapseFilter" CollapsedImage="~/Images/expand.jpg"
                                ExpandedImage="~/Images/collapse.jpg" CollapseControlID="btnExpandCollapseFilter"
                                ExpandControlID="btnExpandCollapseFilter" Collapsed="True" TextLabelID="lblFilter" />
                            <asp:Label ID="lblFilter" runat="server"></asp:Label>&nbsp;
                            <asp:Image ID="btnExpandCollapseFilter" runat="server" ImageUrl="~/Images/expand.jpg"
                                ToolTip="Expand Filters" />
                        </td>
                        <td class="Width13Percent no-wrap">
                            &nbsp;&nbsp;Show Time Entered
                        </td>
                        <td class="Width20Percent Padding0PxTd">
                            <uc:DateInterval ID="diRange" runat="server" FromToDateFieldCssClass="Width70PxImp"
                                IsFromDateRequired="true" IsToDateRequired="true" />
                            <asp:ValidationSummary ID="valSum" runat="server" />
                        </td>
                        <td class="Width3Percent no-wrap">
                            &nbsp;&nbsp;for&nbsp;&nbsp;
                        </td>
                        <td class="Width260Px">
                            <div class="marTop-2px floatRight">
                                <pmc:ScrollingDropDown ID="cblPersons" runat="server" AllSelectedReturnType="AllItems"
                                    onclick="scrollingDropdown_onclick('cblPersons','Person')" NoItemsType="All"
                                    SetDirty="False" DropDownListType="Person" CssClass="TimeEntryByperson_cblPersons" />
                                <ext:ScrollableDropdownExtender ID="sdePersons" runat="server" TargetControlID="cblPersons"
                                    Width="250px" UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                </ext:ScrollableDropdownExtender>
                            </div>
                        </td>
                        <td class="Padding0PxTd">
                            in &nbsp;
                            <asp:DropDownList ID="ddlView" CssClass="Width125Px" runat="server">
                                <asp:ListItem Text="Project Level" Value="1" title="Project Level" Selected="True"></asp:ListItem>
                                <asp:ListItem onclick="EnableResetButton();" Text="Work Type Level" title="Work Type Level"
                                    Value="2"></asp:ListItem>
                            </asp:DropDownList>
                            &nbsp; view
                        </td>
                        <td align="right">
                            <div class="marTop-2px">
                                <table class="Padding0PxTd">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnUpdateView" runat="server" Text="Update View" CssClass="Width100PxImp"
                                                OnClick="btnUpdate_OnClick" EnableViewState="False" />
                                            <asp:HiddenField ID="hdnUpdateClicked" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnResetFilter" runat="server" Text="Reset Filter" CssClass="Width100PxImp"
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
                        <td>
                        </td>
                        <td class="PaddingTop5">
                            <input type="button" runat="server" id="btnExportToXL" value="Export To Excel" disabled="disabled"
                                enableviewstate="false" class="Width100PxImp" onclick="Excel_Click();" title="Export To Excel" />
                            <asp:HyperLink ID="hlnkExportToExcel" runat="server" Style="display: none;" Text="Export To Excel"
                                ToolTip="Export To Excel"></asp:HyperLink>
                            <asp:Button ID="btnExportToPDF" runat="server" Text="Export To PDF" OnClientClick="saveReport();"
                                Enabled="false" CssClass="Width100PxImp" OnClick="ExportToPDF" EnableViewState="False" /><asp:HiddenField
                                    ID="hdnSaveReportText" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <asp:Panel ID="pnlFilters" runat="server">
                    <AjaxControlToolkit:TabContainer ID="tcFilters" runat="server" ActiveTabIndex="0"
                        CssClass="CustomTabStyle">
                        <AjaxControlToolkit:TabPanel runat="server" ID="tpFilters">
                            <HeaderTemplate>
                                <span class="bg DefaultCursor"><span class="NoHyperlink">Filters</span> </span>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table>
                                    <tr align="center" class="BorderBottom1pxTd">
                                        <td class="border vTop Width200Px" colspan="2">
                                            Person Status
                                        </td>
                                        <td class="Width30Px">
                                        </td>
                                        <td class="border vTop Width150px">
                                            Pay Type
                                        </td>
                                        <td class="Width30Px">
                                        </td>
                                        <td class="border Width250Px">
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
                                        <td class="floatRight  practicesTd">
                                            <pmc:ScrollingDropDown ID="cblTimeScales" runat="server" CssClass="TimeEntryByperson_cblTimeScales"
                                                AllSelectedReturnType="Null" onclick="scrollingDropdown_onclick('cblTimeScales','Pay Type')"
                                                NoItemsType="All" SetDirty="False" DropDownListType="Pay Type" />
                                            <ext:ScrollableDropdownExtender ID="sdeTimeScales" runat="server" TargetControlID="cblTimeScales"
                                                UseAdvanceFeature="true" EditImageUrl="~/Images/Dropdown_Arrow.png" Width="200px">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                        <td>
                                        </td>
                                        <td class="floatRight  practicesTd">
                                            <pmc:ScrollingDropDown ID="cblPractices" runat="server" CssClass="TimeEntryByperson_cblPractices"
                                                AllSelectedReturnType="Null" onclick="scrollingDropdown_onclick('cblPractices','Practice Area')"
                                                NoItemsType="All" SetDirty="False" DropDownListType="Practice Area" />
                                            <ext:ScrollableDropdownExtender ID="sdePractices" runat="server" TargetControlID="cblPractices"
                                                UseAdvanceFeature="true" Width="250px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                            </ext:ScrollableDropdownExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chbTerminationPendingPersons" runat="server" Text="Termination Pending" ToolTip="Include termination pending persons into report"
                                                AutoPostBack="true" Checked="false" onclick="EnableResetButton();" OnCheckedChanged="PersonStatus_OnCheckedChanged" />
                                        </td>
                                        <td colspan="4">
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </AjaxControlToolkit:TabPanel>
                    </AjaxControlToolkit:TabContainer>
                </asp:Panel>
                <asp:HiddenField ID="hdnFiltersChanged" runat="server" Value="false" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="updReport" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlList" runat="server">
                <asp:Literal ID="ltrlContainer" runat="server">
                </asp:Literal>
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

