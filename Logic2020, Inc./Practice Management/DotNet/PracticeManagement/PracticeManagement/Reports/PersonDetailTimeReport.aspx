<%@ Page Title="By Person" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="PersonDetailTimeReport.aspx.cs" Inherits="PraticeManagement.Reporting.PersonDetailTimeReport" %>

<%@ Import Namespace="PraticeManagement.Utils" %>
<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register Src="~/Controls/Reports/PersonSummaryReport.ascx" TagPrefix="uc" TagName="PersonSummaryReport" %>
<%@ Register Src="~/Controls/Reports/PersonDetailReport.ascx" TagPrefix="uc" TagName="PersonDetailReport" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src='<%# Generic.GetClientUrl("~/Scripts/ExpandOrCollapse.min.js", this) %>'
        type="text/javascript"></script>
    <link href="<%# Generic.GetClientUrl("~/Css/TableSortStyle.min.css", this) %>" rel="stylesheet"
        type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblPersonSummaryReport").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );

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


            $("#tblPersonSummaryReport").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        }
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

        function ShowPanel(object, displaypnl, position) {

            var obj = $("#" + object);
            var displayPanel = $("#" + displaypnl);
            iptop = obj.offset().top + obj[0].offsetHeight;
            ipleft = obj.offset().left - position;
            displayPanel.offset({ top: iptop, left: ipleft });
            displayPanel.show();
            displayPanel.offset({ top: iptop, left: ipleft });
        }

        function HidePanel(hiddenpnl) {

            var displayPanel = $("#" + hiddenpnl);
            displayPanel.hide();
        }

    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <uc:LoadingProgress ID="LoadingProgress1" runat="server" />
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table class="WholeWidth">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table class="WholeWidth Height160Px">
                <tr>
                    <td id="tdFirst" runat="server" class="Width65Percent">
                    </td>
                    <td class="ReportTdSecond" id="tdSecond" runat="server">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd">
                                    Person:&nbsp;
                                </td>
                                <td class="SecondTd">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlPerson" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPerson_SelectedIndexChanged">
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
                    <td id="tdThird" runat="server" class="Width0Percent">
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd">
                                    Range:&nbsp;
                                </td>
                                <td class="SecondTd">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
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
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td class="ReportTdSecond">
                        <table class="ReportParametersTable">
                            <tr>
                                <td class="FirstTd">
                                </td>
                                <td class="SecondTd">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="Height30Px">
                        &nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="Height30Px">
                        &nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="ReportBorderBottom">
                    </td>
                </tr>
            </table>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeCustomDates" runat="server" TargetControlID="imgCalender"
                BackgroundCssClass="modalBackground" PopupControlID="pnlCustomDates" BehaviorID="bhCustomDates"
                DropShadow="false" />
            <asp:Panel ID="pnlCustomDates" runat="server" CssClass="ConfirmBoxClass CustomDatesPopUp"
                Style="display: none;">
                <table class="WholeWidth">
                    <tr>
                        <td align="center">
                            <uc:DateInterval ID="diRange" runat="server" IsFromDateRequired="true" IsToDateRequired="true"
                                FromToDateFieldCssClass="Width70Px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="custBtns">
                            <asp:Button ID="btnCustDatesOK" runat="server" OnClick="btnCustDatesOK_Click" Text="OK"
                                CausesValidation="true" />
                            &nbsp; &nbsp;
                            <asp:Button ID="btnCustDatesCancel" runat="server" CausesValidation="false" Text="Cancel"
                                OnClick="btnCustDatesCancel_OnClick" />
                        </td>
                    </tr>
                    <tr>
                        <td class="textCenter">
                            <asp:ValidationSummary ID="valSumDateRange" runat="server" ValidationGroup='<%# ClientID %>' />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <div id="divWholePage" runat="server">
                <table class="PaddingTenPx PersonDetailReportHeader">
                    <tr>
                        <td class="PersonDetails">
                            <table>
                                <tr>
                                    <td class="vTop">
                                        <asp:Label ID="lblPersonname" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="vTop font13Px">
                                        <asp:Label ID="lblPersonStatus" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PaddingTop5Imp vBottom">
                                        <asp:Label ID="lbRange" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="PersonTotals">
                            <table class="ReportHeaderTotals">
                                <tr>
                                    <td class="Width27Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd">
                                                    Total Actual Hours
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Literal ID="ltrlTotalHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width27Percent">
                                        <table class="ReportHeaderTotalsTable">
                                            <tr>
                                                <td class="FirstTd">
                                                    Billable Utilization
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="SecondTd">
                                                    <asp:Label ID="lblBillableUtilization" runat="server"></asp:Label>
                                                    <asp:Image alt="Billable Utilization Hint" ImageUrl="~/Images/hint1.png" runat="server"
                                                        ID="imgBillableUtilizationHint" CssClass="PaddingBottom5 CursorPointer" ToolTip="Billable Utilization Calculation" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width27Percent vBottom">
                                        <table class="ReportHeaderBillAndNonBillTable">
                                            <tr>
                                                <td>
                                                    BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="billingHours">
                                                    <asp:Literal ID="ltrlBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    NON-BILLABLE
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Literal ID="ltrlNonBillableHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trBillable" runat="server" title="Billable Percentage.">
                                                            <td class="billingGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="ReportHeaderBandNBGraph">
                                        <table>
                                            <tr>
                                                <td>
                                                    <table class="tableFixed">
                                                        <tr>
                                                            <td>
                                                                <asp:Literal ID="ltrlNonBillablePercent" runat="server"></asp:Literal>%
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <table>
                                                        <tr id="trNonBillable" runat="server" title="Non-Billable Percentage.">
                                                            <td class="nonBillingGraph">
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="Width2Percent">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Table ID="tblPersonViewSwitch" runat="server" CssClass="CommonCustomTabStyle PersonDetailReportCustomTabStyle">
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
                        <asp:Panel ID="pnlPersonSummaryReport" runat="server" CssClass="tab-pane Width98Percent">
                            <uc:PersonSummaryReport ID="ucpersonSummaryReport" runat="server" />
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwPersonDetailReport" runat="server">
                        <asp:Panel ID="pnlPersonDetailReport" runat="server" CssClass="tab-pane Width98Percent">
                            <uc:PersonDetailReport ID="ucpersonDetailReport" runat="server" />
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
            <AjaxControlToolkit:ModalPopupExtender ID="mpePersonSearch" runat="server" TargetControlID="imgSearch"
                CancelControlID="btnclose" BackgroundCssClass="modalBackground" PopupControlID="pnlPersonSearch"
                BehaviorID="mpePersonSearch" DropShadow="false" />
            <asp:Panel ID="pnlPersonSearch" runat="server" CssClass="popUp PersonSearch" Style="display: none;">
                <table class="WholeWidth">
                    <tr class="PopUpHeader">
                        <th colspan="2">
                            Person Search
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-closeNew" ToolTip="Close"
                                UseSubmitBehavior="false" OnClientClick="return btnClose_OnClientClick('mpePersonSearch');"
                                Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="searchTextboxTd">
                            <asp:TextBox runat="server" ID="txtSearch" CssClass="searchTextbox" onkeypress="return txtSearch_onkeypress(event);"
                                onkeyup="return txtSearch_onkeyup(event);"></asp:TextBox>
                            <AjaxControlToolkit:TextBoxWatermarkExtender ID="waterMarkTxtSearch" runat="server"
                                TargetControlID="txtSearch" BehaviorID="waterMarkTxtSearch" WatermarkCssClass="watermarkedtext searchTextbox"
                                WatermarkText="To search for a person, click here to begin typing...">
                            </AjaxControlToolkit:TextBoxWatermarkExtender>
                        </td>
                        <td class="searchButtonTd">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick"
                                disabled="disabled" UseSubmitBehavior="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Padding10 WholeWidth" colspan="2">
                            <div id="dlPersonDiv" class="PersonSearchResultDiv" runat="server" style="display: none;">
                                <asp:Repeater ID="repPersons" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblPersonSearchResult" class="tablesorter CompPerfTable PersonSearchResult">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th class="Width50Percent">
                                                        <div class="ie-bg">
                                                            Person Name
                                                        </div>
                                                    </th>
                                                    <th>
                                                        <div class="ie-bg">
                                                            Pay Type
                                                        </div>
                                                    </th>
                                                    <th>
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
                                            <td class="PersonName">
                                                <asp:LinkButton ID="lnkPerson" Text="<%# GetPersonFirstLastName(((DataTransferObjects.Person)Container.DataItem))  %>"
                                                    OnClick="lnkPerson_OnClick" PersonId='<%# ((DataTransferObjects.Person)Container.DataItem).Id.ToString() %>'
                                                    runat="server"></asp:LinkButton>
                                            </td>
                                            <td>
                                                <asp:Label Text='<%# Eval("CurrentPay.TimescaleName") %>' ID="lbTimescale" runat="server"></asp:Label>
                                            </td>
                                            <td>
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
            <asp:Panel ID="pnlBillableUtilizationCalculation" CssClass="pnlBillableUtilizationCalculation"
                runat="server" Style="display: none;">
                <table>
                    <tr class="vTop font15PxImp PaddingBottom5Imp">
                        <th class="Width50Percent">
                            Variables
                        </th>
                        <th class="Width10Percent">
                        </th>
                        <th class="Width20Percent">
                            Calculation
                        </th>
                        <th class="Width5Percent">
                        </th>
                        <th class="Width15Percent">
                            Billable Utilization&nbsp;%
                        </th>
                    </tr>
                    <tr>
                        <td class="Width50Percent TextAlignLeft">
                            # of total hours billed to a client project(s) during the specified period
                        </td> 
                        <td class="Width10Percent vBottom textCenter">
                            <asp:Label ID="lblTotalBillableHours" runat="server"></asp:Label>
                        </td>
                        <td class="Width20Percent vBottom textCenter">
                            <asp:Label ID="lblTotalBillableHoursInBold" runat="server" CssClass="font15PxImp"></asp:Label>
                        </td>
                        <td class="Width5Percent">
                        </td>
                        <td class="Width15Percent">
                        </td>
                    </tr>
                    <tr>
                        <td class="Width50Percent">
                        </td>
                        <td class="Width10Percent">
                        </td>
                        <td class="Width20Percent">
                            <hr class="hrArritionCalculation" />
                        </td>
                        <td class="Width5Percent textCenter">
                            =
                        </td>
                        <td class="Width15Percent textCenter">
                            <asp:Label ID="lblBillableUtilizationPercentage" runat="server" CssClass="font15PxImp"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Width50Percent TextAlignLeft PaddingBottom10Imp">
                            # of total available hours in specified time period
                        </td>
                        <td class="Width10Percent textCenter">
                            <asp:Label ID="lblTotalAvailableHours" runat="server"></asp:Label>
                        </td>
                        <td class="Width20Percent vTop textCenter">
                            <asp:Label ID="lblTotalAvailableHoursInBold" runat="server" CssClass="font15PxImp"></asp:Label>
                        </td>
                        <td class="Width5Percent">
                        </td>
                        <td class="Width15Percent">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <AjaxControlToolkit:ModalPopupExtender ID="mpeBillableUtilization" runat="server"
                TargetControlID="imgBillableUtilizationHint" CancelControlID="btnCancel" BehaviorID="pnlBillableUtilization"
                BackgroundCssClass="modalBackground" PopupControlID="pnlBillableUtilization"
                DropShadow="false" />
            <asp:Panel ID="pnlBillableUtilization" runat="server" CssClass="popUpBillableUtilization"
                Style="display: none;">
                <table>
                    <tr>
                        <td colspan="3">
                            <asp:Button ID="btnCancel" runat="server" CssClass="mini-report-close floatright"
                                ToolTip="Close" Text="X"></asp:Button>
                        </td>
                    </tr>
                    <tr>
                        <td class="Width20Percent">
                        </td>
                        <td class="Width2Percent">
                        </td>
                        <td class="Width78Percent textCenter vBottom">
                            # of hours billed to a client project(s) during specified time period
                        </td>
                    </tr>
                    <tr>
                        <td class="Width25Percent">
                            <label class="LabelProject">
                                Billable Utilization Calculation:
                            </label>
                        </td>
                        <td class="Width2Percent">
                        </td>
                        <td class="Width73Percent textCenter">
                            <hr class="WholeWidth hrArrition" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Width20Percent">
                        </td>
                        <td class="Width2Percent">
                        </td>
                        <td class="Width78Percent textCenter vTop">
                            # of available hours in specified time period
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="FontSize10PX PaddingTop12">
                            <span class="TextAlignLeft">Note: the number of available hours is based on 2,080 hours
                                in a calendar year (40 hours a week with 52 weeks a year). The number is adjusted
                                accordingly for individuals who join the company during the year. </span>
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

