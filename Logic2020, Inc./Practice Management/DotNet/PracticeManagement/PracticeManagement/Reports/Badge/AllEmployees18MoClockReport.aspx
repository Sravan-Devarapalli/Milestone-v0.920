<%@ Page Title="All Employees' 18-Month Clock Dates" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="AllEmployees18MoClockReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.AllEmployees18MoClockReport" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../../Scripts/jquery.tablesorter.yui.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblEmployeeClockReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblEmployeeClockReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td style="padding-bottom: 10px;" class="vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td colspan="">
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Pay Type:&nbsp;
                    </td>
                    <td class="textLeft">
                        <pmc:ScrollingDropDown ID="cblPayTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPayTypes','Pay Type')" NoItemsType="All"
                            DropDownListType="Pay Type" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePayTypes" runat="server" TargetControlID="cblPayTypes"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td style="text-align: right; padding-right: 1.8%">
                        <asp:Button ID="btnUpdateView" runat="server" Text="View Report" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr style="white-space: nowrap">
                    <td class="ReportFilterLabels">
                        Person Status:&nbsp;
                    </td>
                    <td style="padding-top: 5px;">
                        <pmc:ScrollingDropDown ID="cblPersonStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')" NoItemsType="All"
                            PluralForm="es" DropDownListType="Person Status" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePersonStatus" runat="server" TargetControlID="cblPersonStatus"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <hr />
            <div id="divWholePage" runat="server">
                <table id="tblRange" runat="server" class="WholeWidth">
                    <tr>
                        <td style="text-align: right; padding-right: 30px;">
                            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" OnClick="btnExportToExcel_OnClick"
                                Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel" />
                        </td>
                    </tr>
                    <tr>
                        <td class="paddingBottom10px">
                            <asp:Label ID="lblTitle" runat="server" Text="All Employees' 18-Month Clock Dates:"
                                Style="font-weight: bold; font-size: 20px;"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Repeater ID="repAllEmployeesClock" runat="server" OnItemDataBound="repAllEmployeesClock_ItemDataBound">
                    <HeaderTemplate>
                        <div class="minheight250Px">
                            <table id="tblEmployeeClockReport" class="tablesorter PersonSummaryReport zebra">
                                <thead>
                                    <tr>
                                        <th class="TextAlignLeftImp Padding5Imp">
                                            Resource Name
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Pay Type
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Resource Level
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            18-Month Clock Start Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            18-Month Clock End Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Time Left on Clock
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            6-Month Break Start Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            6-Month Break End Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Organic Break Start Date
                                        </th>
                                         <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Organic Break End Date
                                        </th>
                                        <th class="DayTotalHoursBorderLeft Padding5Imp">
                                            Time Left on Organic Break
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.CurrentPay.TimescaleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
                            </td>
                            <td sorttable_customkey='<%# Eval("BadgeDuration") %>' class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblDuration" runat="server"></asp:Label>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BreakStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BreakEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("OrganicBreakStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("OrganicBreakEndDate"))%>
                            </td>
                             <td sorttable_customkey='<%# Eval("OrganicBreakDuration") %>' class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblOrganicDuration" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alterrow">
                            <td class="padLeft5 textLeft">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.CurrentPay.TimescaleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BadgeEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp" sorttable_customkey='<%# Eval("BadgeDuration") %>'>
                                <asp:Label ID="lblDuration" runat="server"></asp:Label>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BreakStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("BreakEndDate"))%>
                            </td>
                              <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("OrganicBreakStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp">
                                <%# GetDateFormat((DateTime?)Eval("OrganicBreakEndDate"))%>
                            </td>
                             <td sorttable_customkey='<%# Eval("OrganicBreakDuration") %>' class="DayTotalHoursBorderLeft Padding5Imp">
                                <asp:Label ID="lblOrganicDuration" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody></table></div>
                    </FooterTemplate>
                </asp:Repeater>
                <div id="divEmptyMessage" class="EmptyMessagediv" style="display: none;" runat="server">
                    There are no resources for the selected filters.
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
</asp:Content>

