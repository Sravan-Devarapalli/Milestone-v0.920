<%@ Page Title="By Account" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="AccountSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.AccountSummaryReport" %>

<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register TagPrefix="uc" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="ext" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls.Generic.ScrollableDropdown" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessUnit.ascx" TagPrefix="uc"
    TagName="ByBusinessUnit" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByProject.ascx" TagPrefix="uc" TagName="ByProject" %>
<%@ Register Src="~/Controls/Reports/ByAccount/ByBusinessDevelopment.ascx" TagPrefix="uc"
    TagName="ByBusinessDevelopment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/ExpandOrCollapse.js" type="text/javascript"></script>
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
        }
        
        .info-field
        {
            width: 152px;
        }
    </style>
    <link href="../Css/TableSortStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#tblAccountSummaryByBusinessReport").tablesorter(
            {
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        });


        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            $("#tblAccountSummaryByBusinessReport").tablesorter(
                {
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });

            $("#tblAccountSummaryByProject").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });
        }
    </script>
    <script language="javascript" type="text/javascript" src="../Scripts/ScrollinDropDown.js"></script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server"></uc:TimeEntryReportsHeader>
    <asp:UpdatePanel ID="upnlBody" runat="server">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="height30P vBottom fontBold">
                        2.&nbsp;Select report parameters:
                    </td>
                </tr>
            </table>
            <table width="100%" style="height: 160px;">
                <tr>
                    <td style="width: 25%;" id="tdFirst" runat="server">
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top; width: 30%;" id="tdSecond"
                        runat="server">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 25%; font-weight: bold;">
                                    Account:&nbsp;
                                </td>
                                <td style="text-align: left; width: 75%;">
                                    <asp:DropDownList ID="ddlAccount" runat="server" AutoPostBack="true" Width="220"
                                        OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 45%" id="tdThird" runat="server">
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 25%; font-weight: bold;">
                                    Business Unit:&nbsp;
                                </td>
                                <td style="text-align: left; width: 75%;">
                                    <uc:ScrollingDropDown ID="cblProjectGroup" runat="server" SetDirty="false" AllSelectedReturnType="Null"
                                        NoItemsType="All" Height="160px" onclick="scrollingDropdown_onclick('cblProjectGroup','Business Unit')"
                                        DropDownListType="Business Unit" CellPadding="3" />
                                    <ext:ScrollableDropdownExtender ID="sdeProjectGroup" runat="server" TargetControlID="cblProjectGroup"
                                        UseAdvanceFeature="true" Width="220px" EditImageUrl="~/Images/Dropdown_Arrow.png">
                                    </ext:ScrollableDropdownExtender>
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
                    <td style="height: 30px; text-align: center; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="text-align: right; width: 25%; font-weight: bold;">
                                    Range:&nbsp;
                                </td>
                                <td style="text-align: left; width: 75%;">
                                    <asp:DropDownList ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="220px">
                                        <asp:ListItem Selected="True" Text="Please Select" Value="Please Select"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Current" Value="15"></asp:ListItem>
                                        <asp:ListItem Text="Payroll – Previous" Value="-15"></asp:ListItem>
                                        <asp:ListItem Text="This Week" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="This Month" Value="30"></asp:ListItem>
                                        <asp:ListItem Text="This Year" Value="365"></asp:ListItem>
                                        <asp:ListItem Text="Last Week" Value="-7"></asp:ListItem>
                                        <asp:ListItem Text="Last Month" Value="-30"></asp:ListItem>
                                        <asp:ListItem Text="Last Year" Value="-365"></asp:ListItem>
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
                    <td style="height: 30px; text-align: center; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 25%;">
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
                    <td>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 30px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="border-bottom: 3px solid black; width: 100%; height: 10px;">
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
                            <asp:Button ID="btnCustDatesCancel" CausesValidation="false" runat="server" Text="Cancel"
                                Style="float: none !Important;" OnClick="btnCustDatesCancel_OnClick" />
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
                <table class="PaddingTenPx" style="width: 100%; background-color: White; padding-bottom: 5px !important;">
                    <tr>
                        <td style="font-size: 16px; font-weight: bold;">
                            <table>
                                <tr>
                                    <td style="vertical-align: top; padding-bottom: 10px; font-weight: bold;">
                                        <asp:Literal ID="ltAccount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align: top; padding-bottom: 10px;">
                                        <asp:Literal ID="ltHeaderCount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 10px; vertical-align: bottom;">
                                        <asp:Literal ID="ltRange" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="text-align: right; width: 470px; padding-bottom: 10px;">
                            <table style="table-layout: fixed; width: 100%;">
                                <tr>
                                    <td style="width: 27%;">
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px; white-space: nowrap;">
                                                    Total Project Hours
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: 25px; text-align: center;">
                                                    <asp:Literal ID="ltrlTotalProjectHours" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 27%;">
                                        <table width="100%">
                                            <tr>
                                                <td style="font-size: 15px; text-align: center; padding-bottom: 3px;">
                                                    BD
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-size: 25px; text-align: center;">
                                                    <asp:Literal ID="ltrlBDHours" runat="server"></asp:Literal>
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
                <asp:Table ID="tblProjectViewSwitch" runat="server" CssClass="CustomTabStyle">
                    <asp:TableRow ID="rowSwitcher" runat="server">
                        <asp:TableCell ID="cellBusinessUnit" CssClass="SelectedSwitch" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnBusinessUnit" runat="server" Text="Business Unit" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="0" ToolTip="Business Unit"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellProject" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnProject" runat="server" Text="Project" CausesValidation="false"
                                    OnCommand="btnView_Command" CommandArgument="1" ToolTip="Project"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                        <asp:TableCell ID="cellBusinessDevelopment" runat="server">
                            <span class="bg"><span>
                                <asp:LinkButton ID="lnkbtnBusinessDevelopment" runat="server" Text="Business Development"
                                    CausesValidation="false" OnCommand="btnView_Command" CommandArgument="2" ToolTip="Business Development"></asp:LinkButton></span>
                            </span>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <asp:MultiView ID="mvAccountReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwBusinessUnitReport" runat="server">
                        <asp:Panel ID="pnlBusinessUnitReport" runat="server" CssClass="WholeWidth">
                            <uc:ByBusinessUnit ID="tpByBusinessUnit" runat="server"></uc:ByBusinessUnit>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <uc:ByProject ID="tpByProject" runat="server"></uc:ByProject>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwBusinessDevelopmentReport" runat="server">
                        <asp:Panel ID="pnlBusinessDevelopmentReport" runat="server" CssClass="WholeWidth">
                            <uc:ByBusinessDevelopment ID="tpByBusinessDevelopment" runat="server"></uc:ByBusinessDevelopment>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="tpByProject$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="tpByBusinessUnit$btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

