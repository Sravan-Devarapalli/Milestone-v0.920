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
<%@ Register Src="~/Controls/Persons/PersonChooser.ascx" TagName="PersonChooser"
    TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<asp:Content ID="Content3" ContentPlaceHolderID="title" runat="server">
    <title>Time Entry By Person | Practice Management</title>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="../Scripts/jquery-1.4.1.js"></script>
    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <script language="javascript" type="text/javascript">

        $(window).resize(function () {

            SetDivWidth();
        });

        function saveReportExcel() {
            var divPersonListSummary = $("div[id$='divPersonListSummary']");
            var hdnSaveReportExcel = document.getElementById('<%= hdnSaveReportExcel.ClientID %>');
            var html = "";
            if (divPersonListSummary != null && divPersonListSummary.length > 0) {
                for (var i = 0; i < divPersonListSummary.length; i++) {
                    html += divPersonListSummary[i].innerHTML;
                }
            }

            hdnSaveReportExcel.value = html;
        }

        function saveReport() {
            var hdnGuid = document.getElementById('<%= hdnGuid.ClientID %>');
            var divPersonListSummary = $("div[id$='divPersonListSummary']");
            var hdnSaveReportText = document.getElementById('<%= hdnSaveReportText.ClientID %>');
            var html = "";
            if (divPersonListSummary != null && divPersonListSummary.length > 0) {
                for (var i = 0; i < divPersonListSummary.length; i++) {
                    html += divPersonListSummary[i].innerHTML + hdnGuid.value;
                }
            }

            hdnSaveReportText.value = html;
        }

        function SetDivWidth() {
            var datalist = $get("<%= dlPersons.ClientID %>");
            if (datalist != null) {
                var tr = datalist.children[0].children;
                for (var index = 0; index < tr.length; index = index + 1) {
                    var trchildOne = $("div[id$='divTeTable']");
                    var trchildTwo = $("div[id$='divProjects']");
                    var trchildThree = $("div[id$='divPersonNotEntered']");

                    $(trchildOne[0].children[1]).css("width", $(window).width() - 100);
                    $(trchildTwo[0]).css("width", $(window).width() - 100);
                    $(trchildThree[0]).css("width", $(window).width() - 100);
                }
            }
        }

        function EnableResetButton() {
            var button = document.getElementById("<%= btnResetFilter.ClientID%>");
            var hiddenField = document.getElementById("<%= hdnFiltersChanged.ClientID%>")
            if (button != null) {
                button.disabled = false;
                hiddenField.value = "true";
            }
        }

        function CheckIsPostBackRequired(sender) {
            var defaultDate = (new Date(sender.defaultValue)).format('M/d/yyyy');
            if (sender.value == defaultDate) {
                return false;
            }
            return true;
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);


        function endRequestHandle(sender, Args) {
            SetDivWidth();
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
        
        .NotVisible
        {
            display: none;
            visibility: hidden;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Export To Excel" Width="100px"
                                Enabled="false" OnClientClick="saveReportExcel();" OnClick="btnExport_OnClick"
                                EnableViewState="False" /><asp:HiddenField ID="hdnSaveReportExcel" runat="server" />
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
                                            <cc2:ScrollingDropDown ID="cblTimeScales" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
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
                                            <cc2:ScrollingDropDown ID="cblPractices" runat="server" BorderColor="#aaaaaa" AllSelectedReturnType="AllItems"
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
    <asp:UpdatePanel ID="updReport" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlList" runat="server">
                <uc2:CalendarLegend ID="CalendarLegend" runat="server" />
                <asp:DataList ID="dlPersons" runat="server" DataSourceID="odsPersonTimeEntries" CssClass="WholeWidth"
                    OnItemDataBound="dlPersons_OnItemDataBound" OnItemCreated="dlPersons_OnItemCreated ">
                    <ItemTemplate>
                        <div id="divPersonListSummary" runat="server">
                            <table>
                                <tr>
                                    <td colspan="4">
                                        <div runat="server" id="divPersonName" style="padding-bottom: 5px;">
                                            <font style="font-size: 20px; font-weight: bold;">
                                                <%# Eval("Key.PersonName") %></font>
                                        </div>
                                        <br class="NotVisible" />
                                    </td>
                                </tr>
                            </table>
                            <div class="PersonGridLeftPadding" runat="server" id="divTeTable">
                                <table>
                                    <tr>
                                        <td colspan="4">
                                            <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Summary</font>
                                        </td>
                                    </tr>
                                </table>
                                <div style="overflow-x: auto; overflow-y: display;">
                                    <asp:Repeater ID="repTeTable" runat="server" DataSource='<%# Eval("Value")%>' OnItemCreated="repTeTable_OnItemCreated">
                                        <HeaderTemplate>
                                            <table class="time-entry-person-projects WholeWidth" border="1" rules="rows" style="display: inline;">
                                                <thead>
                                                    <tr style="border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;vertical-align:middle; ">
                                                        <th valign="middle" colspan="2" style="text-align: left; border-bottom: 1px solid gray; border-top: 1px solid gray;
                                                            width: 20%;">
                                                            <asp:Label ID="lblPType" runat="server" Font-Bold="true" Text="Client-Project-Time Type"></asp:Label>
                                                        </th>
                                                        <asp:Repeater ID="dlProject" runat="server" DataSourceID="odsCalendar" OnItemCreated="dlProject_OnItemCreated"
                                                            OnInit="dlProject_OnInit">
                                                            <ItemTemplate>
                                                                <th valign="middle" style="padding-left: 5px; padding-right: 5px;vertical-align:middle;text-align:center;  border-bottom: 1px solid gray;
                                                                    font-weight: bold; border-top: 1px solid gray;" class="<%# PraticeManagement.Utils.Calendar.GetCssClassByCalendarItem((CalendarItem) Container.DataItem) %>">
                                                                    <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd<br/>MMM d}")%>
                                                                </th>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <th valign="middle" style="padding-left: 5px; padding-right: 5px;vertical-align:middle;text-align:center;  border-bottom: 1px solid gray;
                                                            font-weight: bold; border-top: 1px solid gray;">
                                                            Totals
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr style="border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;"
                                                class="<%# Container.ItemIndex % 2 == 0 ? "alterrow" : string.Empty %>">
                                                <td valign="middle" colspan="2" style="text-align: left;vertical-align:middle; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;
                                                    width: 20%;">
                                                    <asp:Label ID="lblPTypeValue" runat="server" Text='<%#  DataBinder.Eval(Container.DataItem, "Key")%>'></asp:Label>
                                                </td>
                                                <asp:Repeater ID="dlProject" runat="server" DataSource='<%# GetUpdatedDatasource(DataBinder.Eval(Container.DataItem, "Value")) %>'
                                                    OnItemDataBound="dlProject_OnItemDataBound">
                                                    <ItemTemplate>
                                                        <td valign="middle" style="padding-left: 5px; text-align: center;vertical-align:middle; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                            border-top: 1px solid lightgray;">
                                                            <%#  ((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")) != null ? string.Format("{0:F2}",((TimeEntryRecord)DataBinder.Eval(Container.DataItem, "Value")).ActualHours) : string.Empty%>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td valign="middle" style="font-weight: bold; padding-left: 5px; text-align: center;vertical-align:middle; padding-right: 5px;
                                                    border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;">
                                                    <%# ProjectTotals.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <tr style="font-weight: bold; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;">
                                                <td colspan="2" style="text-align: left; border-bottom: 1px solid lightgray; border-top: 1px solid lightgray;
                                                    width: 20%;">
                                                    Totals
                                                </td>
                                                <asp:Repeater ID="dlTotals" runat="server" OnItemDataBound="dlTotals_OnItemDataBound"
                                                    OnInit="dlTotals_OnInit">
                                                    <ItemTemplate>
                                                        <td style="padding-left: 5px; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                            text-align: center; border-top: 1px solid lightgray;">
                                                            <%# ((double?)DataBinder.Eval(Container.DataItem, "Value"))!=null ?string.Format("{0:F2}",((double?)DataBinder.Eval(Container.DataItem, "Value")).Value) : string.Empty %>
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <td style="font-size: 15px; padding-left: 5px; padding-right: 5px; border-bottom: 1px solid lightgray;
                                                    text-align: center; border-top: 1px solid lightgray;">
                                                    <%# GrandTotal.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                                                </td>
                                            </tr>
                                            </tbody></table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                            <div class="PersonGridLeftPadding" style="overflow-x: auto; padding-top: 10px;" runat="server"
                                id="divProjects">
                                <br class="NotVisible" />
                                <table>
                                    <tr>
                                        <td colspan="4">
                                            <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Detail</font>
                                        </td>
                                    </tr>
                                </table>
                                <br class="NotVisible" />
                                <asp:DataList ID="dlProjects" runat="server" DataSource='<%# Eval("Key.GroupedTimeEtnries") %>'
                                    OnItemDataBound="dlProjects_OnItemDataBound" CssClass="WholeWidth">
                                    <ItemTemplate>
                                        <table>
                                            <tr>
                                                <td colspan="4">
                                                    <div style="padding-top: 3px;">
                                                        <font style="font-size: 14px; font-weight: bold;">
                                                            <%# Eval("Key.Client.Name") + " - " + Eval("Key.Name")%>
                                                        </font>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <br class="NotVisible" />
                                        <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                                            EnableModelValidation="True" CssClass="CompPerfTable WholeWidth" GridLines="Both"
                                            ShowFooter="true" OnRowDataBound="gvTimeEntries_OnRowDataBound" BackColor="White"
                                            EmptyDataText="This person has not entered any time for the period selected.">
                                            <AlternatingRowStyle BackColor="#F9FAFF" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
                                                            Date</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# ((TimeEntryRecord)Container.DataItem).MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="8%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
                                                            Note</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNote" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField FooterStyle-CssClass="AlignRight">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
                                                            Time Type</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%#((TimeEntryRecord)Container.DataItem).TimeType.Name %>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="24%" VerticalAlign="Middle" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <FooterTemplate>
                                                        <div  class="ie-bg" style="text-align: right;">
                                                            <asp:Label  ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="AlignCentre" FooterStyle-CssClass="AlignCentre">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; font-weight: bold;" class="ie-bg">
                                                            Hours</div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="text-align: center;">
                                                            <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                                                        </div>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" VerticalAlign="Middle" HorizontalAlign="Center" />
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <FooterTemplate>
                                                        <div class="ie-bg" style="text-align: center;">
                                                            <asp:Label ID="lblGvGridTotal" runat="server" Font-Bold="true"></asp:Label></div>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ItemTemplate>
                                </asp:DataList>
                            </div>
                            <div id="divPersonNotEntered" runat="server" class="PersonGridLeftPadding">
                                &nbsp;
                                <asp:Label ID="lblnoDataMesssage" runat="server" Text="This person has not entered any time for the period selected."
                                    Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div id="divhr" runat="server" style="padding-top: 10px; padding-bottom: 5px; padding-left: 5px;
                            padding-right: 5px;">
                            &nbsp;
                            <hr width="100%" size="2" color="#888888" align="center" style="margin-left: 5px;
                                margin-right: 5px;" />
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </asp:Panel>
            <asp:HiddenField ID="hdnGuid" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
            <asp:PostBackTrigger ControlID="btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="odsPersonTimeEntries" runat="server" SelectMethod="GetTimeEntriesForPerson"
        TypeName="PraticeManagement.Utils.TimeEntryHelper" OnSelecting="odsPersonTimeEntries_OnSelecting">
        <SelectParameters>
            <asp:Parameter Name="personIds" Type="Object" />
            <asp:Parameter Name="startDate" Type="DateTime" />
            <asp:Parameter Name="endDate" Type="DateTime" />
            <asp:Parameter Name="payTypeIds" Type="Object" />
            <asp:Parameter Name="practiceIds" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="odsCalendar" runat="server" SelectMethod="GetCalendar"
        TypeName="PraticeManagement.CalendarService.CalendarServiceClient" OnSelecting="odsCalendar_OnSelecting">
        <SelectParameters>
            <asp:Parameter Name="startDate" Type="DateTime" />
            <asp:Parameter Name="endDate" Type="DateTime" />
            <asp:Parameter Name="personId" Type="Int32" />
            <asp:Parameter Name="practiceManagerId" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

