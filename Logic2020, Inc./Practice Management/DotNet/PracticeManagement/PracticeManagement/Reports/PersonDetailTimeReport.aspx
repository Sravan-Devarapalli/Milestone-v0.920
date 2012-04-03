<%@ Page Title="By Person" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PersonDetailTimeReport.aspx.cs" Inherits="PraticeManagement.Reporting.PersonDetailTimeReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register Src="~/Controls/Reports/PersonSummaryReport.ascx" TagPrefix="uc" TagName="PersonSummaryReport" %>
<%@ Register Src="~/Controls/Reports/PersonDetailReport.ascx" TagPrefix="uc" TagName="PersonDetailReport" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        function btnClose_OnClientClick(popup) {
            var dlPersonDiv = document.getElementById('<%= dlPersonDiv.ClientID %>');
            var txtSearch = document.getElementById('<%= txtSearch.ClientID %>');
            if (dlPersonDiv != null && dlPersonDiv != undefined) {
                dlPersonDiv.style.display = 'none';
            }
            var waterMarkTxtSearch = $find('waterMarkTxtSearch');
            waterMarkTxtSearch.set_Text('');
            var btnSearch = document.getElementById('<%= btnSearch.ClientID %>');
            btnSearch.setAttribute('disabled', 'disabled');
            $find(popup).hide();
            return false;
        }
        function CollapseOrExpandAll(btnExpandOrCollapseAllClientId, hdnCollapsedClientId, hdncpeExtendersIds) {
            var btn = btnExpandOrCollapseAllClientId;
            var hdnCollapsed = hdnCollapsedClientId;
            var isExpand = false;
            if (btn != null) {
                if (btn.value == "Expand All") {
                    isExpand = true;
                    btn.value = "Collapse All";
                    btn.title = "Collapse All";
                    hdnCollapsed.value = 'false';
                }
                else {
                    btn.value = "Expand All";
                    btn.title = "Expand All";
                    hdnCollapsed.value = 'true';
                }
                var projectPanelskvPair = jQuery.parseJSON(hdncpeExtendersIds.value);
                ExpandOrCollapsePanels(projectPanelskvPair, isExpand);
            }
            return false;
        }


        function setAutoOrZeroHeight() {

            var pnlDateDetailsList = $("[id$='pnlDateDetails']");

            var pnlProjectDetailsList = $("[id$='pnlProjectDetails']");

            for (var i = 0; i < pnlProjectDetailsList.length; i++) {

                if (pnlProjectDetailsList[i].style.height != "0px") {
                    pnlProjectDetailsList[i].style.height = "auto";
                }

            }

            for (var i = 0; i < pnlDateDetailsList.length; i++) {

                if (pnlDateDetailsList[i].style.height != "0px") {
                    pnlDateDetailsList[i].style.height = "auto";
                }

            }
        }

        function ExpandOrcollapseExtender(cpe, isExpand) {
            if (cpe != null) {
                if (isExpand) {
                    ExpandPanel(cpe)
                }
                else {
                    var isCollapsed = cpe.get_Collapsed();
                    if (!isCollapsed)
                        cpe._doClose();
                }
            }
        }


        function ExpandPanel(cpe) {
            var isCollapsed = cpe.get_Collapsed();
            if (isCollapsed) {
                cpe.expandPanel();
            }
        }


        function ExpandOrCollapsePanels(datePanels, isExpand) {

            for (var j = 0; j < datePanels.length; j++) {
                var cpeDate = $find(datePanels[j]);
                ExpandOrcollapseExtender(cpeDate, isExpand);
            }

        }

        function txtSearch_onkeypress(e) {
            var keynum;
            if (window.event) // IE8 and earlier
            {
                keynum = e.keyCode;
            }
            else if (e.which) // IE9/Firefox/Chrome/Opera/Safari
            {
                keynum = e.which;
            }
            if (keynum == 13) {
                var btnSearch = document.getElementById('<%= btnSearch.ClientID %>');
                btnSearch.click();
                return false;
            }
            return true;
        }

        function txtSearch_onkeyup(e) {

            var txtSearch = document.getElementById('<%= txtSearch.ClientID %>');
            var btnSearch = document.getElementById('<%= btnSearch.ClientID %>');
            if (txtSearch.value != '') {
                btnSearch.removeAttribute('disabled');
            }
            else {
                btnSearch.setAttribute('disabled', 'disabled');
            }
            return true;
        }

    </script>
    <style>
        /* --------- Tabs for person and project details pages ------ */
        
        .CustomTabStyle tr
        {
            height: 30px;
        }
        
        .CustomTabStyle td
        {
            background-color: White;
            float: left;
            padding: 8px 0px 5px 0px;
            position: relative;
        }
        
        .CustomTabStyle td a
        {
            text-decoration: none;
        }
        
        .CustomTabStyle td span a
        {
            border-bottom: 1px dashed #0898e6;
        }
        
        .CustomTabStyle td span a:hover
        {
            border-bottom: 1px dashed #006699;
        }
        
        .CustomTabStyle td a.collapse
        {
            display: none;
            position: absolute;
        }
        
        .CustomTabStyle .SelectedSwitch a.collapse
        {
            display: block;
            right: 2px;
            top: 10px;
        }
        
        .CustomTabStyle td span.bg
        {
            padding: 8px 10px 7px 10px;
        }
        
        .CustomTabStyle .SelectedSwitch span.bg
        {
            background-color: #e2ebff;
        }
        
        .tab-pane
        {
            background-color: #e2ebff;
            padding: 5px;
            width: 98%;
        }
        
        .info-field
        {
            width: 152px;
        }
        .wrapword
        {
            white-space: -moz-pre-wrap !important; /* Mozilla, since 1999 */
            white-space: -pre-wrap; /* Opera 4-6 */
            white-space: -o-pre-wrap; /* Opera 7 */
            white-space: pre-wrap; /* css-3 */
            word-wrap: break-word; /* Internet Explorer 5.5+ */
            word-break: break-all;
            white-space: normal;
        }
        .searchResult
        {
            max-height: 150px;
            min-height: 50px;
            overflow-y: auto;
            width: 100%;
        }
        
        TABLE.PaddingRightTen TD
        {
            padding-right: 25px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblPersonSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        });

        function pageLoad() {
            SetTooltipsForallDropDowns();
            intializeExpandAll();
        }

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');
            for (var i = 0; i < optionList.length; ++i) {
                optionList[i].title = optionList[i].innerHTML;
            }
        }

        function intializeExpandAll() {
            var panels = $("[id$='pnlProjectDetails']")
            for (var i = 0; i < panels.length; i++) {
                var panel = panels[i];
                if (panel != null) {
                    if (panel.style.height != "0px") {
                        panel.style.height = "auto";
                    }
                }
            }
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);

        function endRequestHandle(sender, Args) {
            imgCalender = document.getElementById('<%= imgCalender.ClientID %>');
            lblCustomDateRange = document.getElementById('<%= lblCustomDateRange.ClientID %>');
            ddlPeriod = document.getElementById('<%=  ddlPeriod.ClientID %>');
            if (imgCalender.fireEvent && ddlPeriod.value != '0') {
                imgCalender.style.display = "none";
                lblCustomDateRange.style.display = "none";
            }
            SetTooltipsForallDropDowns();
            $("#tblPersonSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        }

    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table width="100%" style="height: 120px;">
                <tr>
                    <td style="width: 65%">
                    </td>
                    <td style="padding-bottom: 10px; text-align: center">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 30%; text-align: right; font-weight: bold;">
                                    Person:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlPerson" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPerson_SelectedIndexChanged"
                                                    Width="150px">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Image ID="imgSearch" runat="server" ToolTip="Person Search" ImageUrl="~/Images/search_24.png" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 65%">
                    </td>
                    <td style="padding-bottom: 10px; text-align: center;">
                        <table width="100%" align="center">
                            <tr>
                                <td style="text-align: right; width: 30%; font-weight: bold;">
                                    Range:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="150px">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – P1" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – P2" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="This Week" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                                        <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                                        <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                        <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
                                        <asp:ListItem Text="Total Employment" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 65%">
                    </td>
                    <td style="padding-bottom: 10px; text-align: center;">
                        <table width="100%" align="center">
                            <tr>
                                <td style="width: 30%;">
                                </td>
                                <td style="text-align: left; height: 15px;">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="border-bottom: 3px solid black; width: 100%;">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" BackColor="White" BorderColor="Black"
                CssClass="ConfirmBoxClass" Style="padding-top: 20px; display: none;" BorderWidth="2px">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldWidth="70" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 10px 0px 10px 0px;">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                Style="float: none !Important;" CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" runat="server" CausesValidation="false" Text="Cancel"
                                OnClick="btnCustDatesCancel_OnClick" Style="float: none !Important;" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <div id="divWholePage" runat="server">
                <table class="PaddingTenPx" style="width: 100%; height: 90px;">
                    <tr>
                        <td style="font-size: 18px; font-weight: bold; padding-bottom: 10px;">
                            <table>
                                <tr>
                                    <td style="vertical-align: top;">
                                        <asp:Label ID="lblPersonname" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: top; font-size: 13px">
                                        <asp:Label ID="lblPersonStatus" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 5px; vertical-align: bottom;">
                                        <asp:Label ID="lbRange" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align: right; width: 470px">
                            <table style="table-layout: fixed; width: 100%;">
                                <tr>
                                    <td style="width: 27%;">
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                                    Total Hours
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: 25px; text-align: center;">
                                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 27%;">
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                                    Utilization
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: 25px; text-align: center;">
                                                    <asp:Literal ID="ltrlUtilization" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 27%; vertical-align: bottom;">
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: center;">
                                                    BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-bottom: 5px; text-align: center;">
                                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: center;">
                                                    NON-BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: center;">
                                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="vertical-align: bottom; width: 8%; padding: 0px !important;">
                                        <table width="100%">
                                            <tr>
                                                <td style="padding: 0px !important;">
                                                    <table width="100%" style="table-layout: fixed;">
                                                        <tr>
                                                            <td style="text-align: center;">
                                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table width="100%">
                                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                                            <td style="background-color: #7FD13B; border: 1px solid Gray;">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="vertical-align: bottom; width: 8%; padding: 0px;">
                                        <table width="100%">
                                            <tr>
                                                <td style="padding: 0px !important;">
                                                    <table width="100%" style="table-layout: fixed;">
                                                        <tr>
                                                            <td style="text-align: center;">
                                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table width="100%">
                                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                                            <td style="background-color: #BEBEBE; border: 1px solid Gray;">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 2%;">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellSummary" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnSummary" runat="server" Text="Summary" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Summary"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellDetail" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnDetail" runat="server" Text="Detail" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="1" ToolTip="Detail"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvPersonDetailReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwPersonSummaryReport" runat="server">
                        <asp:Panel ID="pnlPersonSummaryReport" runat="server" CssClass="tab-pane">
                            <uc:PersonSummaryReport ID="ucpersonSummaryReport" runat="server" />
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwPersonDetailReport" runat="server">
                        <asp:Panel ID="pnlPersonDetailReport" runat="server" CssClass="tab-pane">
                            <uc:PersonDetailReport ID="ucpersonDetailReport" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
            <AjaxControlToolkit:ModalPopupExtender ID="mpePersonSearch" runat="server" TargetControlID="imgSearch"
                CancelControlID="btnclose" BackgroundCssClass="modalBackground" PopupControlID="pnlPersonSearch"
                BehaviorID="mpePersonSearch" DropShadow="false" />
            <asp:Panel ID="pnlPersonSearch" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none; min-height: 100px;" BorderWidth="2px" Width="400px">
                <table width="100%" style="padding: 5px;">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray;" valign="baseline"
                            colspan="2">
                            <b style="font-size: 14px; padding-top: 2px;">Person Search</b>
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                                UseSubmitBehavior="false" Style="float: right;" OnClientClick="return btnClose_OnClientClick('mpePersonSearch');"
                                Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td style="width: 95%; padding: 10px; padding-right: 0px;">
                            <asp:TextBox runat="server" ID="txtSearch" Style="text-align: left; margin-right: 0px !important;
                                width: 300px;" onkeypress="return txtSearch_onkeypress(event);" onkeyup="return txtSearch_onkeyup(event);"></asp:TextBox>
                            <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server" TargetControlID="txtSearch"
                                BehaviorID="waterMarkTxtSearch" WatermarkCssClass="watermarkedtext" WatermarkText="To search for a person, click here to begin typing...">
                            </ajaxToolkit:TextBoxWatermarkExtender>
                        </td>
                        <td style="width: 5%; padding: 0px; padding-right: 10px;">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick"
                                disabled="disabled" UseSubmitBehavior="false" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px; width: 100%;" colspan="2">
                            <div id="dlPersonDiv" class="searchResult" runat="server" style="display: none;">
                                <asp:Repeater ID="repPersons" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblPersonSearchResult" class="tablesorter CompPerfTable WholeWidth">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th style="width: 50%; text-align: center;">
                                                        <div class="ie-bg">
                                                            Person Name
                                                        </div>
                                                    </th>
                                                    <th style="text-align: center;">
                                                        <div class="ie-bg">
                                                            Pay Type
                                                        </div>
                                                    </th>
                                                    <th style="text-align: center;">
                                                        <div class="ie-bg">
                                                            Status
                                                        </div>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 50%;">
                                                <asp:LinkButton ID="lnkPerson" Text="<%# GetPersonFirstLastName(((DataTransferObjects.Person)Container.DataItem))  %>"
                                                    OnClick="lnkPerson_OnClick" PersonId='<%# ((DataTransferObjects.Person)Container.DataItem).Id.ToString() %>'
                                                    runat="server"></asp:LinkButton>
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Label Text='<%# Eval("CurrentPay.TimescaleName") %>' ID="lbTimescale" runat="server"></asp:Label>
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Label Text='<%# Eval("Status.Name")%>' ID="lbStatus" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div id="divEmptyResults" runat="server" style="display: none;">
                                    No Results found.
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ucpersonSummaryReport$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucpersonDetailReport$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

