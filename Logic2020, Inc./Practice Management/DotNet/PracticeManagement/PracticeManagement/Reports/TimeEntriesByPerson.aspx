<%@ Page Title="Practice Management - Person Time Entries Grouped By Projects" Language="C#"
    MasterPageFile="~/PracticeManagementMain.Master" AutoEventWireup="true" CodeBehind="TimeEntriesByPerson.aspx.cs"
    Inherits="PraticeManagement.Sandbox.TimeEntriesByPerson" %>

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
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
    <title>Practice Management - Person Time Entries Grouped By Projects</title>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script language="javascript" type="text/javascript" src="../Scripts/jquery-1.4.1.js"></script>
    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <script language="javascript" type="text/javascript">

        $(window).resize(function () {

            SetDivWidth();
        });


        function SetDivWidth() {
            var datalist = $get("<%= dlPersons.ClientID %>");
            if (datalist != null) {
                var tr = datalist.children[0].children;
                for (var index = 0; index < tr.length; index = index + 1) {
                    $(tr[index].children[0].children[1].children[1]).css("width", $(window).width() - 100);
                    $(tr[index].children[0].children[2]).css("width", $(window).width() - 100);
                    $(tr[index].children[0].children[3]).css("width", $(window).width() - 100);
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
                        <td  style="width: 13%; padding-top: 3px; white-space:nowrap;" align="left">
                            &nbsp;&nbsp;Show Time Entered
                        </td>
                        <td style="width: 20%" align="left">
                            <uc:DateInterval ID="diRange" runat="server" FromToDateFieldWidth="70" IsFromDateRequired="true"
                                IsToDateRequired="true" />
                            <asp:ValidationSummary ID="valSum" runat="server" />
                        </td>
                        <td style="width: 3%;padding-top: 3px;  white-space:nowrap;" align="left">
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
                    OnItemDataBound="dlPersons_OnItemDataBound">
                    <ItemTemplate>
                        <div style="padding-bottom: 5px;">
                            <font style="font-size: 20px; font-weight: bold;">
                                <%# Eval("Key.PersonName") %></font>
                        </div>
                        <div class="PersonGridLeftPadding" runat="server" id="divTeTable">
                            <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Summary</font>
                            <div style="overflow-x: auto; overflow-y: display;">
                                <asp:Repeater ID="repTeTable" runat="server" DataSource='<%# Eval("Value")%>' OnItemCreated="repTeTable_OnItemCreated">
                                    <HeaderTemplate>
                                        <table class="time-entry-person-projects WholeWidth" style="display: inline;">
                                            <thead>
                                                <tr>
                                                    <th style="width: 200px !important; text-align: left">
                                                        <asp:Label ID="lblPType" runat="server" Text="Project-Time Type" Width="200px"></asp:Label>
                                                    </th>
                                                    <asp:Repeater ID="dlProject" runat="server" DataSourceID="odsCalendar" OnItemCreated="dlProject_OnItemCreated"
                                                        OnInit="dlProject_OnInit">
                                                        <ItemTemplate>
                                                            <th>
                                                            </th>
                                                            <th style="padding-left: 10px" class="<%# PraticeManagement.Utils.Calendar.GetCssClassByCalendarItem((CalendarItem) Container.DataItem) %>">
                                                                <%# DataBinder.Eval(Container.DataItem, "Date", "{0:ddd<br/>MMM d}")%>
                                                            </th>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <th>
                                                    </th>
                                                    <th style="padding-left: 10px">
                                                        Totals
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="<%# Container.ItemIndex % 2 == 0 ? "alterrow" : string.Empty %>">
                                            <td style="text-align: left; width: 200px; text-align: left">
                                                <asp:Label ID="lblPTypeValue" runat="server" Text='<%#  DataBinder.Eval(Container.DataItem, "Key")%>'
                                                    Width="200px"></asp:Label>
                                            </td>
                                            <asp:Repeater ID="dlProject" runat="server" DataSource='<%# DataBinder.Eval(Container.DataItem, "Value") %>'
                                                OnItemDataBound="dlProject_OnItemDataBound">
                                                <ItemTemplate>
                                                    <td colspan="<%# GetColspan(((TimeEntryRecord)Container.DataItem).MilestoneDate, Container.ItemIndex) %>">
                                                    </td>
                                                    <td style="padding-left: 10px">
                                                        <%# DataBinder.Eval(Container.DataItem, "ActualHours", "{0:F2}") %>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <td colspan="<%# GetLastColspan() %>">
                                            </td>
                                            <td style="font-weight: bold; padding-left: 10px">
                                                <%# ProjectTotals.ToString(PraticeManagement.Constants.Formatting.DoubleFormat) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <tr style="font-weight: bold">
                                            <td style="text-align: left; width: 200px; text-align: left;">
                                                Totals
                                            </td>
                                            <asp:Repeater ID="dlTotals" runat="server" OnItemDataBound="dlTotals_OnItemDataBound"
                                                OnInit="dlTotals_OnInit">
                                                <ItemTemplate>
                                                    <td colspan="<%# GetColspan(((KeyValuePair<DateTime, double>)Container.DataItem).Key, Container.ItemIndex) %>">
                                                    </td>
                                                    <td style="padding-left: 10px">
                                                        <%# DataBinder.Eval(Container.DataItem, "Value", "{0:F2}") %>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <td id="tdExtracolumns" runat="server">
                                            </td>
                                            <td style="font-size: 125%; padding-left: 10px">
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
                            <font style="font-style: italic; font-size: 16px; font-weight: bold;">Time Entry Detail</font>
                            <asp:DataList ID="dlProjects" runat="server" DataSource='<%# Eval("Key.GroupedTimeEtnries") %>'
                                CssClass="WholeWidth">
                                <ItemTemplate>
                                    <div style="padding-top: 3px;">
                                        <font style="font-size: 14px; font-weight: bold;">
                                            <%# Eval("Key.Name") %>
                                        </font>
                                    </div>
                                    <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" DataSource='<%# Eval("Value") %>'
                                        EnableModelValidation="True" CssClass="CompPerfTable WholeWidth" GridLines="None"
                                        ShowFooter="true" OnRowDataBound="gvTimeEntries_OnRowDataBound" BackColor="White"
                                        EmptyDataText="This person has not entered any time for this project for the period selected.">
                                        <AlternatingRowStyle BackColor="#F9FAFF" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="MilestoneDate" SortExpression="MilestoneDate">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Date</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <%# ((TimeEntryRecord)Container.DataItem).MilestoneDate.ToString(PraticeManagement.Constants.Formatting.EntryDateFormat)%>
                                                </ItemTemplate>
                                                <ItemStyle Width="100" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Note" SortExpression="Note">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Note</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("Note") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ActualHours" SortExpression="ActualHours" FooterStyle-CssClass="AlignRight">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Time Type</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <%#((TimeEntryRecord)Container.DataItem).TimeType.Name %>
                                                </ItemTemplate>
                                                <ItemStyle Width="250" />
                                                <FooterTemplate>
                                                    <div class="ie-bg" style="padding-top: 10px">
                                                        <asp:Label ID="lblGvGridTotalText" runat="server" Text="Total =" Font-Bold="true"></asp:Label></div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ActualHours" SortExpression="ActualHours" ItemStyle-CssClass="AlignCentre"
                                                FooterStyle-CssClass="AlignCentre">
                                                <HeaderTemplate>
                                                    <div class="ie-bg">
                                                        Hours</div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <%#((TimeEntryRecord)Container.DataItem).ActualHours.ToString(PraticeManagement.Constants.Formatting.DoubleFormat)%>
                                                </ItemTemplate>
                                                <ItemStyle Width="100" />
                                                <FooterTemplate>
                                                    <div class="ie-bg" style="padding-top: 10px">
                                                        <asp:Label ID="lblGvGridTotal" runat="server" Font-Bold="true"></asp:Label></div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ItemTemplate>
                            </asp:DataList>
                        </div>
                        <div class="PersonGridLeftPadding">
                            <asp:Label ID="lblnoDataMesssage" runat="server" Text="This person might not be assigned to any project or there are no time entries submitted."
                                Visible="false"></asp:Label>
                        </div>
                        <div style="padding-top: 10px; padding-bottom: 5px; padding-left: 5px; padding-right: 5px;">
                            <hr width="100%" size="2" color="#888888" align="center" style="margin-left: 5px;
                                margin-right: 5px;">
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </asp:Panel>
        </ContentTemplate>
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

