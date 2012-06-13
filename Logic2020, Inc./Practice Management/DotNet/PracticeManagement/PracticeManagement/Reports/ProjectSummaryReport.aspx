﻿<%@ Page Title="By Project" Language="C#" MasterPageFile="~/PracticeManagementMain.Master"
    AutoEventWireup="true" CodeBehind="ProjectSummaryReport.aspx.cs" Inherits="PraticeManagement.Reporting.ProjectSummaryReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="cc2" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register Src="~/Controls/Generic/Filtering/DateInterval.ascx" TagPrefix="uc"
    TagName="DateInterval" %>
<%@ Register Src="~/Controls/Reports/TimeEntryReportsHeader.ascx" TagPrefix="uc"
    TagName="TimeEntryReportsHeader" %>
<%@ Register TagPrefix="uc" TagName="LoadingProgress" Src="~/Controls/Generic/LoadingProgress.ascx" %>
<%@ Register Src="~/Controls/Reports/ProjectSummaryByResource.ascx" TagPrefix="uc"
    TagName="ByResource" %>
<%@ Register Src="~/Controls/Reports/ByworkType.ascx" TagPrefix="uc" TagName="ByWorkType" %>
<%@ Register Src="~/Controls/MessageLabel.ascx" TagName="MessageLabel" TagPrefix="uc" %>
<%@ Register TagPrefix="cc3" Assembly="PraticeManagement" Namespace="PraticeManagement.Controls" %>
<%@ Register TagPrefix="asp" Namespace="PraticeManagement.Controls.Generic.Buttons"
    Assembly="PraticeManagement" %>
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
        
        TABLE.CustomViewStyle TD
        {
            padding: 4px;
        }
    </style>
    <link href="../Css/TableSortStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="body" runat="server">
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.tablesorter.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            SetTooltipsForallDropDowns();
            $("#tblProjectSummaryByResource").tablesorter(
            {
                headers: {
                    6: {
                        sorter: false
                    },
                    1: {
                        sorter: false
                    },
                    7: {
                        sorter: false
                    }
                },
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSummaryByWorkType").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
        });

        function SetTooltipsForallDropDowns() {
            var optionList = document.getElementsByTagName('option');

            for (var i = 0; i < optionList.length; ++i) {

                optionList[i].title = optionList[i].innerHTML;
            }

        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandle);
        function endRequestHandle(sender, Args) {

            SetTooltipsForallDropDowns();

            $("#tblProjectSummaryByResource").tablesorter(
                {
                    headers: {
                        6: {
                            sorter: false
                        },
                        1: {
                            sorter: false
                        },
                        7: {
                            sorter: false
                        }
                    },
                    sortList: [[0, 0]],
                    sortForce: [[0, 0]]
                });

            $("#tblProjectSummaryByWorkType").tablesorter({
                sortList: [[0, 0]],
                sortForce: [[0, 0]]
            });

            $("#tblProjectSearchResult").tablesorter(
                {
                    sortList: [[0, 0]]
                }
                );
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
                var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
                btnSearch.click();
                return false;
            }
            return true;
        }

        function txtSearch_onkeyup(e) {

            var txtProjectSearch = document.getElementById('<%= txtProjectSearch.ClientID %>');
            var btnSearch = document.getElementById('<%= btnProjectSearch.ClientID %>');
            if (txtProjectSearch.value != '') {
                btnSearch.removeAttribute('disabled');
            }
            else {
                btnSearch.setAttribute('disabled', 'disabled');
            }
            return true;
        }

    </script>
    <uc:TimeEntryReportsHeader ID="timeEntryReportHeader" runat="server">
    </uc:TimeEntryReportsHeader>
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
            <table style="width: 100%; height: 160px;">
                <tr>
                    <td id="tdFirst" runat="server" style="width: 50%;">
                        &nbsp;
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;" id="tdSecond"
                        runat="server">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 40%; text-align: right; font-weight: bold;">
                                    Project Number:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtProjectNumber" Width="150px" AutoPostBack="true" OnTextChanged="txtProjectNumber_OnTextChanged"
                                                    runat="server"></asp:TextBox>
                                                <ajaxToolkit:TextBoxWatermarkExtender ID="waterMarkTxtProjectNumber" runat="server"
                                                    TargetControlID="txtProjectNumber" BehaviorID="waterMarkTxtProjectNumber" WatermarkCssClass="watermarkedtext"
                                                    WatermarkText="Ex: P1234767">
                                                </ajaxToolkit:TextBoxWatermarkExtender>
                                            </td>
                                            <td>
                                                <asp:Image ID="imgProjectSearch" runat="server" ToolTip="Project Search" ImageUrl="~/Images/search_24.png" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td id="tdThird" runat="server" style="width: 25%;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 40%; text-align: right; font-weight: bold;">
                                    Range:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <cc2:CustomDropDown ID="ddlPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriod_SelectedIndexChanged"
                                        Width="157px">
                                        <asp:ListItem Selected="True" Text="Entire Project" Value="*">
                                        </asp:ListItem>
                                        <asp:ListItem Text="Custom Dates" Value="0">
                                        </asp:ListItem>
                                    </cc2:CustomDropDown>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 40%;">
                                    &nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:HiddenField ID="hdnStartDate" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnEndDate" runat="server" Value="" />
                                    <asp:Label ID="lblCustomDateRange" Style="font-weight: bold;" runat="server" Text=""></asp:Label>
                                    <asp:Image ID="imgCalender" runat="server" ImageUrl="~/Images/calendar.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 40%; text-align: right; font-weight: bold;">
                                    View:&nbsp;
                                </td>
                                <td style="text-align: left;">
                                    <asp:DropDownList ID="ddlView" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlView_SelectedIndexChanged"
                                        Width="157px">
                                        <asp:ListItem Selected="True" Text="Please Select" Value=""></asp:ListItem>
                                        <asp:ListItem Text="By Resource" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="By WorkType" Value="1"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="text-align: center; height: 30px; vertical-align: top;">
                        <table width="100%" align="center" style="vertical-align: top;">
                            <tr>
                                <td style="width: 300px;" colspan="3">
                                    <uc:MessageLabel ID="msgError" runat="server" ErrorColor="Red" InfoColor="Green"
                                        WarningColor="Orange" EnableViewState="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="border-bottom: 3px solid black; width: 100%; height: 10px">
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
            <AjaxControlToolkit:ModalPopupExtender ID="mpeProjectSearch" runat="server" TargetControlID="imgProjectSearch"
                BackgroundCssClass="modalBackground" PopupControlID="pnlProjectSearch" BehaviorID="mpeProjectSearch"
                DropShadow="false" />
            <asp:Panel ID="pnlProjectSearch" runat="server" BackColor="White" BorderColor="Black"
                Style="display: none;" BorderWidth="2px" Width="430px">
                <table width="100%" class="ProjectSearchPopup">
                    <tr>
                        <th align="center" style="text-align: center; background-color: Gray;" valign="bottom">
                            <b style="font-size: 14px; padding-top: 2px;">Project Search</b>
                            <asp:Button ID="btnclose" runat="server" CssClass="mini-report-close" ToolTip="Close"
                                Style="float: right;" OnClick="btnclose_OnClick" Text="X"></asp:Button>
                        </th>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 100px; text-align: right;">
                                        Account:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlClients" runat="server" Width="250px" OnSelectedIndexChanged="ddlClients_OnSelectedIndexChanged"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td style="width: 100px; text-align: right;">
                                        Project:
                                    </td>
                                    <td>
                                        <cc3:CustomDropDown ID="ddlProjects" runat="server" Enabled="false" AutoPostBack="true"
                                            Width="250px" OnSelectedIndexChanged="ddlProjects_OnSelectedIndexChanged">
                                            <asp:ListItem Text="-- Select a Project --" Value="">
                                            </asp:ListItem>
                                        </cc3:CustomDropDown>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <table class="WholeWidth">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtProjectSearch" onkeypress="return txtSearch_onkeypress(event);"
                                            onkeyup="return txtSearch_onkeyup(event);" Width="330px" runat="server"></asp:TextBox>
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmeProjectSearch" runat="server" TargetControlID="txtProjectSearch"
                                            WatermarkCssClass="watermarkedtext" WatermarkText="To search for a project, click here to begin typing...">
                                        </ajaxToolkit:TextBoxWatermarkExtender>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnProjectSearch" UseSubmitBehavior="false" disabled="disabled" runat="server"
                                            Text="Search" ToolTip="Search" OnClick="btnProjectSearch_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="WholeWidth">
                            <div style="max-height: 200px; overflow-y: auto;">
                                <asp:Repeater ID="repProjectNamesList" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblProjectSearchResult" class="tablesorter CompPerfTable WholeWidth">
                                            <thead>
                                                <tr class="CompPerfHeader">
                                                    <th style="width: 20%; text-align: center; background: url(../images/cp-header-bg.png) repeat-x 0px 0px;">
                                                        <div class="ie-bg">
                                                            Project
                                                        </div>
                                                    </th>
                                                    <th style="width: 50%; text-align: center; background: url(../images/cp-header-bg.png) repeat-x 0px 0px;">
                                                        <div class="ie-bg">
                                                            Project Name
                                                        </div>
                                                    </th>
                                                    <th style="text-align: center; background: url(../images/cp-header-bg.png) repeat-x 0px 0px;">
                                                        <div class="ie-bg">
                                                            Account
                                                        </div>
                                                    </th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="text-align: center;">
                                                <asp:LinkButton ID="lnkProjectNumber" ProjectNumber='<%# Eval("ProjectNumber")%>'
                                                    OnClick="lnkProjectNumber_OnClick" runat="server"><%# Eval("ProjectNumber")%></asp:LinkButton>
                                            </td>
                                            <td>
                                                <%# Eval("Name")%>
                                            </td>
                                            <td>
                                                <%# Eval("Client.Name")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 8px;">
                            <asp:Literal ID="ltrlNoProjectsText" Visible="false" runat="server" Text="No Projects found."></asp:Literal>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div id="divWholePage" runat="server">
                <asp:MultiView ID="mvProjectSummaryReport" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vwResourceReport" runat="server">
                        <asp:Panel Style="padding-top: 10px;" ID="pnlResourceReport" runat="server" CssClass="WholeWidth">
                            <uc:ByResource ID="ucByResource" runat="server">
                            </uc:ByResource>
                        </asp:Panel>
                    </asp:View>
                    <asp:View ID="vwProjectReport" runat="server">
                        <asp:Panel ID="pnlProjectReport" runat="server" CssClass="WholeWidth">
                            <uc:ByWorkType ID="ucByWorktype" runat="server">
                            </uc:ByWorkType>
                        </asp:Panel>
                    </asp:View>
                </asp:MultiView>
                <table width="100%">
                    <tr>
                        <td style="text-align: center; padding-top: 5px;">
                            <asp:CancelAndReturnButton ID="btnCancelAndReturn" runat="server" Text="Return To Pervious Report" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectSummaryReport$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectDetailReport$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByWorktype$btnExportToExcel" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectSummaryReport$btnExportToPDF" />
            <asp:PostBackTrigger ControlID="ucByResource$ucProjectDetailReport$btnExportToPDF" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

