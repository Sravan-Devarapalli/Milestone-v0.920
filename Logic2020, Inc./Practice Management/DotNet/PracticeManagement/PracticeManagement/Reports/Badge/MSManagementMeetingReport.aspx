<%@ Page Title="18-Month Management Meeting Report" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="MSManagementMeetingReport.aspx.cs" Inherits="PraticeManagement.Reports.Badge.MSManagementMeetingReport" %>

<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="Label" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
    <script src="<%# Generic.GetClientUrl("~/Scripts/ScrollinDropDown.min.js", this) %>"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../../Scripts/jquery.tablesorter.yui.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblManagementMeetingReport").tablesorter(
            {
                sortList: [[2, 0], [0, 0]],
                sortForce: [[2, 0], [0, 0]]
            });
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblManagementMeetingReport").tablesorter(
            {
                sortList: [[2, 0], [0, 0]],
                sortForce: [[2, 0], [0, 0]]
            });
        }

        function performClientValidation() {
            if (Page_ClientValidate("Revenue") && Page_ClientValidate("Currency")) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr class="height30P">
                    <td class="PaddingBottom10Imp vBottom fontBold Width3Percent no-wrap">
                        &nbsp;Select report parameters:&nbsp;
                    </td>
                    <td colspan="5">
                    </td>
                    <td class="width60P">
                    </td>
                </tr>
                <tr class="no-wrap">
                    <td class="ReportFilterLabels">
                        Pay Type:&nbsp;
                    </td>
                    <td colspan="4" class="PaddingTop5Imp">
                        <pmc:ScrollingDropDown ID="cblPayTypes" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPayTypes','Pay Type')" NoItemsType="All"
                            DropDownListType="Pay Type" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePayTypes" runat="server" TargetControlID="cblPayTypes"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td class="textLeft Width90Percent">
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateView" runat="server" Text="Update Report" OnClick="btnUpdateView_Click" />
                    </td>
                </tr>
                <tr class="no-wrap">
                    <td class="ReportFilterLabels">
                        Person Status:&nbsp;
                    </td>
                    <td colspan="4" class="PaddingTop5Imp">
                        <pmc:ScrollingDropDown ID="cblPersonStatus" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                            onclick="scrollingDropdown_onclick('cblPersonStatus','Person Status','es')" NoItemsType="All"
                            PluralForm="es" DropDownListType="Person Status" CellPadding="3" CssClass="AllEmpClockCblTimeScales" />
                        <ext:ScrollableDropdownExtender ID="sdePersonStatus" runat="server" TargetControlID="cblPersonStatus"
                            UseAdvanceFeature="true" Width="245px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                        </ext:ScrollableDropdownExtender>
                    </td>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr class="height30P">
                    <td colspan="7">
                        &nbsp;
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdateView" />
        </Triggers>
    </asp:UpdatePanel>
    <hr />
    <div id="divWholePage" runat="server" class="PaddingBottom10" visible="false">
        <table id="tblRange" runat="server" class="WholeWidth">
            <tr>
                <td class="font16Px fontBold">
                    Projected Range:
                    <asp:Label ID="lblRange" runat="server"></asp:Label>
                </td>
                <td class="textRight padRight25">
                    <label class="Width40P">
                        Export:</label>
                    <asp:Button ID="btnExportToExcel" runat="server" Text="Excel" OnClick="btnExportToExcel_OnClick"
                        Enabled="true" UseSubmitBehavior="false" ToolTip="Export To Excel." />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="ManagementReportHeader">
                    18-Month Management Meeting Report
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
        </table>
        <div class="ManagementReportDiv">
            <table id="tblManagementMeetingReport" class="tablesorter zebra">
                <asp:Repeater ID="repAllEmployeesMSReport" runat="server" OnItemDataBound="repAllEmployeesMSReport_ItemDataBound">
                    <HeaderTemplate>
                        <thead class=" no-wrap">
                            <tr class="MSReportTH MSReportBorder">
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Resource Name
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Pay Type
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Resource Level
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Badge Start
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Badge End
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Block Start
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Block End
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Projected Re-Badge Date
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Time Left on Clock
                                </th>
                                <th class="DayTotalHoursBorderLeft Padding5Imp">
                                    Managed Service
                                </th>
                                <asp:Repeater ID="repDatesHeaders" runat="server">
                                    <ItemTemplate>
                                        <th class="ManagementReportDateHeader">
                                            <%#GetHeaderDateFormat((DateTime?)Eval("StartDate"))%>
                                        </th>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate  MSReportBorder">
                            <td class="padLeft5 textLeft MSReportTD">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# Eval("Person.CurrentPay.TimescaleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BadgeEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BlockStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BlockEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.PlannedEndDate"))%>
                            </td>
                            <td sorttable_customkey='<%# Eval("Person.Badge.BadgeDuration") %>' class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <asp:Label ID="lblDuration" runat="server"></asp:Label>
                            </td>
                            <td sorttable_customkey='<%# Eval("Person.Badge.IsMSManagedService") %>' class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <asp:Label ID="lblManagedService" runat="server"></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft MSReportTD">
                                        <%# Eval("Available")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="ReportAlternateItemTemplate  MSReportBorder">
                            <td class="padLeft5 textLeft MSReportTD">
                                <%# Eval("Person.HtmlEncodedName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <asp:Label ID="lblPaytype" runat="server"></asp:Label>
                                <%# Eval("Person.CurrentPay.TimescaleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# Eval("Person.Title.HtmlEncodedTitleName")%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BadgeStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BadgeEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BlockStartDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.BlockEndDate"))%>
                            </td>
                            <td class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <%# GetDateFormat((DateTime?)Eval("Person.Badge.PlannedEndDate"))%>
                            </td>
                            <td sorttable_customkey='<%# Eval("Person.Badge.BadgeDuration") %>' class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <asp:Label ID="lblDuration" runat="server"></asp:Label>
                            </td>
                            <td sorttable_customkey='<%# Eval("Person.Badge.IsMSManagedService") %>' class="DayTotalHoursBorderLeft Padding5Imp MSReportTD">
                                <asp:Label ID="lblManagedService" runat="server"></asp:Label>
                            </td>
                            <asp:Repeater ID="repCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderLeft MSReportTD">
                                        <%# Eval("Available")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <thead>
                            <tr>
                                <td class="BorderBottomNone DayTotalHoursBorderRight" colspan="8" style="border-left: none">
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD">
                                    <b>TOTAL</b>
                                </td>
                                <asp:Repeater ID="repTotal" runat="server">
                                    <ItemTemplate>
                                        <td class="DayTotalHoursBorderRight MSReportTD">
                                            <b>
                                                <%# Eval("Total")%></b>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        </tbody>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="bgcolorwhite BorderBottomNone" colspan="8">
                    </td>
                    <td class="bgcolorwhite MSReportTD" colspan="2">
                    </td>
                </tr>
                <%--TABLE-2--%>
                <asp:Repeater ID="repAvailableResources" runat="server" OnItemDataBound="repAvailableResources_ItemDataBound">
                    <HeaderTemplate>
                        <thead>
                            <th class="bgcolorwhite DayTotalHoursBorderRight DefaultCursorImp BorderBottomNone BorderTopNone"
                                colspan="8">
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH" colspan="2">
                                Total Available
                            </th>
                            <asp:Repeater ID="repDatesHeaders" runat="server">
                                <ItemTemplate>
                                    <th class="ManagementReportDateHeader MSReportTH">
                                        <%#GetHeaderDateFormat((DateTime?)Eval("StartDate"))%>
                                    </th>
                                </ItemTemplate>
                            </asp:Repeater>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                                colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("AvailableResourcesCount")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="ReportAlternateItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderTopNone BorderBottomNone"
                                colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("AvailableResourcesCount")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <thead>
                            <tr>
                                <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                                    colspan="8">
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD" colspan="2">
                                    <b>TOTAL</b>
                                </td>
                                <asp:Repeater ID="repTotal" runat="server">
                                    <ItemTemplate>
                                        <td class="DayTotalHoursBorderRight MSReportTD">
                                            <b>
                                                <%# Eval("Total")%></b>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        </tbody>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="bgcolorwhite BorderBottomNone" colspan="8">
                    </td>
                    <td class="bgcolorwhite MSReportTD" colspan="2">
                    </td>
                </tr>
                <%--TABLE-3--%>
                <asp:Repeater ID="repManagedService" runat="server" OnItemDataBound="repAvailableResources_ItemDataBound">
                    <HeaderTemplate>
                        <thead>
                            <th class="bgcolorwhite DayTotalHoursBorderRight DefaultCursorImp BorderTopNone BorderBottomNone"
                                colspan="8">
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH" colspan="2">
                                Managed Service
                            </th>
                            <asp:Repeater ID="repDatesHeaders" runat="server">
                                <ItemTemplate>
                                    <th class="ManagementReportDateHeader MSReportTH">
                                        <%#GetHeaderDateFormat((DateTime?)Eval("StartDate"))%>
                                    </th>
                                </ItemTemplate>
                            </asp:Repeater>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("MSCount")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="ReportAlternateItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("MSCount")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <thead>
                            <tr>
                                <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD" colspan="2">
                                    <b>TOTAL</b>
                                </td>
                                <asp:Repeater ID="repTotal" runat="server">
                                    <ItemTemplate>
                                        <td class="DayTotalHoursBorderRight MSReportTD">
                                            <b>
                                                <%# Eval("TotalResourcesWithMS")%></b>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        </tbody>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="bgcolorwhite BorderBottomNone" colspan="8">
                    </td>
                    <td class="bgcolorwhite MSReportTD" colspan="2">
                    </td>
                </tr>
                <%--TABLE-4--%>
                <asp:Repeater ID="repAvlResourcesWithOutManagedService" runat="server" OnItemDataBound="repAvailableResources_ItemDataBound">
                    <HeaderTemplate>
                        <thead>
                            <th class="bgcolorwhite DayTotalHoursBorderRight DefaultCursorImp BorderBottomNone BorderTopNone"
                                colspan="8">
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH" colspan="2">
                                Total Available (w/o Managed Service)
                            </th>
                            <asp:Repeater ID="repDatesHeaders" runat="server">
                                <ItemTemplate>
                                    <th class="ManagementReportDateHeader MSReportTH">
                                        <%#GetHeaderDateFormat((DateTime?)Eval("StartDate"))%>
                                    </th>
                                </ItemTemplate>
                            </asp:Repeater>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("AvaliableResourcesWithOutMS")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="ReportAlternateItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <%#Eval("AvaliableResourcesWithOutMS")%>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <thead>
                            <tr>
                                <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="8">
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD" colspan="2">
                                    <b>TOTAL</b>
                                </td>
                                <asp:Repeater ID="repTotal" runat="server">
                                    <ItemTemplate>
                                        <td class="DayTotalHoursBorderRight MSReportTD">
                                            <b>
                                                <%# Eval("TotalAvailableResourcesWithOutMS")%></b>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        </tbody>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="bgcolorwhite BorderBottomNone" colspan="6">
                    </td>
                    <td class="bgcolorwhite MSReportTD" colspan="4">
                    </td>
                </tr>
                <%--Table 5--%>
                <asp:Repeater ID="repDeficit" runat="server" OnItemDataBound="repAvailableResources_ItemDataBound">
                    <HeaderTemplate>
                        <thead>
                            <th class="bgcolorwhite DayTotalHoursBorderRight DefaultCursorImp BorderBottomNone BorderTopNone"
                                colspan="6">
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH">
                                AVG
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH">
                                Target
                            </th>
                            <th class="DayTotalHoursBorderRight MSReportTH" colspan="2">
                                Deficit
                            </th>
                            <asp:Repeater ID="repDatesHeaders" runat="server">
                                <ItemTemplate>
                                    <th class="ManagementReportDateHeader MSReportTH">
                                        <%#GetHeaderDateFormat((DateTime?)Eval("StartDate"))%>
                                    </th>
                                </ItemTemplate>
                            </asp:Repeater>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="ReportItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="6">
                            </td>
                            <td class="DayTotalHoursBorderRight MSReportTD">
                                <%#Eval("Average")%>%
                            </td>
                            <td class="DayTotalHoursBorderRight MSReportTD">
                                <%# Eval("Target")%>
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server" OnItemDataBound="repRequiredCount_ItemDataBound">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <asp:Label ID="lblResources" runat="server" Text='<%#Eval("RequiredResources")%>'> </asp:Label>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="ReportAlternateItemTemplate ">
                            <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="6">
                            </td>
                            <td class="DayTotalHoursBorderRight MSReportTD">
                                <%#Eval("Average")%>%
                            </td>
                            <td class="DayTotalHoursBorderRight MSReportTD">
                                <%# Eval("Target")%>
                            </td>
                            <td class="DayTotalHoursBorderRight padLeft5 textLeft MSReportTD" colspan="2">
                                <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title.HtmlEncodedTitleName")%>'></asp:Label>
                            </td>
                            <asp:Repeater ID="repResourceCount" runat="server" OnItemDataBound="repRequiredCount_ItemDataBound">
                                <ItemTemplate>
                                    <td class="DayTotalHoursBorderRight MSReportTD">
                                        <asp:Label ID="lblResources" runat="server" Text='<%#Eval("RequiredResources")%>'> </asp:Label>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <thead>
                            <tr>
                                <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone" colspan="7">
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD">
                                    <b>
                                        <asp:Label ID="lblTotalTargetResources" runat="server"></asp:Label></b>
                                </td>
                                <td class="DayTotalHoursBorderRight MSReportTD" colspan="2">
                                    <b>TOTAL</b>
                                </td>
                                <asp:Repeater ID="repTotal" runat="server" OnItemDataBound="repTotal_ItemDataBound">
                                    <ItemTemplate>
                                        <td class="DayTotalHoursBorderRight DayTotalHoursBorderRight MSReportTD">
                                            <b>
                                                <asp:Label ID="lblTotalResources" runat="server" Text='<%#Eval("TotalRequiredResources")%>'> </asp:Label>
                                            </b>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </thead>
                        </tbody>
                    </FooterTemplate>
                </asp:Repeater>
                <tr>
                    <td class="bgcolorwhite  BorderBottomNone" colspan="8">
                    </td>
                    <td class="bgcolorwhite MSReportTD " colspan="2" style="border-bottom: none">
                    </td>
                </tr>
                <%--Table-6--%>
                <thead>
                    <tr>
                        <th class="bgcolorwhite DayTotalHoursBorderRight DefaultCursorImp BorderBottomNone BorderTopNone"
                            colspan="4">
                        </th>
                        <th class="DayTotalHoursBorderRight Padding5Imp DefaultCursorImp MSReportTH" width="150px">
                            Actual Rev/Hour
                        </th>
                        <th class="DayTotalHoursBorderRight Padding5Imp DefaultCursorImp MSReportTH" width="150px">
                            Target Rev/Hour
                        </th>
                        <th class="DayTotalHoursBorderRight Padding5Imp DefaultCursorImp MSReportTH">
                            Hours - 90% Utilization
                        </th>
                        <th class="DayTotalHoursBorderRight Padding5Imp DefaultCursorImp MSReportTH">
                            Target Rev/Annual
                        </th>
                        <th class="Padding5Imp DefaultCursorImp BorderBottomNone BorderTopNone" colspan="2">
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                            colspan="4">
                        </td>
                        <td class="DayTotalHoursBorderRight Padding5Imp MSReportTD" style="white-space: nowrap;
                            background-color: rgba(101, 87, 87, 0.1)">
                            $
                            <asp:TextBox ID="txtActualRevenuePerHour" runat="server" MaxLength="100" Width="80"></asp:TextBox>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteActualRevenuePerHour" TargetControlID="txtActualRevenuePerHour"
                                FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                            <asp:RequiredFieldValidator ID="reqActualRevenuePerHour" runat="server" ControlToValidate="txtActualRevenuePerHour"
                                Text="*" ErrorMessage="Actual Rev/Hour is required." ToolTip="Actual Rev/Hour is required."
                                Display="Dynamic" SetFocusOnError="true" ValidationGroup="Revenue"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regExpActualRevenuePerHour" runat="server" ValidationExpression="((\d+)((\.\d{1,2})?))$"
                                Text="*" ValidationGroup="Currency" ToolTip="Please enter valid integer or decimal number with 2 decimal places."
                                ControlToValidate="txtActualRevenuePerHour" />
                        </td>
                        <td class="DayTotalHoursBorderRight Padding5Imp MSReportTD" style="white-space: nowrap;
                            background-color: rgba(101, 87, 87, 0.1)">
                            $
                            <asp:TextBox ID="txtTargetRevenuePerHour" runat="server" MaxLength="100" Width="80"></asp:TextBox>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteTargetRevenuePerHour" TargetControlID="txtTargetRevenuePerHour"
                                FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                            <asp:RequiredFieldValidator ID="reqTargetRevenuePerHour" runat="server" ControlToValidate="txtTargetRevenuePerHour"
                                Text="*" ErrorMessage="Target Rev/Hour is required." ToolTip="Target Rev/Hour is required."
                                Display="Dynamic" SetFocusOnError="true" ValidationGroup="Revenue"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cmpActualRevenuePerHour" runat="server" ControlToValidate="txtTargetRevenuePerHour"
                                Text="*" ErrorMessage="Target Rev/Hour should be greater than zero to avoid division by zero error in Rev/FTE calculation."
                                SetFocusOnError="true" ToolTip="Target Rev/Hour should be greater than zero to avoid division by zero error in Rev/FTE calculation."
                                ValueToCompare="0" Operator="GreaterThan" ValidationGroup="Revenue"></asp:CompareValidator>
                            <asp:RegularExpressionValidator ID="regExpTargetRevenuePerHour" runat="server" ValidationExpression="((\d+)((\.\d{1,2})?))$"
                                Text="*" ValidationGroup="Currency" ToolTip="Please enter valid integer or decimal number with 2 decimal places."
                                ControlToValidate="txtTargetRevenuePerHour" />
                        </td>
                        <td class="DayTotalHoursBorderRight Padding5Imp MSReportTD" style="white-space: nowrap;
                            background-color: rgba(101, 87, 87, 0.1)">
                            <asp:TextBox ID="txtHoursUtilization" runat="server" MaxLength="100" Width="80"></asp:TextBox>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteHoursUtilization" TargetControlID="txtHoursUtilization"
                                FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars="." />
                            <asp:RequiredFieldValidator ID="reqHoursUtilization" runat="server" ControlToValidate="txtHoursUtilization"
                                Text="*" ErrorMessage="Hours - 90% Utilization is required." ToolTip="Hours - 90% Utilization is required."
                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="Revenue"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cmpHoursUtilization" runat="server" ControlToValidate="txtHoursUtilization"
                                Text="*" ErrorMessage="Hours - 90% Utilization should be greater than zero to avoid division by zero error in Rev/FTE calculation."
                                ToolTip="Hours - 90% Utilization should be greater than zero to avoid division by zero error in Rev/FTE calculation."
                                SetFocusOnError="true" ValueToCompare="0.0" Operator="GreaterThan" ValidationGroup="Revenue"></asp:CompareValidator>
                            <asp:RegularExpressionValidator ID="regExpHoursUtilization" runat="server" ValidationExpression="((\d+)((\.\d{1,2})?))$"
                                Text="*" ValidationGroup="Currency" ToolTip="Please enter valid integer or decimal number with 2 decimal places."
                                ControlToValidate="txtHoursUtilization" EnableClientScript="true" />
                        </td>
                        <td class="DayTotalHoursBorderRight Padding5Imp MSReportTD" style="white-space: nowrap;
                            background-color: rgba(101, 87, 87, 0.1)">
                            $&nbsp;
                            <asp:TextBox ID="txtTotalRevenue" runat="server" MaxLength="100"></asp:TextBox>
                            <AjaxControlToolkit:FilteredTextBoxExtender ID="fteTotalRevenue" TargetControlID="txtTotalRevenue"
                                FilterType="Numbers,Custom" FilterMode="ValidChars" runat="server" ValidChars=".," />
                            <asp:RequiredFieldValidator ID="reqFieldTotalRevenue" runat="server" ControlToValidate="txtTotalRevenue"
                                Text="*" ErrorMessage="Target Rev/Annual is required." ToolTip="Target Rev/Annual is required."
                                SetFocusOnError="true" Display="Dynamic" ValidationGroup="Revenue"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cmpTotalRevenue" runat="server" ControlToValidate="txtTotalRevenue"
                                Text="*" ErrorMessage="Target Rev/Annual should be greater than zero." ToolTip="Target Rev/Annual should be greater than zero."
                                SetFocusOnError="true" ValueToCompare="0" Operator="GreaterThan" ValidationGroup="Revenue"></asp:CompareValidator>
                            <asp:RegularExpressionValidator ID="regExpTotalRevenue" runat="server" ValidationExpression="((\d+)((\.\d{1,2})?))$"
                                Text="*" ValidationGroup="Currency" ToolTip="Please enter valid integer or decimal number with 2 decimal places."
                                ControlToValidate="txtTotalRevenue" />
                        </td>
                        <td class="bgcolorwhite Padding5Imp t-left  BorderBottomNone" colspan="2">
                            Total Revenue
                        </td>
                        <td class="BorderBottomNone" rowspan="4" colspan="18" style="background-color: White;
                            text-align: left; padding-left: 10px">
                            <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="Revenue" ShowMessageBox="false"
                                ShowSummary="true" EnableClientScript="true" />
                            <asp:ValidationSummary ID="valSummaryForCurrency" runat="server" ValidationGroup="Currency"
                                ShowMessageBox="false" ShowSummary="true" EnableClientScript="true" HeaderText="Please enter valid integer or decimal number with 2 decimal places." />
                        </td>
                    </tr>
                    <tr>
                        <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                            colspan="7">
                        </td>
                        <td class="bgcolorwhite DayTotalHoursBorderRight MSReportTD">
                            <asp:Label ID="lblRevenuePerFTE" runat="server"></asp:Label>
                        </td>
                        <td class="bgcolorwhite Padding5Imp t-left  BorderBottomNone" colspan="2">
                            Rev/FTE
                            <asp:Image alt="Rev/FTE Hint" ImageUrl="~/Images/hint1.png" runat="server" ID="imgRevPerFTEHint"
                                CssClass="CursorPointer" ToolTip="Rev/FTE Calculation." />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeRevPerFTE" runat="server" TargetControlID="imgRevPerFTEHint"
                                CancelControlID="btnCancel" BehaviorID="pnlRevPerFTE" BackgroundCssClass="modalBackground"
                                PopupControlID="pnlRevPerFTE" DropShadow="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                            colspan="7">
                        </td>
                        <td class="bgcolorwhite DayTotalHoursBorderRight Padding5Imp MSReportTD">
                            <asp:Label ID="lblFTE" runat="server"></asp:Label>
                        </td>
                        <td class="bgcolorwhite BorderBottomNone Padding5Imp t-left " colspan="2">
                            FTE
                            <asp:Image alt="Required Hint" ImageUrl="~/Images/hint1.png" runat="server" ID="imgFTEHint"
                                CssClass="CursorPointer" ToolTip="FTE Calculation." />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeFTE" runat="server" TargetControlID="imgFTEHint"
                                CancelControlID="btnClose" BehaviorID="pnlFTEHit" BackgroundCssClass="modalBackground"
                                PopupControlID="pnlFTEHit" DropShadow="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                            colspan="7">
                        </td>
                        <td class="bgcolorwhite DayTotalHoursBorderRight Padding5Imp MSReportTD">
                            <asp:Label ID="lblManagedServiceCount" runat="server"></asp:Label>
                        </td>
                        <td class="bgcolorwhite BorderBottomNone Padding5Imp t-left " colspan="2">
                            Managed Service
                        </td>
                    </tr>
                    <tr>
                        <td class="bgcolorwhite DayTotalHoursBorderRight BorderBottomNone BorderTopNone"
                            colspan="7">
                        </td>
                        <td class="bgcolorwhite DayTotalHoursBorderRight Padding5Imp MSReportTD">
                            <asp:Label ID="lblRequiredResources" runat="server"></asp:Label>
                        </td>
                        <td class="bgcolorwhite BorderBottomNone Padding5Imp t-left " colspan="2">
                            Required
                            <asp:Image alt="Required Hint" ImageUrl="~/Images/hint1.png" runat="server" ID="imgRequiredHint"
                                CssClass="CursorPointer" ToolTip="Required Resource Calculation." />
                            <AjaxControlToolkit:ModalPopupExtender ID="mpeRequired" runat="server" TargetControlID="imgRequiredHint"
                                CancelControlID="btnCancelHint" BehaviorID="pnlReuiredResourceHit" BackgroundCssClass="modalBackground"
                                PopupControlID="pnlReuiredResourceHit" DropShadow="false" />
                        </td>
                        <td class="bgcolorwhite BorderBottomNone">
                            <asp:Button ID="btnSubmit" runat="server" Text="Update" OnClientClick="performClientValidation();"
                                OnClick="btnSubmit_Clicked" />
                        </td>
                    </tr>
                </tbody>
                <tr>
                    <td class="bgcolorwhite">
                        <br />
                    </td>
                </tr>
                <tr class="MSMeetingReportFooter">
                    <td colspan="28" class="buttons-block">
                        <table>
                            <tr>
                                <td>
                                    <b>Legend</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    1. "1" will be shown in 1<sup>st</sup> table if the resource is available during
                                    the complete month.
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    2. "-" will be shown in 1<sup>st</sup> table if the resource is unavailable for
                                    at least one day during the month.
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="divEmptyMessage" class="EmptyMessagediv" runat="server" visible="false">
        There are no resources for the selected filters.
    </div>
    <asp:Panel ID="pnlRevPerFTE" runat="server" CssClass="popUpBillableUtilization" Style="display: none;">
        <table>
            <tr>
                <td colspan="2" class="textCenter">
                    <label class="LabelProject">
                        Rev/FTE
                    </label>
                </td>
                <td>
                    <asp:Button ID="btnCancel" runat="server" CssClass="mini-report-close floatright"
                        ToolTip="Close" Text="X"></asp:Button>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The Rev/FTE is calculated as product
                        of <b>Target Rev/Hour</b> and <b>Hours-90% Utilization</b>.</p>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Make sure that the value does not
                        equal to zero.</p>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlFTEHit" runat="server" CssClass="popUpBillableUtilization" Style="display: none;">
        <table style="width: 400px">
            <tr>
                <td colspan="5">
                    <asp:Button ID="btnClose" runat="server" CssClass="mini-report-close floatright"
                        ToolTip="Close" Text="X"></asp:Button>
                </td>
            </tr>
            <tr>
                <td class="Width10Percent">
                </td>
                <td class="Width30P">
                </td>
                <td class="Width5Percent">
                </td>
                <td class="Width40P textCenter vBottom">
                    Target Rev/Annual
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
            <tr>
                <td class="Width10Percent">
                </td>
                <td class="Width30P">
                    <label class="LabelProject">
                        FTE Calculation:
                    </label>
                </td>
                <td class="Width5Percent">
                </td>
                <td class="Width40P textCenter">
                    <hr class="Wholewidth hrArrition" />
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
            <tr>
                <td class="Width10Percent">
                </td>
                <td class="Width30P">
                </td>
                <td class="Width5Percent">
                </td>
                <td class="Width40P textCenter vTop">
                    Rev/FTE
                </td>
                <td class="Width15Percent">
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlReuiredResourceHit" runat="server" CssClass="popUpBillableUtilization"
        Style="display: none;">
        <table style="width: 500px">
            <tr>
                <td colspan="4" class="textCenter">
                    <label class="LabelProject">
                        Required Resources
                    </label>
                </td>
                <td>
                    <asp:Button ID="btnCancelHint" runat="server" CssClass="mini-report-close floatright"
                        ToolTip="Close" Text="X"></asp:Button>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="Width5Percent">
                </td>
                <td colspan="4">
                    <p>
                        Required resources value is calculated by subtracting the <b>Managed Resources</b>
                        from <b>FTE</b>.</p>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="Width5Percent">
                </td>
                <td class="Width25Percent">
                </td>
                <td class="Width30P textCenter vBottom">
                    Target Rev/Annual
                </td>
                <td class="Width25Percent">
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="Width5Percent">
                </td>
                <td class="Width25Percent">
                    <label class="LabelProject">
                        FTE Calculation:
                    </label>
                </td>
                <td class="Width30P textCenter">
                    <hr class=" hrArrition" />
                </td>
                <td class="Width25Percent">
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="Width5Percent">
                </td>
                <td class="Width25Percent">
                </td>
                <td class="Width30P textCenter vTop">
                    Rev/FTE
                </td>
                <td class="Width25Percent">
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="Width5Percent">
                </td>
                <td colspan="4">
                    <p>
                        <b>Managed Resources</b> value is the number of available resources with Managed
                        Service = Yes in the current month.
                    </p>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

